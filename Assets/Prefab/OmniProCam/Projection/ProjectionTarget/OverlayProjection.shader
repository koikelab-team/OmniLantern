/**************************************************************
			OmniProCam用	2019/09/21 by 佐藤俊樹@JAIST
**************************************************************/

Shader "OmniProCam/OverlayProjection" {

	Properties{
		_FrontTex("Foreground(RGB)", 2D) = "black" {}
		_BackTex0("Background0(RGB)", 2D) = "black" {}
		_AlphaTex0("Alpha0", 2D) = "gray" {}
	}

		SubShader{

		Pass{
		CGPROGRAM

		struct VertInput {
		float4 vertexPosition :		POSITION;
		float2 texCoord :					TEXCOORD0;
		float2 texCoordCamera0 :	TEXCOORD1;
	};

	struct FragInput {
		float4 vertexPosition :		POSITION;
		float2 texCoord :					TEXCOORD0;
		float2 texCoordCamera0 :	TEXCOORD1;
	};

#pragma vertex vertexShader
#pragma fragment fragmentShader
#pragma target 3.0

#include "UnityCG.cginc"

	uniform sampler2D _FrontTex;
	uniform sampler2D _BackTex0;
	uniform sampler2D _AlphaTex0;
	uniform float _BinarizingThreshold;

	FragInput vertexShader(VertInput vertexInput) {
		FragInput vertexOutput;
		vertexOutput.vertexPosition = UnityObjectToClipPos(vertexInput.vertexPosition);
		vertexOutput.texCoord = MultiplyUV(UNITY_MATRIX_TEXTURE0, vertexInput.texCoord);
		vertexOutput.texCoordCamera0 = MultiplyUV(UNITY_MATRIX_TEXTURE1, vertexInput.texCoordCamera0);
		return vertexOutput;
	} // vertexShader

	float4 fragmentShader(FragInput fragInput) : COLOR{

		if (_BinarizingThreshold > 0) {

			if (tex2D(_AlphaTex0, fragInput.texCoordCamera0).r > _BinarizingThreshold) {
				//return tex2D(_BackTex0, fragInput.texCoord);
				return tex2D(_FrontTex, fragInput.texCoord);
			} else {
				return float4(0, 0, 0, 0);
				//return tex2D(_FrontTex, fragInput.texCoord);
			}

		} else {

			//return tex2D(_AlphaTex0, fragInput.texCoord);

			float4 lightColor = tex2D(_BackTex0, fragInput.texCoord);
			if (lightColor.r < 0.2f) {
				lightColor.r = 0.0f;
				lightColor.g = 0.0f;
				lightColor.b = 0.0f;
				lightColor.a = 0.0f;
			}

			return lerp(
				//tex2D(_FrontTex, fragInput.texCoord),
				float4(0, 0, 0, 0),
				lightColor,
				tex2D(_AlphaTex0, fragInput.texCoordCamera0));
		}
	} // fragmentShader

		ENDCG
	}
	}
}