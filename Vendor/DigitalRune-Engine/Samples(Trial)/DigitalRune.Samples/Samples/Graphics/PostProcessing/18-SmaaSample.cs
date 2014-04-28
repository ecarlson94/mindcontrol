using DigitalRune.Graphics.PostProcessing;


namespace DigitalRune.Samples.Graphics
{
  [Sample(SampleCategory.Graphics,
    "This sample uses a SmaaFilter to apply Subpixel Morphological Antialiasing (SMAA).",
    "",
    48)]
  public class SmaaSample : PostProcessingSample
  {
    public SmaaSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      var smaaFilter = new SmaaFilter(GraphicsService);
      GraphicsScreen.PostProcessors.Add(smaaFilter);
    }
  }
}
