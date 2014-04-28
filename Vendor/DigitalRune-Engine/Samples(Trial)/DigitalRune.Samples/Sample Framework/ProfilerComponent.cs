using System;
using System.Diagnostics;
using System.Text;
using DigitalRune.Diagnostics;
using DigitalRune.Game.Input;
using DigitalRune.Graphics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using DigitalRune.Text;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace DigitalRune.Samples
{
  // This XNA GameComponent displays profiling data if <F4> is pressed.
  // IMPORTANT: DigitalRune Base profiling classes are only active if the 
  // conditional compilation symbol DIGITALRUNE_PROFILE is defined.
  // (See: Project Properties | Build | Conditional compilation symbols)
  public class ProfilerComponent : DrawableGameComponent
  {
    private readonly IServiceLocator _services;
    private readonly IInputService _inputService;
    private readonly IGraphicsService _graphicsService;
    private readonly SampleGraphicsScreen _graphicsScreen;

    private int _numberOfUpdates;
    private int _numberOfDraws;
    private readonly StringBuilder _stringBuilder = new StringBuilder();
    private readonly Stopwatch _stopwatch;


    public ProfilerComponent(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      _services = ServiceLocator.Current;
      _inputService = _services.GetInstance<IInputService>();
      _graphicsService = _services.GetInstance<IGraphicsService>();

      // Add a new graphics screen to the graphics service.
      _graphicsScreen = new SampleGraphicsScreen(_services);
      _graphicsScreen.UseFixedWidthFont = true;
      _graphicsService.Screens.Add(_graphicsScreen);

      _stopwatch = Stopwatch.StartNew();

      // Add format/description for profiler values which are captured in Update().
      Profiler.SetFormat("NumBodies", 1, "The number of rigid bodies in the physics simulation.");
      Profiler.SetFormat("NumContacts", 1, "The number of contact constraints in the physics simulation.");
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clean up.
        _graphicsService.Screens.Remove(_graphicsScreen);
        _graphicsScreen.Dispose();
        _stopwatch.Stop();
      }

      base.Dispose(disposing);
    }


    public override void Update(GameTime gameTime)
    {
      var debugRenderer = _graphicsScreen.DebugRenderer2D;

      _numberOfUpdates++;

      if ((_stopwatch.Elapsed.TotalSeconds > 0.5 && !_inputService.IsDown(Keys.F4))
          || _inputService.IsReleased(Keys.F4))
      {
        // ----- Overlay View
        // Show "Update FPS" and "Draw FPS" in upper right corner.
        // (The data is updated every ~0.5 s or when the profiler view is closed.)
        _graphicsScreen.ClearBackground = false;

        debugRenderer.Clear();
        _stringBuilder.Clear();
        _stringBuilder.Append("Update FPS: ");
        _stringBuilder.AppendNumber((int)Math.Round(_numberOfUpdates / _stopwatch.Elapsed.TotalSeconds));
        _stringBuilder.Append("\nDraw FPS: ");
        _stringBuilder.AppendNumber((int)Math.Round(_numberOfDraws / _stopwatch.Elapsed.TotalSeconds));
#if !WINDOWS_PHONE
        debugRenderer.DrawText(_stringBuilder, new Vector2F(1160, 10), Color.Black);
#else
        debugRenderer.DrawText(_stringBuilder, new Vector2F(680, 10), Color.Black);
#endif

        _numberOfUpdates = 0;
        _numberOfDraws = 0;
        _stopwatch.Reset();
        _stopwatch.Start();
      }

      // Capture general interesting info.
      var simulation = _services.GetInstance<Simulation>();
      Profiler.AddValue("NumBodies", simulation.RigidBodies.Count);
      Profiler.AddValue("NumContacts", simulation.ContactConstraints.Count);

      if (_inputService.IsPressed(Keys.F4, false))
      {
        // ----- Profiler View
        _graphicsScreen.ClearBackground = true;

        debugRenderer.Clear();
        debugRenderer.DrawText("PROFILE\n\n");
#if DIGITALRUNE_PROFILE
        // Dump profiler.
        debugRenderer.DrawText("-------------------------------------------------------------------------------");
        debugRenderer.DrawText("Profiler:");
        debugRenderer.DrawText("-------------------------------------------------------------------------------");
        debugRenderer.DrawText(Profiler.DumpAll());
        Profiler.ClearAll();

        // Dump all Hierarchical Profilers.
        var hierarchicalProfilers = _services.GetAllInstances<HierarchicalProfiler>();
        foreach (var hierarchicalProfiler in hierarchicalProfilers)
        {
          debugRenderer.DrawText("");
          debugRenderer.DrawText("-------------------------------------------------------------------------------");
          debugRenderer.DrawText("Hierarchical Profilers:");
          debugRenderer.DrawText("-------------------------------------------------------------------------------");
          debugRenderer.DrawText(hierarchicalProfiler.Dump(hierarchicalProfiler.Root, int.MaxValue));
          debugRenderer.DrawText("");
          hierarchicalProfiler.Reset();
        }
#else
        debugRenderer.DrawText("Profiling is disabled. To enable profiling, define the conditional compilation symbol 'DIGITALRUNE_PROFILE' in the project.");
#endif
      }
    }


    public override void Draw(GameTime gameTime)
    {
      _numberOfDraws++;
    }
  }
}
