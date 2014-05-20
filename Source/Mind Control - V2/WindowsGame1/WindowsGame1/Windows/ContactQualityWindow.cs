using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using Microsoft.Practices.ServiceLocation;
using System;
using WindowsGame1.Managers;
using WindowsGame1.Paints;

namespace WindowsGame1.Windows
{
    public class ContactQualityWindow : Window
    {
        //----------------------------------------------------------------------
        #region Fields

        private EmoEngineManager emoEngine;
        private TextBlock headsetOff;
        private TextBlock headsetOffHead;
        private TextBlock poorQualityText;
        private StackPanel poorContactQuality;
        private ContactQuality contactQualityDisplay;
        private IServiceLocator _services;

        #endregion

        //----------------------------------------------------------------------
        #region Creation and Cleanup
        public ContactQualityWindow(EmoEngineManager emoEngineParam, IServiceLocator services)
        {
            _services = services;
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
            contactQualityDisplay = new ContactQuality(emoEngine, _services)
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

        #endregion

        //----------------------------------------------------------------------
        #region Private Methods

        private void DetermineWindowContent()
        {
            bool headsetOn = emoEngine.HeadsetOn();
            bool headsetOnHead = emoEngine.HeadsetOnHead();
            bool overallGoodQuality = emoEngine.OverallGoodQuality();

            if (!headsetOn && !Content.Equals(headsetOff))
            {
                Content = headsetOff;
            }
            else if (headsetOn && !headsetOnHead && !Content.Equals(headsetOffHead))
            {
                Content = headsetOffHead;
            }
            else if (headsetOn && headsetOnHead)
            {
                if (!Content.Equals(poorContactQuality))
                    Content = poorContactQuality;

                if (!poorContactQuality.Children.Contains(poorQualityText) && !overallGoodQuality)
                    poorContactQuality.Children.Add(poorQualityText);
                else if (poorContactQuality.Children.Contains(poorQualityText) && overallGoodQuality)
                    poorContactQuality.Children.Remove(poorQualityText);
            }
        }

        #endregion

        //----------------------------------------------------------------------
        #region Overridden Methods

        protected override void OnLoad()
        {
            base.OnLoad();
            X = 250;
            Y = 0;
            Width = Screen.ActualWidth - 250;
            MaxWidth = Screen.ActualWidth - 250;
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if(emoEngine.ProfileLoggedIn())
                DetermineWindowContent();
            base.OnUpdate(deltaTime);
        }

        #endregion
    }
}