sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float2 uTargetPosition;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;
float4 uShaderSpecificData;

float4 FilterMyShader(float2 coords : TEXCOORD0) : COLOR0 {
    float4 color = tex2D(uImage1, coords);
    float4 baseColor = tex2D(uImage0, coords);
    
    // Extract the per-pixel opacity value from the base color's alpha channel
    float pixelOpacity = baseColor.a;
    
    // Apply the opacity to the color
    color *= pixelOpacity;
    
    // Optionally, you can apply other modifications based on the pixel opacity
    
    return color;

}

technique Technique1 {
    pass FilterMyShader {
        PixelShader = compile ps_2_0 FilterMyShader();
    }
}