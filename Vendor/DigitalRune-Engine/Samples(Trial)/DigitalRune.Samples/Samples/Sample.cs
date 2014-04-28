using System.Linq;
using DigitalRune.Animation;
using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using DigitalRune.Graphics;
using DigitalRune.Particles;
using DigitalRune.Physics;
using DigitalRune.ServiceLocation;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace DigitalRune.Samples
{
  // Samples in this solution are derived from the XNA class GameComponent. The
  // abstract class Sample can be used as the base class of samples. It provides
  // access to the game services (input, graphics, physics, etc.).
  // In addition, it creates a new ServiceContainer which can be used in samples.
  public abstract class Sample : GameComponent
  {
    // Services which can be used in derived classes.
    protected readonly ServiceContainer Services;
    protected readonly ContentManager ContentManager;
    protected readonly ContentManager UIContentManager;
    protected readonly IInputService InputService;
    protected readonly IAnimationService AnimationService;
    protected readonly Simulation Simulation;
    protected readonly IParticleSystemService ParticleSystemService;
    protected readonly IGraphicsService GraphicsService;
    protected readonly IGameObjectService GameObjectService;
    protected readonly IUIService UIService;


    // Enable/disable mouse centering and hide/show the mouse cursor.
    public bool EnableMouseCentering
    {
      get
      {
        var mouseComponent = Game.Components.OfType<MouseComponent>().FirstOrDefault();
        if (mouseComponent != null)
          return mouseComponent.EnableMouseCentering;

        return false;
      }
      set
      {
        var mouseComponent = Game.Components.OfType<MouseComponent>().FirstOrDefault();
        if (mouseComponent != null)
          mouseComponent.EnableMouseCentering = value;
      }
    }


    protected Sample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Enable mouse centering and hide mouse by default.
      EnableMouseCentering = true;

      // Get services from the global service container.
      var services = (ServiceContainer)ServiceLocator.Current;
      ContentManager = services.GetInstance<ContentManager>();
      UIContentManager = services.GetInstance<ContentManager>("UIContent");
      InputService = services.GetInstance<IInputService>();
      AnimationService = services.GetInstance<IAnimationService>();
      Simulation = services.GetInstance<Simulation>();
      ParticleSystemService = services.GetInstance<IParticleSystemService>();
      GraphicsService = services.GetInstance<IGraphicsService>();
      GameObjectService = services.GetInstance<IGameObjectService>();
      UIService = services.GetInstance<IUIService>();

      // Create a local service container which can be modified in samples:
      // The local service container is a child container, i.e. it inherits the 
      // services of the global service container. Samples can add new services
      // or override existing entries without affecting the global services container
      // or other samples.
      Services = services.CreateChildContainer();
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clean up.
        GameObjectService.Objects.Clear();
        ((SampleGame)Game).ResetPhysicsSimulation();
        Services.Dispose();
      }

      base.Dispose(disposing);
    }
  }
}
