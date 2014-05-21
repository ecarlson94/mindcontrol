using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using WindowsGame1.Managers;

namespace WindowsGame1.Components
{
    public class LoadingComponent : BaseComponent
    {
        private readonly DelegateGraphicsScreen _graphicsScreen;
        private ContentManager _uiContentManager;
        private UIScreen _uiScreen;

        public LoadingComponent(Game game, EmoEngineManager emoEngine)
            : base(game, emoEngine)
        {
            EnableMouseCentering = false;

            _graphicsScreen = new DelegateGraphicsScreen(GraphicsService)
            {
                RenderCallback = Render,
            };
            GraphicsService.Screens.Insert(0, _graphicsScreen);

            CreateGUI();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Remove UIScreen from UIService
                UIService.Screens.Remove(_uiScreen);

                //Dispose the current ContentManager
                _uiContentManager.Dispose();

                // Remove graphics screen from graphics service
                GraphicsService.Screens.Remove(_graphicsScreen);
            }
            base.Dispose(disposing);
        }

        private void CreateGUI()
        {
            // Dispose old UI.
            if (_uiContentManager != null)
            {
                _uiContentManager.Dispose();
                UIService.Screens.Remove(_uiScreen);
            }

            LoadTheme();

            // Show the text "Loading..." centered on the screen.
            TextBlock loadingTextBlock = new TextBlock
            {
                Name = "LoadingTextBlock",    // Control names are optional - but very helpful for debugging!
                Text = "Loading...",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _uiScreen.Children.Add(loadingTextBlock);
        }

        private void LoadTheme()
        {
            // Load a UI theme, which defines the appearance and default values of UI controls.
            _uiContentManager = new ContentManager(Game.Services, "NeoforceTheme");
            Theme theme = _uiContentManager.Load<Theme>("ThemeRed");
            // Create a UI renderer, which uses the theme info to renderer UI controls.
            UIRenderer renderer = new UIRenderer(Game, theme);

            // Create a UIScreen and add it to the UI service. The screen is the root of 
            // the tree of UI controls. Each screen can have its own renderer. 
            _uiScreen = new UIScreen("Default", renderer)
            {
                // Make the screen transparent.
                Background = new Color(0, 0, 0, 0),
            };

            // Add the screen to the UI service.
            UIService.Screens.Add(_uiScreen);
        }

        private void Render(RenderContext context)
        {
            _uiScreen.Draw(context.DeltaTime);
        }
    }
}