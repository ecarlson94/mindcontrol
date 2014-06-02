using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WindowsGame1.Enums;
using DigitalRune;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Graphics;
using Emotiv;
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

        public List<EdkDll.EE_CognitivAction_t> AllowedActions { get; private set; }
        public MenuState MenuState { get; private set; }

        //----------------------------------------------------------------------
        #region Creation and Cleanup

        public MenuComponent(Game game, EmoEngineManager emoEngine)
            : base(game, emoEngine)
        {
            EnableMouseCentering = false;
            MenuState = MenuState.Main;

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
                Window newWindow;
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
            if (_uiScreen.Children.Contains(_controlPanel) && _controlPanel.MenuState != MenuState.Main)
            {
                _uiScreen.Children.Remove(_controlPanel);

                var directionCheck = new DirectionCheckboxWindow(EmoEngine)
                {
                    IsModal = true,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                directionCheck.Closing += DirectionCheckOnClosing;
                _uiScreen.Children.Add(directionCheck);
            }
            base.Update(gameTime);
        }

        private void DirectionCheckOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            var actions = (sender as DirectionCheckboxWindow).AllowedActions;
            if (actions.Any())
            {
                //switch (_controlPanel.MenuState)
                //{
                //    case MenuState.Practice:
                //        Game.Components.Add(new VehicleComponent(Game, EmoEngine, actions));
                //        break;
                //    case MenuState.RCCar:
                //        Game.Components.Add(new RCCarComponent(Game, EmoEngine, actions));
                //        break;
                //}

                AllowedActions = actions;
                MenuState = _controlPanel.MenuState;
            }
            else
                cancelEventArgs.Cancel = true;
        }

        #endregion
    }
}