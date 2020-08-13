Shader "Unlit/InitializeGrayScott"
{
    Properties
    {
        _MainTex ("NoiseTexture", 2D) = "white" {}
        _QuadWidth("QuadWidth", Range(0, 0.5)) = 0.2
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Name "Initialize"
            CGPROGRAM

            #pragma vertex InitCustomRenderTextureVertexShader
            #pragma fragment frag
            
            #include "UnityCustomRenderTexture.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _QuadWidth;

            fixed4 frag (v2f_init_customrendertexture i) : SV_Target
            {
                float2 uv = i.texcoord;

                // sample the texture
                fixed4 col = tex2D(_MainTex, uv) * 0.1;

                fixed minThreadhold = 0.5 - _QuadWidth;
                fixed maxThreadhold = 0.5 + _QuadWidth;

                if(minThreadhold < uv.x && uv.x < maxThreadhold &&
                    minThreadhold < uv.y && uv.y < maxThreadhold)
                    col.xy += float2(0.5, 0.25);
                else
                    col.xy += float2(1, 0);

                return col;
            }
            ENDCG
        }
    }
}
