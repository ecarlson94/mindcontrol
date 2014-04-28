using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DigitalRune.Diagnostics;
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Graphics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.ServiceLocation;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif


namespace DigitalRune.Samples
{
  // This XNA GameComponent loads the start page and switches between samples.
  // The GuiGraphicsScreen is used to draw a GUI over the samples scenes.
  // Hold <Alt> to show the sample selection menu.
  // Double-click <Esc> or <GamePad.Back> to exit the game.
  public class MenuComponent : GameComponent
  {
    private readonly ServiceContainer _services;
    private readonly IInputService _inputService;
    private readonly IGraphicsService _graphicsService;

    // A collection of all automatically discovered samples.
    private List<Type> _samples;

    // The currently loaded sample.
    private int _sampleIndex = -1;
    private GameComponent _sample;

    // The sample that should be loaded.
    private int _nextSampleIndex = -1;

    // GUI controls.
    private GuiGraphicsScreen _guiGraphicsScreen;
    private Window _menuWindow;
    private Window _settingsWindow;


    public MenuComponent(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      _services = (ServiceContainer)ServiceLocator.Current;
      _inputService = _services.GetInstance<IInputService>();
      _graphicsService = _services.GetInstance<IGraphicsService>();

#if WINDOWS_PHONE
      TouchPanel.EnabledGestures |= GestureType.Flick;
#endif

      // Update this game component after all other game components. (The other
      // game components should get a chance to handle input first.)
      UpdateOrder = int.MaxValue;

      DiscoverSamples();
      InitializeGui();

      // Set _nextSampleIndex to immediately start a specific sample.
#if WINDOWS_PHONE
      _nextSampleIndex = _samples.IndexOf(typeof(DigitalRune.Samples.Graphics.BasicEffectSample));
#elif KINECT
      _nextSampleIndex = _samples.IndexOf(typeof(DigitalRune.Samples.Kinect.KinectSkeletonMappingSample));
#else
      _nextSampleIndex = _samples.IndexOf(typeof(DigitalRune.Samples.Graphics.DeferredLightingSample));
#endif
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clean up.
        if (_sample != null)
        {
          Game.Components.Remove(_sample);
          _sample.Dispose();
        }

        if (_guiGraphicsScreen != null)
        {
          _graphicsService.Screens.Remove(_guiGraphicsScreen);
          _guiGraphicsScreen.Dispose();
        }
      }

      base.Dispose(disposing);
    }


    private void DiscoverSamples()
    {
      // Automatically find all samples using reflection. Samples are derived from 
      // GameComponent and have a SampleAttribute.
      _samples = Assembly.GetCallingAssembly()
                         .GetTypes()
                         .Where(t => typeof(GameComponent).IsAssignableFrom(t)
                                     && SampleAttribute.GetSampleAttribute(t) != null)
                         .ToList();
      _samples.Sort(CompareSamples);

      if (_samples.Count == 0)
        throw new Exception("No samples found.");
    }


    private static int CompareSamples(Type sampleA, Type sampleB)
    {
      var sampleAttributeA = SampleAttribute.GetSampleAttribute(sampleA);
      var sampleAttributeB = SampleAttribute.GetSampleAttribute(sampleB);

      // Sort by category ...
      var categoryA = sampleAttributeA.Category;
      var categoryB = sampleAttributeB.Category;
      if (categoryA < categoryB)
        return -1;
      if (categoryA > categoryB)
        return +1;

      // ... then by sample order.
      int orderA = sampleAttributeA.Order;
      int orderB = sampleAttributeB.Order;
      return orderA - orderB;
    }


    private void InitializeGui()
    {
      // Add the GuiGraphicsScreen to the graphics service.
      _guiGraphicsScreen = new GuiGraphicsScreen(_services);
      _graphicsService.Screens.Add(_guiGraphicsScreen);

      // Add the Settings window (v-sync, fixed/variable timing, parallel game loop).
      _settingsWindow = new SettingsWindow(_services)
      {
        X = 1000,
        Y = 500,
        MinHeight = 0,
      };
      _settingsWindow.Show(_guiGraphicsScreen.UIScreen);

      // Create a window with a tab control.
      _menuWindow = new Window
      {
        Title = "Select Sample",
        X = 50,
        Y = 50,
        Width = 800,
        Height = 554,
        CloseButtonStyle = null
      };
      _menuWindow.Show(_guiGraphicsScreen.UIScreen);

      // Each tab shows a sample category (Base, Mathematics, Geometry, ...).
      var samplesByCategory = _samples.GroupBy(t => SampleAttribute.GetSampleAttribute(t).Category);
      var tabControl = new TabControl
      {
        Margin = new Vector4F(4),
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
      };
      _menuWindow.Content = tabControl;

      int category = -1;
      foreach (var grouping in samplesByCategory)
      {
        category++;
        var tabItem = new TabItem
        {
          Content = new TextBlock { Text = grouping.Key.ToString() },
        };
        tabControl.Items.Add(tabItem);

        // Optional: When the TabControl.SelectedIndex is changed, activate the 
        // first sample of the category.
        //Type firstSample = grouping.First();
        //var firstSampleIndex = _samples.IndexOf(firstSample);
        //var selectedIndexProperty = tabControl.Properties.Get<int>("SelectedIndex");
        //int c = category;
        //selectedIndexProperty.Changed += (s, e) =>
        //{
        //  if (tabControl.SelectedIndex == c)
        //    _nextSampleIndex = firstSampleIndex;
        //};

        // Each TabItem contains a horizontal stack panel. The horizontal panel
        // contains vertical stack panels with up to N buttons per vertical panel.
        var horizontalPanel = new StackPanel
        {
          Orientation = Orientation.Horizontal,
          Margin = new Vector4F(2, 0, 2, 0),
        };
        tabItem.TabPage = horizontalPanel;

        StackPanel verticalPanel = null;
        int i = -1;
        foreach (Type sample in grouping)
        {
          i = (i + 1) % 15;
          if (i == 0)
          {
            verticalPanel = new StackPanel
            {
              Orientation = Orientation.Vertical,
            };
            horizontalPanel.Children.Add(verticalPanel);
          }

          string summary = SampleAttribute.GetSampleAttribute(sample).Summary;
          var button = new Button
          {
            Content = new TextBlock { Text = sample.Name, Margin = new Vector4F(0, 2, 0, 2) },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Vector4F(4),
            ToolTip = string.IsNullOrEmpty(summary) ? null : summary
          };
          var sampleIndex = _samples.IndexOf(sample);
          button.Click += (s, e) => _nextSampleIndex = sampleIndex;

          Debug.Assert(verticalPanel != null);
          verticalPanel.Children.Add(button);
        }
      }
    }


