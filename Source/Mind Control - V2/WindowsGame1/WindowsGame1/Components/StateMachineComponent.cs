using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WindowsGame1.Enums;
using WindowsGame1.Managers;
using DigitalRune.Game.Input;
using DigitalRune.Game.States;
using DigitalRune.ServiceLocation;
using DigitalRune.Threading;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1.Components
{
    public class StateMachineComponent : GameComponent
    {
        // Services used by this game component.
        private readonly IInputService _inputService;
        private readonly EmoEngineManager _emoEngine;
        public static ContentManager Content;

        private StateMachine stateMachine;

        public static GameState gameState;

        public StateMachineComponent(Game game) : base(game)
        {
            // Get the required services from the game's service provider.
            var services = (ServiceContainer)ServiceLocator.Current;
            _inputService = services.GetInstance<IInputService>();
            Content = Game.Content;
            _emoEngine = new EmoEngineManager();
            _emoEngine.StartEmoEngine();
        }

        //----------------------------------------------------------------------
        #region State-Based Delegates

        void menuState_Update(object sender, StateEventArgs e)
        {
            if (!_emoEngine.DonglePluggedIn)
            {
                gameState = GameState.Start;
                stateMachine.States.ActiveState.Transitions["MenuToStart"].Fire();
            }
        }

        void menuState_Enter(object sender, StateEventArgs e)
        {
            Game.Components.Add(new MenuComponent(Game, _emoEngine));
        }

        void startState_Exit(object sender, StateEventArgs e)
        {
            RemoveBaseComponents();
        }

        void startState_Update(object sender, StateEventArgs e)
        {
            if (_emoEngine.DonglePluggedIn)
            {
                if (!_inputService.GetLogicalPlayer(LogicalPlayerIndex.One).HasValue)
                {
                    _inputService.SetLogicalPlayer(LogicalPlayerIndex.One, PlayerIndex.One);
                }
                gameState = GameState.InGame;
            }

            if (gameState == GameState.InGame)
            {
                stateMachine.States.ActiveState.Transitions["StartToMenu"].Fire();
            }
        }

        void startState_Enter(object sender, StateEventArgs e)
        {
            RemoveBaseComponents();
            Game.Components.Add(new StartComponent(Game, _emoEngine));
        }

        /// <summary>
        /// Called when "Loading" state is entered.
        /// </summary>
        private void OnEnterLoadingScreen(object sender, StateEventArgs eventArgs)
        {
            //Add the loading screen
            Game.Components.Add(new LoadingComponent(Game, _emoEngine));

            // Start loading assets in the background.
            Parallel.StartBackground(LoadAssets);
        }

        /// <summary>
        /// Called when "Loading" state is exited.
        /// </summary>
        private void OnExitLoadingScreen(object sender, StateEventArgs eventArgs)
        {
            //remove all BaseComponents here
            RemoveBaseComponents();
        }

        #endregion

        //----------------------------------------------------------------------
        #region Private Methods

        private void RemoveBaseComponents()
        {
            var baseComponents = Game.Components.OfType<BaseComponent>().ToArray();

            foreach (BaseComponent component in baseComponents)
            {
                Game.Components.Remove(component);
                component.Dispose();
            }
        }

        private void LoadAssets()
        {
            // To simulate a loading process we simply wait for 1 seconds.
            Thread.Sleep(TimeSpan.FromSeconds(1));
            gameState = GameState.Start;
        }

        #endregion

        //----------------------------------------------------------------------
        #region Overridden Methods

        public override void Update(GameTime gameTime)
        {
            stateMachine.Update(gameTime.ElapsedGameTime);
        }

        public override void Initialize()
        {
            base.Initialize();
            stateMachine = new StateMachine();

            var loadingState = new State { Name = "Loading" };
            loadingState.Enter += OnEnterLoadingScreen;
            loadingState.Exit += OnExitLoadingScreen;

            stateMachine.States.Add(loadingState);

            var startState = new State { Name = "Start" };
            startState.Enter += startState_Enter;
            startState.Update += startState_Update;
            startState.Exit += startState_Exit;
            stateMachine.States.Add(startState);

            var menuState = new State { Name = "Menu" };
            menuState.Enter += menuState_Enter;
            menuState.Update += menuState_Update;
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
                Guard = () => gameState == GameState.Start,
            };
            loadingState.Transitions.Add(loadingToStartTransition);
        }

        #endregion
    }
}
