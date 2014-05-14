using System;
using WindowsGame1.Components;
using WindowsGame1.Managers;
using DigitalRune.Animation;
using DigitalRune.Diagnostics;
using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using DigitalRune.Geometry.Collisions;
using DigitalRune.Geometry.Partitioning;
using DigitalRune.Graphics;
using DigitalRune.Particles;
using DigitalRune.Physics;
using DigitalRune.ServiceLocation;
using DigitalRune.Threading;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    public class MindControl : Game
    {
        //----------------------------------------------------------------------
        #region Fields

        private readonly GraphicsDeviceManager _graphicsDevice;

        private ServiceContainer _services;

        private InputManager _inputManager;
        private GraphicsManager _graphicsManager;
        private UIManager _uiManager;
        private AnimationManager _animationManager;
        private Simulation _simulation;
        private GameObjectManager _gameObjectManager;
        private EmoEngineManager _emoEngine;

        //update animation and physics
        private Action _updateAnimation;
        private Action _updatePhysics;
        private Task _updateAnimationTask;
        private Task _updatePhysicsTask;

        //allows user to pause a simulation
        private bool _isSimulationPaused;

        private TimeSpan _deltaTime;

        #endregion

        //----------------------------------------------------------------------
        #region Properties
        //TBD enable parallel update of game services
        public bool EnableParallelGameLoop { get; set; }

        #endregion

        //----------------------------------------------------------------------
        #region Creation
        static MindControl()
        {
            // ----- License Keys -----
            DigitalRune.Licensing.AddSerialNumber("tgCYABO7/bPuX88BoL1Y979+0AEnACNNaWNoYWVsIEVyaWMgQ2FybHNvbiMxIzEjTm9uQ29tbWVyY2lhbECDssOQzVFUBAMB/A8+nkxZKRoeFeU6DBZQwPbqzcXgDShq/mWxgXiyJGaY+yVQ7+BoDaxXv7Z2cR1MSfw=");
        }

        public MindControl()
        {
            _graphicsDevice = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                PreferMultiSampling = false,
                SynchronizeWithVerticalRetrace = true,
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            IsFixedTimeStep = false;
            _emoEngine = new EmoEngineManager();
            _emoEngine.StartEmoEngine();
        }

        protected override void Initialize()
        {
            _services = new ServiceContainer();

            ServiceLocator.SetLocatorProvider(() => (IServiceLocator)_services);

            _services.Register(typeof(IGraphicsDeviceService), null, _graphicsDevice);
            _services.Register(typeof(GraphicsDeviceManager), null, _graphicsDevice);

            // Default, shared content manager
            Content = new ContentManager(_services, "Content");
            _services.Register(typeof(ContentManager), null, Content);

            // Content manager to load the GUI.
            var uiContentManager = new ContentManager(_services, "NeoforceTheme");
            _services.Register(typeof(ContentManager), "UIContent", uiContentManager);

            var graphicsContentManager = new ContentManager(_services, "Content");

            //Register the game class.
            _services.Register(typeof(Microsoft.Xna.Framework.Game), null, this);
            _services.Register(typeof(MindControl), null, this);

            //Input
            _inputManager = new InputManager(false);
            _services.Register(typeof(IInputService), null, _inputManager);

            //Graphics
            _graphicsManager = new GraphicsManager(GraphicsDevice, Window, graphicsContentManager);
            _services.Register(typeof(IGraphicsService), null, _graphicsManager);


            //GUI
            _uiManager = new UIManager(this, _inputManager);
            _services.Register(typeof(IUIService), null, _uiManager);


            //Animation
            _animationManager = new AnimationManager();
            _services.Register(typeof(IAnimationService), null, _animationManager);

            //Physics Simulation
            ResetPhysicsSimulation();

            //Game logic
            _gameObjectManager = new GameObjectManager();
            _services.Register(typeof(IGameObjectService), null, _gameObjectManager);

            _updateAnimation = () => _animationManager.Update(_deltaTime);
            _updatePhysics = () => _simulation.Update(_deltaTime);

            //MouseComponent to handle mouse centering and the cursor;
            Components.Add(new MouseComponent(this));

            //MainComponent handles state and flow of Game
            Components.Add(new StateMachineComponent(this));


            base.Initialize();
        }

        #endregion

        //----------------------------------------------------------------------
        #region Public Methods
        public void ResetPhysicsSimulation()
        {
            _simulation = new Simulation();

            _simulation.Settings.Timing.MaxNumberOfSteps = 2;

            _simulation.CollisionDomain.BroadPhase.Filter = new DelegatePairFilter<CollisionObject>(
                pair =>
                {
                    bool isPair = false;
                    var bodyA = pair.First.GeometricObject as RigidBody;
                    if (bodyA == null || bodyA.MotionType != MotionType.Static)
                        isPair = true;

                    var bodyB = pair.Second.GeometricObject as RigidBody;
                    if (bodyB == null || bodyB.MotionType != MotionType.Static)
                        isPair = true;

                    return isPair;
                });

            var filter = (ICollisionFilter) _simulation.CollisionDomain.CollisionDetection.CollisionFilter;
            filter.Set(1, 2, false);
            _services.Register(typeof(Simulation), null, _simulation);
        }

        #endregion

        //----------------------------------------------------------------------
        #region Overridden Methods
        protected override void Update(GameTime gameTime)
        {
            _deltaTime = gameTime.ElapsedGameTime;

            _inputManager.Update(_deltaTime);

            if (EnableParallelGameLoop)
            {
                _updateAnimationTask.Wait();
                _updatePhysicsTask.Wait();

                _animationManager.ApplyAnimations();
            }
            else
            {
                if (_inputManager.IsPressed(Keys.P, true))
                    _isSimulationPaused = !_isSimulationPaused;

                if (!_isSimulationPaused)
                {
                    // Update the physics simulation.
                    _simulation.Update(_deltaTime);
                }

                _animationManager.Update(_deltaTime);

                _animationManager.ApplyAnimations();
            }

            Parallel.RunCallbacks();
            
            base.Update(gameTime);

            _uiManager.Update(_deltaTime);

            _gameObjectManager.Update(_deltaTime);

            if (EnableParallelGameLoop)
            {
                _updateAnimationTask = Parallel.Start(_updateAnimation);
                _updatePhysicsTask = Parallel.Start(_updatePhysics);
            }
        }

        //Draws all game content
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(50, 50, 50));
            //Load DrawableGameComponents registered in the Components
            //of the base Game class
            base.Draw(gameTime);

            //update all graphics (including graphics screens
            _graphicsManager.Update(gameTime.ElapsedGameTime);

            _graphicsManager.Render(false);
        }

        #endregion
    }
}