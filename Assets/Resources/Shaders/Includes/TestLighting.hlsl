#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

// This is a neat trick to work around a bug in the shader graph when
// enabling shadow keywords. Created by @cyanilux
// https://github.com/Cyanilux/URP_ShaderGraphCustomLighting
// Licensed under the MIT License, Copyright (c) 2020 Cyanilux
#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#if (SHADERPASS != SHADERPASS_FORWARD)
#undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
#endif
#endif

struct CustomLightingData {
    // Position and orientation
    float3 positionWS;
    float3 normalWS;
    float3 viewDirectionWS;
    float4 shadowCoord;

    // Surface attributes
    float smoothness;
    float ambientOcclusion;
    float diffuse;
    float colour;

    // Baked lighting
    float3 bakedGI;
    float4 shadowMask;
    float fogFactor;
};

// Translate a [0, 1] smoothness value to an exponent 
float GetSmoothnessPower(float rawSmoothness) {
    return 1; // exp2(10 * rawSmoothness + 1);
}

#ifndef SHADERGRAPH_PREVIEW
float3 CustomGlobalIllumination(CustomLightingData d) {
    float3 indirectDiffuse = d.bakedGI * d.ambientOcclusion;

    float3 reflectVector = reflect(-d.viewDirectionWS, d.normalWS);
    // This is a rim light term, making reflections stronger along
    // the edges of view
    float fresnel = Pow4(1 - saturate(dot(d.viewDirectionWS, d.normalWS)));
    // This function samples the baked reflections cubemap
    // It is located in URP/ShaderLibrary/Lighting.hlsl
    float3 indirectSpecular = GlossyEnvironmentReflection(reflectVector,
        RoughnessToPerceptualRoughness(1 - d.smoothness),
        d.ambientOcclusion) * fresnel;

    return  indirectDiffuse + indirectSpecular;
}

float3 CustomLightHandling(CustomLightingData d, Light light) {

    float3 radiance = light.color;// *(light.distanceAttenuation * light.shadowAttenuation);

    d.diffuse = saturate(dot(d.normalWS, light.direction));
    float specularDot = saturate(dot(d.normalWS, normalize(light.direction + d.viewDirectionWS)));
    float specular = pow(specularDot, GetSmoothnessPower(d.smoothness)) * d.diffuse;

    float3 color = radiance;// *(diffuse + specular);

    return color;
}
#endif

void AddAdditionalLights(float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView,
    float MainDiffuse, float MainSpecular, float3 MainColor,
    out float Diffuse, out float Specular, out float3 Color) {
    Diffuse = MainDiffuse;
    Specular = MainSpecular;
    Color = MainColor * (MainDiffuse + MainSpecular);

#ifndef SHADERGRAPH_PREVIEW
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i) {
        Light light = GetAdditionalLight(i, WorldPosition);
        half NdotL = saturate(dot(WorldNormal, light.direction));
        half atten = light.distanceAttenuation * light.shadowAttenuation;
        half thisDiffuse = atten * NdotL;
        half thisSpecular = LightingSpecular(thisDiffuse, light.direction, WorldNormal, WorldView, 1, Smoothness);
        Diffuse += thisDiffuse;
        Specular += thisSpecular;
        Color += light.color * (thisDiffuse + thisSpecular);
    }
#endif

    half total = Diffuse + Specular;
    // If no light touches this pixel, set the color to the main light's color
    Color = total <= 0 ? MainColor : Color / total;
}


float3 CalculateCustomLighting(CustomLightingData d) {
#ifdef SHADERGRAPH_PREVIEW
    // In preview, estimate diffuse + specular
    float3 lightDir = float3(0.5, 0.5, 0);
    float intensity = 1;//saturate(dot(d.normalWS, lightDir)) + pow(saturate(dot(d.normalWS, normalize(d.viewDirectionWS + lightDir))), GetSmoothnessPower(d.smoothness));
    return intensity;
#else
    // Get the main light. Located in URP/ShaderLibrary/Lighting.hlsl
    Light mainLight = GetMainLight(d.shadowCoord, d.positionWS, d.shadowMask);
    // In mixed subtractive baked lights, the main light must be subtracted
    // from the bakedGI value. This function in URP/ShaderLibrary/Lighting.hlsl takes care of that.
    MixRealtimeAndBakedGI(mainLight, d.normalWS, d.bakedGI);
    float3 color = CustomGlobalIllumination(d);
    // Shade the main light
    color += CustomLightHandling(d, mainLight);

#ifdef _ADDITIONAL_LIGHTS
    // Shade additional cone and point lights. Functions in URP/ShaderLibrary/Lighting.hlsl
    uint numAdditionalLights = GetAdditionalLightsCount();
    for (uint lightI = 0; lightI < numAdditionalLights; lightI++) {
        //AddAdditionalLights(d.smoothness, d.positionWS, d.normalWS, d.viewDirectionWS,)
        Light light = GetAdditionalLight(lightI, d.positionWS, d.shadowMask);
        color += CustomLightHandling(d, light);
    }
#endif

    color = MixFog(color, d.fogFactor);

    return color;
#endif
}


void CalculateCustomLighting_float(float3 Position, float3 Normal, float3 ViewDirection,
    float Smoothness, float AmbientOcclusion,
    float2 LightmapUV,
    out float3 Color) {

    CustomLightingData d;
    d.positionWS = Position;
    d.normalWS = Normal;
    d.viewDirectionWS = ViewDirection;
    d.smoothness = Smoothness;
    d.ambientOcclusion = AmbientOcclusion;
    //Diffuse = 1;
#ifdef SHADERGRAPH_PREVIEW
    // In preview, there's no shadows or bakedGI
    d.shadowCoord = 0;
    d.bakedGI = 0;
    d.shadowMask = 0;
    d.fogFactor = 0;
    Color = CalculateCustomLighting(d);
#else
    // Calculate the main light shadow coord
    // There are two types depending on if cascades are enabled
    float4 positionCS = TransformWorldToHClip(Position);
#if SHADOWS_SCREEN
    d.shadowCoord = ComputeScreenPos(positionCS);
#else
    d.shadowCoord = TransformWorldToShadowCoord(Position);
#endif

    // The following URP functions and macros are all located in
    // URP/ShaderLibrary/Lighting.hlsl
    // Technically, OUTPUT_LIGHTMAP_UV, OUTPUT_SH and ComputeFogFactor
    // should be called in the vertex function of the shader. However, as of
    // 2021.1, we do not have access to custom interpolators in the shader graph.

    // The lightmap UV is usually in TEXCOORD1
    // If lightmaps are disabled, OUTPUT_LIGHTMAP_UV does nothing
    float2 lightmapUV;
    OUTPUT_LIGHTMAP_UV(LightmapUV, unity_LightmapST, lightmapUV);
    // Samples spherical harmonics, which encode light probe data
    float3 vertexSH;
    OUTPUT_SH(Normal, vertexSH);
    // This function calculates the final baked lighting from light maps or probes
    d.bakedGI = SAMPLE_GI(lightmapUV, vertexSH, Normal);
    // This function calculates the shadow mask if baked shadows are enabled
    d.shadowMask = SAMPLE_SHADOWMASK(lightmapUV);
    // This returns 0 if fog is turned off
    // It is not the same as the fog node in the shader graph
    d.fogFactor = ComputeFogFactor(positionCS.z);

    Light mainLight = GetMainLight(d.shadowCoord, d.positionWS, d.shadowMask);

    Color = CalculateCustomLighting(d);
    //Diffuse = d.diffuse;
#endif
}

#endif