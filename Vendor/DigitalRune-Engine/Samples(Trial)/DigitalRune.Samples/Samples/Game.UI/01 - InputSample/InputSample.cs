using Microsoft.Xna.Framework;


namespace DigitalRune.Samples.Game.UI
{
  [Sample(SampleCategory.GameUI,
    @"This sample shows how to handle input from mouse, keyboard and gamepad.",
    @"The sample renders 3 rectangles. The rectangles can be moved and the color of the rectangles 
can be changed. The top rectangle will always handle input first. The lower rectangle will not
react to input if the top rectangle has already handled the input.",
    1)]
  [Controls(@"Sample
  Use <Left Mouse> or <Left Thumbstick> to move rectangle.
  Press <Left Shoulder>/<Right Shoulder> on gamepad to select other rectangle.
  Press <Space> on keyboard or <A> on gamepad to change color of the top rectangle.")]
  public class InputSample : BasicSample
  {
    private readonly RectangleObject _rectangleObject0;
    private readonly RectangleObject _rectangleObject1;
    private readonly RectangleObject _rectangleObject2;


    public InputSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      GraphicsScreen.ClearBackground = true;

      // Disable mouse centering and show the mouse cursor.
      EnableMouseCentering = false;

      // Add 3 simple game objects which draw rectangles and demonstrate input handling.
      _rectangleObject0 = new RectangleObject(Services);
      _rectangleObject1 = new RectangleObject(Services);
      _rectangleObject2 = new RectangleObject(Services);
      GameObjectService.Objects.Add(_rectangleObject0);
      GameObjectService.Objects.Add(_rectangleObject1);
      GameObjectService.Objects.Add(_rectangleObject2);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        GameObjectService.Objects.Remove(_rectangleObject0);
        GameObjectService.Objects.Remove(_rectangleObject1);
        GameObjectService.Objects.Remove(_rectangleObject2);
      }

      base.Dispose(disposing);
    }


    public override void Update(GameTime gameTime)
    {
      GraphicsScreen.DebugRenderer2D.Clear();
    }
  }
}
