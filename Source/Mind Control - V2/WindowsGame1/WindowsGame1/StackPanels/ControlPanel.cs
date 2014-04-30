using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using WindowsGame1.Windows;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Mathematics.Algebra;

namespace WindowsGame1.StackPanels
{
    public class ControlPanel : StackPanel
    {
        private EmoEngineManager emoEngine;
        private bool makePracticeMenu = false;
        private bool makeMainMenu = true;
        private bool settingsClicked = false;
        private Button practiceButton;
        private Button settings;
        private Button contactQuality;
        private Button rcCarButton;
        private Button forwardBackward;
        private Button leftRight;
        private Button allDirections;

        public ControlPanel(EmoEngineManager emoEngine)
        {
            this.emoEngine = emoEngine;
            Initialize();
        }

        protected override void OnLoad()
        {
            //take care of global buttons here
            base.OnLoad();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (makeMainMenu)
            {
                MakeMainMenu();
                makeMainMenu = false;
            }
            else if(makePracticeMenu)
            {
                MakePracticeMenu();
                makePracticeMenu = false;
            }
            else if (settingsClicked)
            {
                CloseWindows();
                Screen.Children.Add(new Settings(emoEngine));
            }

            base.OnUpdate(deltaTime);
        }

        private void Initialize()
        {
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Left;
            Orientation = Orientation.Vertical;
            Width = 200;
        }

        private void MakeMainMenu()
        {
            Children.Clear();

            var practiceButton = new Button
            {
                Name = "Practice",
                Content = new TextBlock { Text = "Practice" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Focusable = emoEngine.HeadsetOn(),
                FocusWhenMouseOver = false,
            };
            practiceButton.Click += (s, e) => makePracticeMenu = true;

            rcCarButton = new Button
            {
                Name = "RCCAR",
                Content = new TextBlock { Text = "RC Car" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Focusable = emoEngine.HeadsetOn(),
                FocusWhenMouseOver = false,
            };
            rcCarButton.Click += (s, e) =>
            {

            };

            settings = new Button
            {
                Name = "Settings",
                Content = new TextBlock { Text = "Settings" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Focusable = emoEngine.HeadsetOn(),
                FocusWhenMouseOver = false,
            };
            settings.Click += (s, e) =>
            {
                if (emoEngine.HeadsetOn() && emoEngine.HeadsetOnHead())
                {
                    settingsClicked = true;
                }
                else
                {
                    //display message box here.
                }
            };

            contactQuality = new Button
            {
                Name = "ContactQuality",
                Content = new TextBlock { Text = "Contact Quality" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            Children.Add(practiceButton);
            Children.Add(rcCarButton);
            Children.Add(settings);
            Children.Add(contactQuality);
        }

        private void MakePracticeMenu()
        {
            Children.Clear();

            forwardBackward = new Button
            {
                Name = "ForwardBackward",
                Content = new TextBlock { Text = "Forward/Backward" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            leftRight = new Button
            {
                Name = "LeftRight",
                Content = new TextBlock { Text = "Left/Right" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            allDirections = new Button
            {
                Name = "AllDirections",
                Content = new TextBlock { Text = "All Directions" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            var backButton = new Button
            {
                Name = "Back",
                Content = new TextBlock{Text = "Back"},
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            backButton.Click += (s, e) => makeMainMenu = true;

            Children.Add(forwardBackward);
            Children.Add(leftRight);
            Children.Add(allDirections);
            Children.Add(backButton);
        }

        private void CloseWindows()
        {
            var windows = Screen.Children.OfType<Window>().ToArray();

            foreach (Window window in windows)
            {
                window.Close();
            }
        }
    }
}