using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XNATutorial.Screens;

namespace XNATutorial.Managers
{
    public class ScreenManager
    {
        private GameScreen currentScreen, newScreen;
        private XmlManager<GameScreen> xmlGameScreenManager;

        private static ScreenManager _instance;

        public static ScreenManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    XmlManager<ScreenManager> xml = new XmlManager<ScreenManager>();

                    _instance = xml.Load("Load/ScreenManager.xml");
                }

                return _instance;
            }
        }
        [XmlIgnore]
        public Vector2 Dimensions { get; private set; }
        [XmlIgnore]
        public ContentManager Content { get; private set; }
        [XmlIgnore]
        public GraphicsDevice GraphicsDevice;
        [XmlIgnore]
        public SpriteBatch SpriteBatch;
        
        public Image Image;
        [XmlIgnore]
        public bool IsTransitioning { get; private set; }

        public void ChangeScreens(string screenName)
        {
            newScreen = (GameScreen) Activator.CreateInstance(Type.GetType("XNATutorial.Screens." + screenName));
            Image.IsActive = true;
            Image.FadeEffect.Increase = true;
            Image.Alpha = 0.0f;
            IsTransitioning = true;
        }

        private void Transition(GameTime gameTime)
        {
            if (IsTransitioning)
            {
                Image.Update(gameTime);
            
                if (Image.Alpha == 1.0f)
                {
                    currentScreen.UnloadContent();
                    currentScreen = newScreen;
                    xmlGameScreenManager.Type = currentScreen.Type;
                    if (File.Exists(currentScreen.XmlPath))
                        currentScreen = xmlGameScreenManager.Load(currentScreen.XmlPath);
                    currentScreen.LoadContent();
                }
                else if (Image.Alpha == 0.0f)
                {
                    Image.IsActive = false;
                    IsTransitioning = false;
                }
            }
        }

    public ScreenManager()
        {
            Dimensions = new Vector2(640, 480);
            currentScreen = new SplashScreen();
            xmlGameScreenManager = new XmlManager<GameScreen>();
            xmlGameScreenManager.Type = currentScreen.Type;
            currentScreen = xmlGameScreenManager.Load("Load/SplashScreen.xml");
        }

        public void LoadContent(ContentManager content)
        {
            this.Content = new ContentManager(content.ServiceProvider, "Content");
            currentScreen.LoadContent();
            Image.LoadContent();
        }

        public void UnloadContent()
        {
            currentScreen.UnloadContent();
            Image.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (InputManager.Instance.DonglePluggedIn() &&
                currentScreen.GetType() != Type.GetType("XNATutorial.Screens.SplashScreen"))
            {
                ChangeScreens("SplashScreen");
            }
            currentScreen.Update(gameTime);
            Transition(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
            if (IsTransitioning)
            {
                Image.Draw(spriteBatch);
            }
        }
    }
}