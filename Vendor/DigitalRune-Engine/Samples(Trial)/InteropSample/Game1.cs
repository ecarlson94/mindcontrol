using System;
using System.Windows.Forms;
using DigitalRune.Graphics;
using DigitalRune.Graphics.Interop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;


namespace InteropSample
{
  public enum GameMode
  {
    Windowed,
    WinForm,
    WpfWindow,
    FullScreen,
  }


  public class Game1 : Game
  {
    // The XNA GraphicsDeviceManager.
    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    // The DigitalRune GraphicsManager.
    private GraphicsManager _graphicsManager;

    // The standard XNA window.
    private Form _gameForm;

    // A custom Windows Forms window (containing two presentation targets).
    private WinForm _winForm;

    // A custom WPF window (containing two presentation targets).
    private WpfWindow _wpfWindow;

    private GameMode _mode;


    static Game1()
    {
      // ----- License Keys -----
      // All license keys must be set before any function of a DigitalRune library can be used.
      // If you have received several license keys, call AddSerialNumber for each license key.
      //DigitalRune.Licensing.AddSerialNumber("A4DsypZMsBsI8xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
    }


    public Game1()
    {
      _graphicsDeviceManager = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
    }


    protected override void Initialize()
    {
      base.Initialize();

      // Get the standard XNA window.
      _gameForm = Control.FromHandle(Window.Handle) as Form;

      // Create the DigitalRune GraphicsManager.
      _graphicsManager = new GraphicsManager(GraphicsDevice, Window, new ContentManager(Services, "Content"));

      // Add graphics service to service provider
      Services.AddService(typeof(IGraphicsService), _graphicsManager);

      // Add a few GraphicsScreens that draw stuff.
      _graphicsManager.Screens.Add(new BackgroundGraphicsScreen(_graphicsManager));
      _graphicsManager.Screens.Add(new TriangleGraphicsScreen(_graphicsManager));
      _graphicsManager.Screens.Add(new TextGraphicsScreen(_graphicsManager, Content));
    }


    protected override void Update(GameTime gameTime)
    {
      // <Esc> --> Exit.
      if (Keyboard.GetState().IsKeyDown(Keys.Escape))
      {
        CloseWinForm();
        CloseWpfWindow();
        Exit();
      }

      ChangeMode();

      base.Update(gameTime);
    }


    // Changes the game mode if the users presses the keys <1> - <4>
    private void ChangeMode()
    {
      // Switching the game mode works very similar in all 4 cases:
      // Toggle the FullScreen mode using the graphics device manager as necessary.
      // We close any WinForm or WPF windows. The XNA game form is only hidden.
      // When switching to the normal Windowed or FullScreen mode, we have to set 
      // the back buffer size.      
      // Finally we show the desired window or switch to full screen. In some cases 
      // it is necessary to call Activate().

      if (Keyboard.GetState().IsKeyDown(Keys.D1) && _mode != GameMode.Windowed)
      {
        // <1> --> Windowed mode.
        _mode = GameMode.Windowed;
        CloseWinForm();
        CloseWpfWindow();
        _graphicsDeviceManager.PreferredBackBufferWidth = 800;
        _graphicsDeviceManager.PreferredBackBufferHeight = 600;
        _graphicsDeviceManager.ApplyChanges();
        if (_graphicsDeviceManager.IsFullScreen)
          _graphicsDeviceManager.ToggleFullScreen();

        _gameForm.Show();
        _gameForm.Activate();
      }
      else if (Keyboard.GetState().IsKeyDown(Keys.D2) && _mode != GameMode.WinForm)
      {
        // <2> --> WinForm mode.
        _mode = GameMode.WinForm;
        CloseWpfWindow();
        if (_graphicsDeviceManager.IsFullScreen)
          _graphicsDeviceManager.ToggleFullScreen();

        _gameForm.Hide();
        OpenWinForm();
        _winForm.Activate();
      }
      else if (Keyboard.GetState().IsKeyDown(Keys.D3) && _mode != GameMode.WpfWindow)
      {
        // <3> --> WPF window mode.
        _mode = GameMode.WpfWindow;
        if (_graphicsDeviceManager.IsFullScreen)
          _graphicsDeviceManager.ToggleFullScreen();

        OpenWpfWindow();

        // Note: Here we close the WinForm windows at last! If we close them before we open
        // the WPF window, then XNA could miss some input window messages (like WM_KEYUP).
        CloseWinForm();
        _gameForm.Hide();
      }
      else if (Keyboard.GetState().IsKeyDown(Keys.D4) && _mode != GameMode.FullScreen)
      {
        // <4> --> FullScreen
        _mode = GameMode.FullScreen;
        CloseWinForm();
        CloseWpfWindow();
        _graphicsDeviceManager.PreferredBackBufferWidth = 800;
        _graphicsDeviceManager.PreferredBackBufferHeight = 600;
        _graphicsDeviceManager.ApplyChanges();
        _gameForm.Show();
        if (!_graphicsDeviceManager.IsFullScreen)
          _graphicsDeviceManager.ToggleFullScreen();
      }
    }


