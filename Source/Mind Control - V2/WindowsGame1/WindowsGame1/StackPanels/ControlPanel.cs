using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using Emotiv;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using WindowsGame1.Enums;
using WindowsGame1.Managers;
using WindowsGame1.Windows;

namespace WindowsGame1.StackPanels
{
    public class ControlPanel : StackPanel
    {
        private EmoEngineManager emoEngine;
        private bool makePracticeMenu;
        private bool makeMainMenu;
        private Button practiceButton;
        private Button settings;
        private Button contactQuality;
        private Button rcCarButton;
        public MenuState MenuState { get; set; }
        private ContactQualityWindow cqWindow;
        private IServiceLocator _services;

        public ControlPanel(EmoEngineManager emoEngine, IServiceLocator services)
        {
            _services = services;
            this.emoEngine = emoEngine;
            Initialize();
        }

        /*
         * Out of the universe, time, space, everything and everyone, Would you go to Senior Ball with me?
         */

        protected override void OnLoad()
        {
            Screen.Children.Add(cqWindow);
            base.OnLoad();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (MenuState != MenuState.Practice)
            {
                bool headsetOn = emoEngine.HeadsetOn();
                bool headsetOnHead = emoEngine.HeadsetOnHead();
                bool allCognitivActionsTrained = emoEngine.AllCognitivActionsTrained();
                bool goodContactQuality = emoEngine.OverallGoodQuality();

                if (MenuState == MenuState.Main)
                {
                    practiceButton.IsEnabled = headsetOn &&
                                               emoEngine.IsCognitivActionTrained(EdkDll.EE_CognitivAction_t.COG_NEUTRAL)
                                               && headsetOnHead && goodContactQuality;

                    rcCarButton.IsEnabled = headsetOn && allCognitivActionsTrained
                                            && headsetOnHead && goodContactQuality;

                    settings.IsEnabled = headsetOn
                                         && headsetOnHead && goodContactQuality;
                }

                if (emoEngine.Profile != String.Empty && !(headsetOn && headsetOnHead && goodContactQuality))
                {
                    if (!Screen.Children.OfType<Window>().ToArray().Contains(cqWindow))
                    {
                        CloseWindows();
                        Screen.Children.Add(cqWindow);
                    }
                }

                base.OnUpdate(deltaTime);
            }
        }

        private void Initialize()
        {
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Left;
            Orientation = Orientation.Vertical;
            Width = 200;
            cqWindow = new ContactQualityWindow(emoEngine, _services);
            MakeMainMenu();
        }

        private void MakeMainMenu()
        {
            Children.Clear();
            MenuState = MenuState.Main;

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
            practiceButton.Click += (s, e) => MenuState = MenuState.Practice;

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
                //TODO: implement remote control car funtionality
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
                Screen.Children.Add(cqWindow);
            };

            Children.Add(practiceButton);
            Children.Add(rcCarButton);
            Children.Add(settings);
            Children.Add(contactQuality);
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