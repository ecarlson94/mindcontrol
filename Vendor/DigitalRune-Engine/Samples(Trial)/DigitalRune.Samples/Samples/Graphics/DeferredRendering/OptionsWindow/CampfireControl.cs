using DigitalRune.Game;
using DigitalRune.Game.UI.Controls;
using Microsoft.Practices.ServiceLocation;


namespace DigitalRune.Samples
{
  // Controls properties of the CampfireObject.
  public class CampfireControl : CheckBox
  {
    private readonly IGameObjectService _gameObjectService;


    public CampfireControl(IServiceLocator services)
    {
      _gameObjectService = services.GetInstance<IGameObjectService>();

      var campfireObject = (CampfireObject)_gameObjectService.Objects["Campfire"];
      Content = new TextBlock { Text = "Enable Campfire Particle System" };
      IsChecked = campfireObject.IsEnabled;
    }


    protected override void OnClick(System.EventArgs eventArgs)
    {
      base.OnClick(eventArgs);

      var campfireObject = (CampfireObject)_gameObjectService.Objects["Campfire"];
      campfireObject.IsEnabled = IsChecked;
    }
  }
}
