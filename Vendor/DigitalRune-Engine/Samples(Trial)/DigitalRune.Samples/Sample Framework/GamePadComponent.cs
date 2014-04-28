using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace DigitalRune.Samples
{
  // This XNA GameComponent controls which gamepad is used for input.
  // To use a gamepad, the user has to press <Start> on the gamepad. This gamepad
  // is then assigned to a LogicalPlayerIndex. If the connection to the gamepad is
  // lost, the LogicalPlayerIndex assignment is removed. 
  // An info text about the gamepad state is shown in the "Default" UIScreen.
  public class GamePadComponent : GameComponent
  {
    private readonly IInputService _inputService;
#if !WINDOWS_PHONE
    private readonly TextBlock _textBlock;
    private bool _controllerIsConnected;
#endif


    public GamePadComponent(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      var services = ServiceLocator.Current;
      _inputService = services.GetInstance<IInputService>();

#if !WINDOWS_PHONE
      // Get the UI service and get a UIScreen.
      var uiService = services.GetInstance<IUIService>();
      var uiScreen = uiService.Screens["Default"];

      // Add a text block to the UI screen.
      _textBlock = new TextBlock
      {
        Text = "Controller disconnected. Press <Start> to use controller.",
        X = 10,
        Y = 680,
      };
      uiScreen.Children.Add(_textBlock);
#else
      // Set logical player 1 to first gamepad. On WP7 the first gamepad is used 
      // to check the Windows Phone's Back button.
      _inputService.SetLogicalPlayer(LogicalPlayerIndex.One, PlayerIndex.One);
#endif
    }


#if !WINDOWS_PHONE
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Remove text block from UI screen.
        _textBlock.Screen.Children.Remove(_textBlock);
      }

      base.Dispose(disposing);
    }


    public override void Update(GameTime gameTime)
    {
      // Check if controller is connected.
      _controllerIsConnected = _inputService.GetGamePadState(LogicalPlayerIndex.One).IsConnected;
      if (!_controllerIsConnected)
      {
        // No controller assigned to LogicalPlayerIndex.One or controller was 
        // disconnected.
        _textBlock.IsVisible = true;

        // Reset the logical player assignment. 
        _inputService.SetLogicalPlayer(LogicalPlayerIndex.One, null);

        // Check if the user presses <Start> on any connected gamepad and assign 
        // a new controller to the first logical player.
        for (var controller = PlayerIndex.One; controller <= PlayerIndex.Four; controller++)
        {
          if (_inputService.IsDown(Buttons.Start, controller))
          {
            _inputService.SetGamePadHandled(controller, true);
            _inputService.SetLogicalPlayer(LogicalPlayerIndex.One, controller);
          }
        }
      }
      else
      {
        _textBlock.IsVisible = false;
      }
    }
#endif
  }
}
