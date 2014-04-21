using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNATutorial.Cameras
{
    public class Camera : GameComponent
    {
        private Vector3 cameraPosition;
        private Vector3 cameraRotation;
        private float cameraSpeed;
        private Vector3 cameraLookAt;

        public Matrix Projection { get; protected set; }

        public Matrix View
        {
            get { return Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up); }
        }

        public Camera(Game game, Vector3 position, Vector3 rotation, float speed)
            : base(game)
        {
            cameraSpeed = speed;

            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio,
                0.05f, 1000.0f);
        }

        private void MoveTo(Vector3 position, Vector3 rotation)
        {
            cameraPosition = position;
            cameraRotation = rotation;
        }
    }
}
