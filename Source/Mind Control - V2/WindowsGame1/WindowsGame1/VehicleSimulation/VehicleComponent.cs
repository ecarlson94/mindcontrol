using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WindowsGame1.Components;
using WindowsGame1.Managers;
using WindowsGame1.Windows;
using WindowsGame1.Screens;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Graphics;
using DigitalRune.Physics.ForceEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using WindowsGame1.BackgroundObjects;
using Microsoft.Xna.Framework.Input;

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

            _graphicsScreen = new DelegateGraphicsScreen(GraphicsService)
            {
                RenderCallback = Render,
            };
            GraphicsService.Screens.Insert(0, _graphicsScreen);


            CreateGUI();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Remove graphics screen from graphics service
                GraphicsService.Screens.Remove(_graphicsScreen);

                DisposeGUI();
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

            LoadTheme();

            _directionCheck = new DirectionCheckboxWindow(EmoEngine);
            _directionCheck.Closing += DirectionCheckOnClosing;
            _directionCheck.Show(_uiGraphicsScreen.UIScreen);
        }

        private void DirectionCheckOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            var actions = (sender as DirectionCheckboxWindow).AllowedActions;
            if (actions.Count() > 0)
            {
                //Add basic force effects
                Simulation.ForceEffects.Add(new Gravity());
                Simulation.ForceEffects.Add(new Damping());

                //Add the sky and ground objects here
                GameObjectService.Objects.Add(new Sky(Services));
                GameObjectService.Objects.Add(new Ground(Services));

                //Add the game object which controls a vehicle here
                _cognitivVehicle = new CognitivVehicle(Services, EmoEngine, actions);
                GameObjectService.Objects.Add(_cognitivVehicle);

                //Add the camera object attached to chassis of the vehicle
                _vehicleCamera = new VehicleCamera(_cognitivVehicle.Vehicle.Chassis, Services);
                GameObjectService.Objects.Add(_vehicleCamera);
                GraphicsScreen.CameraNode3D = _vehicleCamera.CameraNode;

                DisposeGUI();
            }
            else
                cancelEventArgs.Cancel = true;
        }

        private void DisposeGUI()
        {
            if (_uiGraphicsScreen != null)
            {
                GraphicsService.Screens.Remove(_uiGraphicsScreen);
                _uiGraphicsScreen.Dispose();
            }
        }

        private void Render(RenderContext context)
        {
            if (_uiScreen != null)
                _uiScreen.Draw(context.DeltaTime);
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