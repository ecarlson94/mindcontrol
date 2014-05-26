﻿using DigitalRune.Physics.ForceEffects;
using Emotiv;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using WindowsGame1.BackgroundObjects;
using WindowsGame1.Components;
using WindowsGame1.Managers;
using WindowsGame1.Windows;

namespace WindowsGame1.VehicleSimulation
{
    public class VehicleComponent : GraphicsBaseComponent
    {
        //--------------------------------------------------------------
        #region Fields

        private CognitivVehicle _cognitivVehicle;
        private VehicleCamera _vehicleCamera;
        //scene goes here...

        private DirectionCheckboxWindow _directionCheck;

        #endregion

        //--------------------------------------------------------------
        #region Creation and Cleanup

        public VehicleComponent(Game game, EmoEngineManager emoEngine, IEnumerable<EdkDll.EE_CognitivAction_t> allowedActions)
            : base(game, emoEngine)
        {
            RemoveBaseComponents();

            //Add basic force effects
            Simulation.ForceEffects.Add(new Gravity());
            Simulation.ForceEffects.Add(new Damping());

            //Add the sky and ground objects here
            GameObjectService.Objects.Add(new Sky(Services));
            GameObjectService.Objects.Add(new Ground(Services));

            //Add the game object which controls a vehicle here
            _cognitivVehicle = new CognitivVehicle(Services, EmoEngine, allowedActions);
            GameObjectService.Objects.Add(_cognitivVehicle);

            //Add the camera object attached to chassis of the vehicle
            _vehicleCamera = new VehicleCamera(_cognitivVehicle.Vehicle.Chassis, Services);
            GameObjectService.Objects.Add(_vehicleCamera);
            GraphicsScreen.CameraNode3D = _vehicleCamera.CameraNode;

            GraphicsScreen.DrawReticle = true;
            EnableMouseCentering = true;
            RestrictDirections(allowedActions);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GameObjectService.Objects.Remove(_cognitivVehicle);
                GameObjectService.Objects.Remove(_vehicleCamera);
                // Remove graphics screen from graphics service
            }
            base.Dispose(disposing);
        }

        #endregion

        //--------------------------------------------------------------
        #region Private Methods

        private void RestrictDirections(IEnumerable<EdkDll.EE_CognitivAction_t> allowedActions)
        {
            List<EdkDll.EE_CognitivAction_t> allActions = new List<EdkDll.EE_CognitivAction_t>();
            allActions.Add(EdkDll.EE_CognitivAction_t.COG_PUSH);
            allActions.Add(EdkDll.EE_CognitivAction_t.COG_PULL);
            allActions.Add(EdkDll.EE_CognitivAction_t.COG_LEFT);
            allActions.Add(EdkDll.EE_CognitivAction_t.COG_RIGHT);

            foreach (var action in allowedActions)
            {
                if (!allowedActions.Contains(action))
                    EmoEngine.SetCognitivActionInactive(action);
            }
        } 

        #endregion

        //--------------------------------------------------------------
        #region Overridden Methods

        public override void Update(GameTime gameTime)
        {
            if (InputService.IsPressed(Keys.Escape, true))
            {
                Game.Components.Add(new MenuComponent(Game, EmoEngine));
            }
            base.Update(gameTime);
        }

        #endregion
    }
}