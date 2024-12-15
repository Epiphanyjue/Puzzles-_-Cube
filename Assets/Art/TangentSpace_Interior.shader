Shader "Custom/TangentSpace_Interior"
{
    Properties
    {
        _MainTex ("Texture", Cube) = "white" {}
        _Depth("房间深度",Range(0.3,0.9))=0.5
        _Rooms("Room Atlas Rows&Cols (XY)",Vector)=(1,1,0,0)
        _RoomTex("Room Tex 2D",2D)="white"{}
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
                float4 tangent:TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 objPos:TEXCOORD1;
                float3 uvw:TEXCOORD2;
                float3 viewDir:TEXCOORD3;
            }; 
 
            samplerCUBE _MainTex;
            float4 _MainTex_ST;
            sampler2D _RoomTex;
            float4 _RoomTex_ST;
            float _Depth;
            float2 _Rooms;
   
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.objPos=v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _RoomTex);
                o.uvw=v.vertex*_RoomTex_ST.xyx*0.999+_RoomTex_ST.zwz;
                float4 objCam=mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1.0));
                float3 viewDir=v.vertex.xyz-objCam.xyz;
                //计算切线空间矩阵
                float3 bitangent=cross(v.normal.xyz,v.tangent.xyz)*v.tangent.w;
                o.viewDir=float3
                (
                    dot(viewDir,v.tangent.xyz),
                    dot(viewDir,bitangent),
                    dot(viewDir,v.normal)
                );
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 roomIndexUV=floor(frac(i.uv));
                float depthScale=1.0/(1.0-_Depth)-1.0;
                i.viewDir.z*=-depthScale;

                float2 roomUV=frac(i.uv);
                float3 pos=float3(roomUV*2.0-1.0,-1.0);
                float3 id=1.0/i.viewDir;

                float3 k=abs(id)-pos*id;
                float kMin=min(k.x,min(k.y,k.z));
                pos+=kMin*i.viewDir;

                float interp=pos.z*0.5+0.5;
                float2 interiorUV=pos.xy/(interp*depthScale+1);
                interiorUV=interiorUV*0.5+0.5;
                fixed4 room=tex2D(_RoomTex,(roomIndexUV+interiorUV.xy)/_Rooms);

                return room;

                //return i.objPos;
            }
            ENDCG
        }
        pass
        {
            Tags { "LightMode"="ForwardAdd" }
	
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert 
            #pragma fragment frag 

            sampler2D _MainTex;

            struct a2v
            {
                float4 vertex:POSITION;
                float2 uv:TEXCOORD0;
            };

            struct v2f 
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
            };

            v2f vert (a2v v) {
                v2f o;
                //选取模型空间下的原点作为广告牌的锚点
                float3 center = float3(0, 0, 0);
                //获取模型空间下的视角位置
                float3 viewer = mul(unity_WorldToObject,float4(_WorldSpaceCameraPos, 1));
                //获取目标法线方向
                float3 normalDir = viewer - center;
                //根据_VerticalBillboarding属性控制垂直方向的约束度
                //_VerticalBillboarding = 1，法线方向固定为视角方向
                //_VerticalBillboarding = 0，向上方向固定为(0,1,0)
                normalDir = normalize(normalDir);
            
                //我们需要根据准确的法线方向和粗略的向上方向叉积得到准确的向右方向
                //如果法线和向上方向重合，就改成向前（只要保证这个粗略的向上方向和准确的向上方向在同一个平面即可）
                float3 upDir = abs(normalDir.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
                float3 rightDir = normalize(cross(upDir, normalDir));
                //通过准确的向右的方向合准确的发现方向叉积得到准确的向上的方向
                upDir = normalize(cross(normalDir, rightDir));
            
                //根据原始的位置和相对于锚点的偏移量以及3个正交基矢量，以计算得到新的顶点位置
                float3 centerOffs = v.vertex.xyz - center;
                float3 localPos = center + rightDir * centerOffs.x + upDir * centerOffs.y + normalDir * centerOffs.z;
                o.pos = UnityObjectToClipPos(float4(localPos, 1));
                o.uv = v.uv;
                return o;
            } 

            fixed4 frag (v2f i) : SV_Target {
                fixed4 c = tex2D (_MainTex, i.uv);
                return c; 
            }
            ENDCG
        }
    }
}


