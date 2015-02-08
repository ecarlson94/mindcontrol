using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using WindowsGame1.Paints;
using WindowsGame1.Windows;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;

namespace WindowsGame1.StackPanels
{
    public class MainPanel : StackPanel
    {
        private Window rightWindow = null;
        private bool changeScreen = false;
        private EmoEngineManager emoEngine;

        public MainPanel(EmoEngineManager emoEngineParam)
        {
            this.emoEngine = emoEngineParam;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            Orientation = Orientation.Horizontal;
            Initialize();
        }

        private void Initialize()
        {
            ControlPanel control = new ControlPanel(emoEngine);
            //control.SettingsClicked += ControlOnSettingsClicked;
            Children.Add(control);

            //make custom window for contact quality before this call
            //Children.Add();
        }

        private void ControlOnSettingsClicked(object sender, EventArgs eventArgs)
        {
            //rightWindow = new Settings();
            changeScreen = true;
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (changeScreen)
            {
                if(Children[1] != null)
                    Children.RemoveAt(1);
                Children.Add(rightWindow);
            }
            base.OnUpdate(deltaTime);
        }
    }
}