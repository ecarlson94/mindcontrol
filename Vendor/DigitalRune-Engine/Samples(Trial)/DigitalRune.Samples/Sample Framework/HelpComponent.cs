using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Graphics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace DigitalRune.Samples
{
  // This XNA GameComponent displays help text in a separate graphics screen.
  public class HelpComponent : GameComponent
  {
    private readonly IInputService _inputService;
    private readonly IGraphicsService _graphicsService;
    private readonly IGameObjectService _gameObjectService;
    private readonly SampleGraphicsScreen _graphicsScreen;
    private bool _updateText = true;


    public HelpComponent(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      var services = ServiceLocator.Current;
      _inputService = services.GetInstance<IInputService>();
      _graphicsService = services.GetInstance<IGraphicsService>();
      _gameObjectService = services.GetInstance<IGameObjectService>();

      // Add a new graphics screen to the graphics service.
      _graphicsScreen = new SampleGraphicsScreen(services);
      _graphicsService.Screens.Add(_graphicsScreen);

      game.Components.ComponentAdded += OnGameComponentAdded;
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clean up.
        _graphicsService.Screens.Remove(_graphicsScreen);
        _graphicsScreen.Dispose();

        Game.Components.ComponentAdded -= OnGameComponentAdded;
      }

      base.Dispose(disposing);
    }


    private void OnGameComponentAdded(object sender, GameComponentCollectionEventArgs eventArgs)
    {
      _updateText = true;
    }


    public override void Update(GameTime gameTime)
    {
      if (_updateText
          || _inputService.IsPressed(Keys.F1, false)
          || _inputService.IsPressed(Buttons.LeftStick, false, LogicalPlayerIndex.Any)
          || _inputService.IsReleased(Keys.F1)
          || _inputService.IsReleased(Buttons.LeftStick, LogicalPlayerIndex.Any))
      {
        _updateText = false;

        _graphicsScreen.ClearBackground = false;
        var debugRenderer = _graphicsScreen.DebugRenderer2D;
        debugRenderer.Clear();

        // Get the sample name and description from the game components.
        string sampleName;
        SampleAttribute sampleAttribute;
        if (!FindSample(Game.Components, out sampleName, out sampleAttribute))
          return;

        if (_inputService.IsDown(Keys.F1) || _inputService.IsDown(Buttons.LeftStick, LogicalPlayerIndex.Any))
        {
          // ----- Show complete help info.
          _graphicsScreen.ClearBackground = true;

          // Create help text for the sample.
          debugRenderer.DrawText(
            "HELP" +
            "\n" +
            "\nName: " + sampleName +
            "\nCategory: " + sampleAttribute.Category +
            "\n" +
            "\nDescription:" +
            "\n--------------" +
            "\n" + sampleAttribute.Summary);

          if (!string.IsNullOrEmpty(sampleAttribute.Description))
            debugRenderer.DrawText("\n" + sampleAttribute.Description);

          // General controls.
          debugRenderer.DrawText(
            "\nControls:" +
            "\n-----------" +
            "\nGeneral" +
            "\n  Double-click <Esc> or <GamePad Back> to exit." +
            "\n  Hold <F1> or <Left Stick> to show help." +
            "\n  Hold <F4> to show profiling data (and also reset the data)." +
            "\n  Hold <Alt> to show menu." +
            "\n  Press <PageUp/Down> or <DPad Left/Right> to switch samples." +
            "\n  Press <Keyboard End> or <Left Shoulder>+<Right Shoulder> to restart sample." +
            "\n  Press <P> to pause/resume physics and particle simulation." +
            "\n  Press <T> to single step physics and particle simulation when simulation is paused.");

          // Controls of game components.
          foreach (var gameComponent in Game.Components)
          {
            foreach (var controlsAttribute in ControlsAttribute.GetControlsAttribute(gameComponent.GetType()))
            {
              debugRenderer.DrawText("");
              debugRenderer.DrawText(controlsAttribute.Description);
            }
          }

          // Controls of game objects.
          foreach (var gameObject in _gameObjectService.Objects)
          {
            foreach (var controlsAttribute in ControlsAttribute.GetControlsAttribute(gameObject.GetType()))
            {
              debugRenderer.DrawText("");
              debugRenderer.DrawText(controlsAttribute.Description);
            }
          }
        }
        else
        {
          // ----- Show short help info.
          debugRenderer.DrawText(sampleAttribute.Category + " - " + sampleName);
          debugRenderer.DrawText("Hold <F1> or <Left Stick> to show help.", new Vector2F(10, 720 - 22), Color.Black);
        }
      }
    }


    // Look for a game component marked with a SampleAttribute.
    private static bool FindSample(GameComponentCollection gameComponents, out string sampleName, out SampleAttribute sampleAttribute)
    {
      foreach (var gameComponent in gameComponents)
      {
        var type = gameComponent.GetType();
        sampleAttribute = SampleAttribute.GetSampleAttribute(type);
        if (sampleAttribute != null)
        {
          // Sample found.
          sampleName = type.Name;
          return true;
        }
      }

      // No sample found.
      sampleName = null;
      sampleAttribute = null;
      return false;
    }
  }
}