    private void OpenWinForm()
    {
      // Create a new Windows Forms window.
      _winForm = new WinForm { GraphicsServices = _graphicsManager };

      // Optional: When the window closes, we want to exit the application.
      _winForm.FormClosing += OnWindowClosing;

      // Make the window visible.
      _winForm.Show();
    }


    private void OpenWpfWindow()
    {
      // WPF windows need to run on a separate thread. This thread is prepared by 
      // the WpfEnvironment class. (Redundant calls of Startup() are no problem.)
      WpfEnvironment.Startup(Window.Handle);

      // WPF windows must be created and handled on the WPF thread. From the WpfEnvironment 
      // class we can get the Dispatcher of the WPF thread. We can use it to execute actions
      // on the WPF thread. 
      WpfEnvironment.Dispatcher.Invoke((Action)(() =>
      {
        _wpfWindow = new WpfWindow { GraphicsService = _graphicsManager };
        _wpfWindow.Closing += OnWindowClosing;
        _wpfWindow.Show();
      }));
    }


    private void CloseWinForm()
    {
      if (_winForm != null)
      {
        // Close Windows Forms window.
        _winForm.FormClosing -= OnWindowClosing;  // Remove Closing event handler. (Otherwise, the event would exit the game.)
        _winForm.Close();
        _winForm = null;
      }
    }


    private void CloseWpfWindow()
    {
      if (_wpfWindow != null)
      {
        // Close WPF window. This must be executed on the WPF thread!
        WpfEnvironment.Dispatcher.Invoke((Action)(() =>
        {
          _wpfWindow.Closing -= OnWindowClosing;  // Remove Closing event handler. (Otherwise, the event would exit the game.)
          _wpfWindow.Close();
          _wpfWindow = null;
        }));
      }
    }


    private void OnWindowClosing(object sender, EventArgs eventArgs)
    {
      if (_gameForm.InvokeRequired)
      {
        // This method was called from the WPF thread. We want to execute Exit() 
        // on the thread of the XNA window.
        _gameForm.Invoke((Action)(Exit));
      }
      else
      {
        Exit();
      }
    }


    protected override void Draw(GameTime gameTime)
    {
      // Update graphics service (including the graphics screens).
      _graphicsManager.Update(gameTime.ElapsedGameTime);

      if (_graphicsManager.PresentationTargets.Count >= 2)
      {
        // Render the BackgroundGraphicsScreen and the TriangleGraphicsScreen into 
        // the first presentation target control.
        _graphicsManager.Screens[0].IsVisible = true;
        _graphicsManager.Screens[1].IsVisible = true;
        _graphicsManager.Screens[2].IsVisible = false;
        _graphicsManager.Render(_graphicsManager.PresentationTargets[0]);

        // Render the BackgroundGraphicsScreen and the TextGraphicsScreen into the 
        // second presentation target control.
        _graphicsManager.Screens[0].IsVisible = true;
        _graphicsManager.Screens[1].IsVisible = false;
        _graphicsManager.Screens[2].IsVisible = true;
        _graphicsManager.Render(_graphicsManager.PresentationTargets[1]);
      }

      // Render all graphics screens into the back buffer. XNA will automatically
      // present this in the normal XNA window.
      _graphicsManager.Screens[0].IsVisible = true;
      _graphicsManager.Screens[1].IsVisible = true;
      _graphicsManager.Screens[2].IsVisible = true;
      _graphicsManager.Render(false);

      // Call base.Draw() to draw DrawableGameComponents automatically.
      base.Draw(gameTime);
    }


    protected override void OnExiting(object sender, EventArgs args)
    {
      // When we exit the application, we must manually end the WPF thread. Otherwise, 
      // the WPF part continues to run and keeps the application alive.
      WpfEnvironment.Shutdown();

      base.OnExiting(sender, args);
    }
  }
}
