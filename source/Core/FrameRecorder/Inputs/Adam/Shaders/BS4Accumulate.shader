// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BeautyShot/Accumulate" {
Properties {
	_MainTex("Diffuse", 2D) = "white" {}
}

CGINCLUDE

#pragma only_renderers d3d11 ps4 opengl

#include "UnityCG.cginc"

struct v2f {
	float4 pos	: SV_Position;
	float2 uv	: TEXCOORD0;
};

v2f vert(appdata_img v)  {
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord;
	return o;
}

uniform sampler2D _MainTex;
uniform float4 _MainTex_TexelSize;

uniform sampler2D _PreviousTexture;

float4 frag(v2f i) : SV_Target {
	float4 previous = tex2D(_PreviousTexture, i.uv);
	float4 current = tex2D(_MainTex, i.uv);
	
	return previous + current;
}

ENDCG

SubShader {
	Cull Off ZTest Always ZWrite Off

	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		ENDCG
	}
}}