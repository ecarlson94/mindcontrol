using DigitalRune.Graphics.Rendering;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Physics;
using Microsoft.Xna.Framework;
using WindowsGame1.Managers;
using WindowsGame1.Screens;

namespace WindowsGame1.Components
{
    public class GraphicsBaseComponent : BaseComponent
    {
        protected BaseGraphicsScreen GraphicsScreen { get; private set; }

        protected GraphicsBaseComponent(Game game, EmoEngineManager emoEngine) : base(game, emoEngine)
        {
            //Create a graphics screen for rendering simple things
            GraphicsScreen = new BaseGraphicsScreen(Services);

            //Order of screens is back-to-front (index 0 being back)
            //Any other screens like menu and GUI will render on top
            GraphicsService.Screens.Insert(0, GraphicsScreen);

            Services.Register(typeof(DebugRenderer), null, GraphicsScreen.DebugRenderer3D);
            Services.Register(typeof(DebugRenderer), "DebugRenderer2D", GraphicsScreen.DebugRenderer2D);
            Services.Register(typeof(IScene), null, GraphicsScreen.Scene);

            GraphicsScreen.ClearBackground = true;
            GraphicsScreen.BackgroundColor = Color.White;
            GraphicsScreen.DrawReticle = true;

            //TODO: TBD add a default light setup here
        }

        public override void Update(GameTime gameTime)
        {
            var debugRenderer = GraphicsScreen.DebugRenderer3D;
            debugRenderer.Clear();
            foreach (var body in Simulation.RigidBodies)
            {
                if (body.UserData is string && (string) body.UserData == "NoDraw")
                    continue;

                var color = Color.Gray;
                if (body.MotionType == MotionType.Static && body.IsSleeping)
                    color = Color.LightGray;

                debugRenderer.DrawObject(body, color, false, false);
            }
            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Clean up.
                GraphicsService.Screens.Remove(GraphicsScreen);
                GraphicsScreen.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
