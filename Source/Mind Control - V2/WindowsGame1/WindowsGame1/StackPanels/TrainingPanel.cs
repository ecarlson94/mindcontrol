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
        private EdkDll.EE_CognitivAction_t trainingAction;

        public TrainingPanel(EmoEngineManager emoEngineParam, EdkDll.EE_CognitivAction_t trainingActionParam)
        {
            emoEngine = emoEngineParam;
            trainingAction = trainingActionParam;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            Orientation = Orientation.Vertical;
            Margin = new Vector4F(5);

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


        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (!Name.Equals("Neutral"))
            {
                foreach (UIControl control in bottomPanel.Children)
                {
                    control.IsEnabled = emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_NEUTRAL);
                }
            }

            base.OnUpdate(deltaTime);
        }
    }
}
