using System;
using System.Linq;
using DigitalRune.Game;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Meshes;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Graphics;
using DigitalRune.Graphics.Effects;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Mathematics.Statistics;
using DigitalRune.Physics;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MathHelper = DigitalRune.Mathematics.MathHelper;


namespace DigitalRune.Samples
{
  // Creates a dynamic object (model + rigid body). The graphics model (mesh and material)
  // is created in code and not loaded with the content manager.
  public sealed class ProceduralObject : GameObject
  {
    private readonly IServiceLocator _services;
    private MeshNode _meshNode;
    private RigidBody _rigidBody;


    public ProceduralObject(IServiceLocator services)
    {
      _services = services;
    }


    // OnLoad() is called when the GameObject is added to the IGameObjectService.
    protected override void OnLoad()
    {
      var graphicsService = _services.GetInstance<IGraphicsService>();
      var gameObjectService = _services.GetInstance<IGameObjectService>();

      // Check if the game object manager has another ProceduralObject instance.
      var otherProceduralObject = gameObjectService.Objects
                                                   .OfType<ProceduralObject>()
                                                   .FirstOrDefault(o => o != this);
      Mesh mesh;
      if (otherProceduralObject != null)
      {
        // This ProceduralObject is not the first. We re-use rigid body data and 
        // the mesh from the existing instance.
        var otherBody = otherProceduralObject._rigidBody;
        _rigidBody = new RigidBody(otherBody.Shape, otherBody.MassFrame, otherBody.Material);
        mesh = otherProceduralObject._meshNode.Mesh;
      }
      else
      {
        // This is the first ProceduralObject instance. We create a new rigid body 
        // and a new mesh. 
        var shape = new MinkowskiSumShape(new GeometricObject(new SphereShape(0.05f)), new GeometricObject(new BoxShape(0.5f, 0.5f, 0.5f)));
        _rigidBody = new RigidBody(shape);

        // Create a DigitalRune.Geometry.Meshes.TriangleMesh from the RigidBody's 
        // shape and convert this to a DigitalRune.Graphics.Mesh.
        var triangleMesh = _rigidBody.Shape.GetMesh(0.01f, 4);
        mesh = new Mesh { Name = "ProceduralObject" };
        var submesh = CreateSubmeshWithTexCoords(graphicsService.GraphicsDevice, triangleMesh, MathHelper.ToRadians(70));
        mesh.Submeshes.Add(submesh);

        // Next, we need a material. We can load a predefined material (*.drmat file)
        // with the content manager.
        //var material = content.Load<Material>("Default");

        // Alternatively, we can load some effects and build the material here:
        Material material = new Material();

        // We need an EffectBinding for each render pass. 
        // The "Default" pass uses a BasicEffectBinding (which is an EffectBinding
        // for the XNA BasicEffect). 
        // Note: The "Default" pass is not used by the DeferredLightingScreen, so
        // we could ignore this pass in this sample project.
        BasicEffectBinding defaultEffectBinding = new BasicEffectBinding(graphicsService, null)
        {
          LightingEnabled = true,
          TextureEnabled = true,
          VertexColorEnabled = false
        };
        defaultEffectBinding.Set("Texture", graphicsService.GetDefaultTexture2DWhite());
        defaultEffectBinding.Set("DiffuseColor", new Vector3(1, 1, 1));
        defaultEffectBinding.Set("SpecularColor", new Vector3(1, 1, 1));
        defaultEffectBinding.Set("SpecularPower", 100f);
        material.Add("Default", defaultEffectBinding);

        // EffectBinding for the "ShadowMap" pass.
        // Note: EffectBindings which are used in a Material must be marked with 
        // the EffectParameterHint Material.
        var content = _services.GetInstance<ContentManager>();
        EffectBinding shadowMapEffectBinding = new EffectBinding(
          graphicsService,
          content.Load<Effect>("DigitalRune\\Materials\\ShadowMap"),
          null,
          EffectParameterHint.Material);
        material.Add("ShadowMap", shadowMapEffectBinding);

        // EffectBinding for the "GBuffer" pass.
        EffectBinding gBufferEffectBinding = new EffectBinding(
          graphicsService,
          content.Load<Effect>("DigitalRune\\Materials\\GBuffer"),
          null,
          EffectParameterHint.Material);
        gBufferEffectBinding.Set("SpecularPower", 100f);
        material.Add("GBuffer", gBufferEffectBinding);

        // EffectBinding for the "Material" pass.
        EffectBinding materialEffectBinding = new EffectBinding(
          graphicsService,
          content.Load<Effect>("DigitalRune\\Materials\\Material"),
          null,
          EffectParameterHint.Material);
        materialEffectBinding.Set("DiffuseTexture", graphicsService.GetDefaultTexture2DWhite());
        materialEffectBinding.Set("DiffuseColor", new Vector3(1, 1, 1));
        materialEffectBinding.Set("SpecularColor", new Vector3(1, 1, 1));
        material.Add("Material", materialEffectBinding);

        // Assign this material to the submesh.
        submesh.SetMaterial(material);
      }

      // Create a scene graph node for the mesh.
      _meshNode = new MeshNode(mesh);

      // Set a random pose.
      var randomPosition = new Vector3F(
        RandomHelper.Random.NextFloat(-10, 10),
        RandomHelper.Random.NextFloat(2, 5),
        RandomHelper.Random.NextFloat(-20, 0));
      _rigidBody.Pose = new Pose(randomPosition, RandomHelper.Random.NextQuaternionF());
      _meshNode.PoseWorld = _rigidBody.Pose;

      // Add mesh node to scene graph.
      var scene = _services.GetInstance<IScene>();
      scene.Children.Add(_meshNode);

      // Add rigid body to the physics simulation.
      var simulation = _services.GetInstance<Simulation>();
      simulation.RigidBodies.Add(_rigidBody);
    }


