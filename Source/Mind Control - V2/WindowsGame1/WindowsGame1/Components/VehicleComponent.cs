using System.Collections.Generic;
using DigitalRune.Game.UI;
using DigitalRune.Graphics;
using DigitalRune.Physics.ForceEffects;
using DigitalRune.Threading;
using Emotiv;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using System.Linq;
using WindowsGame1.BackgroundObjects;
using WindowsGame1.Components;
using WindowsGame1.Managers;
using WindowsGame1.Screens;
using WindowsGame1.Windows;

namespace WindowsGame1.VehicleSimulation
{
    public class VehicleComponent : GraphicsBaseComponent
    {
        //--------------------------------------------------------------
        #region Fields

        private readonly DelegateGraphicsScreen _graphicsScreen;
        private ContentManager _uiContentManager;
        private UIGraphicsScreen _uiGraphicsScreen;

        private CognitivVehicle _cognitivVehicle;
        private VehicleCamera _vehicleCamera;
        //scene goes here...

        private DirectionCheckboxWindow _directionCheck;

        #endregion

        //--------------------------------------------------------------
        #region Creation and Cleanup

        public VehicleComponent(Game game, EmoEngineManager emoEngine)
            : base(game, emoEngine)
        {
            RemoveBaseComponents();

            EnableMouseCentering = false;
            GraphicsScreen.DrawReticle = false;

            CreateGUI();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Remove graphics screen from graphics service
                GraphicsService.Screens.Remove(_uiGraphicsScreen);
                _uiGraphicsScreen.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        //--------------------------------------------------------------
        #region Private Methods

        private void CreateGUI()
        {

            _uiGraphicsScreen = new UIGraphicsScreen(Services);
            GraphicsService.Screens.Add(_uiGraphicsScreen);

            _directionCheck = new DirectionCheckboxWindow(EmoEngine)
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            _directionCheck.Closing += DirectionCheckOnClosing;
            _directionCheck.Show(_uiGraphicsScreen.UIScreen);
        }

        private void DirectionCheckOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            var actions = (sender as DirectionCheckboxWindow).AllowedActions;
            if (actions.Any())
            {
                _cognitivVehicle.AllowedActions = actions;

                //Add basic force effects
                Simulation.ForceEffects.Add(new Gravity());
                Simulation.ForceEffects.Add(new Damping());

                //Add the sky and ground objects here
                GameObjectService.Objects.Add(new Sky(Services));
                GameObjectService.Objects.Add(new Ground(Services));

                //Add the game object which controls a vehicle here
                _cognitivVehicle = new CognitivVehicle(Services, EmoEngine);
                GameObjectService.Objects.Add(_cognitivVehicle);

                //Add the camera object attached to chassis of the vehicle
                _vehicleCamera = new VehicleCamera(_cognitivVehicle.Vehicle.Chassis, Services);
                GameObjectService.Objects.Add(_vehicleCamera);
                GraphicsScreen.CameraNode3D = _vehicleCamera.CameraNode;

                GraphicsScreen.DrawReticle = true;
                EnableMouseCentering = true;
                RestrictDirections(actions);
                GraphicsService.Screens.Remove(_uiGraphicsScreen);
                //_uiGraphicsScreen.Dispose();
            }
            else
                cancelEventArgs.Cancel = true;
        }

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