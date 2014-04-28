using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;

namespace WindowsGame1.Windows
{
    public class Settings : Window
    {
        private EmoEngineManager emoEngine;

        public Settings(EmoEngineManager emoEngineParam)
        {
            emoEngine = emoEngineParam;
            VerticalAlignment = VerticalAlignment.Stretch;
            Title = "Settings";
            HideOnClose = false;
            Initialize();
        }

        private void Initialize()
        {
            var stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Name = "SettingsPanel",
            };
            Content = stackPanel;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            X = 250;
            Y = 0;
            Width = Screen.ActualWidth - 250;
        }
    }
}