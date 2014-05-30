using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Renderers;
using DigitalRune.Graphics;
using DigitalRune.Graphics.PostProcessing;
using DigitalRune.Graphics.Rendering;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1.Screens
{
    public class DeferredGraphicsScreen : GraphicsScreen
    {
        //--------------------------------------------------------------
        #region Fields

        protected readonly SpriteBatch SpriteBatch;
        private readonly BillboardRenderer _billboardRenderer;
        private readonly CloudMapRenderer _cloudMapRenderer;
        private readonly SceneCaptureRenderer _sceneCaptureRenderer;
        private readonly ShadowMapRenderer _shadowMapRenderer;
        private readonly ShadowMaskRenderer _shadowMaskRenderer;
        private readonly SkyRenderer _skyRenderer;
        private readonly DebugRenderer _internalDebugRenderer;
        private readonly RebuildZBufferRenderer _rebuildZBufferRenderer;
        private readonly Texture2D _reticle;

        #endregion

        //--------------------------------------------------------------
        #region Properties

        public bool IsDisposed { get; private set; }

        public MeshRenderer MeshRenderer { get; private set; }
        public LightBufferRenderer LightBufferRenderer { get; private set; }
        public SceneRenderer AlphaBlendSceneRenderer { get; private set; }

        // The active camera is used to render the scene. This property is
        // set outside of this class.
        public CameraNode ActiveCameraNode { get; set; }

        public Scene Scene { get; private set; }
        
        public PostProcessorChain PostProcessors { get; private set; }

        // A debug renderer which can be used by the samples and game objects.
        public DebugRenderer DebugRenderer { get; private set; }

        public bool VisualizeIntermediatRenderTargets { get; set; }

        public bool EnableLod { get; set; }

        public bool EnableSoftParticles
        {
            get { return _billboardRenderer.EnableSoftParticles; }
            set { _billboardRenderer.EnableSoftParticles = value; }
        }

        public bool EnableOffscreenParticles
        {
            get { return _billboardRenderer.EnableOffscreenRendering; }
            set { _billboardRenderer.EnableOffscreenRendering = value; }
        }

        public bool DrawReticle { get; set; }

        #endregion

        //--------------------------------------------------------------
        #region Creation and Cleanup

        public DeferredGraphicsScreen(IServiceLocator services)
            : base(services.GetInstance<IGraphicsService>())
        {
            var contentManager = services.GetInstance<ContentManager>();

            SpriteBatch = new SpriteBatch(GraphicsService.GraphicsDevice);

            MeshRenderer = new MeshRenderer();

            // Shadows
            _shadowMapRenderer = new ShadowMapRenderer(MeshRenderer);
            _shadowMaskRenderer = new ShadowMaskRenderer(GraphicsService, 2);

            // Renderers which create the intermediat render targets
            LightBufferRenderer = new LightBufferRenderer(GraphicsService);

            _skyRenderer = new SkyRenderer(GraphicsService);
            _internalDebugRenderer = new DebugRenderer(GraphicsService, SpriteBatch, null);
            _rebuildZBufferRenderer = new RebuildZBufferRenderer(GraphicsService);

            Scene = new Scene();

            // Use 2D texture for reticle.
            _reticle = contentManager.Load<Texture2D>("Reticle");

            // Use the sprite font of the GUI.
            var uiContentManager = services.GetInstance<ContentManager>("UIContent");
            var spriteFont = uiContentManager.Load<SpriteFont>("Default");
            DebugRenderer = new DebugRenderer(GraphicsService, SpriteBatch, spriteFont)
            {
                DefaultColor = new Color(0, 0, 0),
                DefaultTextPosition = new Vector2F(10),
            };

            EnableLod = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    
                }
            }
        }

        #endregion
    }
}