using DigitalRune.Graphics.Rendering;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using Microsoft.Xna.Framework;
using WindowsGame1.Managers;
using WindowsGame1.Screens;

namespace WindowsGame1.Components
{
    public class GraphicsBaseComponent : BaseComponent
    {
        private readonly DefaultLightsObject _defaultLightsObject;
        private CameraObject _cameraObject;

        protected BaseGraphicsScreen GraphicsScreen { get; private set; }

        protected GraphicsBaseComponent(Game game, EmoEngineManager emoEngine) : base(game, emoEngine)
        {
            //Create a graphics screen for rendering simple things
            GraphicsScreen = new BaseGraphicsScreen(Services);

            //Order of screens is back-to-front (index 0 being back)
            //Any other screens like menu and GUI will render on top
            GraphicsService.Screens.Insert(0, GraphicsScreen);

            Services.Register(typeof(DebugRenderer), null, GraphicsScreen.DebugRenderer3D);
            Services.Register(typeof(DebugRenderer), "DebugRenderer2D", GraphicsScreen.DebugRenderer2D);
            Services.Register(typeof(IScene), null, GraphicsScreen.Scene);

            //Add a default light setup (ambient light +3 directional lights
            _defaultLightsObject = new DefaultLightsObject(Services);
            GameObjectService.Objects.Add(_defaultLightsObject);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Clean up.
                GameObjectService.Objects.Remove(_defaultLightsObject);
                GameObjectService.Objects.Remove(_cameraObject);

                GraphicsService.Screens.Remove(GraphicsScreen);
                GraphicsScreen.Dispose();
            }

            base.Dispose(disposing);
        }

        protected void SetCamera(Vector3F position, float yaw, float pitch)
        {
            if (_cameraObject == null)
            {
                _cameraObject = new CameraObject(Services);
                GameObjectService.Objects.Add(_cameraObject);
                GraphicsScreen.CameraNode3D = _cameraObject.CameraNode;
            }

            _cameraObject.ResetPose(position, yaw, pitch);
        }
    }
}