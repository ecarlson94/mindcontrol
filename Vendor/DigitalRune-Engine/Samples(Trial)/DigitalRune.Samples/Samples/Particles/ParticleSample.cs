using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples.Particles
{
  // The base class for particle samples.
  public abstract class ParticleSample : BasicSample
  {
    private readonly SandboxObject _sandboxObject;


    protected ParticleSample(Microsoft.Xna.Framework.Game game) 
      : base(game)
    {
      GraphicsScreen.ClearBackground = true;
      GraphicsScreen.BackgroundColor = Color.CornflowerBlue;
      SetCamera(new Vector3F(0, 2, 10), 0, 0);

      _sandboxObject = new SandboxObject(Services);
      GameObjectService.Objects.Add(_sandboxObject);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        GameObjectService.Objects.Remove(_sandboxObject);
      }

      base.Dispose(disposing);
    }
  }
}
