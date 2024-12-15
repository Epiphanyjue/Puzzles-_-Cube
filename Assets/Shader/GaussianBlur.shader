Shader "Custom/GrabPassGaussianBlur"
{
    Properties
    {
        _MainTex("maintex",2D)="white"{}
        _BlurSize ("Blur Size", Float) = 2.0
        _Downsample ("Downsample Factor", Float) = 1.0
    }
    SubShader
    {
        GrabPass { "_GrabTex" }

        CGINCLUDE

        #include "UnityCG.cginc"

        // Blur parameters
        float _BlurSize;
        float _Downsample;
        sampler2D _GrabTex;
        sampler2D _MainTex;
        half4 _GrabTex_TexelSize;


        struct appdata_t
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
            float4 screenUV[5] : TEXCOORD1;
            float2 uv : TEXCOORD0;
        };

        v2f vertBlurVertical(appdata_t v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            float4 screenUV = ComputeGrabScreenPos(o.pos);
            o.screenUV[0] = screenUV;
            o.screenUV[1] = screenUV + float4(0.0, _GrabTex_TexelSize.y * 1.0, 0.0, 0.0) * _BlurSize;
            o.screenUV[2] = screenUV + float4(0.0, _GrabTex_TexelSize.y * -1.0, 0.0, 0.0) * _BlurSize;
            o.screenUV[3] = screenUV + float4(0.0, _GrabTex_TexelSize.y * 2.0, 0.0, 0.0) * _BlurSize;
            o.screenUV[4] = screenUV + float4(0.0, _GrabTex_TexelSize.y * -2.0, 0.0, 0.0) * _BlurSize;

            return o;
        }

        v2f vertBlurHorizontal(appdata_t v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            float4 screenUV = ComputeGrabScreenPos(o.pos);
            o.screenUV[0] = screenUV;
            o.screenUV[1] = screenUV + float4(_GrabTex_TexelSize.x * 1.0, 0.0, 0.0, 0.0) * _BlurSize;
            o.screenUV[2] = screenUV + float4(_GrabTex_TexelSize.x * -1.0, 0.0, 0.0, 0.0) * _BlurSize;
            o.screenUV[3] = screenUV + float4(_GrabTex_TexelSize.x * 2.0, 0.0, 0.0, 0.0) * _BlurSize;
            o.screenUV[4] = screenUV + float4(_GrabTex_TexelSize.x * -2.0, 0.0, 0.0, 0.0) * _BlurSize;

            return o;
        }

        half4 fragBlur(v2f i) : SV_Target
        {
            // High pass weights for Gaussian blur (3-tap example)
            float3 weight = {0.4026, 0.2442, 0.0545};

            fixed3 sum = tex2D(_GrabTex, float2(i.screenUV[0].xy / i.screenUV[0].w)).rgb * weight[0];
            
            for (int it = 1; it < 3; it++)
            {
                sum += tex2D(_GrabTex, float2(i.screenUV[it * 2 - 1].xy / i.screenUV[it * 2 - 1].w)).rgb * weight[it];
                sum += tex2D(_GrabTex, float2(i.screenUV[it * 2].xy / i.screenUV[it * 2].w)).rgb * weight[it];
            }

            return fixed4(sum, 1.0);
        }
        ENDCG

        ZWrite Off
        Cull Off
        LOD 100
        Tags { "Queue" = "Transparent" }

        Pass
        {
            Name "Gaussian_Blur_Vertical"
            CGPROGRAM
            #pragma vertex vertBlurVertical
            #pragma fragment fragBlur
            ENDCG
        }
        
        Pass
        {
            Name "Gaussian_Blur_Horizontal"
            CGPROGRAM
            #pragma vertex vertBlurHorizontal
            #pragma fragment fragBlur
            ENDCG
        }
    }
    Fallback Off
}
