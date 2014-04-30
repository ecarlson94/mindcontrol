using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using Emotiv;

namespace WindowsGame1.StackPanels
{
    public class TrainingPanel : StackPanel
    {
        private EmoEngineManager emoEngine;
        private StackPanel bottomPanel;
        private readonly EdkDll.EE_CognitivAction_t trainingAction;
        private Button trainButton;
        private Button eraseButton;

        public TrainingPanel(EmoEngineManager emoEngineParam, EdkDll.EE_CognitivAction_t trainingActionParam)
        {
            emoEngine = emoEngineParam;
            trainingAction = trainingActionParam;
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
            //figure out spacing
        }

        private void BottomPanel()
        {
            bottomPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal,
            };
            Children.Add(bottomPanel);

            trainButton = new Button
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
            trainButton.Click += (s, e) => emoEngine.StartCognitivTraining(trainingAction);
            bottomPanel.Children.Add(trainButton);

            eraseButton = new Button
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
            eraseButton.Click += (s, e) => emoEngine.EraseCognitivTraining(trainingAction);
            bottomPanel.Children.Add(eraseButton);
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (Name.Equals("Neutral"))
            {
                trainButton.IsEnabled = !emoEngine.IsTraining;
            }
            else
            {
                trainButton.IsEnabled = emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_NEUTRAL) &&
                    !emoEngine.IsTraining;
            }

            eraseButton.IsEnabled = emoEngine.IsCognitivActionTrained(trainingAction) && !emoEngine.IsTraining;

            base.OnUpdate(deltaTime);
        }
    }
}