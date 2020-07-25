// Copyright 2020 Visual Design Cafe. All rights reserved.
// Package: Nature Shaders
// Website: https://www.visualdesigncafe.com/nature-shaders
// Documentation: https://support.visualdesigncafe.com/hc/categories/900000043503

#ifndef NODE_INSTANCED_INCLUDED
#define NODE_INSTANCED_INCLUDED

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

struct CompressedFloat4x4
{
    uint positionXY;
    uint positionZ_scaleXZ;
    uint scaleY_rotationX;
    uint rotationZW;
};

uniform float3 _CompressionRange;
uniform float3 _CompressionBase;

inline void Unpack( uint packed, out float a, out float b )
{
    a = (packed >> 16) / 65535.0;
    b = ((packed << 16) >> 16) / 65535.0;
}

inline float4x4 QuaternionToMatrix(float4 quaternion)
{
    float4x4 result = (float4x4)0;
    float x = quaternion.x;
    float y = quaternion.y;
    float z = quaternion.z;
    float w = quaternion.w;

    float x2 = x + x;
    float y2 = y + y;
    float z2 = z + z;
    float xx = x * x2;
    float xy = x * y2;
    float xz = x * z2;
    float yy = y * y2;
    float yz = y * z2;
    float zz = z * z2;
    float wx = w * x2;
    float wy = w * y2;
    float wz = w * z2;

    result[0][0] = 1.0 - (yy + zz);
    result[0][1] = xy - wz;
    result[0][2] = xz + wy;

    result[1][0] = xy + wz;
    result[1][1] = 1.0 - (xx + zz);
    result[1][2] = yz - wx;

    result[2][0] = xz - wy;
    result[2][1] = yz + wx;
    result[2][2] = 1.0 - (xx + yy);

    result[3][3] = 1.0;

    return result;
}

inline void Decompress( inout float4x4 instance, CompressedFloat4x4 compressed )
{
    float positionX;
    float positionY;
    float positionZ;

    float scaleXZ;
    float scaleY;

    float rotationX;
    float rotationY;
    float rotationZ;
    float rotationW;

    Unpack( compressed.positionXY, positionX, positionY );
    Unpack( compressed.positionZ_scaleXZ, positionZ, scaleXZ );
    Unpack( compressed.scaleY_rotationX, scaleY, rotationX );
    Unpack( compressed.rotationZW, rotationZ, rotationW );

    positionX = positionX * _CompressionRange.x + _CompressionBase.x;
    positionY = positionY * _CompressionRange.y + _CompressionBase.y;
    positionZ = positionZ * _CompressionRange.z + _CompressionBase.z;

    scaleXZ *= 16.0;
    scaleY *= 16.0;

    rotationX = rotationX * 2.0 - 1.0;
    rotationZ = rotationZ * 2.0 - 1.0;
    rotationW = rotationW * 2.0 - 1.0;
    rotationY = 
        sqrt( 1.0 - (rotationX * rotationX + rotationZ * rotationZ + rotationW * rotationW) );

    float3 position = float3(positionX, positionY, positionZ);
    float3 scale = float3(scaleXZ, scaleY, scaleXZ);
    instance = QuaternionToMatrix( float4(rotationX, rotationY, rotationZ, rotationW) );
    
    instance[0][0] *= scale.x; instance[1][0] *= scale.y; instance[2][0] *= scale.z;
    instance[0][1] *= scale.x; instance[1][1] *= scale.y; instance[2][1] *= scale.z;
    instance[0][2] *= scale.x; instance[1][2] *= scale.y; instance[2][2] *= scale.z;
    instance[0][3] *= scale.x; instance[1][3] *= scale.y; instance[2][3] *= scale.z;

    instance[0][3] = position.x;
    instance[1][3] = position.y;
    instance[2][3] = position.z;
}

#if defined(SHADER_API_GLCORE) || defined(SHADER_API_D3D11) || defined(SHADER_API_GLES3) || defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN) || defined(SHADER_API_PSSL) || defined(SHADER_API_XBOXONE)
uniform StructuredBuffer<CompressedFloat4x4> _NatureRendererBuffer;
#endif

float4x4 inverse(float4x4 input)
 {
     #define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
     
     float4x4 cofactors = float4x4(
          minor(_22_23_24, _32_33_34, _42_43_44), 
         -minor(_21_23_24, _31_33_34, _41_43_44),
          minor(_21_22_24, _31_32_34, _41_42_44),
         -minor(_21_22_23, _31_32_33, _41_42_43),
         
         -minor(_12_13_14, _32_33_34, _42_43_44),
          minor(_11_13_14, _31_33_34, _41_43_44),
         -minor(_11_12_14, _31_32_34, _41_42_44),
          minor(_11_12_13, _31_32_33, _41_42_43),
         
          minor(_12_13_14, _22_23_24, _42_43_44),
         -minor(_11_13_14, _21_23_24, _41_43_44),
          minor(_11_12_14, _21_22_24, _41_42_44),
         -minor(_11_12_13, _21_22_23, _41_42_43),
         
         -minor(_12_13_14, _22_23_24, _32_33_34),
          minor(_11_13_14, _21_23_24, _31_33_34),
         -minor(_11_12_14, _21_22_24, _31_32_34),
          minor(_11_12_13, _21_22_23, _31_32_33)
     );
     #undef minor
     return transpose(cofactors) / determinant(input);
 }
#endif

void SetupNatureRenderer()
{
    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        #ifdef HIGH_DEFINITION_RENDER_PIPELINE
            #undef unity_ObjectToWorld
            #undef unity_WorldToObject
        #endif
        
        Decompress(unity_ObjectToWorld, _NatureRendererBuffer[unity_InstanceID]);
        unity_WorldToObject = inverse(unity_ObjectToWorld);
    #endif
}

void Instanced_float( float3 vertex, out float3 vertexOut )
{
    vertexOut = vertex;
}
#endif