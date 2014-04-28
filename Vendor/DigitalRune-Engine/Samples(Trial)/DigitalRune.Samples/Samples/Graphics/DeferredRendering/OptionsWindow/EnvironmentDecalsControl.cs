using DigitalRune.Game;
using DigitalRune.Game.UI.Controls;
using Microsoft.Practices.ServiceLocation;


namespace DigitalRune.Samples
{
  // Controls properties of the EnvironmentDecalsObject.
  public class EnvironmentDecalsControl : CheckBox
  {
    private readonly IGameObjectService _gameObjectService;


    public EnvironmentDecalsControl(IServiceLocator services)
    {
      _gameObjectService = services.GetInstance<IGameObjectService>();

      Content = new TextBlock { Text = "Enable Decals" };
      var environmentDecalsObject = (EnvironmentDecalsObject)_gameObjectService.Objects["EnvironmentDecals"];
      IsChecked = environmentDecalsObject.IsEnabled;
    }


    protected override void OnClick(System.EventArgs eventArgs)
    {
      base.OnClick(eventArgs);

      var environmentDecalsObject = (EnvironmentDecalsObject)_gameObjectService.Objects["EnvironmentDecals"];
      environmentDecalsObject.IsEnabled = IsChecked;
    }
  }
}
