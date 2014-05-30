using DigitalRune.Game;
using DigitalRune.Geometry;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DirectionalLight = DigitalRune.Graphics.DirectionalLight;

namespace WindowsGame1.BackgroundObjects
{
    public class Sky : GameObject
    {
        private readonly IServiceLocator _services;
        private SkyboxNode _skyboxNode;
        private LightNode _ambientLightNode;
        private LightNode _sunLightNode;

        //The Brightness of the sky box.
        public float SkyExposure { get; set; }

        public Sky(IServiceLocator services)
        {
            _services = services;
            Name = "StaticSky";
            SkyExposure = 0.2f;
        }

        protected override void OnLoad()
        {
            var contentManager = _services.GetInstance<ContentManager>();
            _skyboxNode = new SkyboxNode(contentManager.Load<TextureCube>("Sky2"))
            {
                Color = new Vector3F(SkyExposure),
            };

            //The ambient light
            var ambientLight = new AmbientLight
            {
                Color = new Vector3F(0.9f, 0.9f, 1f),
                HdrScale = 0.1f,
                Intensity = 0.5f,
                HemisphericAttenuation = 0.8f,
            };
            _ambientLightNode = new LightNode(ambientLight)
            {
                Name = "Ambient",
            };

            //Main light source
            var sunlight = new DirectionalLight
            {
                Color = new Vector3F(1, 0.9607844f, 0.9078432f),
                HdrScale = 0.4f,
                DiffuseIntensity = 1,
                SpecularIntensity = 1,
            };
            _sunLightNode = new LightNode(sunlight)
            {
                Name = "Sunlight",
                Priority = 10,
                PoseWorld = new Pose(QuaternionF.CreateRotationY(-1.0f) * QuaternionF.CreateRotationX(-1.0f)),

                //Cascaded Shadow Mapping
                Shadow = new CascadedShadow
                {
                    PreferredSize = 1024,
                    DepthBiasScale = new Vector4F(0.98f),
                    DepthBiasOffset = new Vector4F(0.0f),
                    FadeOutDistance = 55,
                    MaxDistance = 75,
                    MinLightDistance = 100,
                    ShadowFog = 0.0f,
                    JitterResolution = 3000,
                    SplitDistribution = 0.7f,
                }
            };

            //Add scene nodes to scene graph
            var scene = _services.GetInstance<IScene>();
            scene.Children.Add(_skyboxNode);
            scene.Children.Add(_ambientLightNode);
            scene.Children.Add(_sunLightNode);
        }

        protected override void OnUnload()
        {
            _skyboxNode.Parent.Children.Remove(_skyboxNode);
            _skyboxNode.Dispose(false);
            _skyboxNode = null;

            _ambientLightNode.Parent.Children.Remove(_ambientLightNode);
            _ambientLightNode.Dispose(false);
            _ambientLightNode = null;

            _sunLightNode.Parent.Children.Remove(_sunLightNode);
            _sunLightNode.Dispose(false);
            _sunLightNode = null;
        }
    }
}