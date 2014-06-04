using WindowsGame1.StackPanels;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using System;
using WindowsGame1.Managers;
using WindowsGame1.Paints;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1.Windows
{
    public class ContactQualityWindow : Window
    {
        //----------------------------------------------------------------------
        #region Fields

        private EmoEngineManager _emoEngine;
        private TextBlock _headsetOff;
        private TextBlock _headsetOffHead;
        private TextBlock _poorQualityText;
        private StackPanel _poorContactQuality;
        private StackPanel _legend;
        private ContactQuality _contactQualityDisplay;
        private IServiceLocator _services;

        #endregion

        //----------------------------------------------------------------------
        #region Creation and Cleanup
        public ContactQualityWindow(EmoEngineManager emoEngineParam, IServiceLocator services)
        {
            _services = services;
            _emoEngine = emoEngineParam;
            VerticalAlignment = VerticalAlignment.Stretch;
            Title = "Contact Quality";
            HideOnClose = false;
            CanDrag = false;
            Initialize();
            DetermineWindowContent();
        }

        private void Initialize()
        {
            _contactQualityDisplay = new ContactQuality(_emoEngine, _services)
            {
                Name = "ContactQualityDisplay",
            };

            _headsetOff = new TextBlock
            {
                Name = "HeadsetOff",
                Text = "Please turn on the headset...",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            _headsetOffHead = new TextBlock
            {
                Name = "HeadsetOffHead",
                Text = "Please position the headset on the your head...",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };


            _poorContactQuality = new StackPanel
            {
                Name = "PoorContactQuality",
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Orientation = Orientation.Vertical,
            };

            Canvas legendAndContact = new Canvas
            {
                Name = "LegendAndContact",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            var legendPanel = new StackPanel
            {
                Name = "LegendPanel",
                X = 0,
                Y = 0,
                Orientation = Orientation.Vertical,
            };

            var contentManager = _services.GetInstance<ContentManager>();
            Image goodQuality = new Image { Texture = contentManager.Load<Texture2D>("Brain/ContactQualities/Good") };
            Image fairQuality = new Image { Texture = contentManager.Load<Texture2D>("Brain/ContactQualities/Fair") };
            Image poorQuality = new Image { Texture = contentManager.Load<Texture2D>("Brain/ContactQualities/Poor") };
            Image veryPoorQuality = new Image { Texture = contentManager.Load<Texture2D>("Brain/ContactQualities/VeryBad") };
            Image noQuality = new Image { Texture = contentManager.Load<Texture2D>("Brain/ContactQualities/NoSignal") };

            legendPanel.Children.Add(new LegendItem(goodQuality, "Good Quality") { Margin = new Vector4F(2) });
            legendPanel.Children.Add(new LegendItem(fairQuality, "Fair Quality") { Margin = new Vector4F(2) });
            legendPanel.Children.Add(new LegendItem(poorQuality, "Poor Quality") { Margin = new Vector4F(2) });
            legendPanel.Children.Add(new LegendItem(veryPoorQuality, "Very Bad Quality") { Margin = new Vector4F(2) });
            legendPanel.Children.Add(new LegendItem(noQuality, "No Signal") { Margin = new Vector4F(2) });
            legendAndContact.Children.Add(legendPanel);
            legendAndContact.Children.Add(_contactQualityDisplay);

            _poorQualityText = new TextBlock
            {
                Text = "Try repositioning each node to get a better contact quality.",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                WrapText = true,
            };
            _poorContactQuality.Children.Add(legendAndContact);
            _poorContactQuality.Children.Add(_poorQualityText);

            Content = _poorContactQuality;
        }

        #endregion

        //----------------------------------------------------------------------
        #region Private Methods

        private void DetermineWindowContent()
        {
            bool headsetOn = _emoEngine.HeadsetOn();
            bool headsetOnHead = _emoEngine.HeadsetOnHead();
            bool overallGoodQuality = _emoEngine.OverallGoodQuality();

            if (!headsetOn && !Content.Equals(_headsetOff))
            {
                Content = _headsetOff;
            }
            else if (headsetOn && !headsetOnHead && !Content.Equals(_headsetOffHead))
            {
                Content = _headsetOffHead;
            }
            else if (headsetOn && headsetOnHead)
            {
                if (!Content.Equals(_poorContactQuality))
                    Content = _poorContactQuality;

                if (!_poorContactQuality.Children.Contains(_poorQualityText) && !overallGoodQuality)
                    _poorContactQuality.Children.Add(_poorQualityText);
                else if (_poorContactQuality.Children.Contains(_poorQualityText) && overallGoodQuality)
                    _poorContactQuality.Children.Remove(_poorQualityText);
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
            if(_emoEngine.ProfileLoggedIn())
                DetermineWindowContent();
            base.OnUpdate(deltaTime);
        }

        #endregion
    }
}