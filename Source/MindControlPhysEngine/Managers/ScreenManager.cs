using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MindControlPhysEngine.Screens;

namespace MindControlPhysEngine.Managers
{

    //Divide the Game up into many different screens
    class ScreenManager
    {
        private int width = 640;
        private int height = 480;

        public ContentManager Content { get; private set; }
        public Vector2 Dimensions { private set; get; }
        private XMLManager<GameScreen> xmlGameScreenManager = new XMLManager<GameScreen>();

        private GameScreen currentScreen;

        private static ScreenManager instance;
        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScreenManager();
                }

                return instance;
            }
        }

        public ScreenManager()
        {
            Dimensions = new Vector2(width, height);
            currentScreen = new SplashScreen();
            xmlGameScreenManager = new XMLManager<GameScreen>();
            xmlGameScreenManager.Type = currentScreen.Type;
            currentScreen = xmlGameScreenManager.Load("Load/SplashScreen.xml");
        }

        public void LoadContent(ContentManager content)
        {
            this.Content = new ContentManager(content.ServiceProvider);
            currentScreen.LoadContent();
        }

        public void UnloadContent()
        {
            currentScreen.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
        }
    }
}
