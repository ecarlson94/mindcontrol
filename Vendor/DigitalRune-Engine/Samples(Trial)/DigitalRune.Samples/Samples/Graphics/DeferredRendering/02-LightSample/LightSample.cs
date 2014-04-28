using DigitalRune.Graphics.Rendering;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Physics.ForceEffects;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples.Graphics
{
  [Sample(SampleCategory.Graphics,
    @"This sample tests different light types.",
    @"",
    102)]
  public class LightSample : Sample
  {
    private readonly DeferredGraphicsScreen _graphicsScreen;


    public LightSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      _graphicsScreen = new DeferredGraphicsScreen(Services);
      _graphicsScreen.DrawReticle = true;
      GraphicsService.Screens.Insert(0, _graphicsScreen);

      Services.Register(typeof(DebugRenderer), null, _graphicsScreen.DebugRenderer);
      Services.Register(typeof(IScene), null, _graphicsScreen.Scene);

      // Add gravity and damping to the physics simulation.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // Add a custom game object which controls the camera.
      var cameraGameObject = new CameraObject(Services);
      GameObjectService.Objects.Add(cameraGameObject);
      _graphicsScreen.ActiveCameraNode = cameraGameObject.CameraNode;

      GameObjectService.Objects.Add(new GrabObject(Services));
      GameObjectService.Objects.Add(new StaticSkyObject(Services));
      GameObjectService.Objects.Add(new GroundObject(Services));

      // Disable the main lights (of the StaticSkyObject) and add some test lights instead.
      _graphicsScreen.Scene.GetSceneNode("Sunlight").IsEnabled = false;
      _graphicsScreen.Scene.GetSceneNode("Ambient").IsEnabled = false;
      GameObjectService.Objects.Add(new TestLightsObject(Services));

      GameObjectService.Objects.Add(new ObjectCreatorObject(Services));
      GameObjectService.Objects.Add(new LavaBallsObject(Services));
      GameObjectService.Objects.Add(new FogObject(Services));

      GameObjectService.Objects.Add(new OptionsObject(Services));
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clean up.
        GameObjectService.Objects.Clear();

        Simulation.RigidBodies.Clear();
        Simulation.ForceEffects.Clear();

        _graphicsScreen.GraphicsService.Screens.Remove(_graphicsScreen);
        _graphicsScreen.Dispose();
      }

      base.Dispose(disposing);
    }


    public override void Update(GameTime gameTime)
    {
      // This sample clears the debug renderer each frame.
      _graphicsScreen.DebugRenderer.Clear();
    }
  }
}
