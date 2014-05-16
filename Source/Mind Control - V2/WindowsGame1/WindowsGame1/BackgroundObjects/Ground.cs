using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1.BackgroundObjects
{
    public class Ground : GameObject
    {
        private IServiceLocator _services;
        private ModelNode _modelNode;
        private RigidBody _rigidBody;

        public Ground(IServiceLocator services)
        {
            _services = services;
            Name = "CheckeredGround";
        }

        // OnLoad() is called when the GameObject is added to the IGameObjectService
        protected override void OnLoad()
        {
            var contentManager = _services.GetInstance<ContentManager>();
            _modelNode = contentManager.Load<ModelNode>("Ground").Clone();
            _modelNode.ScaleLocal = new Vector3F(1.25f);

            foreach (var node in _modelNode.GetSubtree())
            {
                node.CastsShadows = false;
                node.IsStatic = true;
            }

            //Add model node to scene graph
            var scene = _services.GetInstance<IScene>();
            scene.Children.Add(_modelNode);

            _rigidBody = new RigidBody(new PlaneShape(Vector3F.UnitY, 0))
            {
                MotionType = MotionType.Static,
            };

            var simulation = _services.GetInstance<Simulation>();
            simulation.RigidBodies.Add(_rigidBody);
        }

        // OnUnload() is called when the GameObject is removed to the IGameObjectService
        protected override void OnUnload()
        {
            //remove model and rigid body
            _modelNode.Parent.Children.Remove(_modelNode);
            _modelNode.Dispose(false);
            _modelNode = null;

            _rigidBody.Simulation.RigidBodies.Remove(_rigidBody);
            _rigidBody = null;
        }
    }
}
