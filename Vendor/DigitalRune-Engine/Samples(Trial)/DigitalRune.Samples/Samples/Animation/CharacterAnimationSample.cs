using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics.ForceEffects;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples.Animation
{
  // The base class for character animation samples.
  public class CharacterAnimationSample : BasicSample
  {
    private readonly GroundObject _groundObject;


    public CharacterAnimationSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      GraphicsScreen.ClearBackground = true;
      GraphicsScreen.BackgroundColor = Color.CornflowerBlue;
      SetCamera(new Vector3F(0, 1, 3), 0, 0);

      // Add gravity and damping to the physics simulation. 
      // Note: The physics simulation is only used by the ragdoll samples.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // Add a ground object.
      _groundObject = new GroundObject(Services);
      GameObjectService.Objects.Add(_groundObject);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        GameObjectService.Objects.Remove(_groundObject);

        // Undo changes in physics simulation.
        Simulation.RigidBodies.Clear();
        Simulation.ForceEffects.Clear();
      }

      base.Dispose(disposing);
    }
  }
}
