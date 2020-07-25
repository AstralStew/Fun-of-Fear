// Copyright 2020 Visual Design Cafe. All rights reserved.
// Package: Nature Shaders
// Website: https://www.visualdesigncafe.com/nature-shaders
// Documentation: https://support.visualdesigncafe.com/hc/categories/900000043503

#ifndef STANDARD_RENDER_PIPELINE
    #error Surface Shader is only supported on the Standard Render Pipeline
#endif

float _Cutoff = 0.5;
float _ObjectHeight;
float _ObjectRadius;
float2 _TrunkBendFactor;
float _VertexNormalStrength;
float _WindVariation;
float _WindStrength;
float _TurbulenceStrength;
float2 _GlossRemap;
float2 _OcclusionRemap;
float _BumpScale;
float _Metallic;
float _Glossiness;

float3 _HSL;
float3 _HSLVariation;
float4 _Tint;
float4 _TintVariation;
float _ColorVariationSpread;

float2 _WindFade;
float2 _ScaleFade;

float _Translucency;
float _TranslucencyDistortion;
float _TranslucencyScattering;
float4 _TranslucencyColor;
float2 _ThicknessRemap;
sampler2D _ThicknessMap;

sampler2D _Albedo;
sampler2D _BumpMap;
sampler2D _MaskMap;
sampler2D _MetallicGlossMap;
sampler2D _OcclusionMap;

#include "Nodes/Common.cginc"
#include "Nodes/Wind/Wind.cginc"
#include "Nodes/Color Correction/HSL.cginc"
#include "Nodes/Lighting/Translucency.cginc"
#include "Nodes/Integrations/Nature Renderer.cginc"
#include "Nodes/Procedural/Perlin Noise.cginc"
#include "Nodes/Input/Fade.cginc"

#ifdef HIGH_DEFINITION_RENDER_PIPELINE
    #define ObjectToWorld UNITY_MATRIX_M
#else
    #define ObjectToWorld unity_ObjectToWorld
#endif

struct Input
{
    float2 uv_Albedo;
    float3 vertex;
    float3 worldPos;
    float noise;
};

#if defined(_TYPE_TREE_BILLBOARD)
    #include "Billboard.cginc"
#endif

void NatureShaderVertex( inout appdata_full v, out Input o )
{
    #if defined(_TYPE_TREE_BILLBOARD)
        Billboard_float(
            v.vertex, 
            v.texcoord, 
            v.texcoord1, 
            v.vertex, 
            v.normal, 
            v.tangent, 
            v.texcoord, 
            v.texcoord1 );
    #endif

    o.uv_Albedo = v.texcoord;
    o.vertex = v.vertex;

    #if !defined(_TYPE_TREE_BILLBOARD)
        float windFade;
        float scaleFade;
        GetFade_float( 
            float3(ObjectToWorld[0][3], ObjectToWorld[1][3], ObjectToWorld[2][3]),
            windFade,
            scaleFade );
    #endif

    o.worldPos = TransformObjectToWorld(v.vertex);
    #if !defined(_TYPE_TREE_BILLBOARD)
        if( windFade > 0)
        {
            float3 vertexOut;
            float3 normalOut;
            Wind_float( 
                v.vertex, 
                o.worldPos,
                v.normal, 
                v.texcoord, 
                v.texcoord1, 
                v.color, 
                vertexOut, 
                normalOut );

            vertexOut = lerp(o.worldPos, vertexOut, windFade);
            ApplyScaleFade_float( 
                vertexOut, 
                float3(ObjectToWorld[0][3], ObjectToWorld[1][3], ObjectToWorld[2][3]),
                scaleFade, 
                vertexOut );

            v.vertex = float4(TransformWorldToObject(vertexOut), v.vertex.w);
            v.normal = lerp(v.normal, normalOut, windFade);
        }
        else
        {
            ApplyScaleFade_OS_float( v.vertex.xyz, scaleFade, v.vertex.xyz );
        }

        v.normal = lerp(float3(0,1,0), v.normal, _VertexNormalStrength);
    #endif
    
    PerlinNoise_float(
        float2(ObjectToWorld[0][3], ObjectToWorld[2][3]), 
        _ColorVariationSpread, 
        o.noise);
}

