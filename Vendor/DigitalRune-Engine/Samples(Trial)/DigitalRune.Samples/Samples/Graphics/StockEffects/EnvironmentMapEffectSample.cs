using DigitalRune.Geometry;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Mathematics.Statistics;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples.Graphics
{
  [Sample(SampleCategory.Graphics,
    "This sample shows how to use the XNA EnvironmentMapEffect.",
    "",
    23)]
  public class EnvironmentMapEffectSample : BasicSample
  {
    private readonly ModelNode _model;


    public EnvironmentMapEffectSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      GraphicsScreen.ClearBackground = true;
      GraphicsScreen.BackgroundColor = Color.CornflowerBlue;
      SetCamera(new Vector3F(8, 6, 8), ConstantsF.PiOver4, -0.4f);

      // Load the saucer model. This model is processed using the DigitalRune Model 
      // Processor - not the default XNA model processor!
      // In the folder that contains saucer.fbx, there are several XML files (*.drmdl and *.drmat) 
      // which define the materials of the model. These material description files are 
      // automatically processed by the DigitalRune Model Processor. Please browse 
      // to the content folder and have a look at the *.drmdl and *.drmat files.
      _model = ContentManager.Load<ModelNode>("EnvironmentMapped/saucer");

      // The XNA ContentManager manages a single instance of each model. We clone 
      // the models, to get a copy that we can modify without changing the original 
      // instance. 
      _model = _model.Clone();

      // Position the model and add it to the scene.
      _model.PoseWorld = new Pose(RandomHelper.Random.NextQuaternionF());
      _model.ScaleLocal = new Vector3F(0.4f);
      GraphicsScreen.Scene.Children.Add(_model);

      //// Here is another example showing how you can change material properties 
      //// of the model: 
      //// Get the mesh (the saucer model contains only a single mesh node).
      //MeshNode meshNode = _model.GetDescendants().OfType<MeshNode>().First();
      //Mesh mesh = meshNode.Mesh;
      //// Get the second material (which is the material of the cockpit).
      //Material material = mesh.Materials[1];
      //// Get the effect used in the "Default" render pass.
      //EffectBinding effectBindings = material["Default"];
      //// Change some parameters.
      //effectBindings.Set("EnvironmentMapAmount", 1f);
      //effectBindings.Set("FresnelFactor", 0.5f);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        GraphicsScreen.Scene.Children.Remove(_model);
        _model.Dispose(false);
      }

      base.Dispose(disposing);
    }
  }
}
