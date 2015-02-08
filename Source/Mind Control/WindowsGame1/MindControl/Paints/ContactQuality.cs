using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1.Paints
{
    public class ContactQuality : Canvas
    {
        //Width = 391 + 50
        private const int WIDTH = 441;
        //Height = 469 + 50
        private const int HEIGHT = 519;

        private EmoEngineManager emoEngine;
        private Image brain;
        
        //images for each of the nodes
        private Image AF3;

        public ContactQuality(EmoEngineManager emoEngineParam)
        {
            emoEngine = emoEngineParam;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

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

            AF3 = new Image
            {
                Texture = MainComponent.Content.Load<Texture2D>("Brain/ContactQualities/Good"),
                X = 150.0f,
                Y = 50.0f,
            };
            Children.Add(AF3);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }
    }
}