﻿Shader "Custom/Toon_Texture_Outline" {
	Properties{
		_MainTex("Main Texture",2D) = "white" {}
		_Tint("Tint", Color) = (1, 1, 1, 1)

		_LambertStep("Lambert Step", Range(0,1)) = 0
		_ShadingIntesity("Shading Palette Intensity", Range(0,1)) = 0
		_LightColorIntensity("Light Color Intensity", Range(0,1)) = 0
		_ShadowIntesity("Shadow Intesity", Range(0,1)) = 0

		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineWidth("Outline Width", Range(0, 400)) = 0.03
		_DistanceInfluence("Distance Influence", Range(0,1)) = 0
	}

	Subshader{

		Tags {
			"RenderType" = "Opaque"
		}

		Pass {
			Tags {
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			CGPROGRAM

			#pragma vertex vertexShader
			#pragma fragment fragmentShader
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct vertexInput {
				float4 pos : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				LIGHTING_COORDS(1,2)
				float4 pos : SV_POSITION;
				float3 worldNormal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			fixed4 _Tint;
			float _Glossiness;
			float _Metallic;
			float _LightColorIntensity;
			float _ShadowIntesity;
			float _ShadingIntesity;
			float _LambertStep;

			float map(float s, float a1, float a2, float b1, float b2)
			{
				return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
			}

			float4 UnityBlendOverlay(float4 Base, float4 Blend, float Opacity)
			{
				float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
				float4 result2 = 2.0 * Base * Blend;
				float4 zeroOrOne = step(Base, 0.5);
				float4 Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
				Out = lerp(Base, Out, Opacity);
				return Out;
			}

			v2f vertexShader(vertexInput IN) {
				v2f OUT;
				OUT.pos = UnityObjectToClipPos(IN.pos);
				OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);
				OUT.uv = IN.uv;
				TRANSFER_VERTEX_TO_FRAGMENT(OUT);
				return OUT;
			}

			fixed4 fragmentShader(v2f IN) : SV_Target {
				//lambert
				float3 normal = normalize( IN.worldNormal );
				float lambert = dot( _WorldSpaceLightPos0, normal );
				lambert = map( lambert, -1, 1, 0.01, 0.99 );
				lambert = step(lambert, _LambertStep);
				lambert = 1 - lambert;

				//light attenuation
				float shadow = SHADOW_ATTENUATION( IN );

				//color
				fixed4 color = tex2D( _MainTex, IN.uv );
				color *= _Tint;
				color *= lerp( 1, _LightColor0, _LightColorIntensity );
				color *= lerp( 1, shadow, _ShadowIntesity );

				if( lambert <= 0 )
					color = UnityBlendOverlay( color, lambert, _ShadingIntesity );

				return saturate( color );
			}

			ENDCG
		}

		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

		Pass {

			Cull Front

			CGPROGRAM

			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			half _OutlineWidth;
			float _DistanceInfluence;

			float4 VertexProgram(float4 position : POSITION, float3 normal : NORMAL) : SV_POSITION {
				float4 clipPosition = UnityObjectToClipPos(position);
				float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, normal));
				float distanceInfluence = lerp(1, clipPosition.w, _DistanceInfluence);
				float2 offset = normalize(clipNormal.xy) / _ScreenParams.xy * _OutlineWidth * distanceInfluence * 2;
				clipPosition.xy += offset;
				return clipPosition;
			}

			half4 _OutlineColor;

			half4 FragmentProgram() : SV_TARGET {
				return _OutlineColor;
			}

			ENDCG

		}
	}
}