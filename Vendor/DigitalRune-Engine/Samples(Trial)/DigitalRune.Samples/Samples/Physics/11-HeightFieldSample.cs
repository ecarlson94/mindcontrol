using System;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Mathematics.Statistics;
using DigitalRune.Physics;
using DigitalRune.Physics.ForceEffects;


namespace DigitalRune.Samples.Physics
{
  [Sample(SampleCategory.Physics,
    "This sample demonstrates how to create a landscape using a height field.",
    "",
    11)]
  public class HeightFieldSample : PhysicsSample
  {
    public HeightFieldSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Add basic force effects.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // Create a height field.
      HeightField heightField = new HeightField();

      // The height field has a size of 100 m x 100 m.
      heightField.WidthX = 100;
      heightField.WidthZ = 100;

      // We can set following flag to get a significant performance gain - but the collision
      // detection will be less accurate. For smooth height fields this flag can be set.
      heightField.UseFastCollisionApproximation = true;

      // The height data has a resolution of 20 entries per dimension.
      heightField.Array = new float[20, 20];
      for (int x = 0; x < heightField.Array.GetLength(0); x++)
      {
        for (int z = 0; z < heightField.Array.GetLength(1); z++)
        {
          // Set the y values (height).
          heightField.Array[x, z] = (float)(Math.Cos(z / 2f) * Math.Sin(x / 2f) * 5f + 5f);
        }
      }

      // Create a static rigid body using the height field and add it to the simulation.
      RigidBody landscape = new RigidBody(heightField)
      {
        Pose = new Pose(new Vector3F(-50, 0, -50f)),
        MotionType = MotionType.Static,
      };
      Simulation.RigidBodies.Add(landscape);

      // Distribute a few spheres and boxes across the landscape.
      SphereShape sphereShape = new SphereShape(0.5f);
      for (int i = 0; i < 30; i++)
      {
        Vector3F position = RandomHelper.Random.NextVector3F(-30, 30);
        position.Y = 20;

        RigidBody body = new RigidBody(sphereShape)
        {
          Pose = new Pose(position),
        };
        Simulation.RigidBodies.Add(body);
      }

      BoxShape boxShape = new BoxShape(1, 1, 1);
      for (int i = 0; i < 30; i++)
      {
        Vector3F position = RandomHelper.Random.NextVector3F(-30, 30);
        position.Y = 20;

        RigidBody body = new RigidBody(boxShape)
        {
          Pose = new Pose(position),
        };
        Simulation.RigidBodies.Add(body);
      }
    }
  }
}
