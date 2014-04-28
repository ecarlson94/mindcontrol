using System;
using DigitalRune.Game;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Collisions;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Content;


namespace DigitalRune.Samples
{
  // Represents the "Saucer" model.
  public class SaucerObject : GameObject
  {
    private readonly IServiceLocator _services;
    private ModelNode _modelNode;
    private GeometricObject _geometricObject;
    private CollisionObject _collisionObject;


    // The collision object which can be used for contact queries.
    public CollisionObject CollisionObject
    {
      get { return _collisionObject; }
    }


    // The position and orientation of the saucer.
    public Pose Pose
    {
      get { return _modelNode.PoseWorld; }
      set
      {
        _modelNode.PoseWorld = value;
        _geometricObject.Pose = value;
      }
    }


    public SaucerObject(IServiceLocator services)
    {
      _services = services;
    }


    protected override void OnLoad()
    {
      var contentManager = _services.GetInstance<ContentManager>();

      // ----- Graphics
      // Load graphics model (created using the ModelWithCollisionMeshProcessor).
      var originalModelNode = contentManager.Load<ModelNode>("Saucer/saucer");

      // The collision shape is stored in the UserData.
      var shape = (Shape)originalModelNode.UserData;

      // We do not want to change the shared Saucer instance in the content manager.
      // Let's create a clone. But we do not want to clone the shape in the UserData,
      // this would be a waste of time and memory.
      originalModelNode.UserData = null;
      _modelNode = originalModelNode.Clone(); // Clone without UserData.
      originalModelNode.UserData = shape;

      _modelNode.PoseWorld = new Pose(Vector3F.Zero, Matrix33F.CreateRotationY(-ConstantsF.PiOver2));

      // Add model to the scene for rendering.
      var scene = _services.GetInstance<IScene>();
      scene.Children.Add(_modelNode);

      // ----- Collision Detection
      // Create a collision object and add it to the collision domain.
      _geometricObject = new GeometricObject(shape, _modelNode.PoseWorld);
      _collisionObject = new CollisionObject(_geometricObject);

      // Important: We do not need detailed contact information when a collision
      // is detected. The information of whether we have contact or not is sufficient.
      // Therefore, we can set the type to "Trigger". This increases the performance 
      // dramatically.
      _collisionObject.Type = CollisionObjectType.Trigger;

      var collisionDomain = _services.GetInstance<CollisionDomain>();
      collisionDomain.CollisionObjects.Add(_collisionObject);
    }


    protected override void OnUnload()
    {
      // Remove the collision object from the collision domain.
      var collisionDomain = _collisionObject.Domain;
      collisionDomain.CollisionObjects.Remove(_collisionObject);

      // Detach objects to avoid any "memory leaks".
      _collisionObject.GeometricObject = null;
      _geometricObject.Shape = Shape.Empty;

      // Remove the model from the scene.
      _modelNode.Parent.Children.Remove(_modelNode);
      _modelNode.Dispose(false);
    }


    protected override void OnUpdate(TimeSpan deltaTime)
    {
    }
  }
}