    // Creates a submesh to draw a triangle mesh, similar to MeshHelper.CreateSubmesh()
    // (see documentation of MeshHelper.CreateSubmesh()), but this method also
    // creates texture coordinates because they are required by many shader.
    private static Submesh CreateSubmeshWithTexCoords(GraphicsDevice graphicsDevice, TriangleMesh mesh, float angleLimit)
    {
      var numberOfTriangles = mesh.NumberOfTriangles;
      if (numberOfTriangles == 0)
        return null;

      var submesh = new Submesh
      {
        PrimitiveType = PrimitiveType.TriangleList,
        PrimitiveCount = numberOfTriangles,
        VertexCount = numberOfTriangles * 3,
      };

      // Create vertex data for a triangle list.
      var vertices = new VertexPositionNormalTexture[submesh.VertexCount];

      // Create vertex normals. 
      var normals = mesh.ComputeNormals(false, angleLimit);

      for (int i = 0; i < numberOfTriangles; i++)
      {
        var i0 = mesh.Indices[i * 3 + 0];
        var i1 = mesh.Indices[i * 3 + 1];
        var i2 = mesh.Indices[i * 3 + 2];

        var v0 = mesh.Vertices[i0];
        var v1 = mesh.Vertices[i1];
        var v2 = mesh.Vertices[i2];

        Vector3F n0, n1, n2;
        if (angleLimit < 0)
        {
          // If the angle limit is negative, ComputeNormals() returns one normal per vertex.
          n0 = normals[i0];
          n1 = normals[i1];
          n2 = normals[i2];
        }
        else
        {
          // If the angle limits is >= 0, ComputeNormals() returns 3 normals per triangle.
          n0 = normals[i * 3 + 0];
          n1 = normals[i * 3 + 1];
          n2 = normals[i * 3 + 2];
        }

        // Add new vertex data.
        // DigitalRune.Geometry uses counter-clockwise front faces. XNA uses
        // clockwise front faces (CullMode.CullCounterClockwiseFace) per default. 
        // Therefore we change the vertex orientation of the triangles. 
        vertices[i * 3 + 0] = new VertexPositionNormalTexture((Vector3)v0, (Vector3)n0, new Vector2());
        vertices[i * 3 + 1] = new VertexPositionNormalTexture((Vector3)v2, (Vector3)n2, new Vector2());  // v2 instead of v1!
        vertices[i * 3 + 2] = new VertexPositionNormalTexture((Vector3)v1, (Vector3)n1, new Vector2());
      }

      // Create a vertex buffer.
      submesh.VertexBuffer = new VertexBuffer(
        graphicsDevice,
        typeof(VertexPositionNormalTexture),
        vertices.Length,
        BufferUsage.None);
      submesh.VertexBuffer.SetData(vertices);

      return submesh;
    }


    // OnUnload() is called when the GameObject is removed from the IGameObjectService.
    protected override void OnUnload()
    {
      _meshNode.Parent.Children.Remove(_meshNode);
      _meshNode.Dispose(false);
      _meshNode = null;

      _rigidBody.Simulation.RigidBodies.Remove(_rigidBody);
      _rigidBody = null;
    }


    // OnUpdate() is called once per frame.
    protected override void OnUpdate(TimeSpan deltaTime)
    {
      // Synchronize graphics <--> physics.
      if (_meshNode != null)
      {
        // Update SceneNode.LastPoseWorld - this is required for some effects, 
        // like object motion blur.
        _meshNode.SetLastPose(true);

        _meshNode.PoseWorld = _rigidBody.Pose;
      }
    }
  }
}
