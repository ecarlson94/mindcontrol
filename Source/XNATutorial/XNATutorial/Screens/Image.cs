using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XNATutorial.Effects.ImageEffects;
using XNATutorial.Managers;

namespace XNATutorial.Screens
{
    public class Image
    {
        private Vector2 origin;
        private ContentManager content;
        private RenderTarget2D renderTarget;
        private SpriteFont font;
        private Dictionary<string, ImageEffect> effects; 

        public float Alpha;
        public string Text, FontName, Path;
        public Texture2D Texture;
        public Vector2 Position, Scale;
        public Rectangle SourceRect;
        public string Effects;
        public bool IsActive;
        public FadeEffect FadeEffect;

        void SetEffect<T>(ref T effect)
        {
            if (effect == null)
            {
                effect = (T) Activator.CreateInstance(typeof (T));
            }
            else
            {
                (effect as ImageEffect).IsActive = true;
                var obj = this;
                (effect as ImageEffect).LoadContent(ref obj);
            }

            effects.Add(effect.GetType().ToString().Replace("XNATutorial.Effects.ImageEffects.", ""), (effect as ImageEffect));
        }

        public void ActivateEffect(string effect)
        {
            if (effects.ContainsKey(effect))
            {
                effects[effect].IsActive = true;
                var obj = this;
                effects[effect].LoadContent(ref obj);
            }
        }

        public void DeactivateEffect(string effect)
        {
            if (effects.ContainsKey(effect))
            {
                effects[effect].IsActive = false;
                effects[effect].UnloadContent();
            }
        }

        public void StoreEffects()
        {
            Effects = String.Empty;
            foreach (var effect in effects)
            {
                if (effect.Value.IsActive)
                    Effects += effect.Key + ":";
            }

            if(Effects != String.Empty)
                Effects.Remove(Effects.Length - 1);
        }

        public void RestoreEffects()
        {
            foreach (var effect in effects)
                DeactivateEffect(effect.Key);

            string[] split = Effects.Split(':');
            foreach (string s in split)
                ActivateEffect(s);
        }

        public Image()
        {
            Path = Text = Effects = String.Empty;
            FontName = "Fonts/Orbitron";
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Alpha = 1.0f;
            SourceRect = Rectangle.Empty;
            effects = new Dictionary<string, ImageEffect>();
        }

        public void LoadContent()
        {
            content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
            if (Path != String.Empty)
            {
                Texture = content.Load<Texture2D>(Path);
            }

            font = content.Load<SpriteFont>(FontName);

            Vector2 dimensions = Vector2.Zero;

            if (Texture != null)
            {
                dimensions.X += Texture.Width;
                dimensions.Y = Math.Max(Texture.Height, font.MeasureString(Text).Y);
            }
            else
            {
                dimensions.Y = font.MeasureString(Text).Y;
            }
            dimensions.X += font.MeasureString(Text).X;

            if (SourceRect == Rectangle.Empty)
            {
                SourceRect = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);
            }

            renderTarget = new RenderTarget2D(ScreenManager.Instance.GraphicsDevice,
                (int)dimensions.X, (int)dimensions.Y);
            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(renderTarget);
            ScreenManager.Instance.GraphicsDevice.Clear(Color.Transparent);

            ScreenManager.Instance.SpriteBatch.Begin();
            if(Texture != null){
                ScreenManager.Instance.SpriteBatch.Draw(Texture, Vector2.Zero, Color.White * Alpha);
            }
            ScreenManager.Instance.SpriteBatch.DrawString(font, Text, Vector2.Zero, Color.White);
            ScreenManager.Instance.SpriteBatch.End();

            Texture = renderTarget;

            //Sets ScreenManager's GraphicsDevice's renderTarget to default
            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(null);

            SetEffect<FadeEffect>(ref FadeEffect);

            if (Effects != String.Empty)
            {
                string[] split = Effects.Split(':');
                foreach (string item in split)
                {
                    ActivateEffect(item);
                }
            }
        }

        public void UnloadContent()
        {
            content.Unload();
            foreach (var effect in effects)
            {
                DeactivateEffect(effect.Key);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var effect in effects)
            {
                if (effect.Value.IsActive)
                {
                    effect.Value.Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
            //spriteBatch.Draw(Texture, Position+origin, Color.White);
            spriteBatch.Draw(Texture, Position + origin, SourceRect, Color.White * Alpha,
                0.0f, origin, Scale, SpriteEffects.None, 0.0f);
        }
    }
}