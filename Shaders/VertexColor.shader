Shader "Custom/VertexColor" {

    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Flat vertex:vert fullforwardshadows
        #pragma target 3.0
        struct Input {
            float4 vertexColor;
            float2 texcoord;
        };

        half4 LightingFlat(SurfaceOutput s, half3 lightDir, half atten) {
            half4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            return c;
        }

        struct v2f {
            float4 pos : SV_POSITION;
            half2 texcoord : TEXCOORD0;
            fixed4 color : COLOR;
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
            o.vertexColor = v.color;
        }


        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutput o) 
        {
            fixed4 c = tex2D(_MainTex, IN.texcoord) * _Color;
            o.Albedo = c.rgb * IN.vertexColor.rgb;
            o.Alpha = c.a * IN.vertexColor.a;
        }
        ENDCG
    } 
    FallBack "Diffuse"
}