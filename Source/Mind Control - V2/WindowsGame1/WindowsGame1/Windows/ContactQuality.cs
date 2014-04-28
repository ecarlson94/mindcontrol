using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;

namespace WindowsGame1.Windows
{
    public class ContactQuality : Window
    {
        private EmoEngineManager emoEngine;

        public ContactQuality(EmoEngineManager emoEngineParam)
        {
            emoEngine = emoEngineParam;
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Content = new TextBlock { Text = "You are doing it right" };
        }
    }
}