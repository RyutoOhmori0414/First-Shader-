Shader"Unlit/GearEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _NormalMap ("NormalMap", 2D) = "white"{}
        [NoScaleOffset] _MetallicMap ("MetallicMap", 2D) = "white" {}
        [NoScaleOffset] _OcculusionMap ("OcculusionMap", 2D) = "white"{}
        [NoScaleOffset] _EmissionMap ("EmissionMap", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
        {
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half3 normal : NORMAL;
                half4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half3 normal : TEXCOORD1;
                half3 tangent : TEXCOORD2;
                half3 binormal : TEXCOORD3;
                float4 worldPos : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NormalMap;
            float4 _NormalMap_ST;

            sampler2D _MetallicMap;
            float4 _MetallicMap_ST;

            sampler2D _OcculusionMap;
            float4 _OcculusionMap_ST;

            sampler2D _EmissionMap;
            float4 _EmissionMap_ST;

            float3 _AmbientLight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = TransformObjectToWorldNormal(v.normal);
                o.tangent = normalize(TransformObjectToWorldDir(v.tangent.xyz));
                o.binormal = cross(v.normal, v.tangent.xyz) * v.tangent.w;
                o.binormal = normalize(TransformObjectToWorldDir(o.binormal));
                o.worldPos = v.vertex;
    
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half3 normalmap = UnpackNormal(tex2D(_NormalMap, i.uv));
    
                float3 normal = (i.tangent * normalmap.x) + (i.binormal * normalmap.y) + (i.normal * normalmap.z);
                normal = normalize(normal);
                Light light = GetMainLight();
                float diffuse = dot(normal, light.direction);
                half4 col = tex2D(_MainTex, i.uv);
                col *= diffuse;
    col.xyz += UNITY_LIGHTMODEL_AMBIENT.rgb;
    
                return col;
            }
            ENDHLSL
        }
    }
}
