using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Graphics;
using DigitalRune.Graphics.PostProcessing;
using DigitalRune.Graphics.Rendering;
using DigitalRune.Graphics.SceneGraph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1.Renderers
{
    // This renderer renders light nodes and creates the light buffer which stores 
    // the accumulated diffuse and specular light intensities. 
    // The light buffer is stored in the render context. It can be used by the 
    // following renderers (usually by the "Material" pass), and it must be recycled 
    // by the graphics screen.
    public class LightBufferRenderer : IDisposable
    {
        private bool _disposed;

        // Pre-allocate array to avoid allocations at runtime.
        private readonly RenderTargetBinding[] _renderTargetBindings = new RenderTargetBinding[2];

        private readonly SsaoFilter _ssaoFilter;
        private readonly CopyFilter _copyFilter;

        public LightRenderer LightRenderer { get; private set; }


        public LightBufferRenderer(IGraphicsService graphicsService)
        {
            LightRenderer = new LightRenderer(graphicsService);

            _ssaoFilter = new SsaoFilter(graphicsService)
            {
                // Normally the SsaoFilter applies the occlusion values directly to the 
                // source texture. But here the filter should ignore the input image and 
                // create grayscale image (white = no occlusion, black = max occlusion).
                CombineWithSource = false,
            };

            _copyFilter = new CopyFilter(graphicsService);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    LightRenderer.Dispose();
                    _ssaoFilter.Dispose();
                    _copyFilter.Dispose();
                }

                _disposed = true;
            }
        }


        public void Render(IList<SceneNode> lights, RenderContext context)
        {
            var graphicsService = context.GraphicsService;
            var graphicsDevice = graphicsService.GraphicsDevice;
            var renderTargetPool = graphicsService.RenderTargetPool;

            var target = context.RenderTarget;
            var viewport = context.Viewport;
            var width = viewport.Width;
            var height = viewport.Height;

            // Render ambient occlusion info into a render target.
            var aoRenderTarget = renderTargetPool.Obtain2D(new RenderTargetFormat(
                width/_ssaoFilter.DownsampleFactor,
                height/_ssaoFilter.DownsampleFactor,
                false,
                SurfaceFormat.Color,
                DepthFormat.None));

            // PostProcessors require that context.SourceTexture is set. But since 
            // _ssaoFilter.CombineWithSource is set to false, the SourceTexture is not 
            // used and we can set it to anything except null.
            context.SourceTexture = aoRenderTarget;
            context.RenderTarget = aoRenderTarget;
            context.Viewport = new Viewport(0, 0, aoRenderTarget.Width, aoRenderTarget.Height);
            _ssaoFilter.Process(context);
            context.SourceTexture = null;

            // The light buffer consists of two full-screen render targets into which we 
            // render the accumulated diffuse and specular light intensities.
            var lightBufferFormat = new RenderTargetFormat(width, height, false, SurfaceFormat.HdrBlendable,
                DepthFormat.Depth24Stencil8);
            context.LightBuffer0 = renderTargetPool.Obtain2D(lightBufferFormat);
            context.LightBuffer1 = renderTargetPool.Obtain2D(lightBufferFormat);

            // Set the device render target to the light buffer.
            _renderTargetBindings[0] = new RenderTargetBinding(context.LightBuffer0); // Diffuse light accumulation
            _renderTargetBindings[1] = new RenderTargetBinding(context.LightBuffer1); // Specular light accumulation
            graphicsDevice.SetRenderTargets(_renderTargetBindings);
            context.RenderTarget = context.LightBuffer0;
            context.Viewport = graphicsDevice.Viewport;

            // Clear the light buffer. (The alpha channel is not used. We can set it to anything.)
            graphicsDevice.Clear(new Color(0, 0, 0, 255));

            // Render all lights into the light buffers.
            LightRenderer.Render(lights, context);

            // Render the ambient occlusion texture using multiplicative blending.
            // This will darken the light buffers depending on the ambient occlusion term.
            // Note: Theoretically, this should be done after the ambient light renderer 
            // and before the directional light renderer because AO should not affect 
            // directional lights. But doing this here has more impact.
            context.SourceTexture = aoRenderTarget;
            graphicsDevice.BlendState = GraphicsHelper.BlendStateMultiply;
            _copyFilter.Process(context);

            // Clean up.
            graphicsService.RenderTargetPool.Recycle(aoRenderTarget);
            context.SourceTexture = null;
            context.RenderTarget = target;
            context.Viewport = viewport;
            _renderTargetBindings[0] = new RenderTargetBinding();
            _renderTargetBindings[1] = new RenderTargetBinding();
        }
    }
}