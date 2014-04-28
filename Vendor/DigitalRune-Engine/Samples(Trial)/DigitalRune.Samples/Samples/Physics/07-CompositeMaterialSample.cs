using DigitalRune.Geometry;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using DigitalRune.Physics.ForceEffects;
using DigitalRune.Physics.Materials;


namespace DigitalRune.Samples.Physics
{
  [Sample(SampleCategory.Physics,
    @"This sample demonstrates how to create a height field with different material properties.",
    @"One side of the height field is made slippery.
Tip: Press <M> to see the triangulated height field cells.",
    7)]
  public class CompositeMaterialSample : PhysicsSample
  {
    public CompositeMaterialSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Add basic force effects.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // ----- Create a simple height field.
      const int resolution = 20;
      HeightField heightField = new HeightField();

      // Set the size of the height field in world space. (WidthX/Z determine the extent
      // of the height field but not the resolution.)
      heightField.WidthX = 40;
      heightField.WidthZ = 40;

      // Create the height field data. 
      // The size of the array determines the resolution of the height field.
      heightField.Array = new float[resolution, resolution];
      for (int x = 0; x < heightField.Array.GetLength(0); x++)
      {
        for (int z = 0; z < heightField.Array.GetLength(1); z++)
        {
          // Set the y value (height) at the given index.
          heightField.Array[x, z] = 20 - z;
        }
      }

      RigidBody ground = new RigidBody(heightField)
      {
        Pose = new Pose(new Vector3F(-20, -10, -20f)),
        MotionType = MotionType.Static,
      };
      Simulation.RigidBodies.Add(ground);

      // Assign two different materials to the height field.
      // A rough material (high friction) should be used for the left cells of the height field.
      UniformMaterial roughMaterial = new UniformMaterial
      {
        DynamicFriction = 1,
        StaticFriction = 1,
      };

      // A slippery material (low friction) should be used for the right cells of the height field.
      UniformMaterial slipperyMaterial = new UniformMaterial
      {
        DynamicFriction = 0,
        StaticFriction = 0,
      };

      // Use a CompositeMaterial two assign the materials to the features of the height field.
      CompositeMaterial compositeMaterial = new CompositeMaterial();

      // A "feature" of a height field is a triangle:
      // The height field is triangulated. Each cell consists of two triangles. The triangles are 
      // numbered from left-to-right and top-to-bottom. 
      // (For more information: See the description of HeightField.)

      // Loop over the cells.
      // (If the resolution is 20, we have 20 height values in one row. Between these height
      // values are 19 cells.)
      for (int z = 0; z < resolution - 1; z++)
      {
        for (int x = 0; x < resolution - 1; x++)
        {
          // Assign the rough material to the left cells and the slippery material to the 
          // right cells.
          if (x < resolution / 2)
          {
            // Each cell contains 2 triangles, therefore we have to add 2 entries to the 
            // CompositeMaterial.
            compositeMaterial.Materials.Add(roughMaterial);
            compositeMaterial.Materials.Add(roughMaterial);
          }
          else
          {
            compositeMaterial.Materials.Add(slipperyMaterial);
            compositeMaterial.Materials.Add(slipperyMaterial);
          }
        }
      }
      ground.Material = compositeMaterial;

      // Create a few boxes on the height field.
      // The left boxes will roll or stop on the height field because of the high friction.
      // The right boxes will slide down because of the low friction.
      BoxShape boxShape = new BoxShape(1, 1, 1);
      for (int i = 0; i < 10; i++)
      {
        RigidBody body = new RigidBody(boxShape, null, roughMaterial)
        {
          Pose = new Pose(new Vector3F(-19 + i * 4, 10, -10)),
        };
        Simulation.RigidBodies.Add(body);
      }
    }
  }
}
