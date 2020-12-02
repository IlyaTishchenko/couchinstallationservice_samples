Shader "Custom/DiagonalStripes" {
    Properties {
        _MainTex ("Font Texture", 2D) = "white" {}
        _Color ("Text Color", Color) = (1,1,1,1)
        
        _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader {
        
        Tags
        {
            "RenderType"="Opaque"
        }

        Lighting Off 
        Cull Off 
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform fixed4 _Color;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                #ifdef UNITY_HALF_TEXEL_OFFSET
                    o.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
                #endif
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                i.vertex.x = floor((lerp(i.vertex.x, i.vertex.y, 0.5) + _Time.y * 6.0) * 1 / _ScreenParams.y * 256.0) * 0.5;
                float checker = -frac(i.vertex.r);

                fixed4 col = i.color;
                col.a *= tex2D(_MainTex, i.texcoord).a;

                clip(checker);
                return col;
            }
            ENDCG 
        }
    }
}
