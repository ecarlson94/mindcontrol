using DigitalRune.Diagnostics;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Particles;
using DigitalRune.Physics;
using DigitalRune.Physics.ForceEffects;
using Microsoft.Xna.Framework;


namespace DigitalRune.Samples.Particles
{
  [Sample(SampleCategory.Particles,
    @"This sample shows how to emit particles on a mesh, and the difference between a ""Local"" 
and ""World"" reference frame.",
    @"Both ball meshes emit particles on the mesh surface.
The left ball uses ParticleReferenceFrame.World.
The right ball uses ParticleReferenceFrame.Local.",
    11)]
  public class ReferenceFrameSample : ParticleSample
  {
    private GrabObject _grabObject;
    private ParticleSystem _particleSystem0;
    private ParticleSystem _particleSystem1;
    private ParticleSystemNode _particleSystemNode0;
    private ParticleSystemNode _particleSystemNode1;
    private RigidBody _rigidBody0;
    private RigidBody _rigidBody1;
    private MeshNode _meshNode0;
    private MeshNode _meshNode1;


    public ReferenceFrameSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      GraphicsScreen.DrawReticle = true;

      _grabObject = new GrabObject(Services);
      GameObjectService.Objects.Add(_grabObject);

      CreateParticleSystem();
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        GameObjectService.Objects.Remove(_grabObject);

        ParticleSystemService.ParticleSystems.Remove(_particleSystem0);
        ParticleSystemService.ParticleSystems.Remove(_particleSystem1);

        GraphicsScreen.Scene.Children.Remove(_particleSystemNode0);
        GraphicsScreen.Scene.Children.Remove(_particleSystemNode1);
        _particleSystemNode0.Dispose(false);
        _particleSystemNode1.Dispose(false);

        GraphicsScreen.Scene.Children.Remove(_meshNode0);
        GraphicsScreen.Scene.Children.Remove(_meshNode1);
        _meshNode0.Dispose(false);
        _meshNode1.Dispose(false);

        // Remove all rigid bodies and force effects.
        Simulation.RigidBodies.Remove(_rigidBody0);
        Simulation.RigidBodies.Remove(_rigidBody1);
      }

      base.Dispose(disposing);
    }


    private void CreateParticleSystem()
    {
      // Load a sphere model.
      var modelNode = ContentManager.Load<ModelNode>("Particles/Sphere");
      var meshNode = (MeshNode)modelNode.Children[0];

      // Add gravity and damping to the physics simulation.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // Create two instances of the sphere model.
      _meshNode0 = meshNode.Clone();
      GraphicsScreen.Scene.Children.Add(_meshNode0);
      _meshNode1 = meshNode.Clone();
      GraphicsScreen.Scene.Children.Add(_meshNode1);

      // Create a rigid body for the left sphere.
      _rigidBody0 = new RigidBody(new SphereShape(0.5f))
      {
        Pose = new Pose(new Vector3F(-3, 4, 0)),
      };
      Simulation.RigidBodies.Add(_rigidBody0);

      // Create a rigid body for the right sphere. (Sharing the same shape, mass and material.)
      _rigidBody1 = new RigidBody(_rigidBody0.Shape, _rigidBody0.MassFrame, _rigidBody0.Material)
      {
        Pose = new Pose(new Vector3F(3, 4, 0)),
      };
      Simulation.RigidBodies.Add(_rigidBody1);

      // Extract basic triangle mesh from the sphere model.
      var triangleMesh = meshNode.Mesh.Submeshes[0].ToTriangleMesh();

      // Create a particle system for the left ball. This particle system uses
      // ReferenceFrame == ParticleReferenceFrame.World - which is the default for all 
      // particle systems. Particles are all relative to world space. The particle system pose 
      // determines the start positions and direction (when the StartPositionEffector and 
      // StartDirectionEffector are in use). Particles do not move with the particle system.
      _particleSystem0 = GlowingMeshEffect.Create(triangleMesh, ContentManager);
      _particleSystem0.ReferenceFrame = ParticleReferenceFrame.World;
      ParticleSystemService.ParticleSystems.Add(_particleSystem0);

      _particleSystemNode0 = new ParticleSystemNode(_particleSystem0);
      _meshNode0.Children = new SceneNodeCollection { _particleSystemNode0 };

      // Create a particle system for the right ball. This particle system uses
      // ReferenceFrame == ParticleReferenceFrame.Local. Particles are all relative to the 
      // particle system pose. Particles move with the particle system.
      _particleSystem1 = GlowingMeshEffect.Create(triangleMesh, ContentManager);
      _particleSystem1.ReferenceFrame = ParticleReferenceFrame.Local;
      ParticleSystemService.ParticleSystems.Add(_particleSystem1);

      _particleSystemNode1 = new ParticleSystemNode(_particleSystem1);
      _meshNode1.Children = new SceneNodeCollection { _particleSystemNode1 };
    }


    public override void Update(GameTime gameTime)
    {
      // Update SceneNode.LastPoseWorld (required for optional effects, like motion blur).
      _meshNode0.SetLastPose(true);
      _meshNode1.SetLastPose(true);

      // Synchronize pose of rigid body and model.
      _meshNode0.PoseWorld = _rigidBody0.Pose;
      _meshNode1.PoseWorld = _rigidBody1.Pose;

      // The particle system nodes are attached to the mesh nodes. Their pose is
      // updated automatically.

      // _particleSystem0 is relative to world space, we need to update its 
      // pose explicitly.
      _particleSystem0.Pose = _rigidBody0.Pose;

      // Synchronize particles <-> graphics.
      _particleSystemNode0.Synchronize(GraphicsService);
      _particleSystemNode1.Synchronize(GraphicsService);

      Profiler.AddValue("ParticleCount", ParticleHelper.CountNumberOfParticles(ParticleSystemService.ParticleSystems));
    }
  }
}
