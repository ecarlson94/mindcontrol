using DigitalRune.Diagnostics;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Particles;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples.Particles
{
  [Sample(SampleCategory.Particles,
    @"This sample uses a particle system to draw grass.",
    @"(Note: This sample demonstrates capabilities of DigitalRune Particles. But in a real game, 
grass is not created using generic particle systems. Specialized GPU-based grass effects are 
much faster.)",
    13)]
  public class GrassSample : ParticleSample
  {
    private readonly ParticleSystem _particleSystem;
    private readonly ParticleSystemNode _particleSystemNode;


    public GrassSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      _particleSystem = Grass.Create(ContentManager);
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
