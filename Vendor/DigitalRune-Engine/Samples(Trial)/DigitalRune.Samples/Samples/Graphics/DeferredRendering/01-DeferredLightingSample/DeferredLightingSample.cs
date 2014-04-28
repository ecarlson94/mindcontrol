using System;
using DigitalRune.Geometry;
using DigitalRune.Graphics.PostProcessing;
using DigitalRune.Graphics.Rendering;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Mathematics.Statistics;
using DigitalRune.Physics.ForceEffects;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples.Graphics
{
  [Sample(SampleCategory.Graphics,
    @"This sample shows a simple scene rendered with deferred lighting.",
    @"The class DeferredGraphicsScreen implements a deferred lighting render pipeline.
This pipeline supports: 
  - lights and shadows, 
  - screen-space ambient occlusion (SSAO),
  - high dynamic range (HDR) lighting, 
  - sky rendering, 
  - particle systems with soft particles and low-resolution offscreen rendering,
  - post-processing, 
  - and more...
The intermediate render targets (G-buffer, light buffer, shadow masks) can be
visualized for debugging and understanding.
Beginners can use this graphics screen as it is. Advanced developers can adapt 
the render pipeline to their needs.
Have a look at the source code comments of the DeferredGraphicsScreen for more details.",
    101)]
  public class DeferredLightingSample : Sample
  {
    private readonly DeferredGraphicsScreen _graphicsScreen;


    public DeferredLightingSample(Microsoft.Xna.Framework.Game game)
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
      GameObjectService.Objects.Add(new StaticSkyObject(Services)); // Skybox + some lights.
      GameObjectService.Objects.Add(new GroundObject(Services));

      // Add a god ray post-process filter and a game object which updates the god ray directions.
      var godRayFilter = new GodRayFilter(GraphicsService)
      {
        Exposure = 0.08f,
        NumberOfSamples = 12,
        Scale = 0.3f,
      };
      _graphicsScreen.PostProcessors.Add(godRayFilter);
      GameObjectService.Objects.Add(new GodRayObject(Services, godRayFilter));

      GameObjectService.Objects.Add(new DudeObject(Services));
      GameObjectService.Objects.Add(new DynamicObject(Services, 1));
      GameObjectService.Objects.Add(new DynamicObject(Services, 2));
      GameObjectService.Objects.Add(new DynamicObject(Services, 3));
      GameObjectService.Objects.Add(new DynamicObject(Services, 4));
      GameObjectService.Objects.Add(new DynamicObject(Services, 5));
      GameObjectService.Objects.Add(new DynamicObject(Services, 6));
      GameObjectService.Objects.Add(new DynamicObject(Services, 7));
      GameObjectService.Objects.Add(new ObjectCreatorObject(Services));
      GameObjectService.Objects.Add(new FogObject(Services));
      GameObjectService.Objects.Add(new CampfireObject(Services));

      // The LavaBalls class controls all lava ball instances.
      var lavaBalls = new LavaBallsObject(Services);
      GameObjectService.Objects.Add(lavaBalls);

      // Create a lava ball instance.
      lavaBalls.Spawn();

      // Add a few palm trees.
      Random random = new Random(12345);
      for (int i = 0; i < 10; i++)
      {
        Vector3F position = new Vector3F(random.NextFloat(-3, -8), 0, random.NextFloat(0, -5));
        Matrix33F orientation = Matrix33F.CreateRotationY(random.NextFloat(0, ConstantsF.TwoPi));
        float scale = random.NextFloat(0.5f, 1.2f);
        GameObjectService.Objects.Add(new StaticObject(Services, "PalmTree/palm_tree", scale, new Pose(position, orientation)));
      }

      // Add an options window.
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
