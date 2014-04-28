using DigitalRune.Game.Input;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace DigitalRune.Samples
{
  // This XNA GameComponent controls mouse centering and the visibility of the 
  // mouse cursor.
  // Per default, the mouse cursor is invisible and "mouse centering" of the 
  // input service is used. This means, the mouse position is reset to a fixed
  // position each frame to avoid that the mouse movement is blocked by the 
  // screen borders. 
  // Mouse centering is disabled and the mouse is shown if <Alt> or <Ctrl> are
  // pressed or if the game is inactive (not in the foreground). 
  // <Alt> is used by the MenuComponent to show the menu. 
  // <Ctrl> can be used in samples.
  public class MouseComponent : GameComponent
  {
    private readonly IInputService _inputService;


    public bool EnableMouseCentering
    {
      get { return _enableMouseCentering; }
      set { _enableMouseCentering = value; }
    }
    private bool _enableMouseCentering;


    public MouseComponent(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      _inputService = ServiceLocator.Current.GetInstance<IInputService>();

#if !WINDOWS_PHONE
      // Set the mouse centering position to center of the game window.
      var presentationParameters = Game.GraphicsDevice.PresentationParameters;
      _inputService.Settings.MouseCenter = new Vector2F(
        presentationParameters.BackBufferWidth / 2.0f,
        presentationParameters.BackBufferHeight / 2.0f);

      // By default, enable mouse centering and hide mouse cursor.
      EnableMouseCentering = true;
#endif
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
        EnableMouseCentering = false;

      base.Dispose(disposing);
    }


    public override void Update(GameTime gameTime)
    {
#if !WINDOWS_PHONE
      bool isAltDown = _inputService.IsDown(Keys.LeftAlt) || _inputService.IsDown(Keys.RightAlt);
      bool isControlDown = _inputService.IsDown(Keys.LeftControl) || _inputService.IsDown(Keys.RightControl);
      bool isMouseVisible = isControlDown || isAltDown || !Game.IsActive;

      _inputService.EnableMouseCentering = _enableMouseCentering && !isMouseVisible;
      Game.IsMouseVisible = !_enableMouseCentering || isMouseVisible;
#endif
    }
  }
}
