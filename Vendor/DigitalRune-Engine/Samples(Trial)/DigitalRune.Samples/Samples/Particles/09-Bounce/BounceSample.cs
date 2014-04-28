using DigitalRune.Diagnostics;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Particles;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples.Particles
{
  [Sample(SampleCategory.Particles,
    "This sample shows how to create a bouncing particles using a custom effector.",
    "Particles get stretched in motion direction.",
    9)]
  public class BounceSample : ParticleSample
  {
    private readonly ParticleSystem _particleSystem;
    private readonly ParticleSystemNode _particleSystemNode;


    public BounceSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      _particleSystem = BouncingSparks.Create(ContentManager);
      ParticleSystemService.ParticleSystems.Add(_particleSystem);

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
