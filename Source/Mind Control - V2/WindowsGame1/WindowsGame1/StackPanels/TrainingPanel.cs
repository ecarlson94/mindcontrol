using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using Emotiv;
using Microsoft.Xna.Framework;
using System;
using WindowsGame1.Managers;

namespace WindowsGame1.StackPanels
{
    public class TrainingPanel : StackPanel
    {
        private EmoEngineManager _emoEngine;
        private StackPanel _bottomPanel;
        private readonly EdkDll.EE_CognitivAction_t _trainingAction;
        private Button _trainButton;
        private Button _eraseButton;
        private bool _isTraining;

        public TrainingPanel(EmoEngineManager emoEngineParam, EdkDll.EE_CognitivAction_t trainingActionParam)
        {
            _emoEngine = emoEngineParam;
            _trainingAction = trainingActionParam;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            Orientation = Orientation.Vertical;
            Margin = new Vector4F(20);
            RenderScale = new Vector2F(1.5f);

            switch (trainingActionParam)
            {
                    case EdkDll.EE_CognitivAction_t.COG_NEUTRAL:
                        Name = "Neutral";
                        break;
                    case EdkDll.EE_CognitivAction_t.COG_PULL:
                        Name = "Backward";
                        break;
                    case EdkDll.EE_CognitivAction_t.COG_PUSH:
                        Name = "Forward";
                        break;
                    case EdkDll.EE_CognitivAction_t.COG_LEFT:
                        Name = "Turn Left";
                        break;
                    case EdkDll.EE_CognitivAction_t.COG_RIGHT:
                        Name = "Turn Right";
                        break;
            }

            Initialize();
        }

        private void Initialize()
        {
            TopPanel();
            BottomPanel();
            _emoEngine.CognitivTrainingCompleted += EmoEngineOnCognitivTrainingCompleted;
        }

        private void TopPanel()
        {
            var topPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal,
            };
            Children.Add(topPanel);

            var name = new TextBlock
            {
                Text = Name,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3),
                RenderScale = new Vector2F(1.25f),
            };
            topPanel.Children.Add(name);

            //add image here
        }

        private void BottomPanel()
        {
            _bottomPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal,
            };
            Children.Add(_bottomPanel);

            _trainButton = new Button
            {
                Name = "TrainButton",
                Content = new TextBlock { Text = "Train" },
                Margin = new Vector4F(4),
                //RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Focusable = false,
                FocusWhenMouseOver = false,
            };
            _trainButton.Click += (s, e) =>
            {
                _emoEngine.StartCognitivTraining(_trainingAction);
                _isTraining = true;
            };
            _bottomPanel.Children.Add(_trainButton);

            _eraseButton = new Button
            {
                Name = "EraseButton",
                Content = new TextBlock { Text = "Erase Data" },
                Margin = new Vector4F(4),
                //RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Focusable = false,
                FocusWhenMouseOver = false,
            };
            _eraseButton.Click += (s, e) => _emoEngine.EraseCognitivTraining(_trainingAction);
            _bottomPanel.Children.Add(_eraseButton);
        }

        private void EmoEngineOnCognitivTrainingCompleted(object sender, EmoEngineEventArgs emoEngineEventArgs)
        {
            _isTraining = false;
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (Name.Equals("Neutral"))
            {
                _trainButton.IsEnabled = !_emoEngine.IsTraining; //TODO: // && headsetOnHead;
            }
            else
            {
                _trainButton.IsEnabled = _emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_NEUTRAL) &&
                    !_emoEngine.IsTraining;//TODO: && headsetOnHead;
            }

            Background = _isTraining ? new Color(18, 172, 219, 125) : new Color(0, 0, 0, 0);

            _eraseButton.IsEnabled = _emoEngine.IsCognitivActionTrained(_trainingAction) && !_emoEngine.IsTraining;

            base.OnUpdate(deltaTime);
        }
    }
}