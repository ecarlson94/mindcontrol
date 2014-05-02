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
        private TextBlock headsetOff;
        private TextBlock headsetOffHead;
        private TextBlock poorQualityText;
        private StackPanel poorContactQuality;
        private ContactQuality contactQualityDisplay;

        public ContactQualityWindow(EmoEngineManager emoEngineParam)
        {
            emoEngine = emoEngineParam;
            VerticalAlignment = VerticalAlignment.Stretch;
            Title = "Contact Quality";
            HideOnClose = false;
            CanDrag = false;
            Initialize();
            DetermineWindowContent();
        }

        private void Initialize()
        {
            contactQualityDisplay = new ContactQuality(emoEngine)
            {
                Name = "ContactQualityDisplay",
            };

            headsetOff = new TextBlock
            {
                Name = "HeadsetOff",
                Text = "Please turn on the headset...",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            headsetOffHead = new TextBlock
            {
                Name = "HeadsetOffHead",
                Text = "Please position the headset on the your head...",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };


            poorContactQuality = new StackPanel
            {
                Name = "PoorContactQuality",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Vertical,
            };

            poorQualityText = new TextBlock
            {
                Text = "Try repositioning each node to get a better contact quality.",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                WrapText = true,
            };
            poorContactQuality.Children.Add(poorQualityText);
            poorContactQuality.Children.Add(contactQualityDisplay);

            Content = poorContactQuality;
        }

        private void DetermineWindowContent()
        {
            bool headsetOn = emoEngine.HeadsetOn();
            bool headsetOnHead = emoEngine.HeadsetOnHead();
            bool overallGoodQuality = emoEngine.OverallGoodQuality();

            if (!headsetOn && !Content.Name.Equals(headsetOff.Name))
            {
                Content = headsetOff;
            }
            else if (!headsetOnHead && !Content.Name.Equals(headsetOffHead.Name))
            {
                Content = headsetOffHead;
            }
            else if (!overallGoodQuality)
            {
                if (!Content.Name.Equals(poorContactQuality.Name))
                    Content = poorContactQuality;

                if(!poorContactQuality.Children.Contains(poorQualityText))
                    poorContactQuality.Children.Add(poorQualityText);
            }
            else
            {
                if (!Content.Name.Equals(poorContactQuality.Name))
                    Content = poorContactQuality;
                
                if (poorContactQuality.Children.Contains(poorQualityText))
                    poorContactQuality.Children.Remove(poorQualityText);
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            X = 250;
            Y = 0;
            Width = Screen.ActualWidth - 250;
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if(emoEngine.Profile != String.Empty)
                DetermineWindowContent();
            base.OnUpdate(deltaTime);
        }
    }
}
