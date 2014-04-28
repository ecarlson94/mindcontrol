using System;
using System.Linq;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Graphics;
using Microsoft.Practices.ServiceLocation;


namespace DigitalRune.Samples
{
  // Controls properties of the ReticleObject.
  public class ReticleControl : CheckBox
  {
    private readonly IGraphicsService _graphicsService;


    public ReticleControl(IServiceLocator services)
    {
      _graphicsService = services.GetInstance<IGraphicsService>();

      Content = new TextBlock { Text = "Draw Reticle" };
      var graphicsScreen = _graphicsService.Screens.OfType<DeferredGraphicsScreen>().First();
      IsChecked = graphicsScreen.DrawReticle;
    }


    protected override void OnClick(EventArgs eventArgs)
    {
      base.OnClick(eventArgs);

      var graphicsScreen = _graphicsService.Screens.OfType<DeferredGraphicsScreen>().First();
      graphicsScreen.DrawReticle = IsChecked;
    }
  }
}
