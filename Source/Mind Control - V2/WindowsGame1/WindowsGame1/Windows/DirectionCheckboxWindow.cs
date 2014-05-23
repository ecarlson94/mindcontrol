using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WindowsGame1.Managers;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using Emotiv;

namespace WindowsGame1.Windows
{
    public class DirectionCheckboxWindow : Window
    {
        private EmoEngineManager _emoEngine;

        public List<EdkDll.EE_CognitivAction_t> AllowedActions { get; private set; }

        public DirectionCheckboxWindow(EmoEngineManager emoEngine)
        {
            _emoEngine = emoEngine;

            AllowedActions = new List<EdkDll.EE_CognitivAction_t>();
            Name = "DirectionCheckboxWindow";
            Title = "Choose Directions";
            CloseButtonStyle = String.Empty;

            CreateGUI();
        }

        private void CreateGUI()
        {
            var stackPanel = new StackPanel()
            {
                Name = "DirectionPanel",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Vertical,
                Margin = new Vector4F(5),
            };
            Content = stackPanel;

            var message = new TextBlock
            {
                Text = "Choose directions to practice.",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(4),
            };
            stackPanel.Children.Add(message);

            var forwardCheck = new CheckBox
            {
                Margin = new Vector4F(2),
                Content = new TextBlock{ Text = "Forward" },
                IsEnabled = _emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_PUSH),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            forwardCheck.Click += (s, e) =>
            {
                lock (AllowedActions)
                {
                    if (AllowedActions.Contains(EdkDll.EE_CognitivAction_t.COG_PUSH))
                        AllowedActions.Remove(EdkDll.EE_CognitivAction_t.COG_PUSH);
                    else
                        AllowedActions.Add(EdkDll.EE_CognitivAction_t.COG_PUSH);
                }
            };
            stackPanel.Children.Add(forwardCheck);

            var backwardCheck = new CheckBox
            {
                Margin = new Vector4F(2),
                Content = new TextBlock { Text = "Backward" },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                IsEnabled = _emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_PULL),
            };
            backwardCheck.Click += (s, e) =>
            {
                lock (AllowedActions)
                {
                    if (AllowedActions.Contains(EdkDll.EE_CognitivAction_t.COG_PULL))
                        AllowedActions.Remove(EdkDll.EE_CognitivAction_t.COG_PULL);
                    else
                        AllowedActions.Add(EdkDll.EE_CognitivAction_t.COG_PULL);
                }
            };
            stackPanel.Children.Add(backwardCheck);

            var leftCheck = new CheckBox
            {
                Margin = new Vector4F(2),
                Content = new TextBlock { Text = "Turn Left" },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                IsEnabled = _emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_LEFT),
            };
            leftCheck.Click += (s, e) =>
            {
                lock (AllowedActions)
                {
                    if (AllowedActions.Contains(EdkDll.EE_CognitivAction_t.COG_LEFT))
                        AllowedActions.Remove(EdkDll.EE_CognitivAction_t.COG_LEFT);
                    else
                        AllowedActions.Add(EdkDll.EE_CognitivAction_t.COG_LEFT);
                }
            };
            stackPanel.Children.Add(leftCheck);

            var rightCheck = new CheckBox
            {
                Margin = new Vector4F(2),
                Content = new TextBlock { Text = "Turn Right" },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                IsEnabled = _emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_RIGHT),
            };
            rightCheck.Click += (s, e) =>
            {
                lock (AllowedActions)
                {
                    if (AllowedActions.Contains(EdkDll.EE_CognitivAction_t.COG_RIGHT))
                        AllowedActions.Remove(EdkDll.EE_CognitivAction_t.COG_RIGHT);
                    else
                        AllowedActions.Add(EdkDll.EE_CognitivAction_t.COG_RIGHT);
                }
            };
            stackPanel.Children.Add(rightCheck);

            var okButton = new Button
            {
                Margin = new Vector4F(3),
                Content = new TextBlock{ Text = "Ok" },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            okButton.Click += (s, e) => Close();
            stackPanel.Children.Add(okButton);
        }
    }
}
