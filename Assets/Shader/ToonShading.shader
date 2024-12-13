Shader "Unlit/ToonShading"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineWidth ("Outline Width", Range(0.001, 1)) = 0.01
        [HDR]_OutLineColor ("OutLine Color", Color) = (0.5,0.5,0.5,1)
        _RampStart ("交界起始 RampStart", Range(0.1, 1)) = 0.3
        _RampSize ("交界大小 RampSize", Range(0, 1)) = 0.1
        [IntRange] _RampStep("交界段数 RampStep", Range(1,10)) = 1
        _RampSmooooth ("交界柔和度 RampSmooth", Range(0.01, 1)) = 0.1
        [HDR]_DarkColor ("暗面 DarkColor", Color) = (0.4, 0.4, 0.4, 1)
        [HDR]_LightColor ("亮面 LightColor", Color) = (0.8, 0.8, 0.8, 1)
        _SpecPow("SpecPow 光泽度", Range(0, 1)) = 0.1
        _SpecularColor ("SpecularColor 高光", Color) = (1.0, 1.0, 1.0, 1)
        _SpecIntensity("SpecIntensity 高光强度", Range(0, 1)) = 0
        _SpecSmooth("SpecSmooth 高光柔和度", Range(0, 0.5)) = 0.1
        _RimColor ("RimColor 边缘光", Color) = (1.0, 1.0, 1.0, 1)
        _RimThreshold("RimThreshold 边缘光阈值", Range(0, 1)) = 0.45
        _RimSmooth("RimSmooth 边缘光柔和度", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal: TEXCOORD1;
                float3 worldPos:TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _RampStart;
            float _RampSize;
            float _RampStep;
            float _RampSmooooth;
            float3 _DarkColor;
            float3 _LightColor;
            float _SpecPow;
            float3 _SpecularColor;
            float _SpecIntensity;
            float _SpecSmooth;
            float3 _RimColor;
            float _RimThreshold;
            float _RimSmooth;

            float linearstep (float min, float max, float t)
            {
                return saturate((t - min) / (max - min));
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                //------------------------ 漫反射 ------------------------
                float3 normal = normalize(i.worldNormal);
                float3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
                float NoL = dot(i.worldNormal, worldLightDir);
                float halfLambert = NoL * 0.5 + 0.5;

                //------------------------ 高光 ------------------------
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                float3 halfDir = normalize(viewDir + worldLightDir);
                float NoH = dot(normal, halfDir);
                float blinnPhone = pow(max(0, NoH), _SpecPow * 128.0);
                float3 specularColor = smoothstep(0.7 - _SpecSmooth / 2, 0.7 + _SpecSmooth / 2, blinnPhone)
                                        * _SpecularColor * _SpecIntensity;

                //------------------------ 边缘光 ------------------------
                float NoV = dot(i.worldNormal, viewDir);
                float rim = (1 - max(0, NoV)) * NoL;
                float3 rimColor = smoothstep(_RimThreshold - _RimSmooth / 2, _RimThreshold + _RimSmooth / 2, rim) * _RimColor;

                //------------------------ 色阶 ------------------------
                float ramp = linearstep(_RampStart, _RampStart + _RampSize, halfLambert);
                float step = ramp * _RampStep;
                float gridStep = floor(step);
                float smoothStep = smoothstep(gridStep, gridStep + _RampSmooooth, step) + gridStep;
                ramp = smoothStep / _RampStep;
                float3 rampColor = lerp(_DarkColor, _LightColor, ramp);
                rampColor *= col;

                // 混合颜色
                float3 finalColor = saturate(rampColor + specularColor + rimColor);
                return float4(finalColor,1);
            }
            ENDCG
        }

        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 color:COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _OutlineWidth;
            float4 _OutLineColor;

            v2f vert (appdata v)
            {
                v2f o;
                float4 newVertex = float4(v.vertex.xyz +  normalize(v.color.xyz*2-1) * _OutlineWidth * 0.05,1);
                o.vertex = UnityObjectToClipPos(newVertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutLineColor;
            }
            ENDCG
        }
    }
}
