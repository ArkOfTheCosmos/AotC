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

float uShift;

static const float PI = 3.14159265f;

float3 ApplyHue(float3 col, float hueAdjust)
{
    const float3 k = float3(0.57735, 0.57735, 0.57735);
    hueAdjust *= PI * 2;
    half cosAngle = sin(hueAdjust + 1.57079);
    return col * cosAngle + cross(k, col) * sin(hueAdjust) + k * dot(k, col) * (1.0 - cosAngle);
}

float4 HueShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    color.rgb = ApplyHue(color.rgb, uShift); 
    return color * uOpacity;
}


technique Technique1
{
    pass HueShiftPass
    {
        PixelShader = compile ps_2_0 HueShaderFunction();
    }
}