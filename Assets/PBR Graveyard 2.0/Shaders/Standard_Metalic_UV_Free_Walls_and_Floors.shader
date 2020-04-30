// Upgrade NOTE: upgraded instancing buffer 'NatureManufactureShadersStandardMetalicUVWallsandFloors' to new syntax.

// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "NatureManufacture Shaders/Standard Metalic UV Walls and Floors"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_TilledAlbedoRGB("Tilled Albedo (RGB)", 2D) = "white" {}
		_Tilling("Tilling", Range( 0 , 100)) = -1.570796
		_AlbedoColor("Albedo Color", Color) = (1,1,1,1)
		_TilledNormalRGB("Tilled Normal (RGB)", 2D) = "bump" {}
		_TilledNormalPower("Tilled Normal Power", Range( 0 , 2)) = 1
		_TilledMetalicRAmbientOcclusionGSmothnessA("Tilled Metalic (R) Ambient Occlusion (G) Smothness (A)", 2D) = "white" {}
		_TilledMetalicPower("Tilled Metalic Power", Range( 0 , 2)) = 1
		_TilledAOPower("Tilled AO Power", Range( 0 , 1)) = 0
		_TilledSmothnessPower("Tilled Smothness Power", Range( 0 , 2)) = 1
		_SnowAmount("SnowAmount", Range( 0 , 2)) = 0
		_SnowHardness("Snow Hardness", Range( 0.01 , 10)) = 1
		_SnowAlbedoRGB("Snow Albedo (RGB)", 2D) = "white" {}
		_SnowMaxAngle("Snow Max Angle", Range( 0.001 , 180)) = 1
		_SnowAlbedoColor("Snow Albedo Color", Color) = (1,1,1,1)
		_SnowTilling("Snow Tilling", Range( 0 , 100)) = -1.570796
		_SnowNormalRGB("Snow Normal (RGB)", 2D) = "bump" {}
		_SnowNormalScale("Snow Normal Scale", Range( 0 , 2)) = 1
		_SnowMetalicRAmbientOcclusionGSmothnessA("Snow Metalic (R) Ambient Occlusion(G) Smothness (A)", 2D) = "black" {}
		_SnowMetalicPower("Snow Metalic Power", Range( 0 , 2)) = 1
		_SnowAmbientOcclusionPower("Snow Ambient Occlusion Power", Range( 0 , 1)) = 0
		_SnowSmothnessPower("Snow Smothness Power", Range( 0 , 2)) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _TilledNormalPower;
		uniform sampler2D _TilledNormalRGB;
		uniform float _Tilling;
		uniform float _SnowNormalScale;
		uniform sampler2D _SnowNormalRGB;
		uniform float _SnowTilling;
		uniform float _SnowAmount;
		uniform sampler2D _TilledAlbedoRGB;
		uniform sampler2D _SnowAlbedoRGB;
		uniform float _SnowMaxAngle;
		uniform float _SnowHardness;
		uniform sampler2D _TilledMetalicRAmbientOcclusionGSmothnessA;
		uniform float _TilledMetalicPower;
		uniform sampler2D _SnowMetalicRAmbientOcclusionGSmothnessA;
		uniform float _SnowMetalicPower;
		uniform float _TilledSmothnessPower;
		uniform float _SnowSmothnessPower;
		uniform float _TilledAOPower;
		uniform float _SnowAmbientOcclusionPower;

		UNITY_INSTANCING_BUFFER_START(NatureManufactureShadersStandardMetalicUVWallsandFloors)
			UNITY_DEFINE_INSTANCED_PROP(float4, _AlbedoColor)
#define _AlbedoColor_arr NatureManufactureShadersStandardMetalicUVWallsandFloors
			UNITY_DEFINE_INSTANCED_PROP(float4, _SnowAlbedoColor)
