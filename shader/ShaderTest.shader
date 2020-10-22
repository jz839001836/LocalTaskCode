
Shader "Unlit/Shader"
{
    Properties{
        _Diffuse("Diffuse", Color) = (0,0,0,0)

        _OutlineColor ("Outline Color", COLOR) = (0,0,0,1)
        _OutlineScale ("Outline Scale", Range(0,1)) = 0.001
    }
        SubShader
    {
        Tags { "Queue" = "Geometry + 1000" "RenderType" = "Opaque" }
        LOD 200

        Pass {
            Name "Outline"
            ZWrite Offset
            Cull Front
            CGPROGRAM
            
            #pragma vertex vert 
            #pragma fragment frag 
            #include "UnityCG.cgnic"

            float4 _OutlineColor;
            float _OutlineScale;

            struct v2f {
                float4 vertex : SV_POSITION;
            };
        
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            fixed4 _Diffuse;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                o.color = v.color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLightDir));
                fixed3 color = diffuse + i.color;
                return fixed4(color, 1.0);
            }

                ENDCG
        }
    }
}
