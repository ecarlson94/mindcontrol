using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRune.Animation;
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        static Game1()
        {
            //DigitalRune.Licensing.AddSerialNumber("A4DsypZMsBsI8xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //DigitalRune.Licensing.AddSerialNumber("A4DsypZMsBsI8xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        }

        private InputManager _inputManager;
        private UIManager _uiManager;
        private AnimationManager _animationManager;
        protected override void Initialize()
        {
            // ----- Initialize Services
            // The services are stored in Game.Services to make them accessible by all
            // game components.

            // Add the input service, which manages device input, button presses, etc.
            _inputManager = new InputManager(false);
            Services.AddService(typeof(IInputService), _inputManager);

            // Add the UI service, which manages UI screens.
            _uiManager = new UIManager(this, _inputManager);
            Services.AddService(typeof(IUIService), _uiManager);

            // Add the animation service.
            _animationManager = new AnimationManager();
            Services.AddService(typeof(IAnimationService), _animationManager);

            // ----- Add GameComponents
            // The component that shows the individual screen.
            Components.Add(new MainComponent(this));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.ElapsedGameTime;

            // Update input manager. The input manager gets the device states and performs other work.
            _inputManager.Update(deltaTime);

            // Update UI manager. The UI manager updates all registered UIScreens and handles
            // button clicks, etc.
            _uiManager.Update(deltaTime);

            // Update game components.
            base.Update(gameTime);

            // Update the animations. The animations results are stored internally but not yet applied.
            _animationManager.Update(deltaTime);

            // Apply the animations. This method changes the animated objects.
            _animationManager.ApplyAnimations();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        //protected override void Draw(GameTime gameTime)
        //{
        //    GraphicsDevice.Clear(Color.CornflowerBlue);

        //    // TODO: Add your drawing code here

        //    base.Draw(gameTime);
        //}
    }
}