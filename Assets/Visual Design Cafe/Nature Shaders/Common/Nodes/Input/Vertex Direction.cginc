// Copyright 2020 Visual Design Cafe. All rights reserved.
// Package: Nature Shaders
// Website: https://www.visualdesigncafe.com/nature-shaders
// Documentation: https://support.visualdesigncafe.com/hc/categories/900000043503

#ifndef NODE_VERTEX_DIRECTION_INCLUDED
#define NODE_VERTEX_DIRECTION_INCLUDED

#include "Wind Properties.cginc"

/// <summary>
/// Returns the vertex normal in world space when vertex normals are anbled.
/// Otherwise, returns the object's forward (Z+) direction.
/// </summary>
float3 GetWorldNormal( 
    float3 vertexNormal, // The vertex normal in object space.
    float3 objectPivot ) // The object pivot in world space.
{
    #if defined(_TYPE_GRASS)
        return GetAbsolutePositionWS( TransformObjectToWorld( float3(0, 0, 1) ) ) - objectPivot;
    #elif defined(_TYPE_TREE_LEAVES)
        return TransformWorldToObjectDir(vertexNormal);
    #elif defined( _TYPE_TREE_BARK )
        return GetAbsolutePositionWS( TransformObjectToWorld( float3(0, 0, 1) ) ) - objectPivot;
    //#elif defined(_TYPE_PLANT)
    #else
        return GetAbsolutePositionWS( TransformObjectToWorld( float3(0, 0, 1) ) ) - objectPivot;
    #endif
    //#else
    //    #error Invalid Vegetation Type
    //#endif
}

void GetWorldNormal_float( 
    float3 vertexNormal, // The vertex normal in object space.
    float3 objectPivot,  // The object pivot in world space.
    out float3 worldNormal )
{
    worldNormal = GetWorldNormal( vertexNormal, objectPivot );
}

#endif