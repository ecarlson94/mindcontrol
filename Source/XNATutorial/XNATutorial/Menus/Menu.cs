using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNATutorial.Managers;

namespace XNATutorial.Menus
{
    public class Menu
    {
        private int itemNumber;
        private string _id;

        public event EventHandler OnMenuChange;

        public string Axis;
        public string Effects;
        [XmlElement("Item")]
        public List<MenuItem> Items;
        public string ID
        {
            get { return _id; }
            set
            {
                _id = value;
                OnMenuChange(this, null);
            }
        }
        public int ItemNumber{get { return itemNumber; }}

        public void Transition(float alpha)
        {
            foreach (MenuItem item in Items)
            {
                item.Image.IsActive = true;
                item.Image.Alpha = alpha;
                item.Image.FadeEffect.Increase = alpha == 0.0f;
            }
        }

        public void StoreAllEffectsBut(string effect)
        {
            foreach (MenuItem item in Items)
            {
                item.Image.StoreEffects();
                item.Image.ActivateEffect(effect);
            }
        }

        public void RestoreAllEffects()
        {
            foreach (MenuItem item in Items)
            {
                item.Image.RestoreEffects();
            }
        }

        private void AlignMenuItems()
        {
            Vector2 dimensions = Vector2.Zero;
            foreach (MenuItem item in Items)
                dimensions += new Vector2(item.Image.SourceRect.Width,
                    item.Image.SourceRect.Height);

            dimensions = new Vector2((ScreenManager.Instance.Dimensions.X - dimensions.X) / 2,
                (ScreenManager.Instance.Dimensions.Y - dimensions.Y) / 2);

            foreach (MenuItem item in Items)
            {
                if (Axis == "X")
                {
                    item.Image.Position = new Vector2(dimensions.X,
                        (ScreenManager.Instance.Dimensions.Y - item.Image.SourceRect.Height) / 2);
                }
                else if (Axis == "Y")
                {
                    item.Image.Position = new Vector2((ScreenManager.Instance.Dimensions.X - item.Image.SourceRect.Width) / 2,
                        dimensions.Y);
                }

                dimensions += new Vector2(item.Image.SourceRect.Width,
                    item.Image.SourceRect.Height);
            }
        }

        public Menu()
        {
            _id = Effects = String.Empty;
            itemNumber = 0;
            Axis = "Y";
        }

        public void LoadContent()
        {
            string[] split = Effects.Split(':');
            foreach (MenuItem item in Items)
            {
                item.Image.LoadContent();
                foreach (string s in split)
                {
                    item.Image.ActivateEffect(s);
                }
            }
            AlignMenuItems();
        }

        public void UnloadContent()
        {
            if (Items != null)
                foreach (MenuItem item in Items)
                {
                    item.Image.UnloadContent();
                }
        }

        public void Update(GameTime gameTime)
        {
            if (Axis == "X")
            {
                if (InputManager.Instance.KeyPressed(Keys.Right))
                    itemNumber++;
                else if (InputManager.Instance.KeyPressed(Keys.Left))
                    itemNumber--;
            }
            else if (Axis == "Y")
            {
                if (InputManager.Instance.KeyPressed(Keys.Down))
                    itemNumber++;
                else if (InputManager.Instance.KeyPressed(Keys.Up))
                    itemNumber--;
            }

            if (itemNumber < 0)
                itemNumber = Items.Count - 1;
            else if (itemNumber > Items.Count)
                itemNumber = 0;

            for (int i = 0; i < Items.Count; i++)
            {
                if (i == itemNumber)
                    Items[i].Image.IsActive = true;
                else
                    Items[i].Image.IsActive = false;

                Items[i].Image.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (MenuItem item in Items)
            {
                item.Image.Draw(spriteBatch);
            }
        }
    }
}
