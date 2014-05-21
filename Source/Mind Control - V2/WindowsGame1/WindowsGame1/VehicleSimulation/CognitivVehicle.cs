using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using DigitalRune.Physics.Materials;
using DigitalRune.Physics.Specialized;
using Emotiv;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using WindowsGame1.Managers;
using Microsoft.Xna.Framework.Input;
using MathHelper = DigitalRune.Mathematics.MathHelper;

namespace WindowsGame1.VehicleSimulation
{
    public class CognitivVehicle : GameObject
    {
        //--------------------------------------------------------------
        #region Fields

        private readonly IServiceLocator _services;
        private readonly IInputService _inputService;

        //Emo Engine properties
        private EmoEngineManager _emoEngine;
        private EdkDll.EE_CognitivAction_t _currentAction;
            //Actions the car can use for it's directional movement
        private IEnumerable<EdkDll.EE_CognitivAction_t> _allowedActions; 

        private readonly Simulation _simulation;

        //Models for rendering
        private readonly ModelNode _vehicleModelNode;
        private readonly ModelNode[] _wheelModelNodes;

        //Vehicle values.
        private float _steeringAngle;
        private float _motorForce;

        #endregion

        //--------------------------------------------------------------
        #region Properties

        public Vehicle Vehicle { get; private set; }

        public IEnumerable<EdkDll.EE_CognitivAction_t> AllowedActions
        {
            get { return _allowedActions; }
            set { _allowedActions = value; }
        }

        #endregion

        //--------------------------------------------------------------
        #region Creation and Cleanup

        public CognitivVehicle(IServiceLocator services, EmoEngineManager emoEngine, IEnumerable<EdkDll.EE_CognitivAction_t> allowedActions = null)
        {
            _services = services;
            _emoEngine = emoEngine;
            _allowedActions = allowedActions ?? new List<EdkDll.EE_CognitivAction_t>();
            Name = "Vehicle";

            _inputService = _services.GetInstance<IInputService>();
            _simulation = services.GetInstance<Simulation>();

            //Loading all the models
            var contentManager = _services.GetInstance<ContentManager>();
            _vehicleModelNode = contentManager.Load<ModelNode>("Car/Car").Clone();
            _wheelModelNodes = new ModelNode[4];
            _wheelModelNodes[0] = contentManager.Load<ModelNode>("Car/Wheel").Clone();
            _wheelModelNodes[1] = _wheelModelNodes[0].Clone();
            _wheelModelNodes[2] = _wheelModelNodes[0].Clone();
            _wheelModelNodes[3] = _wheelModelNodes[0].Clone();

            //Giving the car some sweet rims
            _vehicleModelNode.Children.Add(_wheelModelNodes[0]);
            _vehicleModelNode.Children.Add(_wheelModelNodes[1]);
            _vehicleModelNode.Children.Add(_wheelModelNodes[2]);
            _vehicleModelNode.Children.Add(_wheelModelNodes[3]);

            //Extract the vertices from the car model.
            var meshNode = _vehicleModelNode.GetDescendants()
                .OfType<MeshNode>()
                .First(mn => mn.Name == "Car");

            var mesh = MeshHelper.ToTriangleMesh(meshNode.Mesh);
            mesh.Transform(meshNode.PoseWorld * Matrix44F.CreateScale(meshNode.ScaleWorld));

            //Create simplified convex hull from mesh
            var convexHull = GeometryHelper.CreateConvexHull(mesh.Vertices, 64, -0.04f);
            
            //Create convex polyedron shape using the vertices of the convex hull.
            var chassisShape = new ConvexPolyhedron(convexHull.Vertices.Select(v => v.Position));

            var mass = MassFrame.FromShapeAndMass(chassisShape, Vector3F.One, 800, 0.1f, 1);

            var pose = mass.Pose;
            pose.Position.Y -= 0.5f; //Lower the center of mass.
            pose.Position.Z = -0.5f; //The center should be below the driver;
            mass.Pose = pose;

            //Material for the chassis
            var material = new UniformMaterial
            {
                Restitution = 0.1f,
                StaticFriction = 0.2f,
                DynamicFriction = 0.2f,
            };

            var chassis = new RigidBody(chassisShape, mass, material)
            {
                Pose = new Pose(new Vector3F(0, 2, 0)),// Start Position
                UserData = "NoDraw",
            };

            //Create the vehicle
            Vehicle = new Vehicle(_simulation, chassis);

            //Add 4 wheels
            Vehicle.Wheels.Add(new Wheel { Offset = new Vector3F(-0.9f, 0.6f, -2.0f), Radius = 0.36f, SuspensionRestLength = 0.55f, MinSuspensionLength = 0.25f, Friction = 2 });  // Front left
            Vehicle.Wheels.Add(new Wheel { Offset = new Vector3F(0.9f, 0.6f, -2.0f), Radius = 0.36f, SuspensionRestLength = 0.55f, MinSuspensionLength = 0.25f, Friction = 2 });   // Front right
            Vehicle.Wheels.Add(new Wheel { Offset = new Vector3F(-0.9f, 0.6f, 0.98f), Radius = 0.36f, SuspensionRestLength = 0.55f, MinSuspensionLength = 0.25f, Friction = 1.8f });// Back left
            Vehicle.Wheels.Add(new Wheel { Offset = new Vector3F(0.9f, 0.6f, 0.98f), Radius = 0.36f, SuspensionRestLength = 0.55f, MinSuspensionLength = 0.25f, Friction = 1.8f }); // Back right

            Vehicle.Enabled = false;
        }

