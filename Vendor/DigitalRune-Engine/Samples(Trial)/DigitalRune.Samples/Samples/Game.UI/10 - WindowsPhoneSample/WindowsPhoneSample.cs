using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace DigitalRune.Samples.Game.UI
{
  [Sample(SampleCategory.GameUI,
    "This sample shows how to use the DigitalRune Game UI on the Windows Phone 7.",
    "Note: This sample was created before we could mix Silverlight and XNA on WP7.",
    10)]
  public class WindowsPhoneSample : Sample
  {
    private readonly DelegateGraphicsScreen _graphicsScreen;

    private readonly ContentManager _uiContentManager;
    private readonly UIScreen _uiScreen;


    public WindowsPhoneSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Disable mouse centering and show the mouse cursor.
      EnableMouseCentering = false;

      // Add a DelegateGraphicsScreen as the first graphics screen to the graphics
      // service. This lets us do the rendering in the Render method of this class.
      _graphicsScreen = new DelegateGraphicsScreen(GraphicsService)
      {
        RenderCallback = Render,
      };
      GraphicsService.Screens.Insert(0, _graphicsScreen);

      // Load a UI theme, which defines the appearance and default values of UI controls.
      _uiContentManager = new ContentManager(Services, "WindowsPhone7Theme");
      Theme theme = _uiContentManager.Load<Theme>("ThemeDark");

      // Create a UI renderer, which uses the theme info to renderer UI controls.
      UIRenderer renderer = new UIRenderer(Game, theme);

      // Create a UIScreen and add it to the UI service. The screen is the root of the 
      // tree of UI controls. Each screen can have its own renderer.
      _uiScreen = new UIScreen("SampleUIScreen", renderer)
      {
        // Make the screen transparent.
        Background = new Color(0, 0, 0, 0),
      };
      UIService.Screens.Add(_uiScreen);

      // Open a window.
      var window = new WpWindow(Game);
      window.Show(_uiScreen);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Remove UIScreen from UI service.
        UIService.Screens.Remove(_uiScreen);
        _uiContentManager.Dispose();

        // Remove graphics screen from graphics service.
        GraphicsService.Screens.Remove(_graphicsScreen);
      }

      base.Dispose(disposing);
    }


    private void Render(RenderContext context)
    {
      _uiScreen.Draw(context.DeltaTime);
    }
  }
}
