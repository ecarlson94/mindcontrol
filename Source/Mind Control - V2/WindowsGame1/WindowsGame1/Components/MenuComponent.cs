﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using WindowsGame1.StackPanels;
using WindowsGame1.Windows;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1.Components
{
    public class MenuComponent : BaseComponent
    {
        private readonly DelegateGraphicsScreen _graphicsScreen;
        private ContentManager _uiContentManager;
        private UIScreen _uiScreen;

        public MenuComponent(Game game, EmoEngineManager emoEngine)
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

            _uiScreen.Children.Add(new ControlPanel(EmoEngine));
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
            _uiScreen.Children.Add(newWindow);
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