#ifdef TRANSLUCENT
    void NatureShaderSurface( Input i , inout SurfaceOutputStandardCustom o )
#else
    void NatureShaderSurface( Input i , inout SurfaceOutputStandard o )
#endif
{
    // Albedo
    float4 albedo = tex2D( _Albedo, i.uv_Albedo );
    #ifndef _TYPE_TREE_BARK
        clip( albedo.a  - _Cutoff );
    #endif
    
    #ifdef _COLOR_HSL
        float4 albedo1 = albedo;
        float4 albedo2 = albedo;
        HSL_float( albedo1, _HSL, albedo1 );
        HSL_float( albedo2, _HSLVariation, albedo2 );
        albedo = lerp(albedo2, albedo1, i.noise);
    #else
        albedo *= lerp(_TintVariation, _Tint, i.noise);
    #endif

    o.Albedo = albedo.rgb;
    o.Alpha = albedo.a;

    // Normal
    o.Normal = UnpackScaleNormal( tex2D( _BumpMap, i.uv_Albedo ), _BumpScale );

    // Mask Map
    #ifdef _SURFACE_MAP_MASK
        float4 maskMap = tex2D(_MaskMap, i.uv_Albedo);
        o.Metallic = maskMap.r;
        o.Smoothness = Remap(maskMap.a, _GlossRemap);
        o.Occlusion = Remap(maskMap.g, _OcclusionRemap);
    #else
        #ifdef _METALLICGLOSSMAP_ON
            float4 metallicGloss = tex2D( _MetallicGlossMap, i.uv_Albedo );
            float metallic = metallicGloss.r;
            float glossiness = Remap(metallicGloss.a, _GlossRemap);
        #else
            float glossiness = _Glossiness;
            float metallic = _Metallic;
        #endif

        o.Metallic = metallic;
        o.Smoothness = glossiness;

        #ifdef PROPERTY_OcclusionMap
            float occlusion = tex2D( _OcclusionMap, i.uv_Albedo ).g;
            o.Occlusion = Remap(occlusion, _OcclusionRemap);
        #else
            o.Occlusion = 1;
        #endif
    #endif

    // Translucency
    #ifdef TRANSLUCENT
        #ifdef PROPERTY_ThicknessMap
            o.Thickness = tex2D( _ThicknessMap, i.uv_Albedo ).r;
        #else
            o.Thickness = 0;
        #endif

        #ifdef PROPERTY_ThicknessRemap
            o.Thickness = Remap( o.Thickness, _ThicknessRemap );
        #endif

        #if defined(_TYPE_GRASS)
            o.Thickness = 1 - (1 - o.Thickness) * saturate(i.vertex.y / GetObjectHeight());
        #endif
    #endif
}

void NatureShaderSurface_float(
    float2 uv0,
    float3 vertex,
    float3 vertexWorldPosition,
    float noise,
    out float3 albedo,
    out float alpha,
    out float3 normal,
    out float metallic,
    out float smoothness,
    out float occlusion,
    out float thickness )
{
    Input i;
    i.uv_Albedo = uv0;
    i.vertex = vertex;
    i.worldPos = vertexWorldPosition;
    i.noise = noise;

    #ifdef TRANSLUCENT
        SurfaceOutputStandardCustom o = (SurfaceOutputStandardCustom)0;
    #else
        SurfaceOutputStandard o = (SurfaceOutputStandard)0;
    #endif
    NatureShaderSurface(i, o);

    albedo = o.Albedo;
    alpha = o.Alpha;
    normal = o.Normal;
    metallic = o.Metallic;
    smoothness = o.Smoothness;
    occlusion = o.Occlusion;
    #ifdef TRANSLUCENT
        thickness = o.Thickness;
    #else
        thickness = 0;
    #endif
}