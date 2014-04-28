using System;
using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Input;


namespace DigitalRune.Samples
{
  // Manages an Options window. This window is only visible when <Ctrl> is pressed.
  [Controls(@"More Options
  Hold <Ctrl> to show options window.")]
  public class OptionsObject : GameObject
  {
    private readonly IServiceLocator _services;
    private readonly IInputService _inputService;
    private readonly Window _window;


    public OptionsObject(IServiceLocator services)
    {
      Name = "Options";

      _services = services;
      _inputService = services.GetInstance<IInputService>();
      _window = new OptionsWindow(services);
    }


    // OnLoad() is called when the GameObject is added to the IGameObjectService.
    protected override void OnLoad()
    {
      // Find a UIScreen.
      var uiService = _services.GetInstance<IUIService>();
      var screen = uiService.Screens[0];

      // Open the windows but make it invisible. OnUpdate controls the visibility.
      _window.Show(screen);
      _window.IsVisible = false;
    }


    // OnUnload() is called when the GameObject is removed from the IGameObjectService.
    protected override void OnUnload()
    {
      _window.Close();
    }


    // OnUpdate() is called once per frame.
    protected override void OnUpdate(TimeSpan deltaTime)
    {
      _window.IsVisible = _inputService.IsDown(Keys.LeftControl) || _inputService.IsDown(Keys.RightControl);
    }
  }
}
