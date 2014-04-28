using System;


namespace DigitalRune.Samples
{
#if WINDOWS || XBOX
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
#if WINDOWS
    [STAThread]
#endif
    static void Main(string[] args)
    {
      using (SampleGame game = new SampleGame())
      {
        game.Run();
      }
    }
  }
#endif
}
