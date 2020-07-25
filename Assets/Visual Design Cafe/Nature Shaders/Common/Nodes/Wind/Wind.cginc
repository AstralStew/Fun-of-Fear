// Copyright 2020 Visual Design Cafe. All rights reserved.
// Package: Nature Shaders
// Website: https://www.visualdesigncafe.com/nature-shaders
// Documentation: https://support.visualdesigncafe.com/hc/categories/900000043503

#ifndef NODE_WIND_INCLUDED
#define NODE_WIND_INCLUDED

#include "../Common.cginc"
#include "../Input/Wind Properties.cginc"
#include "../Input/Wind Direction.cginc"
#include "../Input/Object Pivot.cginc"
#include "../Input/Phase Offset.cginc"
#include "../Input/Wind Speed.cginc"
#include "../Input/Vertex Direction.cginc"
#include "../Input/Height Mask.cginc"
#include "../Input/Wind Variation.cginc"
#include "../Input/Vertex Mask.cginc"
#include "../Input/Edge Flutter.cginc"
#include "../Input/Fade.cginc"
#include "Compute Ambient Wind.cginc"
#include "Compute Gust.cginc"
#include "Compute Turbulence.cginc"
#include "Combine Wind.cginc"
#include "Compute Wind.cginc"
#include "Apply Wind.cginc"
#include "Trunk Movement.cginc"

/// <summary>
/// Calculates the wind for the vertex with the default values and applies it.
/// </summary>
void Wind_float(
    float4 vertex,              // The vertex position in model space.
    float3 vertexWorldPosition, // The vertex position in world space.
    float3 normal,              // The vertex normal in model space.
    float4 texcoord0,           // The first UV coordinate.
    float4 texcoord1,           // The second UV coordinate.
    float4 color,               // Per-vertex color
    out float3 vertexOut,
    out float3 normalOut )
{
    // Properties.
    float3 windDirection = GetWindDirection();
    float3 pivot = GetObjectPivot();

    #if !defined(_TYPE_TREE_BILLBOARD)
        float phaseOffset = GetPhaseOffset( color, texcoord0, vertexWorldPosition.xyz, pivot );
        float speed = GetWindSpeed();
        float3 worldNormal = GetWorldNormal( normal, pivot );

        // Mask.
        float heightMask = GetHeightMask( vertex, color, texcoord0, texcoord1 );
        float windVariation = GetWindVariation( pivot );
        float vertexMask = GetVertexMask( color );
        float mask = heightMask * vertexMask * windVariation;
        float edgeFlutter = GetEdgeFlutter( color, texcoord0 );

        // Compute wind.
        float3 wind;
        ComputeWind_float( windDirection, pivot, vertexWorldPosition.xyz, color, phaseOffset, edgeFlutter, speed, worldNormal, wind );

        // Distance fade.
        float windFade;
        float scaleFade;
        GetFade_float( pivot, windFade, scaleFade );
        windFade = 1;

        // Apply wind to vertex.
        ApplyWind_float( vertexWorldPosition.xyz, pivot, wind, mask, windFade, vertexOut );
    #else
        vertexOut = vertexWorldPosition;
    #endif

    #if defined(_TYPE_GRASS)
        normalOut = 
            normalize(
                normal + TransformWorldToObjectDir( vertexOut - vertexWorldPosition + float3(0,1,0) ) );
    #else
        normalOut = normal;
    #endif

    // Trunk Rotation
    #if defined(_TYPE_TREE_LEAVES) || defined( _TYPE_TREE_BARK ) || defined(_TYPE_TREE_BILLBOARD)
        TrunkMovement_float( vertex, vertexWorldPosition.xyz, vertexOut, texcoord1, pivot, windDirection, vertexOut );
    #endif
}

#endif