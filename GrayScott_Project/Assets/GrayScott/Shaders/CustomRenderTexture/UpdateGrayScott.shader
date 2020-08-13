Shader "Unlit/UpdateGrayScott"
{
    Properties
    {
        _F("F", Float) = 0.04
        _K("K", Float) = 0.06

        _SimulateSpeed("SimulateSpeed", Float) = 1
        
        _GridSize("Delta UV", Float) = 1

        _Du("Du", Float) = 0.2
        _Dv("Dv", Float) = 0.1

    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            Name "Update"
            CGPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            #include "UnityCustomRenderTexture.cginc"

            half _S2;
            half _Atten;
            float _DeltaUV;
            sampler2D _MainTex;

            float _F;
            float _K;
            float _SimulateSpeed;

            float _GridSize;
            
            float _Du;
            float _Dv;

            fixed4 frag (v2f_customrendertexture i) : SV_Target
            {
                float2 uv = i.globalTexcoord;

                // 1pxあたりの単位を計算する
                float du = 1.0 / _CustomRenderTextureWidth;
                float dv = 1.0 / _CustomRenderTextureHeight;
                float3 duv = float3(du, dv, 0) * _GridSize;

                // 現在の位置のテクセルをフェッチ
                float2 c = tex2D(_SelfTexture2D, uv);
                float u = c.x;
                float v = c.y;

                float uvv = u*v*v;

                float2 laplacian = 
                    tex2D(_SelfTexture2D, uv - duv.zy).xy +
                    tex2D(_SelfTexture2D, uv + duv.zy).xy +
                    tex2D(_SelfTexture2D, uv - duv.xz).xy +
                    tex2D(_SelfTexture2D, uv + duv.xz) - 4.0*float2(u, v);


                // laplacian /= (_GridSize*_GridSize);

                float dudt = _Du * laplacian.x - uvv + _F * (1.0 - u);
                float dvdt = _Dv * laplacian.y + uvv - (_F + _K) * v;

                return float4(saturate(u + dudt * _SimulateSpeed), saturate(v + dvdt * _SimulateSpeed), 0, 0);
            }
            ENDCG
        }
    }
}
