using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Geometry;
using DigitalRune.Graphics;
using DigitalRune.Graphics.Rendering;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1.Screens
{
    public class BaseGraphicsScreen : GraphicsScreen, IDisposable
    {
        //----------------------------------------------------------------------
        #region Fields

        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _defaultFont;
        private readonly SpriteFont _fixedWidthFont;
        private readonly Texture2D _reticle;

        //Node renderers
        private readonly MeshRenderer _meshRenderer;
        private readonly BillboardRenderer _billboardRenderer;

        //CameraNode for 2D output
        private CameraNode _cameraNode2D;

        #endregion
        
        //----------------------------------------------------------------------
        #region Properties

        public bool ClearBackground { get; set; }
        public Color BackgroundColor { get; set; }
        public bool UseFixedWidthFont { get; set; }
        public bool DrawReticle { get; set; }
        //Camera node for 3D output
        public CameraNode CameraNode3D { get; set; }
        public DebugRenderer DebugRenderer2D { get; private set; }
        public DebugRenderer DebugRenderer3D { get; private set; }
        public Scene Scene { get; private set; }
        #endregion

        //----------------------------------------------------------------------
        #region Creation and Cleanup

        public BaseGraphicsScreen(IServiceLocator services)
          : base(services.GetInstance<IGraphicsService>())
        {
            Name = "BaseGraphicsScreen";
            ClearBackground = false;
            BackgroundColor = new Color(220, 220, 220);
            DrawReticle = false;
            UseFixedWidthFont = false;

            //Load the reticle
            var contentManager = services.GetInstance<ContentManager>();
            _reticle = contentManager.Load<Texture2D>("Reticle");

            // Get the sprite fonts frome the Current UI Theme
            var uiContentManager = services.GetInstance<ContentManager>("UIContent");
            _defaultFont = uiContentManager.Load<SpriteFont>("Default");
            _fixedWidthFont = uiContentManager.Load<SpriteFont>("Console");

            //Set up for the 2D camera. Like all 2 dimensional planes, (0, 0)
            //is upper, left corner and (width, height) is lower right corner of screen
            var graphicsDevice = GraphicsService.GraphicsDevice;
            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;
            var projection = new OrthographicProjection
            {
                Near = 0,
                Far = 2000,
                Left = 0,
                Right = width,
                Top = 0,
                Bottom = height,
            };
            var camera = new Camera(projection);
            _cameraNode2D = new CameraNode(camera)
            {
                PoseWorld = new Pose(new Vector3F(0, 0, 1000))
            };

            // Initialize renderers.
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _meshRenderer = new MeshRenderer();
            _billboardRenderer = new BillboardRenderer(GraphicsService, 2048);
            DebugRenderer2D = new DebugRenderer(GraphicsService, _spriteBatch, _defaultFont)
            {
                SpriteFont = _defaultFont,
                DefaultColor = new Color(0, 0, 0),
                DefaultTextPosition = new Vector2F(10),
            };
            DebugRenderer3D = new DebugRenderer(GraphicsService, _spriteBatch, _defaultFont)
            {
                SpriteFont = _defaultFont,
                DefaultColor = new Color(0, 0, 0),
                DefaultTextPosition = new Vector2F(10)
            };

            Scene = new Scene();
        }

        public void Dispose()
        {
            _spriteBatch.Dispose();
            _meshRenderer.Dispose();
            _billboardRenderer.Dispose();
            DebugRenderer2D.Dispose();
            DebugRenderer3D.Dispose();
            Scene.Dispose(false);
        }
        #endregion

        //----------------------------------------------------------------------
        #region Overridden Methods

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            Scene.Update(deltaTime);
        }

        protected override void OnRender(RenderContext context)
        {
            var graphicsDevice = GraphicsService.GraphicsDevice;

            if(ClearBackground)
                graphicsDevice.Clear(BackgroundColor);

            if (CameraNode3D != null)
            {
                context.CameraNode = CameraNode3D;
                context.Scene = Scene;

                //Frustrm Culling: Only paint nodes that can be scene by the Camera angle
                var query = Scene.Query<CameraFrustumQuery>(context.CameraNode, context);

                // render states for opaque objects
                graphicsDevice.DepthStencilState = DepthStencilState.Default;
                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                graphicsDevice.BlendState = BlendState.Opaque;
                graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                //Render all nodes that are visible by the camera angle
                context.RenderPass = "Default";
                _meshRenderer.Render(query.SceneNodes, context);
                context.RenderPass = null;

                //render states for alpha blended objects
                graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                graphicsDevice.RasterizerState = RasterizerState.CullNone;
                graphicsDevice.BlendState = BlendState.NonPremultiplied;
                graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                //render the billboardnodes and particlesystemnodes
                _billboardRenderer.Render(query.SceneNodes, context);

                context.CameraNode = null;
                context.Scene = null;
            }

            var font = UseFixedWidthFont ? _fixedWidthFont : _defaultFont;
            DebugRenderer2D.SpriteFont = font;
            DebugRenderer3D.SpriteFont = font;

            //Render 3D Graphics
            if (CameraNode3D != null)
            {
                context.CameraNode = CameraNode3D;
                DebugRenderer3D.Render(context);
            }

            //Render 2D graphics
            context.CameraNode = _cameraNode2D;
            DebugRenderer2D.Render(context);

            //Draw reticle
            if (DrawReticle)
            {
                var viewport = context.Viewport;
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                _spriteBatch.Draw(
                    _reticle,
                    new Vector2(viewport.Width / 2 - _reticle.Width / 2, viewport.Height / 2 - _reticle.Height / 2),
                    Color.Black);
                _spriteBatch.End();
            }

            context.CameraNode = null;
        }
        #endregion
    }
}
