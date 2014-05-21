using DigitalRune.Game.UI;
using DigitalRune.Graphics;
using DigitalRune.Physics.ForceEffects;
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

        private void LoadAssets()
        {
            //Add basic force effects
            Simulation.ForceEffects.Add(new Gravity());
            Simulation.ForceEffects.Add(new Damping());

            //Add the sky and ground objects here
            GameObjectService.Objects.Add(new Sky(Services));
            GameObjectService.Objects.Add(new Ground(Services));

            //Add the camera object attached to chassis of the vehicle
            _vehicleCamera = new VehicleCamera(_cognitivVehicle.Vehicle.Chassis, Services);
            GameObjectService.Objects.Add(_vehicleCamera);
            GraphicsScreen.CameraNode3D = _vehicleCamera.CameraNode;

            GraphicsService.Screens.Remove(_uiGraphicsScreen);
            _uiGraphicsScreen.Dispose();
        }

        private void DirectionCheckOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            var actions = (sender as DirectionCheckboxWindow).AllowedActions;
            if (actions.Any())
            {
                //Add the game object which controls a vehicle here
                _cognitivVehicle = new CognitivVehicle(Services, EmoEngine, actions);
                GameObjectService.Objects.Add(_cognitivVehicle);

                GraphicsScreen.DrawReticle = true;
                EnableMouseCentering = true;
            }
            else
                cancelEventArgs.Cancel = true;
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