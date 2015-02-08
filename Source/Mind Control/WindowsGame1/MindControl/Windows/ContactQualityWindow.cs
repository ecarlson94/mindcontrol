using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using WindowsGame1.Paints;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using Emotiv;

namespace WindowsGame1.Windows
{
    public class ContactQualityWindow : Window
    {
        private EmoEngineManager emoEngine;

        public ContactQualityWindow(EmoEngineManager emoEngineParam)
        {
            emoEngine = emoEngineParam;
            VerticalAlignment = VerticalAlignment.Stretch;
            Title = "Contact Quality";
            HideOnClose = false;
            CanDrag = false;
            Initialize();
        }

        private void Initialize()
        {
            Content = new ContactQuality(emoEngine);
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
