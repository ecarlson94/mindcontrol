using System.Linq;
using DigitalRune.Diagnostics;
using DigitalRune.Geometry;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Mathematics.Statistics;
using DigitalRune.Particles;
using DigitalRune.Particles.Effectors;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples.Particles
{
  [Sample(SampleCategory.Particles,
    @"This sample shows how to create a particle system consisting of two child particle systems.
The effect is a ring of fire consisting of a fire and a smoke effect.",
    @"",
    3)]
  public class RingOfFireSample : ParticleSample
  {
    private readonly ParticleSystem _particleSystem; 
    private readonly ParticleSystemNode _particleSystemNode;


    public RingOfFireSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Create a new "empty" particle system.
      _particleSystem = new ParticleSystem();

      // Particle systems can have child particle systems. 
      // Add a fire and a smoke effect as children.
      var fire = Fire.Create(ContentManager);
      var smoke = Smoke.Create(ContentManager);  // The smoke effect from the previous sample.
      _particleSystem.Children = new ParticleSystemCollection { fire, smoke };

      // If we need to, we can modify the predefined effects.
      // Change the smoke particle lifetime.
      smoke.Parameters.Get<float>(ParticleParameterNames.Lifetime).DefaultValue = 4;
      // Change the smoke's start positions to a ring.
      smoke.Effectors.OfType<StartPositionEffector>().First().Distribution =
        new CircleDistribution { InnerRadius = 2, OuterRadius = 2 };

      // Position the particle system (including its child) in the level.
      _particleSystem.Pose = new Pose(new Vector3F(0, 3, 0));

      // We only need to add the parent particle system to the particle system service.
      // The service will automatically update the parent system each frame. The parent
      // system will automatically update its children.
      ParticleSystemService.ParticleSystems.Add(_particleSystem);

      // Add the particle system to the scene graph.
      _particleSystemNode = new ParticleSystemNode(_particleSystem);
      GraphicsScreen.Scene.Children.Add(_particleSystemNode);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clean up.
        ParticleSystemService.ParticleSystems.Remove(_particleSystem);

        GraphicsScreen.Scene.Children.Remove(_particleSystemNode);
        _particleSystemNode.Dispose(false);
      }

      base.Dispose(disposing);
    }


    public override void Update(GameTime gameTime)
    {
      // Synchronize particles <-> graphics.
      _particleSystemNode.Synchronize(GraphicsService);

      Profiler.AddValue("ParticleCount", ParticleHelper.CountNumberOfParticles(ParticleSystemService.ParticleSystems));
    }
  }
}
