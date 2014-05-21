using WindowsGame1.Enums;
using DigitalRune;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Graphics;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using System;
using WindowsGame1.Managers;
using WindowsGame1.StackPanels;
using WindowsGame1.VehicleSimulation;
using WindowsGame1.Windows;

namespace WindowsGame1.Components
{
    public class MenuComponent : BaseComponent
    {
        //----------------------------------------------------------------------
        #region Fields

        private readonly DelegateGraphicsScreen _graphicsScreen;
        private UIScreen _uiScreen;
        private ControlPanel _controlPanel;

        #endregion

        //----------------------------------------------------------------------
        #region Creation and Cleanup

        public MenuComponent(Game game, EmoEngineManager emoEngine)
            : base(game, emoEngine)
        {
            RemoveBaseComponents();

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
                _uiScreen.SafeDispose();

                // Remove graphics screen from graphics service
                GraphicsService.Screens.Remove(_graphicsScreen);
                _graphicsScreen.SafeDispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        //----------------------------------------------------------------------
        #region Private Methods

        private void CreateGUI()
        {
            LoadTheme();

            if (EmoEngine.Profile == String.Empty)
            {
                Window newWindow = null;
                string[] profileNames = EmoEngine.GetProfileNames();
                if (profileNames.Length == 0)
                {
                    newWindow = new CreateUser(EmoEngine);
                }
                else
                {
                    newWindow = new LoadUser(EmoEngine);
                }
                newWindow.Closing += (s, e) =>
                {
                    _controlPanel = new ControlPanel(EmoEngine, Services);
                    _uiScreen.Children.Add(_controlPanel);
                };
                _uiScreen.Children.Add(newWindow);
            }
            else
            {
                _controlPanel = new ControlPanel(EmoEngine, Services);
                _uiScreen.Children.Add(_controlPanel);
            }
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
                Background = new Color(0, 0, 0, 0),
            };

            // Add the screen to the UI service.
            UIService.Screens.Add(_uiScreen);
        }

        private void Render(RenderContext context)
        {
            _uiScreen.Draw(context.DeltaTime);
        }

        #endregion

        //----------------------------------------------------------------------
        #region Overridden Methods

        public override void Update(GameTime gameTime)
        {
            if (_controlPanel != null)
            {
                if (_controlPanel.MenuState == MenuState.Practice)
                {
                    Game.Components.Add(new VehicleComponent(Game, EmoEngine));
                }
                else if (_controlPanel.MenuState == MenuState.RCCar)
                {
                    Game.Components.Add(new RCCarComponent(Game, EmoEngine));
                }
            }
            base.Update(gameTime);
        }

        #endregion
    }
}