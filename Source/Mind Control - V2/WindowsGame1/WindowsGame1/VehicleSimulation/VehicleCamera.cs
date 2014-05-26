using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Geometry;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using System;

namespace WindowsGame1.VehicleSimulation
{
    public class VehicleCamera : GameObject
    {
        //----------------------------------------------------------------------
        #region Fields

        private readonly IServiceLocator _services;
        private readonly IInputService _inputService;
        private readonly IGeometricObject _vehicle;
        //x-axis pivot
        private float _yaw;
        //y-axis pivot
        private float _pitch;

        #endregion

        //----------------------------------------------------------------------
        #region Properties

        public CameraNode CameraNode { get; private set; }

        #endregion

        //----------------------------------------------------------------------
        #region Creation and Cleanup

        public VehicleCamera(IGeometricObject vehicle, IServiceLocator services)
        {
            Name = "VehicleCamera";

            _vehicle = vehicle;
            _services = services;
            _inputService = services.GetInstance<IInputService>();
        }

        #endregion

        //----------------------------------------------------------------------
        #region Overridden Methods

        protected override void OnLoad()
        {
            var graphicsService = _services.GetInstance<IGraphicsService>();

            // Define camera projection.
            var projection = new PerspectiveProjection();
            projection.SetFieldOfView(
                ConstantsF.PiOver4,
                graphicsService.GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                1000.0f);

            //Create a camera node
            CameraNode = new CameraNode(new Camera(projection));
        }

        protected override void OnUnload()
        {
            CameraNode.Dispose(false);
            CameraNode = null;
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (_inputService.EnableMouseCentering)
            {
                float deltaTimeF = (float)deltaTime.TotalSeconds;

                // Compute new yaw and pitch from mouse movement.
                float deltaYaw = 0;
                deltaYaw -= _inputService.MousePositionDelta.X;
                deltaYaw -= _inputService.GetGamePadState(LogicalPlayerIndex.One).ThumbSticks.Right.X * 10;
                _yaw += deltaYaw * deltaTimeF * 0.1f;

                float deltaPitch = 0;
                deltaPitch -= _inputService.MousePositionDelta.Y;
                deltaPitch += _inputService.GetGamePadState(LogicalPlayerIndex.One).ThumbSticks.Right.Y * 10;
                _pitch += deltaPitch * deltaTimeF * 0.1f;

                // Limit the pitch angle to less than +/- 90°.
                float limit = ConstantsF.PiOver2 - 0.01f;
                _pitch = MathHelper.Clamp(_pitch, -limit, limit);

                CameraNode.LastPoseWorld = CameraNode.PoseWorld;

                var vehiclePose = _vehicle.Pose;

                //Player Camera
                //Fixed third person
                Matrix33F yaw = Matrix33F.CreateRotationY(_yaw);
                Matrix33F pitch = Matrix33F.CreateRotationX(_pitch);
                Matrix33F orientation = vehiclePose.Orientation*yaw*pitch;
                Vector3F forward = orientation*-Vector3F.UnitZ;
                Vector3F up = Vector3F.UnitY;
                Vector3F position = vehiclePose.Position - 10*forward + 5*up;
                Vector3F target = vehiclePose.Position + 1*up;

                CameraNode.View = Matrix44F.CreateLookAt(position, target, up);
            }
        }

        #endregion
    }
}