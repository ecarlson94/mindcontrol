using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WindowsGame1.Enums;
using WindowsGame1.Managers;
using WindowsGame1.Windows;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Mathematics.Algebra;
using Emotiv;

namespace WindowsGame1.StackPanels
{
    public class ControlPanel : StackPanel
    {
        private EmoEngineManager emoEngine;
        private bool makePracticeMenu = false;
        private bool makeMainMenu = false;
        private Button practiceButton;
        private Button settings;
        private Button contactQuality;
        private Button rcCarButton;
        private Button forwardBackward;
        private Button leftRight;
        private Button allDirections;
        private MenuState menu;

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

            bool headsetOn = emoEngine.HeadsetOn();
            bool headsetOnHead = emoEngine.HeadsetOnHead();
            bool allCognitivActionsTrained = emoEngine.AllCognitivActionsTrained();

            if (menu == MenuState.Main)
            {
                practiceButton.IsEnabled = headsetOn &&
                                           emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_NEUTRAL);//TODO: && headsetOnHead;
                rcCarButton.IsEnabled = headsetOn && allCognitivActionsTrained;//TODO: && headsetOnHead;
                settings.IsEnabled = headsetOn;//TODO: && headsetOnHead;
            }else if (menu == MenuState.Practice)
            {
                forwardBackward.IsEnabled = headsetOn &&
                                            (emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_PUSH)
                                             || emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_PULL));
                leftRight.IsEnabled = headsetOn &&
                                      (emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_LEFT)
                                      || emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_RIGHT));

                allDirections.IsEnabled = headsetOn &&
                                          emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_PUSH) &&
                                          emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_PULL) &&
                                          emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_LEFT) &&
                                          emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_RIGHT);
            }

            base.OnUpdate(deltaTime);
        }

        private void Initialize()
        {
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Left;
            Orientation = Orientation.Vertical;
            Width = 200;
            MakeMainMenu();
            Screen.Children.Add(new ContactQualityWindow(emoEngine));
        }

        private void MakeMainMenu()
        {
            Children.Clear();
            menu = MenuState.Main;

            practiceButton = new Button
            {
                Name = "Practice",
                Content = new TextBlock { Text = "Practice" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                IsEnabled = emoEngine.HeadsetOn() && emoEngine.HeadsetOnHead() && emoEngine.AllCognitivActionsTrained(),
                Focusable = false,
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
                IsEnabled = emoEngine.HeadsetOn() && emoEngine.HeadsetOnHead() && emoEngine.AllCognitivActionsTrained(),
                Focusable = false,
                FocusWhenMouseOver = false,
            };
            rcCarButton.Click += (s, e) =>
            {
                //do stuff here
            };

            settings = new Button
            {
                Name = "Settings",
                Content = new TextBlock { Text = "Settings" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                IsEnabled = emoEngine.HeadsetOn(),
                Focusable = false,
                FocusWhenMouseOver = false,
            };
            settings.Click += (s, e) =>
            {
                CloseWindows();
                Screen.Children.Add(new Settings(emoEngine));
            };

            contactQuality = new Button
            {
                Name = "ContactQuality",
                Content = new TextBlock { Text = "Contact Quality" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                Focusable = false,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            contactQuality.Click += (s, e) =>
            {
                CloseWindows();
                Screen.Children.Add(new ContactQualityWindow(emoEngine));
            };

            Children.Add(practiceButton);
            Children.Add(rcCarButton);
            Children.Add(settings);
            Children.Add(contactQuality);
        }

        private void MakePracticeMenu()
        {
            Children.Clear();
            menu = MenuState.Practice;

            forwardBackward = new Button
            {
                Name = "ForwardBackward",
                Content = new TextBlock { Text = "Forward/Backward" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                Focusable = false,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            forwardBackward.Click += (s, e) =>
            {

            };

            leftRight = new Button
            {
                Name = "LeftRight",
                Content = new TextBlock { Text = "Left/Right" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                Focusable = false,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            leftRight.Click += (s, e) =>
            {

            };

            allDirections = new Button
            {
                Name = "AllDirections",
                Content = new TextBlock { Text = "All Directions" },
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                Focusable = false,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            allDirections.Click += (s, e) =>
            {

            };

            var backButton = new Button
            {
                Name = "Back",
                Content = new TextBlock{Text = "Back"},
                Margin = new Vector4F(10),
                RenderScale = new Vector2F(1.50f),
                Focusable = false,
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