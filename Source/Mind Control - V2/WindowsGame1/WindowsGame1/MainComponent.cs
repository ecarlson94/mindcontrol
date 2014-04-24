﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using WindowsGame1.Managers;
using WindowsGame1.Windows;
using DigitalRune.Animation;
using DigitalRune.Game.Input;
using DigitalRune.Game.States;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    public class MainComponent : DrawableGameComponent
    {
        // Services used by this game component.
        private readonly IInputService _inputService;
        private readonly IUIService _uiService;
        private readonly IAnimationService _animationService;
        private readonly EmoEngineManager _emoEngine;

        private StateMachine stateMachine;

        // The UI screen renders our controls, such as text labels, buttons, etc.
        private UIScreen _screen;

        public MainComponent(Game game)
            : base(game)
        {
            // Get the required services from the game's service provider.
            _inputService = (IInputService)game.Services.GetService(typeof(IInputService));
            _uiService = (IUIService)game.Services.GetService(typeof(IUIService));
            _animationService = (IAnimationService)game.Services.GetService(typeof(IAnimationService));
            _emoEngine = new EmoEngineManager();
            _emoEngine.StartEmoEngine();
        }

        public override void Initialize()
        {
            base.Initialize();
            stateMachine = new StateMachine();

            var loadingState = new State {Name = "Loading"};
            loadingState.Enter += OnEnterLoadingScreen;
            loadingState.Exit += OnExitLoadingScreen;

            stateMachine.States.Add(loadingState);

            var startState = new State {Name = "Start"};
            startState.Enter += startState_Enter;
            startState.Update += startState_Update;
            startState.Exit += startState_Exit;
            stateMachine.States.Add(startState);

            var menuState = new State{Name = "Menu"};
            menuState.Enter += menuState_Enter;
            menuState.Update += menuState_Update;
            menuState.Exit += menuState_Exit;
            stateMachine.States.Add(menuState);

            var menuToStartTransition = new Transition
            {
                Name = "MenuToStart",
                TargetState = startState,
            };
            menuState.Transitions.Add(menuToStartTransition);

            var startToMenuTransition = new Transition
            {
                Name = "StartToMenu",
                TargetState = menuState,
            };
            startState.Transitions.Add(startToMenuTransition);

            var loadingToStartTransition = new Transition
            {
                Name = "LoadingToStart",
                TargetState = startState,
                FireAlways = true,
                Guard = () => allAssetsLoaded,
            };
            loadingState.Transitions.Add(loadingToStartTransition);

        }

        void menuState_Exit(object sender, StateEventArgs e)
        {
            _screen.Children.Clear();
        }

        void menuState_Update(object sender, StateEventArgs e)
        {
            if (!_emoEngine.DonglePluggedIn)
            {
                stateMachine.States.ActiveState.Transitions["MenuToStart"].Fire();
            }
        }

        void menuState_Enter(object sender, StateEventArgs e)
        {
            Window newWindow = null;
            string[] profileNames = _emoEngine.GetProfileNames();
            if (profileNames.Length == 0)
            {
                newWindow = new CreateUser(_emoEngine);
            }
            else
            {
                newWindow = new LoadUser(_emoEngine);
            }
            _screen.Children.Add(newWindow);

            //menuWindow = new Window
            //{
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //};
            //_screen.Children.Add(menuWindow);

            //stackPanel = new StackPanel
            //{
            //    Orientation = Orientation.Vertical,
            //    HorizontalAlignment = HorizontalAlignment.Left,
            //    VerticalAlignment = VerticalAlignment.Bottom,
            //    Margin = new Vector4F(150, 0, 0, 200),
            //};
            //_screen.Children.Add(stackPanel);

            //var startButton = new Button
            //{
            //    Name = "StartButton",
            //    Content = new TextBlock { Text = "Start" },
            //    FocusWhenMouseOver = true,
            //};

            //var subMenu1Button = new Button
            //{
            //    Name = "SubMenu1Button",
            //    Content = new TextBlock { Text = "Sub-menu 1" },
            //    FocusWhenMouseOver = true,
            //};

            //var subMenu2Button = new Button
            //{
            //    Name = "SubMenu2Button",
            //    Content = new TextBlock { Text = "Sub-menu 2"},
            //    FocusWhenMouseOver = true,
            //};

            //var exitButton = new Button
            //{
            //    Name = "ExitButton",
            //    Content = new TextBlock { Text = "Exit" },
            //    FocusWhenMouseOver = true,
            //};

            //stackPanel.Children.Add(startButton);
            //stackPanel.Children.Add(subMenu1Button);
            //stackPanel.Children.Add(subMenu2Button);
            //stackPanel.Children.Add(exitButton);

            // By default, the first button should be selected.
            //startButton.Focus();
        }

        void startState_Exit(object sender, StateEventArgs e)
        {
            _screen.Children.Clear();
        }

        void startState_Update(object sender, StateEventArgs e)
        {
            bool transitionToMenu = false;
            if (_emoEngine.DonglePluggedIn)
            {
                if(!_inputService.GetLogicalPlayer(LogicalPlayerIndex.One).HasValue)
                {
                    _inputService.SetLogicalPlayer(LogicalPlayerIndex.One, PlayerIndex.One);
                }
                transitionToMenu = true;
            }

            if (transitionToMenu)
            {
                stateMachine.States.ActiveState.Transitions["StartToMenu"].Fire();
            }
        }

        
        void startState_Enter(object sender, StateEventArgs e)
        {
            TextBlock startTextBlock = new TextBlock
            {
                Name = "StartTextBlock",
                Text = "Plug in the Emotiv headset dongle...",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _screen.Children.Add(startTextBlock);
        }

        protected override void LoadContent()
        {
            // Load a UI theme, which defines the appearance and default values of UI controls.
            var content = new ContentManager(Game.Services, "NeoforceTheme");
            var theme = content.Load<Theme>("ThemeRed");
            // Create a UI renderer, which uses the theme info to renderer UI controls.
            var renderer = new UIRenderer(Game, theme);

            // Create a UIScreen and add it to the UI service. The screen is the root of 
            // the tree of UI controls. Each screen can have its own renderer. 
            _screen = new UIScreen("Default", renderer)
            {
                // Make the screen transparent.
                Background = new Color(0, 0, 0, 0),
            };

            // Add the screen to the UI service.
            _uiService.Screens.Add(_screen);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            stateMachine.Update(gameTime.ElapsedGameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Clear background.
            GraphicsDevice.Clear(new Color(50, 50, 50));

            // Draw the UI screen. 
            _screen.Draw(gameTime);
        }

          // Shows the text "Loading...".

        /// <summary>
        /// Called when "Loading" state is entered.
        /// </summary>
        private void OnEnterLoadingScreen(object sender, StateEventArgs eventArgs)
        {
            // Show the text "Loading..." centered on the screen.
            TextBlock loadingTextBlock = new TextBlock
            {
                Name = "LoadingTextBlock",    // Control names are optional - but very helpful for debugging!
                Text = "Loading...",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _screen.Children.Add(loadingTextBlock);

            // Start loading assets in the background.
            Parallel.StartBackground(LoadAssets);
        }

        private volatile bool allAssetsLoaded;
        private void LoadAssets()
        {
            // To simulate a loading process we simply wait for 2 seconds.
            Thread.Sleep(TimeSpan.FromSeconds(2));
            allAssetsLoaded = true;
        }

        /// <summary>
        /// Called when "Loading" state is exited.
        /// </summary>
        private void OnExitLoadingScreen(object sender, StateEventArgs eventArgs)
        {
            // Clean up.
            _screen.Children.Clear();
        }
    }
}
