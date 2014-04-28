using DigitalRune.Graphics;
using Microsoft.Xna.Framework.Graphics;


namespace DigitalRune.Samples.Animation
{
  // The base class for basic animation samples. The class provides a SpriteBatch,
  // SpriteFont and an image for rendering. Derived classes need to override OnRender()
  // and add the rendering code.
  public abstract class AnimationSample : Sample
  {
    private readonly DelegateGraphicsScreen _graphicsScreen;

    // Properties which can be used by derived sample classes.
    protected readonly SpriteBatch SpriteBatch;
    protected readonly SpriteFont SpriteFont;
    protected readonly Texture2D Logo;
    protected readonly Texture2D Reticle;


    protected AnimationSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Add a DelegateGraphicsScreen and use the OnRender method of this class to
      // do the rendering.
      _graphicsScreen = new DelegateGraphicsScreen(GraphicsService)
      {
        RenderCallback = OnRender,
      };
      // The order of the graphics screens is back-to-front. Add the screen at index 0,
      // i.e. behind all other screens. The screen should be rendered first and all other
      // screens (menu, GUI, help, ...) should be on top.
      GraphicsService.Screens.Insert(0, _graphicsScreen);

      // Provide a SpriteBatch, SpriteFont and images for rendering.
      SpriteBatch = new SpriteBatch(GraphicsService.GraphicsDevice);
      SpriteFont = UIContentManager.Load<SpriteFont>("Default");
      Logo = ContentManager.Load<Texture2D>("Logo");
      Reticle = ContentManager.Load<Texture2D>("Reticle");
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clean up.
        GraphicsService.Screens.Remove(_graphicsScreen);
        SpriteBatch.Dispose();
      }

      base.Dispose(disposing);
    }


    // Needs to be implemented in derived class.
    protected abstract void OnRender(RenderContext context);
  }
}
