using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WindowsGame1.Components;
using WindowsGame1.Managers;
using WindowsGame1.Windows;
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
        private UIScreen _uiScreen;

        private CognitivVehicle _cognitivVehicle;
        private VehicleCamera _vehicleCamera;
        //scene goes here...

        #endregion

        //--------------------------------------------------------------
        #region Creation and Cleanup

        public VehicleComponent(Game game, EmoEngineManager emoEngine)
            : base(game, emoEngine)
        {
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
                // Remove UIScreen from UIService
                UIService.Screens.Remove(_uiScreen);

                //Dispose the current ContentManager
                _uiContentManager.Dispose();

                // Remove graphics screen from graphics service
                GraphicsService.Screens.Remove(_graphicsScreen);
            }
            base.Dispose(disposing);
        }

        #endregion

        //--------------------------------------------------------------
        #region Private Methods

        private void CreateGUI()
        {
            // Dispose old UI.
            if (_uiContentManager != null)
            {
                _uiContentManager.Dispose();
                UIService.Screens.Remove(_uiScreen);
            }

            LoadTheme();

            var directionCheck = new DirectionCheckboxWindow(EmoEngine);
            directionCheck.Closing += DirectionCheckOnClosing;
            _uiScreen.Children.Add(directionCheck);
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
            if (_uiContentManager != null)
            {
                _uiContentManager.Dispose();
                UIService.Screens.Remove(_uiScreen);
            }
        }

        private void LoadTheme()
        {
            // Load a UI theme, which defines the appearance and default values of UI controls.
            _uiContentManager = new ContentManager(Game.Services, "NeoforceTheme");
            Theme theme = _uiContentManager.Load<Theme>("ThemeRed");
            // Create a UI renderer, which uses the theme info to renderer UI controls.
            UIRenderer renderer = new UIRenderer(Game, theme);

            // Create a UIScreen and add it to the UI service. The screen is the root of 
            // the tree of UI controls. Each screen can have its own renderer. 
            _uiScreen = new UIScreen("Default", renderer)
            {
                // Make the screen transparent.
                Background = new Color(0, 0, 0, 0),
            };

            // Add the screen to the UI service.
            UIService.Screens.Add(_uiScreen);
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
                Game.Components.Remove(this);
                Dispose();
            }
            base.Update(gameTime);
        }

        #endregion
    }
}