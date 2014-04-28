using DigitalRune.Game;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;
using Microsoft.Practices.ServiceLocation;


namespace DigitalRune.Samples
{
  // Adds distance and height-based fog. Fog is disabled by default.
  public class FogObject : GameObject
  {
    private readonly IServiceLocator _services;

    // Optionally, we can move the fog node with the camera node. If camera and 
    // fog are independent, then the camera can fly up and "escape" the height-based 
    // fog. If camera and fog move together, then the fog will always have the
    // same height at the horizon (e.g. to hide the horizon).
    private bool _attachToCamera;


    public FogNode FogNode { get; private set; }


    public FogObject(IServiceLocator services) : this(services, false)
    {
    }


    public FogObject(IServiceLocator services, bool attachToCamera) 
    {
      _services = services;
      _attachToCamera = attachToCamera;
      Name = "Fog";
    }


    // OnLoad() is called when the GameObject is added to the IGameObjectService.
    protected override void OnLoad()
    {
      FogNode = new FogNode(new Fog())
      {
        IsEnabled = false,
        Name = "Fog",
      };

      var scene = _services.GetInstance<IScene>();
      if (!_attachToCamera)
      {
        scene.Children.Add(FogNode);
      }
      else
      {
        var cameraNode = ((Scene)scene).GetSceneNode("PlayerCamera");
        if (cameraNode.Children == null)
          cameraNode.Children = new SceneNodeCollection();

        cameraNode.Children.Add(FogNode);
      } 
    }


    // OnUnload() is called when the GameObject is removed from the IGameObjectService.
    protected override void OnUnload()
    {
      FogNode.Parent.Children.Remove(FogNode);
      FogNode.Dispose(false);
      FogNode = null;
    }
  }
}
