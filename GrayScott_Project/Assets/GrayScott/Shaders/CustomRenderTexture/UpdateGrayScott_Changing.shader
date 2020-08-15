Shader "Unlit/UpdateGrayScott_Changing"
{
    Properties
    {
        _F1("F1", Float) = 0.04
        _K1("K1", Float) = 0.06

        _F2("F2", Float) = 0.035
        _K2("K2", Float) = 0.065

        _SimulateSpeed1("SimulateSpeed1", Float) = 1
        _SimulateSpeed2("SimulateSpeed2", Float) = 1

        [Space(10)]
        _WaveSpeed("WaveSpeed", Float) = 1

        _T("T", Float) = 1
        
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

            float _F1;
            float _K1;

            float _F2;
            float _K2;

            fixed _WaveSpeed;
            fixed _T;

            float _SimulateSpeed1;
            float _SimulateSpeed2;

            float _GridSize;
            
            float _Du;
            float _Dv;

            fixed4 frag (v2f_customrendertexture i) : SV_Target
            {
                float2 uv = i.globalTexcoord;

                float distance = (uv.x-0.5)*(uv.x-0.5)+(uv.y-0.5)*(uv.y-0.5);

                float sinValue = sin(_Time.y * _WaveSpeed - distance * _T);
                float rate = (sinValue + 1) / 2.0;

                float f = lerp(_F1, _F2, rate);
                float k = lerp(_K1, _K2, rate);

                // 1pxあたりの単位を計算
                float du = 1.0 / _CustomRenderTextureWidth;
                float dv = 1.0 / _CustomRenderTextureHeight;
                float3 duv = float3(du, dv, 0) * _GridSize;

                // 現在のテクスチャの取得
                float2 c = tex2D(_SelfTexture2D, uv);
                float u = c.x;
                float v = c.y;

                // ラプラスの演算を行う
                float2 laplacian = 
                    tex2D(_SelfTexture2D, uv - duv.zy).xy +
                    tex2D(_SelfTexture2D, uv + duv.zy).xy +
                    tex2D(_SelfTexture2D, uv - duv.xz).xy +
                    tex2D(_SelfTexture2D, uv + duv.xz) - 4.0*float2(u, v);

                float uvv = u*v*v;

                // Gray-Scottモデルの反応拡散方程式
                float dudt = _Du * laplacian.x - uvv + f * (1.0 - u);
                float dvdt = _Dv * laplacian.y + uvv - (f + k) * v;

                float speed = lerp(_SimulateSpeed1, _SimulateSpeed2, rate);

                return float4(saturate(u + dudt * speed), saturate(v + dvdt * speed), 0, 0);
                
            }
            ENDCG
        }
    }
}
