using DigitalRune.Diagnostics;
using DigitalRune.Geometry;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Particles;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples.Particles
{
  [Sample(SampleCategory.Particles,
    "A waterfall effect.",
    @"This effect uses preloading. The water particles use a special billboard type to follow 
the direction of the water flow.",
    8)]
  public class WaterFallSample : ParticleSample
  {
    private readonly ParticleSystem _particleSystem;
    private readonly ParticleSystemNode _particleSystemNode;


    public WaterFallSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      _particleSystem = WaterFall.CreateWaterFall(ContentManager);
      _particleSystem.Pose = new Pose(new Vector3F(0, 2, 0), Matrix33F.CreateRotationY(ConstantsF.Pi));
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
