using System;
using DigitalRune.Game.Input;
using DigitalRune.Graphics.PostProcessing;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace DigitalRune.Samples.Graphics
{
  [Sample(SampleCategory.Graphics,
    "This sample uses a GodRayFilter to create light shafts.",
    "",
    49)]
  public class GodRaySample : PostProcessingSample
  {
    private readonly GodRayFilter _godRayFilter;


    public GodRaySample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      _godRayFilter = new GodRayFilter(GraphicsService)
      {
        Exposure = 0.2f
      };
      GraphicsScreen.PostProcessors.Add(_godRayFilter);

      // The god ray filter light direction should match the direction of the sun light,
      // which was added by the StaticSkyObject.
      var lightNode = GraphicsScreen.Scene.GetSceneNode("Sunlight");
      _godRayFilter.LightDirection = lightNode.PoseWorld.ToWorldDirection(Vector3F.Forward);
    }


    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      // <1> / <Shift> + <1> --> Change number of samples.
      if (InputService.IsPressed(Keys.D1, true))
      {
        bool isShiftDown = (InputService.ModifierKeys & ModifierKeys.Shift) != 0;
        if (isShiftDown)
          _godRayFilter.NumberOfSamples++;
        else
          _godRayFilter.NumberOfSamples = Math.Max(1, _godRayFilter.NumberOfSamples - 1);
      }

      // <2> / <Shift> + <2> --> Change scale.
      if (InputService.IsDown(Keys.D2))
      {
        // Increase or decrease value by a factor of 1.01 every frame (1/60 s).
        bool isShiftDown = (InputService.ModifierKeys & ModifierKeys.Shift) != 0;
        float factor = isShiftDown ? 1.01f : 1.0f / 1.01f;
        _godRayFilter.Scale *= (float)Math.Pow(factor, deltaTime * 60);
      }

      // <3> / <Shift> + <3> --> Change exposure.
      if (InputService.IsDown(Keys.D3))
      {
        // Increase or decrease value by a factor of 1.01 every frame (1/60 s).
        bool isShiftDown = (InputService.ModifierKeys & ModifierKeys.Shift) != 0;
        float factor = isShiftDown ? 1.01f : 1.0f / 1.01f;
        _godRayFilter.Exposure *= (float)Math.Pow(factor, deltaTime * 60);
      }

      // <4> / <Shift> + <4> --> Change weight.
      if (InputService.IsDown(Keys.D4))
      {
        // Increase or decrease value by a factor of 1.01 every frame (1/60 s).
        bool isShiftDown = (InputService.ModifierKeys & ModifierKeys.Shift) != 0;
        float factor = isShiftDown ? 1.01f : 1.0f / 1.01f;
        _godRayFilter.Weight *= (float)Math.Pow(factor, deltaTime * 60);
      }

      // <5> / <Shift> + <5> --> Change decay.
      if (InputService.IsDown(Keys.D5))
      {
        // Increase or decrease value by a factor of 1.01 every frame (1/60 s).
        bool isShiftDown = (InputService.ModifierKeys & ModifierKeys.Shift) != 0;
        float factor = isShiftDown ? 1.01f : 1.0f / 1.01f;
        _godRayFilter.Decay *= (float)Math.Pow(factor, deltaTime * 60);
      }

      GraphicsScreen.DebugRenderer.DrawText(
        "\n\nPress <1> or <Shift>+<1> to decrease or increase the number of samples: " + _godRayFilter.NumberOfSamples
        + "\nHold <2> or <Shift>+<2> to decrease or increase the scale: " + _godRayFilter.Scale
        + "\nHold <3> or <Shift>+<3> to decrease or increase the exposure: " + _godRayFilter.Exposure
        + "\nHold <4> or <Shift>+<4> to decrease or increase the weight: " + _godRayFilter.Weight
        + "\nHold <5> or <Shift>+<5> to decrease or increase the decay: " + _godRayFilter.Decay);
    }
  }
}
