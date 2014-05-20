using DigitalRune.Game.Input;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1.Components
{
    public class MouseComponent : GameComponent
    {
        private readonly IInputService _inputService;

        private bool _enableMouseCentering;
        public bool EnableMouseCentering
        {
            get { return _enableMouseCentering; }
            set{
                _enableMouseCentering = value;
                Game.IsMouseVisible = !value;
            }
        }

        public MouseComponent(Game game) : base(game)
        {
            _inputService = ServiceLocator.Current.GetInstance<IInputService>();

            var presentationParameters = Game.GraphicsDevice.PresentationParameters;
            _inputService.Settings.MouseCenter = new Vector2F(
                presentationParameters.BackBufferWidth / 2.0f,
                presentationParameters.BackBufferHeight / 2.0f);

            EnableMouseCentering = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                EnableMouseCentering = false;
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            bool isAltDown = _inputService.IsDown(Keys.LeftAlt) || _inputService.IsDown(Keys.RightAlt);
            bool isControlDown = _inputService.IsDown(Keys.LeftControl) || _inputService.IsDown(Keys.RightControl);
            bool isMouseVisible = isControlDown || isAltDown || !Game.IsActive;

            _inputService.EnableMouseCentering = _enableMouseCentering && !isMouseVisible;
            Game.IsMouseVisible = !_enableMouseCentering || isMouseVisible;
        }
    }
}
