using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Graphics;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1.Screens
{
    public class UIGraphicsScreen : GraphicsScreen, IDisposable
    {
        private readonly IUIService _uiService;

        public UIScreen UIScreen { get; private set; }

        public UIGraphicsScreen(IServiceLocator services)
        {
            Name = "UI";

            _uiService = services.GetInstance<IUIService>();
            var contentManager = services.GetInstance<ContentManager>("UIContent");

            LoadTheme();
        }

        private void LoadTheme()
        {
            // Load a UI theme, which defines the appearance and default values of UI controls.
            Theme theme = UIContentManager.Load<Theme>("ThemeRed");
            // Create a UI renderer, which uses the theme info to renderer UI controls.
            UIRenderer renderer = new UIRenderer(Game, theme);

            // Create a UIScreen and add it to the UI service. The screen is the root of 
            // the tree of UI controls. Each screen can have its own renderer. 
            _uiScreen = new UIScreen("Default", renderer)
            {
                // Make the screen transparent.
                Background = Color.Transparent,
                ZIndex = int.MaxValue,
            };

            // Add the screen to the UI service.
            _uiService.Screens.Add(_uiScreen);
        }

        public void Dispose()
        {
            _uiService.Screens.Remove(UIScreen);
            UIScreen.Renderer.SafeDispose();
        }

        protected override void OnRender(RenderContext context)
        {
            UIScreen.Draw(context.DeltaTime);
        }
    }
}
