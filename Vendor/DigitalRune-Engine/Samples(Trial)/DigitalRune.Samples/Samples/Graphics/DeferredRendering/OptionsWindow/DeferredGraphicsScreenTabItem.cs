using System.Linq;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Graphics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;


namespace DigitalRune.Samples
{
  // A tab item which controls the settings of a DeferredGraphicsScreen.
  public class DeferredGraphicsScreenTabItem : TabItem
  {
    private readonly DeferredGraphicsScreen _graphicsScreen;

    private readonly CheckBox _drawIntermediateRenderTargetsCheckBox;
    private readonly CheckBox _lodCheckBox;
    private readonly CheckBox _softParticlesCheckBox;
    private readonly CheckBox _offscreenParticlesCheckBox;


    public DeferredGraphicsScreenTabItem(IServiceLocator services)
    {
      var graphicsService = services.GetInstance<IGraphicsService>();
      _graphicsScreen = graphicsService.Screens.OfType<DeferredGraphicsScreen>().First();

      Content = new TextBlock { Text = "DeferredGraphicsScreen", };

      var defaultMargin = new Vector4F(6, 6, 6, 0);

      var panel = new StackPanel
      {
        HorizontalAlignment = HorizontalAlignment.Stretch,
      };
      TabPage = panel;

      _drawIntermediateRenderTargetsCheckBox = new CheckBox
      {
        Content = new TextBlock { Text = "Draw Intermediate Render Targets" },
        Margin = defaultMargin + new Vector4F(0, 6, 0, 0),
      };
      _drawIntermediateRenderTargetsCheckBox.Click += (s, e) =>
      {
        _graphicsScreen.VisualizeIntermediateRenderTargets = _drawIntermediateRenderTargetsCheckBox.IsChecked;
      };
      panel.Children.Add(_drawIntermediateRenderTargetsCheckBox);

      var textBlock = new TextBlock
      {
        Text = "First row: G-Buffer 0 (depth), G-Buffer 1 (normals and glossiness),\n" +
        "Light Buffer 0 (diffuse), Light Buffer 1 (specular),\n" +
        "Second row: Shadow masks",
        Margin = defaultMargin,
      };
      panel.Children.Add(textBlock);

      var lodGroupBox = new GroupBox
      {
        Title = "Level of Detail (LOD)",
        TitleTextBlockStyle = "GroupBoxTitleInTabPage",
        Margin = defaultMargin,
        HorizontalAlignment = HorizontalAlignment.Stretch
      };
      var lodPanel = new StackPanel
      {
        Margin = new Vector4F(0, 6, 0, 6)
      };
      lodGroupBox.Content = lodPanel;
      panel.Children.Add(lodGroupBox);

      _lodCheckBox = new CheckBox
      {
        Content = new TextBlock { Text = "Enable LOD" },
        Margin = defaultMargin,
      };
      _lodCheckBox.Click += (s, e) =>
      {
        _graphicsScreen.EnableLod = _lodCheckBox.IsChecked;
      };
      lodPanel.Children.Add(_lodCheckBox);

      var particleGroupBox = new GroupBox
      {
        Title = "Particle Rendering",
        TitleTextBlockStyle = "GroupBoxTitleInTabPage",
        Margin = defaultMargin,
        HorizontalAlignment = HorizontalAlignment.Stretch
      };
      var particlePanel = new StackPanel
      {
        Margin = new Vector4F(0, 6, 0, 6)
      };
      particleGroupBox.Content = particlePanel;
      panel.Children.Add(particleGroupBox);

      _softParticlesCheckBox = new CheckBox
      {
        Content = new TextBlock { Text = "Enable soft particles" },
        Margin = defaultMargin,
      };
      _softParticlesCheckBox.Click += (s, e) =>
      {
        _graphicsScreen.EnableSoftParticles = _softParticlesCheckBox.IsChecked;
      };
      particlePanel.Children.Add(_softParticlesCheckBox);

      _offscreenParticlesCheckBox = new CheckBox
      {
        Content = new TextBlock { Text = "Render particles into low-resolution offscreen buffer" },
        Margin = defaultMargin,
      };
      _offscreenParticlesCheckBox.Click += (s, e) =>
      {
        _graphicsScreen.EnableOffscreenParticles = _offscreenParticlesCheckBox.IsChecked;
      };
      particlePanel.Children.Add(_offscreenParticlesCheckBox);

      TabPage = panel;
    }

    protected override void OnLoad()
    {
      // Get initial control values from the screen.
      _drawIntermediateRenderTargetsCheckBox.IsChecked = _graphicsScreen.VisualizeIntermediateRenderTargets;
      _lodCheckBox.IsChecked = _graphicsScreen.EnableLod;
      _softParticlesCheckBox.IsChecked = _graphicsScreen.EnableSoftParticles;
      _offscreenParticlesCheckBox.IsChecked = _graphicsScreen.EnableOffscreenParticles;

      base.OnLoad();
    }
  }
}
