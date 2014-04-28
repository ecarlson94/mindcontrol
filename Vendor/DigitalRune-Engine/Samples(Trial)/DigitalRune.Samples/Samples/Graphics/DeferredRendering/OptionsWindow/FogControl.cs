using DigitalRune.Game;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;


namespace DigitalRune.Samples
{
  // Controls properties of the ReticleObject.
  public class FogControl : GroupBox
  {
    private readonly CheckBox _fogEnabledCheckBox;
    private readonly Slider _startSlider;
    private readonly Slider _endSlider;
    private readonly Slider _densitySlider;
    private readonly Slider _heightFalloffSlider;
    private readonly Slider _heightSlider;


    public FogControl(IServiceLocator services)
    {
      var gameObjectService = services.GetInstance<IGameObjectService>();

      Title = "Fog";
      TitleTextBlockStyle = "GroupBoxTitleInTabPage";
      
      var panel = new StackPanel
      {
        Orientation = Orientation.Vertical,
      };
      Content = panel;

      var defaultMargin = new Vector4F(6, 6, 6, 0);

      _fogEnabledCheckBox = new CheckBox
      {
        Content = new TextBlock { Text = "Enable Fog" },
        Margin = defaultMargin + new Vector4F(0, 6, 0, 0),
      };
      _fogEnabledCheckBox.Click += (s, e) =>
      {
        var fogObject = (FogObject)gameObjectService.Objects["Fog"];
        fogObject.FogNode.IsEnabled = _fogEnabledCheckBox.IsChecked;
      };
      panel.Children.Add(_fogEnabledCheckBox);

      var startSliderPanel = new StackPanel
      {
        Orientation = Orientation.Horizontal,
        Margin = defaultMargin,
      };
      panel.Children.Add(startSliderPanel);
      var startTextBlock = new TextBlock
      {
        Text = "Fog Ramp Start: 0",
        Width = 150,
      };
      startSliderPanel.Children.Add(startTextBlock);
      _startSlider = new Slider
      {
        Minimum = 0,
        Maximum = 100,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Width = 200,
      };
      var valueProperty = _startSlider.Properties.Get<float>("Value");
      valueProperty.Changed += (s, e) =>
      {
        var fogObject = (FogObject)gameObjectService.Objects["Fog"];
        fogObject.FogNode.Fog.Start = _startSlider.Value;
        startTextBlock.Text = "Fog Ramp Start: " + (int)_startSlider.Value;
      };
      startSliderPanel.Children.Add(_startSlider);

      var endSliderPanel = new StackPanel
      {
        Orientation = Orientation.Horizontal,
        Margin = defaultMargin,
      };
      panel.Children.Add(endSliderPanel);
      var endTextBlock = new TextBlock
      {
        Text = "Fog Ramp End: 0",
        Width = 150,
      };
      endSliderPanel.Children.Add(endTextBlock);
      _endSlider = new Slider
      {
        Minimum = 0,
        Maximum = 100,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Width = 200,
      };
      valueProperty = _endSlider.Properties.Get<float>("Value");
      valueProperty.Changed += (s, e) =>
      {
        var fogObject = (FogObject)gameObjectService.Objects["Fog"];
        fogObject.FogNode.Fog.End = _endSlider.Value;
        endTextBlock.Text = "Fog Ramp End: " + (int)_endSlider.Value;
      };
      endSliderPanel.Children.Add(_endSlider);

      var densitySliderPanel = new StackPanel
      {
        Orientation = Orientation.Horizontal,
        Margin = defaultMargin,
      };
      panel.Children.Add(densitySliderPanel);
      var densityTextBlock = new TextBlock
      {
        Text = "Fog Density: 0",
        Width = 150,
      };
      densitySliderPanel.Children.Add(densityTextBlock);
      _densitySlider = new Slider
      {
        Minimum = 0.01f,
        Maximum = 2,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Width = 200,
        SmallChange = 0.01f
      };
      valueProperty = _densitySlider.Properties.Get<float>("Value");
      valueProperty.Changed += (s, e) =>
      {
        var fogObject = (FogObject)gameObjectService.Objects["Fog"];
        fogObject.FogNode.Fog.Density = _densitySlider.Value;
        densityTextBlock.Text = "Fog Density: " + _densitySlider.Value.ToString("F2");
      };
      densitySliderPanel.Children.Add(_densitySlider);

      var heightFalloffSliderPanel = new StackPanel
      {
        Orientation = Orientation.Horizontal,
        Margin = defaultMargin,
      };
      panel.Children.Add(heightFalloffSliderPanel);
      var heightFalloffTextBlock = new TextBlock
      {
        Text = "Fog Height-Falloff: 0",
        Width = 150,
      };
      heightFalloffSliderPanel.Children.Add(heightFalloffTextBlock);
      _heightFalloffSlider = new Slider
      {
        Minimum = -5,
        Maximum = 5,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Width = 200,
      };
      valueProperty = _heightFalloffSlider.Properties.Get<float>("Value");
      valueProperty.Changed += (s, e) =>
      {
        var fogObject = (FogObject)gameObjectService.Objects["Fog"];
        fogObject.FogNode.Fog.HeightFalloff = _heightFalloffSlider.Value;
        heightFalloffTextBlock.Text = "Fog Height-Falloff: " + _heightFalloffSlider.Value.ToString("F2");
      };
      heightFalloffSliderPanel.Children.Add(_heightFalloffSlider);

      var heightSliderPanel = new StackPanel
      {
        Orientation = Orientation.Horizontal,
        Margin = defaultMargin,
      };
      panel.Children.Add(heightSliderPanel);
      var heightTextBlock = new TextBlock
      {
        Text = "Fog Height-Y: 0",
        Width = 150,
      };
      heightSliderPanel.Children.Add(heightTextBlock);
      _heightSlider = new Slider
      {
        Minimum = 0,
        Maximum = 3,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Width = 200,
        SmallChange = 0.1f
      };
      valueProperty = _heightSlider.Properties.Get<float>("Value");
      valueProperty.Changed += (s, e) =>
      {
        var fogObject = (FogObject)gameObjectService.Objects["Fog"];
        var pose = fogObject.FogNode.PoseWorld;
        pose.Position.Y = _heightSlider.Value;
        fogObject.FogNode.PoseWorld = pose;
        heightTextBlock.Text = "Fog Height-Y: " + _heightSlider.Value.ToString("F2");
      };
      heightSliderPanel.Children.Add(_heightSlider);

      // Set initial values.
      var fogNode = ((FogObject)gameObjectService.Objects["Fog"]).FogNode;
      _fogEnabledCheckBox.IsChecked = fogNode.IsEnabled;
      _densitySlider.Value = fogNode.Fog.Density;
      _startSlider.Value = fogNode.Fog.Start;
      _endSlider.Value = fogNode.Fog.End;
      _heightFalloffSlider.Value = fogNode.Fog.HeightFalloff;
      _heightSlider.Value = fogNode.PoseWorld.Position.Y;
    }
  }
}
