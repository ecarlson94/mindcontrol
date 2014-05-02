using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;
using DigitalRune.Game.UI.Controls;
using Emotiv;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1.Images
{
    public class ContactQualityImage : Image
    {
        //Textures for contact quality
        private static Texture2D Good;
        private static Texture2D Fair;
        private static Texture2D Poor;
        private static Texture2D VeryBad;
        private static Texture2D NoSignal;

        private static readonly int QualityPropertyID = CreateProperty<EdkDll.EE_EEG_ContactQuality_t>(
                "Quality",
                "Common",
                "Defines Quality to use for image",
                EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_NO_SIGNAL).Id;  
        public EdkDll.EE_EEG_ContactQuality_t Quality
        {
            get { return GetValue<EdkDll.EE_EEG_ContactQuality_t>(QualityPropertyID); }
            set { SetValue(QualityPropertyID, value); }
        }

        public ContactQualityImage()
        {
            LoadTextures();
            Texture = NoSignal;
            PropertyChanged += ImageNamePropertyChanged;
        }

        private void LoadTextures()
        {
            if(Good == null)
                Good = MainComponent.Content.Load<Texture2D>("Brain/ContactQualities/Good");
            if(Fair == null)
                Fair = MainComponent.Content.Load<Texture2D>("Brain/ContactQualities/Fair");
            if(Poor == null)
                Poor = MainComponent.Content.Load<Texture2D>("Brain/ContactQualities/Poor");
            if(VeryBad == null)
                VeryBad = MainComponent.Content.Load<Texture2D>("Brain/ContactQualities/VeryBad");
            if(NoSignal == null)
                NoSignal = MainComponent.Content.Load<Texture2D>("Brain/ContactQualities/NoSignal");
        }

        private void ImageNamePropertyChanged(object sender, GamePropertyEventArgs gameEvent)
        {
            if (gameEvent.Property.Name.Equals("Quality"))
            {
                Texture2D newQuality = null;
                if ((EdkDll.EE_EEG_ContactQuality_t) gameEvent.Property.Value ==
                    EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_GOOD)
                    newQuality = Good;
                else if ((EdkDll.EE_EEG_ContactQuality_t)gameEvent.Property.Value ==
                    EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_FAIR)
                    newQuality = Fair;
                else if ((EdkDll.EE_EEG_ContactQuality_t)gameEvent.Property.Value ==
                    EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_POOR)
                    newQuality = Poor;
                else if ((EdkDll.EE_EEG_ContactQuality_t)gameEvent.Property.Value ==
                    EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_VERY_BAD)
                    newQuality = VeryBad;
                else if ((EdkDll.EE_EEG_ContactQuality_t)gameEvent.Property.Value ==
                    EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_NO_SIGNAL)
                    newQuality = NoSignal;

                Texture = newQuality;
            }
        }
    }
}
