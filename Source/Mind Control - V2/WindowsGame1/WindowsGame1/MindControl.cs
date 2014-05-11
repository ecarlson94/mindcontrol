using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using DigitalRune.Animation;
using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using DigitalRune.Graphics;
using DigitalRune.Physics;
using DigitalRune.ServiceLocation;
using DigitalRune.Threading;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    public class MindControl : Game
    {
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

        private TimeSpan _deltaTime;

        //TBD enable parallel update of game services
        //public bool EnableParallelGameLoop { get; set; }

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
            IsMouseVisible = true;
            _emoEngine = new EmoEngineManager();
            _emoEngine.StartEmoEngine();
        }

        protected override void Initialize()
        {
            _services = new ServiceContainer();

            ServiceLocator.SetLocatorProvider(() => (IServiceLocator)_services);

            _services.Register(typeof(IGraphicsDeviceService), null, GraphicsDevice);
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
            



            base.Initialize();
        }
    }
}
