// A simple post-processing effect that inverts colors.

// The size of the viewport in pixels.
uniform const float2 ViewportSize : VIEWPORTSIZE = float2(1280, 720);

// The strength of the effect in the range [0, 1].
uniform const float Strength = 1;

// The texture containing the original image.
uniform const texture SourceTexture : SOURCETEXTURE;
uniform const sampler SourceSampler = sampler_state
{
  Texture = <SourceTexture>;
  AddressU = CLAMP;
  AddressV = CLAMP;
  MagFilter = LINEAR;
  MinFilter = LINEAR;
  MipFilter = POINT;
};


// Converts a position from screen space to projection space.
// See VSScreenSpaceDraw() in Common.fxh for a detailed description.
void VS(inout float2 texCoord : TEXCOORD0, 
        inout float4 position : SV_POSITION)
{
#if !SM4
  position.xy -= 0.5;
#endif
  position.xy /= ViewportSize;
  position.xy *= float2(2, -2);
  position.xy -= float2(1, -1);
}


// Inverts the RGB color.
float4 PS(float2 texCoord : TEXCOORD0) : COLOR0
{
  // Get original color.
  float4 color = tex2D(SourceSampler, texCoord);
  
  // Linearly interpolate between original and inverted color.
  color.rgb = lerp(color.rgb, 1 - color.rgb, Strength);
    
  return color;
}


technique Technique0
{
  pass Pass0
  {
#if !SM4
    VertexShader = compile vs_2_0 VS();    
    PixelShader = compile ps_2_0 PS();
#else    
    VertexShader = compile vs_4_0_level_9_1 VS();
    PixelShader = compile ps_4_0_level_9_1 PS();
#endif
  }  
}
