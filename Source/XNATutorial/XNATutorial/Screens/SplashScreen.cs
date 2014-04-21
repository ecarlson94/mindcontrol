using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNATutorial.Managers;

namespace XNATutorial.Screens
{
    public class SplashScreen : GameScreen
    {
        public Image Image;

        public override void LoadContent()
        {
            base.LoadContent();
            Image.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            Image.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Image.Update(gameTime);

            if (InputManager.Instance.DonglePluggedIn() && !ScreenManager.Instance.IsTransitioning)
            {
                ScreenManager.Instance.ChangeScreens("TitleScreen");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
    }
}
