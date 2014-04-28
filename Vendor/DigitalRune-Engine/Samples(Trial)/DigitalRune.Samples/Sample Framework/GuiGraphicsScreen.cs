using System;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Graphics;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace DigitalRune.Samples
{
  // This class implements a GraphicsScreen (DigitalRune Graphics), which displays
  // a GUI using a UIScreen (DigitalRune Game UI).
  public sealed class GuiGraphicsScreen : GraphicsScreen, IDisposable
  {
    private readonly IUIService _uiService;

    public UIScreen UIScreen { get; private set; }

    public bool HideBackground
    {
      get { return UIScreen.Background.A == 0; }
      set { UIScreen.Background = value ? new Color(0, 0, 0, 192) : Color.Transparent; }
    }


    public GuiGraphicsScreen(IServiceLocator services)
      : base(services.GetInstance<IGraphicsService>())
    {
      Name = "GUI";

      _uiService = services.GetInstance<IUIService>();
      var contentManager = services.GetInstance<ContentManager>("UIContent");

      // Load a UI theme and create the UI renderer and the UI screen. See the
      // DigitalRune Game UI documentation and samples for more details.
      var theme = contentManager.Load<Theme>("Theme");
      UIRenderer renderer = new UIRenderer(GraphicsService.GraphicsDevice, theme);
      UIScreen = new UIScreen("Default", renderer)
      {
        Background = Color.Transparent,
        ZIndex = int.MaxValue,
      };
      _uiService.Screens.Add(UIScreen);
    }


    public void Dispose()
    {
      // Clean up.
      _uiService.Screens.Remove(UIScreen);
      UIScreen.Renderer.SafeDispose();
    }


    protected override void OnUpdate(TimeSpan deltaTime)
    {
    }


    protected override void OnRender(RenderContext context)
    {
      // Draw GUI.
      UIScreen.Draw(context.DeltaTime);
    }
  }
}
