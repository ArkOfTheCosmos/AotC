sampler uImage0 : register(s0);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 uShaderSpecificData;

float uRotation;
float2 uWorldPosition;
float3 uLightSource;
float2 uImageSize0;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float4 TrailShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{   
    float4 color = tex2D(uImage0, coords);
    
    return float4(95.0 / 255.0, 205.0 / 255.0, 228.0 / 255.0, 1) * color.a * uOpacity;
}

technique Technique1
{
    pass ExampleCyclePass
    {
        PixelShader = compile ps_2_0 TrailShaderFunction();
    }
}