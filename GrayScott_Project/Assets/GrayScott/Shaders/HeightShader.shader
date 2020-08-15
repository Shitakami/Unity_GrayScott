Shader "Unlit/HeightShader"
{
	Properties{
		_TessFactor("Tess Factor", Vector) = (2, 2, 2, 2)
		_LODFactor("LOD Factor", Range(0, 10)) = 1
		_MainTex("Main Texture", 2D) = "white" {}

		_ParallaxScale("ParallaxScale", Float) = 1


	}

		SubShader{
				Pass {

				Tags { "LightMode" = "ForwardBase" }

				CGPROGRAM
	#include "UnityCG.cginc"
	#include "UnityLightingCommon.cginc"

	#pragma vertex VS
	#pragma fragment FS
	#pragma hull HS
	#pragma domain DS
	#define INPUT_PATCH_SIZE 3
	#define OUTPUT_PATCH_SIZE 3

		uniform vector _TessFactor;
		uniform float _LODFactor;
		uniform sampler2D _MainTex;

		uniform sampler2D _ParallaxMap;
		uniform float _ParallaxScale;


		struct appdata {
			float4 w_vert : POSITION;
			float2 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
		};

		struct v2h {
			float4 pos : POS;
			float2 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
			float4 worldPos : TEXCOORD1;
		};

		struct h2d_main {
			float3 pos : POS;
			float2 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
			float4 worldPos : TEXCOORD1;
		};

		struct h2d_const {
			float tess_factor[3] : SV_TessFactor;
			float InsideTessFactor : SV_InsideTessFactor;
		};

		struct d2f {
			float4 pos : SV_Position;
			float2 uv : TEXCOORD0;
			float3 normal : NORMAL;
			float3 worldPos : TEXCOORD1;
		};

		struct f_input {
			float4 vertex : SV_Position;
			float2 uv : TEXCOORD0;
			float3 normal : NORMAL;
			float3 worldPos : TEXCOORD1;
		};

		v2h VS(appdata i) {
			v2h o = (v2h)0;

			o.pos = float4(i.w_vert.xyz, 1.0f);

			o.texcoord = i.texcoord;

			o.normal = i.normal;

			o.worldPos = mul(unity_ObjectToWorld, i.w_vert);
			return o;
		}

		h2d_const HSConst(InputPatch<v2h, INPUT_PATCH_SIZE> i) {
			h2d_const o = (h2d_const)0;
			o.tess_factor[0] = _TessFactor.x * _LODFactor;
			o.tess_factor[1] = _TessFactor.y * _LODFactor;
			o.tess_factor[2] = _TessFactor.z * _LODFactor;
			o.InsideTessFactor = _TessFactor.w * _LODFactor;
			return o;
		}

		[domain("tri")]
		[partitioning("integer")]
		[outputtopology("triangle_cw")]
		[outputcontrolpoints(OUTPUT_PATCH_SIZE)]
		[patchconstantfunc("HSConst")]
		h2d_main HS(InputPatch<v2h, INPUT_PATCH_SIZE> i, uint id : SV_OutputControlPointID) {
			h2d_main o = (h2d_main)0;
			o.pos = i[id].pos;
			o.texcoord = i[id].texcoord;
			o.normal = i[id].normal;
			o.worldPos = i[id].worldPos;
			return o;
		}

		[domain("tri")]
		d2f DS(h2d_const hs_const_data, const OutputPatch<h2d_main, OUTPUT_PATCH_SIZE> i, float3 bary:SV_DomainLocation) {
			d2f o = (d2f)0;
			float3 pos = i[0].pos * bary.x + i[1].pos * bary.y + i[2].pos * bary.z;
			float2 uv = i[0].texcoord * bary.x + i[1].texcoord * bary.y + i[2].texcoord * bary.z;
			float3 normal = i[0].normal * bary.x + i[1].normal * bary.y + i[2].normal * bary.z;
			float3 worldPos = i[0].worldPos * bary.x + i[1].worldPos * bary.y + i[2].worldPos * bary.z;


			// 表面の凹凸を計算
			float parallax = tex2Dlod(_MainTex, float4(uv.xy, 0, 0)).r;
			float parallaxHeight = parallax * _ParallaxScale;

			pos += parallaxHeight * normal;

			o.pos = UnityObjectToClipPos(float4(pos, 1));
			o.uv = uv;
			o.normal = UnityObjectToWorldNormal(normal);
			o.worldPos = worldPos;

			return o;
		}



		float4 FS(f_input i) : SV_Target{

			// テクスチャマップからカラー値をサンプリング
			float4 tex = tex2D(_MainTex, i.uv);

			return float4(tex.r, tex.r, tex.r, 1);
		}
            ENDCG
        }
    }
}
