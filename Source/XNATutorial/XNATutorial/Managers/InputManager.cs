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
        private EmoEngineManager emoEngine;
        public EmoEngineManager EmoEngine{get { return emoEngine; }}

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
            if (!ScreenManager.Instance.IsTransitioning)
            {
                currentKeyState = Keyboard.GetState();
            }
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

        public bool DonglePluggedIn()
        {
            return emoEngine.DonglePluggedIn;
        }
    }
}