#define _SnowAlbedoColor_arr NatureManufactureShadersStandardMetalicUVWallsandFloors
		UNITY_INSTANCING_BUFFER_END(NatureManufactureShadersStandardMetalicUVWallsandFloors)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float2 appendResult879 = float2( ase_worldPos.y , ase_worldPos.z );
			float2 Front_Back = appendResult879;
			float2 appendResult3212 = float2( _Tilling , _Tilling );
			float cos3198 = cos( -1.570796 );
			float sin3198 = sin( -1.570796 );
			float2 rotator3198 = mul(( Front_Back * ( float2( 1,1 ) * appendResult3212 ) ) - float2( 0.5,0.5 ), float2x2(cos3198,-sin3198,sin3198,cos3198)) + float2( 0.5,0.5 );
			float2 appendResult3464 = float2( ( rotator3198.x * -1 ) , rotator3198.y );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float UVslopeY = ( 1.0 - pow( ( 1.0 - ( clamp( ( clamp( ase_worldNormal.y , 0.0 , 0.9999 ) - ( 1.0 - ( 45.0 / 90.0 ) ) ) , 0.0 , 2.0 ) * ( 1.0 / ( 1.0 - ( 1.0 - ( 45.0 / 90.0 ) ) ) ) ) ) , 5.0 ) );
			float2 appendResult2002 = float2( ase_worldPos.x , ase_worldPos.y );
			float2 Left_Right = appendResult2002;
			float2 appendResult1998 = float2( ase_worldPos.x , ase_worldPos.z );
			float2 Top_Bottom = appendResult1998;
			float2 appendResult3396 = float2( _SnowTilling , _SnowTilling );
			float cos3398 = cos( -1.570796 );
			float sin3398 = sin( -1.570796 );
			float2 rotator3398 = mul(( Front_Back * ( float2( 1,1 ) * appendResult3396 ) ) - float2( 0.5,0.5 ), float2x2(cos3398,-sin3398,sin3398,cos3398)) + float2( 0.5,0.5 );
			float2 appendResult3467 = float2( ( rotator3398.x * -1 ) , rotator3398.y );
			o.Normal = normalize( lerp( normalize( ( ( lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,appendResult3464) ,_TilledNormalPower ) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,( Left_Right * ( float2( 1,1 ) * appendResult3212 ) )) ,_TilledNormalPower ) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.5 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3212 ) )) ,_TilledNormalPower ) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) ) , ( ( lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,appendResult3467) ,_SnowNormalScale ) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,( Left_Right * ( float2( 1,1 ) * appendResult3396 ) )) ,_SnowNormalScale ) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3396 ) )) ,_SnowNormalScale ) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) , saturate( ( ase_worldNormal.y * _SnowAmount ) ) ) );
			o.Albedo = clamp( lerp( ( ( ( lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledAlbedoRGB,appendResult3464) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledAlbedoRGB,( Left_Right * ( float2( 1,1 ) * appendResult3212 ) )) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledAlbedoRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3212 ) )) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) * UNITY_ACCESS_INSTANCED_PROP(_AlbedoColor_arr, _AlbedoColor) ) , ( ( ( lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowAlbedoRGB,appendResult3467) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowAlbedoRGB,( Left_Right * ( float2( 1,1 ) * appendResult3396 ) )) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowAlbedoRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3396 ) )) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) * UNITY_ACCESS_INSTANCED_PROP(_SnowAlbedoColor_arr, _SnowAlbedoColor) ) , saturate( lerp( ( WorldNormalVector( i , lerp( normalize( ( ( lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,appendResult3464) ,_TilledNormalPower ) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,( Left_Right * ( float2( 1,1 ) * appendResult3212 ) )) ,_TilledNormalPower ) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.5 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3212 ) )) ,_TilledNormalPower ) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) ) , ( ( lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,appendResult3467) ,_SnowNormalScale ) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,( Left_Right * ( float2( 1,1 ) * appendResult3396 ) )) ,_SnowNormalScale ) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3396 ) )) ,_SnowNormalScale ) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) , saturate( ( ase_worldNormal.y * _SnowAmount ) ) ) ).y * _SnowAmount ) , 0.0 , pow( ( 1.0 - ( clamp( ( clamp( ase_worldNormal.y , 0.0 , 0.999999 ) - ( 1.0 - ( _SnowMaxAngle / 90.0 ) ) ) , 0.0 , 2.0 ) * ( 1.0 / ( 1.0 - ( 1.0 - ( _SnowMaxAngle / 90.0 ) ) ) ) ) ) , _SnowHardness ) ) ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) ).rgb;
			o.Metallic = lerp( ( ( ( lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledMetalicRAmbientOcclusionGSmothnessA,appendResult3464) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledMetalicRAmbientOcclusionGSmothnessA,( Left_Right * ( float2( 1,1 ) * appendResult3212 ) )) * ase_worldNormal.z ) , ase_worldNormal.z ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledMetalicRAmbientOcclusionGSmothnessA,( Top_Bottom * ( float2( 1,1 ) * appendResult3212 ) )) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ).x * _TilledMetalicPower ) , ( ( ( lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowMetalicRAmbientOcclusionGSmothnessA,appendResult3467) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * ( 1.0 - UVslopeY ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowMetalicRAmbientOcclusionGSmothnessA,( Left_Right * ( float2( 1,1 ) * appendResult3396 ) )) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.y * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowMetalicRAmbientOcclusionGSmothnessA,( Top_Bottom * ( float2( 1,1 ) * appendResult3396 ) )) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ).x * _SnowMetalicPower ) , saturate( lerp( ( WorldNormalVector( i , lerp( normalize( ( ( lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,appendResult3464) ,_TilledNormalPower ) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,( Left_Right * ( float2( 1,1 ) * appendResult3212 ) )) ,_TilledNormalPower ) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.5 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3212 ) )) ,_TilledNormalPower ) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) ) , ( ( lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,appendResult3467) ,_SnowNormalScale ) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,( Left_Right * ( float2( 1,1 ) * appendResult3396 ) )) ,_SnowNormalScale ) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3396 ) )) ,_SnowNormalScale ) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) , saturate( ( ase_worldNormal.y * _SnowAmount ) ) ) ).y * _SnowAmount ) , 0.0 , pow( ( 1.0 - ( clamp( ( clamp( ase_worldNormal.y , 0.0 , 0.999999 ) - ( 1.0 - ( _SnowMaxAngle / 90.0 ) ) ) , 0.0 , 2.0 ) * ( 1.0 / ( 1.0 - ( 1.0 - ( _SnowMaxAngle / 90.0 ) ) ) ) ) ) , _SnowHardness ) ) ) );
			o.Smoothness = lerp( ( ( ( lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledMetalicRAmbientOcclusionGSmothnessA,appendResult3464) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledMetalicRAmbientOcclusionGSmothnessA,( Left_Right * ( float2( 1,1 ) * appendResult3212 ) )) * ase_worldNormal.z ) , ase_worldNormal.z ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledMetalicRAmbientOcclusionGSmothnessA,( Top_Bottom * ( float2( 1,1 ) * appendResult3212 ) )) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ).w * _TilledSmothnessPower ) , ( ( ( lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowMetalicRAmbientOcclusionGSmothnessA,appendResult3467) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * ( 1.0 - UVslopeY ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowMetalicRAmbientOcclusionGSmothnessA,( Left_Right * ( float2( 1,1 ) * appendResult3396 ) )) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.y * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowMetalicRAmbientOcclusionGSmothnessA,( Top_Bottom * ( float2( 1,1 ) * appendResult3396 ) )) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ).w * _SnowSmothnessPower ) , saturate( lerp( ( WorldNormalVector( i , lerp( normalize( ( ( lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,appendResult3464) ,_TilledNormalPower ) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,( Left_Right * ( float2( 1,1 ) * appendResult3212 ) )) ,_TilledNormalPower ) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.5 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3212 ) )) ,_TilledNormalPower ) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) ) , ( ( lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,appendResult3467) ,_SnowNormalScale ) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,( Left_Right * ( float2( 1,1 ) * appendResult3396 ) )) ,_SnowNormalScale ) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3396 ) )) ,_SnowNormalScale ) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) , saturate( ( ase_worldNormal.y * _SnowAmount ) ) ) ).y * _SnowAmount ) , 0.0 , pow( ( 1.0 - ( clamp( ( clamp( ase_worldNormal.y , 0.0 , 0.999999 ) - ( 1.0 - ( _SnowMaxAngle / 90.0 ) ) ) , 0.0 , 2.0 ) * ( 1.0 / ( 1.0 - ( 1.0 - ( _SnowMaxAngle / 90.0 ) ) ) ) ) ) , _SnowHardness ) ) ) );
			o.Occlusion = lerp( clamp( ( ( lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledMetalicRAmbientOcclusionGSmothnessA,appendResult3464) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledMetalicRAmbientOcclusionGSmothnessA,( Left_Right * ( float2( 1,1 ) * appendResult3212 ) )) * ase_worldNormal.z ) , ase_worldNormal.z ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _TilledMetalicRAmbientOcclusionGSmothnessA,( Top_Bottom * ( float2( 1,1 ) * appendResult3212 ) )) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ).y , ( 1.0 - _TilledAOPower ) , 1.0 ) , clamp( ( ( lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowMetalicRAmbientOcclusionGSmothnessA,appendResult3467) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * ( 1.0 - UVslopeY ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowMetalicRAmbientOcclusionGSmothnessA,( Left_Right * ( float2( 1,1 ) * appendResult3396 ) )) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.y * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float4( 0,0,0,0 ) , ( tex2D( _SnowMetalicRAmbientOcclusionGSmothnessA,( Top_Bottom * ( float2( 1,1 ) * appendResult3396 ) )) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ).y , ( 1.0 - _SnowAmbientOcclusionPower ) , 1.0 ) , saturate( lerp( ( WorldNormalVector( i , lerp( normalize( ( ( lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,appendResult3464) ,_TilledNormalPower ) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,( Left_Right * ( float2( 1,1 ) * appendResult3212 ) )) ,_TilledNormalPower ) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.5 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _TilledNormalRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3212 ) )) ,_TilledNormalPower ) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) ) , ( ( lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,appendResult3467) ,_SnowNormalScale ) * ase_worldNormal.x ) , ( clamp( ( ase_worldNormal.x * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,( Left_Right * ( float2( 1,1 ) * appendResult3396 ) )) ,_SnowNormalScale ) * ase_worldNormal.z ) , ( clamp( ( ase_worldNormal.z * 1.0 ) , -1.0 , 1.0 ) * clamp( ( 1.0 - UVslopeY ) , -1.0 , 1.0 ) ) ) ) + lerp( float3( 0,0,0 ) , ( UnpackScaleNormal( tex2D( _SnowNormalRGB,( Top_Bottom * ( float2( 1,1 ) * appendResult3396 ) )) ,_SnowNormalScale ) * ase_worldNormal.y ) , clamp( ( UVslopeY * 1.5 ) , -1.0 , 1.0 ) ) ) , saturate( ( ase_worldNormal.y * _SnowAmount ) ) ) ).y * _SnowAmount ) , 0.0 , pow( ( 1.0 - ( clamp( ( clamp( ase_worldNormal.y , 0.0 , 0.999999 ) - ( 1.0 - ( _SnowMaxAngle / 90.0 ) ) ) , 0.0 , 2.0 ) * ( 1.0 / ( 1.0 - ( 1.0 - ( _SnowMaxAngle / 90.0 ) ) ) ) ) ) , _SnowHardness ) ) ) );
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD6;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