        #endregion

        //--------------------------------------------------------------
        #region Overridden Methods

        protected override void OnLoad()
        {
            Vehicle.Enabled = true;

            //Add graphics model to scene graph.
            var scene = _services.GetInstance<IScene>();
            scene.Children.Add(_vehicleModelNode);
        }

        protected override void OnUnload()
        {
            Vehicle.Enabled = false;

            //remove graphics model from scene graph
            _vehicleModelNode.Parent.Children.Remove(_vehicleModelNode);
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (_inputService.EnableMouseCentering)
            {
                _currentAction = _emoEngine.CurrentCognitivAction();
                float deltaTimeF = (float) deltaTime.TotalSeconds;

                //Update steering direction from left/right movement
                UpdateSteeringAngle(deltaTimeF);

                //Update acceleration from up/down arrow keys.
                //If the vehicle is not accelerating or at top speed, the brakes
                //will automatically be turned on
                UpdateAcceleration(deltaTimeF);
//                if (UpdateAcceleration(deltaTimeF))
//                {
//                    const float brakeForce = 6000;
//                    Vehicle.Wheels[2].MotorForce = 0;
//                    Vehicle.Wheels[3].MotorForce = 0;
//                    Vehicle.Wheels[2].BrakeForce = brakeForce;
//                    Vehicle.Wheels[3].BrakeForce = brakeForce;
//                }
//                else
//                {
//                    Vehicle.Wheels[2].BrakeForce = 0;
//                    Vehicle.Wheels[3].BrakeForce = 0;
//                }

                //Update poses of graphics models
                _vehicleModelNode.SetLastPose(true);
                _vehicleModelNode.PoseWorld = Vehicle.Chassis.Pose;
                for (int i = 0; i < _wheelModelNodes.Length; i++)
                {
                    var pose = Vehicle.Wheels[i].Pose;
                    if (Vehicle.Wheels[i].Offset.X < 0)
                    {
                        pose.Orientation = pose.Orientation * Matrix33F.CreateRotationY(ConstantsF.Pi);
                    }
                    _wheelModelNodes[i].SetLastPose(true);
                    _wheelModelNodes[i].PoseWorld = pose;
                }
            }
        }

        #endregion

        //--------------------------------------------------------------
        #region Private Methods

        private void UpdateSteeringAngle(float deltaTime)
        {
            const float MaxAngle = 0.5f;
            const float SteeringRate = 3.25f;

            //Limit the amount of change per frame.
            float change = SteeringRate*deltaTime;

            float direction = 0;
            if (_allowedActions.Contains(_currentAction))
            {
                if (_currentAction == EdkDll.EE_CognitivAction_t.COG_LEFT)
                    direction += 1;
                else if (_currentAction == EdkDll.EE_CognitivAction_t.COG_RIGHT)
                    direction -= 1;
            }

            if (direction == 0)
            {
                //Steer back to neutral position (angle 0)
                if (_steeringAngle > 0)
                    _steeringAngle = MathHelper.Clamp(_steeringAngle - change, 0, +MaxAngle);
                else if (_steeringAngle < 0)
                    _steeringAngle = MathHelper.Clamp(_steeringAngle + change, -MaxAngle, 0);
            }
            else
            {
                //Increase steering angle
                _steeringAngle = MathHelper.Clamp(_steeringAngle + direction*change, -MaxAngle, +MaxAngle);
            }

            Vehicle.SetCarSteeringAngle(_steeringAngle, Vehicle.Wheels[0], Vehicle.Wheels[1], Vehicle.Wheels[2], Vehicle.Wheels[3]);
        }

        //returns false if not accelerating
        private bool UpdateAcceleration(float deltaTime)
        {
            bool brake = false;
            const float MaxForce = 2000;
            const float AccelerationRate = 10000;

            //Limit the amount of change per frame

            float change = AccelerationRate*deltaTime;

            float direction = 0;
//            if (_allowedActions.Contains(_currentAction))
//            {
//                if (_currentAction == EdkDll.EE_CognitivAction_t.COG_PUSH
//                    || _currentAction == EdkDll.EE_CognitivAction_t.COG_LEFT
//                    || _currentAction == EdkDll.EE_CognitivAction_t.COG_RIGHT)
                if(_inputService.IsDown(Keys.W))
                {
                    direction += 1;
                }
                else if (_currentAction == EdkDll.EE_CognitivAction_t.COG_PULL)
                    direction -= 1;
//            }

            if (direction == 0)
            {
                //No acceleratoin Bring motor frce down to 0;
                brake = true;
                if (_motorForce > 0)
                    _motorForce = MathHelper.Clamp(_motorForce - change, 0, +MaxForce);
                else if (_motorForce < 0)
                    _motorForce = MathHelper.Clamp(_motorForce + change, -MaxForce, 0);
            }
            else
            {
                //Increase motor force
                brake = false;
                _motorForce = MathHelper.Clamp(_motorForce + direction*change, -MaxForce, +MaxForce);
            }

            //Motorize each wheel
            Vehicle.Wheels[0].MotorForce = _motorForce;
            Vehicle.Wheels[1].MotorForce = _motorForce;
            Vehicle.Wheels[2].MotorForce = _motorForce;
            Vehicle.Wheels[3].MotorForce = _motorForce;

            return brake;
        }

        #endregion
    }
}