using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MindControlPhysEngine.Screens
{
    public class SplashScreen : GameScreen
    {
        private Texture2D image;
        public string Path;

        public override void LoadContent()
        {
            base.LoadContent();
            //Path = "Images/SplashScreen/parror.jpg";
            image = content.Load<Texture2D>(Path);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, new Vector2(50,50), Color.White);
        }
    }
}
