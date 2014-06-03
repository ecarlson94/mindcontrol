using DigitalRune;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Graphics;
using Emotiv;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using WindowsGame1.Managers;
using WindowsGame1.RCCar;
using WindowsGame1.Windows;

namespace WindowsGame1.Components
{
    public class RCCarComponent : BaseComponent
    {
        //--------------------------------------------------------------
        #region Fields

        //UI
        private readonly DelegateGraphicsScreen _graphicsScreen;
        private UIScreen _uiScreen;
        private TextBlock _rcCarTextBlock;

        //USB Relay interface
        private USBRelay _relay;
        //RC Car actions
        private IEnumerable<EdkDll.EE_CognitivAction_t> _allowedActions;

        //EmoEngine fields
        private EdkDll.EE_CognitivAction_t _currentAction;

        #endregion

        //--------------------------------------------------------------
        #region Properties

        public bool Exit { get; private set; }

        #endregion

        //--------------------------------------------------------------
        #region Creation

        public RCCarComponent(Game game, EmoEngineManager emoEngine, IEnumerable<EdkDll.EE_CognitivAction_t> allowedActions)
            : base(game, emoEngine)
        {
            _allowedActions = allowedActions;
            RestrictDirections(_allowedActions);
            _relay = new USBRelay();

            EnableMouseCentering = false;

            _graphicsScreen = new DelegateGraphicsScreen(GraphicsService)
            {
                RenderCallback = Render,
            };
            GraphicsService.Screens.Insert(0, _graphicsScreen);

            CreateGUI();
        }

        #endregion

        //--------------------------------------------------------------
        #region Private Methods

        private void CreateGUI()
        {
            LoadTheme();

            // Show USB Information
            _rcCarTextBlock = new TextBlock
            {
                Name = "Message",    // Control names are optional - but very helpful for debugging!
                Text = "Please plug in the USB Relay",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _uiScreen.Children.Add(_rcCarTextBlock);
        }

        private void LoadTheme()
        {
            Theme theme = UIContentManager.Load<Theme>("ThemeRed");
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
            _uiScreen.Draw(context.DeltaTime);
        }

        private void RestrictDirections(IEnumerable<EdkDll.EE_CognitivAction_t> allowedActions)
        {
            List<EdkDll.EE_CognitivAction_t> allActions = new List<EdkDll.EE_CognitivAction_t>();
            allActions.Add(EdkDll.EE_CognitivAction_t.COG_PUSH);
            allActions.Add(EdkDll.EE_CognitivAction_t.COG_PULL);
            allActions.Add(EdkDll.EE_CognitivAction_t.COG_LEFT);
            allActions.Add(EdkDll.EE_CognitivAction_t.COG_RIGHT);

            foreach (var action in allActions)
            {
                if(!allowedActions.Contains(action))
                    EmoEngine.SetCognitivActionInactive(action);
            }
        } 

        private void RcCarForward()
        {
            _relay.RelaySwitch(RelayNumber.One, RelayState.Off);
            _relay.RelaySwitch(RelayNumber.Two, RelayState.Off);
            _relay.RelaySwitch(RelayNumber.Three, RelayState.On);
            _relay.RelaySwitch(RelayNumber.Four, RelayState.Off);
        }

        private void RcCarBackward()
        {
            _relay.RelaySwitch(RelayNumber.One, RelayState.Off);
            _relay.RelaySwitch(RelayNumber.Two, RelayState.Off);
            _relay.RelaySwitch(RelayNumber.Three, RelayState.Off);
            _relay.RelaySwitch(RelayNumber.Four, RelayState.On);
        }

        private void RcCarTurnLeft()
        {
            _relay.RelaySwitch(RelayNumber.One, RelayState.On);
            _relay.RelaySwitch(RelayNumber.Two, RelayState.Off);
            _relay.RelaySwitch(RelayNumber.Three, RelayState.On);
            _relay.RelaySwitch(RelayNumber.Four, RelayState.Off);
        }

        private void RcCarTurnRight()
        {
            _relay.RelaySwitch(RelayNumber.One, RelayState.Off);
            _relay.RelaySwitch(RelayNumber.Two, RelayState.On);
            _relay.RelaySwitch(RelayNumber.Three, RelayState.On);
            _relay.RelaySwitch(RelayNumber.Four, RelayState.Off);
        }

        #endregion

        //--------------------------------------------------------------
        #region Overridden Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Remove UIScreen from UIService
                UIService.Screens.Remove(_uiScreen);

                // Remove graphics screen from graphics service
                GraphicsService.Screens.Remove(_graphicsScreen);

                //Dispose the _relay
                _relay.SafeDispose();

                EmoEngine.SetAllTrainedCognitivActionActive();
            }
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            if (InputService.IsPressed(Keys.Escape, true))
            {
                Exit = true;
            }
            else
            {
                _currentAction = EmoEngine.CurrentCognitivAction();
                _rcCarTextBlock.Text = _relay.USBConnected ? "Have Fun!!!!" : "Please connect the USB relay to the computer...";
                if (_relay.USBConnected && _allowedActions.Contains(_currentAction))
                {
                    // Turn on relays here
                    switch (_currentAction)
                    {
                        case EdkDll.EE_CognitivAction_t.COG_PUSH:
                            RcCarForward();
                            break;
                        case EdkDll.EE_CognitivAction_t.COG_PULL:
                            RcCarBackward();
                            break;
                        case EdkDll.EE_CognitivAction_t.COG_LEFT:
                            RcCarTurnLeft();
                            break;
                        case EdkDll.EE_CognitivAction_t.COG_RIGHT:
                            RcCarTurnRight();
                            break;
                    }
                }
                else
                {
                    // Turn off all Relays here
                    _relay.TurnOffAllRelays();
                }
            }
            base.Update(gameTime);
        }

        #endregion
    }
}