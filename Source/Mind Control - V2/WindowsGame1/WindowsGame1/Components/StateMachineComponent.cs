using DigitalRune.Game.Input;
using DigitalRune.Game.States;
using DigitalRune.ServiceLocation;
using DigitalRune.Threading;
using Emotiv;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Threading;
using WindowsGame1.Enums;
using WindowsGame1.Managers;
using WindowsGame1.VehicleSimulation;

namespace WindowsGame1.Components
{
    public class StateMachineComponent : GameComponent
    {
        // Services used by this game component.
        private readonly IInputService _inputService;
        private readonly EmoEngineManager _emoEngine;

        private StateMachine stateMachine;

        public static GameState gameState;

        private BaseComponent currentComponent;
        private BaseComponent previousComponent;

        public StateMachineComponent(Game game) : base(game)
        {
            // Get the required services from the game's service provider.
            var services = (ServiceContainer)ServiceLocator.Current;
            _inputService = services.GetInstance<IInputService>();
            _emoEngine = new EmoEngineManager();
            _emoEngine.StartEmoEngine();
        }

        //----------------------------------------------------------------------
        #region State-Based Delegates

        private void RcStateOnUpdate(object sender, StateEventArgs stateEventArgs)
        {
            if (((RCCarComponent) currentComponent).Exit)
                stateMachine.States.ActiveState.Transitions["RCToMenu"].Fire();
        }

        private void RcStateOnEnter(object sender, StateEventArgs stateEventArgs)
        {
            Game.Components.Remove(currentComponent);
            currentComponent.Dispose();
            currentComponent = new RCCarComponent(Game, _emoEngine, ((MenuComponent)currentComponent).AllowedActions);
            Game.Components.Add(currentComponent);
        }

        private void PracticeStateOnUpdate(object sender, StateEventArgs stateEventArgs)
        {
            if (((VehicleComponent)currentComponent).Exit)
                stateMachine.States.ActiveState.Transitions["PracticeToMenu"].Fire();
        }

        private void PracticeStateOnEnter(object sender, StateEventArgs stateEventArgs)
        {
            Game.Components.Remove(currentComponent);
            currentComponent.Dispose();
            currentComponent = new VehicleComponent(Game, _emoEngine, ((MenuComponent)currentComponent).AllowedActions);
            Game.Components.Add(currentComponent);
        }

        void menuState_Update(object sender, StateEventArgs e)
        {
            if (!_emoEngine.DonglePluggedIn)
            {
                gameState = GameState.Start;
                stateMachine.States.ActiveState.Transitions["MenuToStart"].Fire();
            }
            else if (((MenuComponent)currentComponent).MenuState != MenuState.Main)
            {
                switch (((MenuComponent)currentComponent).MenuState)
                {
                    case MenuState.Practice:
                        stateMachine.States.ActiveState.Transitions["MenuToPractice"].Fire();
                        break;
                    case MenuState.RCCar:
                        stateMachine.States.ActiveState.Transitions["MenuToRC"].Fire();
                        break;
                }
            }
        }

        void menuState_Enter(object sender, StateEventArgs e)
        {
            Game.Components.Remove(currentComponent);
            currentComponent.Dispose();
            currentComponent = new MenuComponent(Game, _emoEngine);
            Game.Components.Add(currentComponent);
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
            Game.Components.Remove(currentComponent);
            currentComponent.Dispose();
            currentComponent = new StartComponent(Game, _emoEngine);
            Game.Components.Add(currentComponent);
        }

        /// <summary>
        /// Called when "Loading" state is entered.
        /// </summary>
        private void OnEnterLoadingScreen(object sender, StateEventArgs eventArgs)
        {
            //Add the loading screen
            Game.Components.Remove(currentComponent);
            currentComponent = new LoadingComponent(Game, _emoEngine);
            Game.Components.Add(currentComponent);

            // Start loading assets in the background.
            Parallel.StartBackground(LoadAssets);
        }

        #endregion

        //----------------------------------------------------------------------
        #region Private Methods

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
            stateMachine.States.Add(loadingState);

            var startState = new State { Name = "Start" };
            startState.Enter += startState_Enter;
            startState.Update += startState_Update;
            stateMachine.States.Add(startState);

            var menuState = new State { Name = "Menu" };
            menuState.Enter += menuState_Enter;
            menuState.Update += menuState_Update;
            stateMachine.States.Add(menuState);

            var practiceState = new State {Name = "Practice"};
            practiceState.Enter += PracticeStateOnEnter;
            practiceState.Update += PracticeStateOnUpdate;
            stateMachine.States.Add(practiceState);

            var rcState = new State {Name = "RC"};
            rcState.Enter += RcStateOnEnter;
            rcState.Update += RcStateOnUpdate;
            stateMachine.States.Add(rcState);

            var menuToStartTransition = new Transition
            {
                Name = "MenuToStart",
                TargetState = startState,
            };
            menuState.Transitions.Add(menuToStartTransition);

            var menuToRCTransition = new Transition
            {
                Name = "MenuToRC",
                TargetState = rcState,
            };
            menuState.Transitions.Add(menuToRCTransition);

            var menuToPracticeTransition = new Transition
            {
                Name = "MenuToPractice",
                TargetState = practiceState,
            };
            menuState.Transitions.Add(menuToPracticeTransition);

            var rcToMenuTransition = new Transition
            {
                Name = "RCToMenu",
                TargetState = menuState,
            };
            rcState.Transitions.Add(rcToMenuTransition);

            var practiceToMenuTransition = new Transition
            {
                Name = "PracticeToMenu",
                TargetState = menuState,
            };
            practiceState.Transitions.Add(practiceToMenuTransition);

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