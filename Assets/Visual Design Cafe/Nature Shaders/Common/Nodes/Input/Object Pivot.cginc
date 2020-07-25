// Copyright 2020 Visual Design Cafe. All rights reserved.
// Package: Nature Shaders
// Website: https://www.visualdesigncafe.com/nature-shaders
// Documentation: https://support.visualdesigncafe.com/hc/categories/900000043503

#ifndef NODE_OBJECT_PIVOT_INCLUDED
#define NODE_OBJECT_PIVOT_INCLUDED

#include "Wind Properties.cginc"

/// <summary>
/// Returns the pivot of the object in world space.
/// </summary>
float3 GetObjectPivot()
{
    return GetAbsolutePositionWS( TransformObjectToWorld( float3(0,0,0) ) );
}

void GetObjectPivot_float( out float3 objectPivot )
{
    objectPivot = GetObjectPivot();
}

#endif