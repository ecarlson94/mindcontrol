using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples
{
  // Displays general application settings (e.g. V-Sync, fixed or variable frame rate).
  public class SettingsWindow : Window
  {
    private readonly IServiceLocator _services;
    private readonly CheckBox _frameRateCheckBox;
    private readonly CheckBox _vSyncCheckBox;
    private readonly CheckBox _parallelGameLoopCheckBox;


    public SettingsWindow(IServiceLocator services)
    {
      _services = services;
      var game = services.GetInstance<Microsoft.Xna.Framework.Game>();
      var graphicsDeviceManager = services.GetInstance<GraphicsDeviceManager>();

      Title = "Settings";

      // Remove Close (X) button of window.
      CloseButtonStyle = null;

      var panel = new StackPanel
      {
        Orientation = Orientation.Vertical,
      };
      Content = panel;

      _frameRateCheckBox = new CheckBox
      {
        Content = new TextBlock { Text = "Use fixed frame rate" },
        IsChecked = game.IsFixedTimeStep,
        Margin = new Vector4F(4),
      };
      _frameRateCheckBox.Click += (s, e) => ToggleFrameRate();
      panel.Children.Add(_frameRateCheckBox);

      _vSyncCheckBox = new CheckBox
      {
        Content = new TextBlock { Text = "Enable V-Sync" },
        IsChecked = graphicsDeviceManager.SynchronizeWithVerticalRetrace,
        Margin = new Vector4F(4),
      };
      _vSyncCheckBox.Click += (s, e) => ToggleVSync();
      panel.Children.Add(_vSyncCheckBox);

      _parallelGameLoopCheckBox = new CheckBox
      {
        Content = new TextBlock { Text = "Enable parallel game loop" },
        IsChecked = ((SampleGame)game).EnableParallelGameLoop,
        Margin = new Vector4F(4),
      };
      _parallelGameLoopCheckBox.Click += (s, e) => ToggleParallelGameLoop();
      panel.Children.Add(_parallelGameLoopCheckBox);
    }


    private void ToggleFrameRate()
    {
      var game = _services.GetInstance<Microsoft.Xna.Framework.Game>();
      game.IsFixedTimeStep = !game.IsFixedTimeStep;

      _frameRateCheckBox.IsChecked = game.IsFixedTimeStep;
    }


    private void ToggleVSync()
    {
      var graphicsDeviceManager = _services.GetInstance<GraphicsDeviceManager>();
      graphicsDeviceManager.SynchronizeWithVerticalRetrace = !graphicsDeviceManager.SynchronizeWithVerticalRetrace;
      graphicsDeviceManager.ApplyChanges();

      _vSyncCheckBox.IsChecked = graphicsDeviceManager.SynchronizeWithVerticalRetrace;
    }


    private void ToggleParallelGameLoop()
    {
      var game = _services.GetInstance<SampleGame>();
      game.EnableParallelGameLoop = !game.EnableParallelGameLoop;

      _parallelGameLoopCheckBox.IsChecked = game.EnableParallelGameLoop;
    }
  }
}
