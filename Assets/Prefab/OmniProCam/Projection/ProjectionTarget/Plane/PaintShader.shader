/******************************************************************************************
Fisheye Projection Shader for Baloonigen, OmniProCamLight and OmniProCam Project
Created on 2018/09/05 by SATOToshiki@TokyoTech
******************************************************************************************/

Shader "Projection/FisheyePaintProjection"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_CanvasTex("Texture", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 100
		Cull off

		Pass{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata {
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f {
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	sampler2D _MainTex;
	sampler2D _CanvasTex;

	FragInput vertexShader(VertInput vertexInput) {
		FragInput vertexOutput;
		vertexOutput.vertexPosition = UnityObjectToClipPos(vertexInput.vertexPosition);
		vertexOutput.texCoord = MultiplyUV(UNITY_MATRIX_TEXTURE0, vertexInput.texCoord);
		return vertexOutput;
	} // vertexShader

	fixed4 frag(v2f i) : SV_Target{
		return tex2D(_MainTex, i.uv) + tex2D(_CanvasTex, i.uv);
	}
	ENDCG
	}
	}
}