    public override void Update(GameTime gameTime)
    {
      if (_nextSampleIndex != _sampleIndex)
      {
        LoadSample(_nextSampleIndex);
        return;
      }

      // <Alt> ... Show Menu and Settings windows.
      bool isAltDown = _inputService.IsDown(Keys.LeftAlt) || _inputService.IsDown(Keys.RightAlt);
      _guiGraphicsScreen.HideBackground = isAltDown;
      _menuWindow.IsVisible = isAltDown;
      _settingsWindow.IsVisible = isAltDown;

#if !WINDOWS_PHONE
      bool isControlDown = _inputService.IsDown(Keys.LeftControl) || _inputService.IsDown(Keys.RightControl);

      if (!isAltDown 
          && !isControlDown 
          && !_inputService.IsKeyboardHandled 
          && !_inputService.IsGamePadHandled(LogicalPlayerIndex.Any))
      {
        // If <Esc> or <GamePad Back> is double-clicked, the game exits.
        if (_inputService.IsDoubleClick(Keys.Escape) || _inputService.IsDoubleClick(Buttons.Back, LogicalPlayerIndex.One))
          Game.Exit();

        // If <Keyboard End> or <LeftShoulder>+<RightShoulder> is pressed, the current sample restarts.
        if (_inputService.IsPressed(Keys.End, true) || _inputService.IsPressed(Buttons.LeftShoulder | Buttons.RightShoulder, true, LogicalPlayerIndex.One))
          RestartSample();

        // If <Keyboard PageUp> is pressed or <D-pad Left>, the previous sample is loaded.
        if (_inputService.IsPressed(Keys.PageUp, true) || _inputService.IsPressed(Buttons.DPadLeft, true, LogicalPlayerIndex.One))
          LoadPreviousSample();

        // If <Keyboard PageDown> is pressed or <D-pad Right>, the next sample is loaded.
        if (_inputService.IsPressed(Keys.PageDown, true) || _inputService.IsPressed(Buttons.DPadRight, true, LogicalPlayerIndex.One))
          LoadNextSample();
      }
#else
      // If <Back> is double-clicked, the game exits.
      if (_inputService.IsDoubleClick(Buttons.Back, LogicalPlayerIndex.One))
        Game.Exit();

      foreach (var gesture in _inputService.Gestures)
      {
        if (gesture.GestureType == GestureType.Flick)
        {
          if (Math.Abs(gesture.Delta.X) > Math.Abs(gesture.Delta.Y)) // Is flick horizontal?
          {
            if (gesture.Delta.X < 0)
              LoadNextSample();
            else
              LoadPreviousSample();
          }
          break;
        }
      }
#endif
    }


    private void LoadSample(int sampleIndex)
    {
      EndSample();

      var type = _samples[sampleIndex];

      // Use the service container to create and instance of the sample. The service 
      // container will automatically provide the sample constructor with the necessary
      // arguments.
      // If this code throws an exception, then open the Visual Studio menu 
      //   Debug > Exceptions... 
      // and check the check box next to "Common Language Runtime Exceptions".
      // If you run the game again, then the debugger should break where the
      // inner exception is created and not on this line.
      _sample = (GameComponent)_services.CreateInstance(type);
      _sampleIndex = sampleIndex;
      _nextSampleIndex = sampleIndex;
      Game.Components.Add(_sample);
    }


    public void EndSample()
    {
      if (_sample != null)
      {
        Game.Components.Remove(_sample);
        _sample.Dispose();
        _sample = null;

        Profiler.ClearAll();
        ResourcePool.ClearAll();
        GC.Collect();
      }
    }


    private void RestartSample()
    {
      LoadSample(_sampleIndex);
    }


    public void LoadPreviousSample()
    {
      if (_sampleIndex <= 0)
        _sampleIndex = _samples.Count - 1;
      else
        _sampleIndex--;

      LoadSample(_sampleIndex);
    }


    public void LoadNextSample()
    {
      if (_sampleIndex >= _samples.Count - 1)
        _sampleIndex = 0;
      else
        _sampleIndex++;

      LoadSample(_sampleIndex);
    }
  }
}
