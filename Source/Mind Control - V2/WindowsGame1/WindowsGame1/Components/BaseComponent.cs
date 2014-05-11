using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Animation;
using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using DigitalRune.Graphics;
using DigitalRune.Physics;
using DigitalRune.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Practices.ServiceLocation;

namespace WindowsGame1.Components
{
    public class BaseComponent : GameComponent
    {
        // Services to be used in derived classes
        protected readonly ServiceContainer Services;
        protected readonly ContentManager ContentManager;
        protected readonly ContentManager UIContentManager;
        protected readonly IInputService InputService;
        protected readonly Simulation Simulation;
        protected readonly IGraphicsService GraphicsService;
        protected readonly IGameObjectService GameObjectService;
        protected readonly IAnimationService AnimationService;
        protected readonly IUIService UIService;

        // Centers and hides the mouse
        public bool EnableMouseCentering
        {
            get
            {
                var mouseComponent = Game.Components.OfType<MouseComponent>().FirstOrDefault();
                bool enableMouseCentering = false;
                if (mouseComponent != null)
                    enableMouseCentering = mouseComponent.EnableMouseCentering;

                return enableMouseCentering;
            }
            set
            {
                var mouseComponent = Game.Components.OfType<MouseComponent>().FirstOrDefault();
                if (mouseComponent != null)
                    mouseComponent.EnableMouseCentering = value;
            }
        }

        protected BaseComponent(Game game) : base(game)
        {
            EnableMouseCentering = true;

            var services = (ServiceContainer)ServiceLocator.Current;
            ContentManager = services.GetInstance<ContentManager>();
            UIContentManager = services.GetInstance<ContentManager>("UIContent");
            InputService = services.GetInstance<IInputService>();
            AnimationService = services.GetInstance<IAnimationService>();
            Simulation = services.GetInstance<Simulation>();
            GraphicsService = services.GetInstance<IGraphicsService>();
            GameObjectService = services.GetInstance<IGameObjectService>();
            UIService = services.GetInstance<IUIService>();

            Services = services.CreateChildContainer();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GameObjectService.Objects.Clear();
                //TODO: make sure to come back and fix this
                //((SampleGame)Game).ResetPhysicsSimulation();
                Services.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
