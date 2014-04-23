using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNATutorial.Managers
{
    public class InputManager
    {
        private KeyboardState currentKeyState, prevKeyState;
        private MouseState currentMouseState, prevMouseState;
        private EmoEngineManager emoEngine;

        private static InputManager instance;
        public static InputManager Instance
        {
            get { return instance ?? (instance = new InputManager()); }
        }

        public InputManager()
        {
            emoEngine = new EmoEngineManager();
            emoEngine.StartEmoEngine();
        }

        public void Update()
        {
            prevKeyState = currentKeyState;
            prevMouseState = currentMouseState;
            if (!ScreenManager.Instance.IsTransitioning)
            {
                currentKeyState = Keyboard.GetState();
                currentMouseState = Mouse.GetState();
            }
        }

        public bool LeftMousePressed()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed &&
                   prevMouseState.LeftButton == ButtonState.Released;
        }

        public bool RightMousePressed()
        {
            return currentMouseState.RightButton == ButtonState.Pressed &&
                   prevMouseState.RightButton == ButtonState.Released;
        }

        public bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (currentKeyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key))
                {
                    return true;
                }
            }

            return false;
        }

        public bool RightMouseReleased()
        {
            return currentMouseState.RightButton == ButtonState.Released &&
                   prevMouseState.RightButton == ButtonState.Pressed;
        }

        public bool LeftMouseReleased()
        {
            return currentMouseState.LeftButton == ButtonState.Released &&
                   prevMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool KeyReleased(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (currentKeyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key))
                {
                    return true;
                }
            }

            return false;
        }

        public bool KeyDown(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (currentKeyState.IsKeyDown(key))
                {
                    return true;
                }
            }

            return false;
        }

        public bool RightMouseDown()
        {
            return currentMouseState.RightButton == ButtonState.Pressed;
        }

        public bool LeftMouseDown()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public int GetMouseX()
        {
            return currentMouseState.X;
        }

        public int GetMouseY()
        {
            return currentMouseState.Y;
        }

        public bool DonglePluggedIn()
        {
            return emoEngine.DonglePluggedIn;
        }
    }
}