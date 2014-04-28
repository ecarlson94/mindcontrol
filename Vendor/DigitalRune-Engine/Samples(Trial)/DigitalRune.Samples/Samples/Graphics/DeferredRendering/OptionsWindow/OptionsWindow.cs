using System.Linq;
using DigitalRune.Game;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Graphics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;


namespace DigitalRune.Samples
{
  // Adds a GUI window containing a tab control.
  // Other game objects can add their UI controls to this window.
  // This window is only visible when <Ctrl> is pressed.
  [Controls(@"More Options
  Hold <Ctrl> to show options window.")]
  public class OptionsWindow : Window
  {
    public OptionsWindow(IServiceLocator services)
    {
      Title = "Options";
      Width = 400;
      Height = 400;

      // Hide close window button.
      CloseButtonStyle = null;

      // Set a start position at the top right.
      var graphicsService = services.GetInstance<IGraphicsService>();
      var viewport = graphicsService.GraphicsDevice.Viewport;
      var titleSafeArea = viewport.TitleSafeArea;
      X = titleSafeArea.Right - Width - 10;
      Y = titleSafeArea.Top + 10;

      var tabControl = new TabControl
      {
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
      };
      Content = tabControl;

      var tabItem = new TabItem();
      tabControl.Items.Add(tabItem);
      tabItem.Content = new TextBlock { Text = "Game Objects" };

      var gameObjectsPanel = new StackPanel
      {
        Orientation = Orientation.Vertical,
      };
      tabItem.TabPage = gameObjectsPanel;

      var defaultMargin = new Vector4F(6, 6, 6, 0);

      // Add controls for the game objects (if the corresponding game object is in use.)
      // TODO: We could automate this using class attributes. Example:
      //   [Options(typeof(FogControl))]
      //   class FogObject : GameObject { ... }
      //   --> Automatically check attributes of all game objects and create options controls...
      var gameObjectService = services.GetInstance<IGameObjectService>();
      if (gameObjectService.Objects.Contains("Reticle"))
        gameObjectsPanel.Children.Add(new ReticleControl(services) { Margin = defaultMargin });
      if (gameObjectService.Objects.Contains("Campfire"))
        gameObjectsPanel.Children.Add(new CampfireControl(services) { Margin = defaultMargin });
      if (gameObjectService.Objects.Contains("EnvironmentDecals"))
        gameObjectsPanel.Children.Add(new EnvironmentDecalsControl(services) { Margin = defaultMargin });
      if (gameObjectService.Objects.Contains("Fog"))
        gameObjectsPanel.Children.Add(new FogControl(services) { Margin = defaultMargin });

      if (graphicsService.Screens.OfType<DeferredGraphicsScreen>().Any())
        tabControl.Items.Add(new DeferredGraphicsScreenTabItem(services));
    }
  }
}
