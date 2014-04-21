using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNATutorial.Menus;

namespace XNATutorial.Managers
{
    public class MenuManager
    {
        private Menu menu;
        private bool isTransitioning;

        private void Transition(GameTime gameTime)
        {
            if (isTransitioning)
            {
                
                for (int i = 0; i < menu.Items.Count; i++)
                {
                    menu.Items[i].Image.Update(gameTime);
                    float first = menu.Items[0].Image.Alpha;
                    float last = menu.Items[menu.Items.Count - 1].Image.Alpha;
                    if (first == 0.0f && last == 0.0f)
                        menu.ID = menu.Items[menu.ItemNumber].LinkID;
                    else if (first == 1.0f && last == 1.0f){
                        isTransitioning = false;
                        menu.RestoreAllEffects();
                    }
                }
            }
        }

        public MenuManager()
        {
            menu = new Menu();
            menu.OnMenuChange += menu_OneMenuChange;
        }

        private void menu_OneMenuChange(object sender, EventArgs eventArgs)
        {
            XmlManager<Menu> xmlMenuManager = new XmlManager<Menu>();
            menu.UnloadContent();

            menu = xmlMenuManager.Load(menu.ID);
            menu.LoadContent();

            menu.OnMenuChange += menu_OneMenuChange;
            menu.Transition(0.0f);

            menu.StoreAllEffectsBut("FadeEffect");
        }

        public void LoadContent(String menuPath)
        {
            if (menuPath != String.Empty)
            {
                menu.ID = menuPath;
            }
        }

        public void UnloadContent()
        {
            menu.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            if(!isTransitioning)
                menu.Update(gameTime);
            if (InputManager.Instance.KeyPressed(Keys.Enter) && !isTransitioning)
            {
                if (menu.Items[menu.ItemNumber].LinkType == "Screen")
                    ScreenManager.Instance.ChangeScreens(menu.Items[menu.ItemNumber].LinkID);
                else
                {
                    isTransitioning = true;
                    menu.Transition(1.0f);
                    menu.StoreAllEffectsBut("FadeEffect");
                }
            }

            Transition(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            menu.Draw(spriteBatch);
        }
    }
}
