using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Images;
using WindowsGame1.Managers;
using DigitalRune.Game;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using Emotiv;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1.Paints
{
    public class ContactQuality : Canvas
    {
        //Width = 391 + 50
        private const int WIDTH = 675;
        //Height = 469 + 50
        private const int HEIGHT = 662;

        private EmoEngineManager emoEngine;
        private Image brain;
        
        //images for each of the nodes
        private Dictionary<EdkDll.EE_DataChannel_t, ContactQualityImage> qualityImages;

        public ContactQuality(EmoEngineManager emoEngineParam)
        {
            emoEngine = emoEngineParam;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            qualityImages = new Dictionary<EdkDll.EE_DataChannel_t, ContactQualityImage>();
            Initialize();
        }

        private void Initialize()
        {
            brain = new Image
            {
                Texture = MainComponent.Content.Load<Texture2D>("Brain/Head"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Children.Add(brain);

            var AF3 = new ContactQualityImage
            {
                X = 246.0f,
                Y = 155.0f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.AF3, AF3);
            Children.Add(AF3);

            var AF4 = new ContactQualityImage
            {
                X = 392.0f,
                Y = 155.0f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.AF4, AF4);
            Children.Add(AF4);

            var F7 = new ContactQualityImage
            {
                X = 125.0f,
                Y = 190.0f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.F7, F7);
            Children.Add(F7);

            var F3 = new ContactQualityImage
            {
                X = 217f,
                Y = 211f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.F3, F3);
            Children.Add(F3);

            var F4 = new ContactQualityImage
            {
                X = 420f,
                Y = 211f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.F4, F4);
            Children.Add(F4);

            var F8 = new ContactQualityImage
            {
                X = 512f,
                Y = 190f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.F8, F8);
            Children.Add(F8);

            var FC5 = new ContactQualityImage
            {
                X = 145f,
                Y = 265f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.FC5, FC5);
            Children.Add(FC5);

            var FC6 = new ContactQualityImage
            {
                X = 492f,
                Y = 265f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.FC6, FC6);
            Children.Add(FC6);

            var T7 = new ContactQualityImage
            {
                X = 75f,
                Y = 330f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.T7, T7);
            Children.Add(T7);

            var T8 = new ContactQualityImage
            {
                X = 560f,
                Y = 330f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.T8, T8);
            Children.Add(T8);

            var P7 = new ContactQualityImage
            {
                X = 125f,
                Y = 470f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.P7, P7);
            Children.Add(P7);

            var P8 = new ContactQualityImage
            {
                X = 512f,
                Y = 470f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.P8, P8);
            Children.Add(P8);

            var O1 = new ContactQualityImage
            {
                X = 246f,
                Y = 554f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.O1, O1);
            Children.Add(O1);

            var O2 = new ContactQualityImage
            {
                X = 391f,
                Y = 553f,
            };
            qualityImages.Add(EdkDll.EE_DataChannel_t.O2, O2);
            Children.Add(O2);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Width = WIDTH;
            Height = HEIGHT;
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            Dictionary<EdkDll.EE_DataChannel_t, EdkDll.EE_EEG_ContactQuality_t > contactQualities = emoEngine.GetContactQualityMap();

            if (contactQualities.Count > 0)
            {
                foreach (KeyValuePair<EdkDll.EE_DataChannel_t, EdkDll.EE_EEG_ContactQuality_t> kvp in contactQualities)
                {
                    ContactQualityImage current = qualityImages[kvp.Key];
                    if (current.Quality != kvp.Value)
                    {
                        current.Quality = kvp.Value;
                    }
                }
            }
            base.OnUpdate(deltaTime);
        }
    }
}