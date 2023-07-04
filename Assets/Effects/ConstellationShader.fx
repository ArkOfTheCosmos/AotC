sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
    
sampler2D imageTexture : register(s0);

float4 ImageShader(float4 position : SV_Position, float2 texCoord : TEXCOORD0) : SV_Target
{
    return tex2D(imageTexture, texCoord);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 ImageShader();
    }
}