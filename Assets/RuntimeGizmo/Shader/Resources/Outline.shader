Shader "Custom/Outline" {
    Properties {
        _BaseMap ("Main Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range (0, 0.1)) = 0.01
    }
 
    SubShader {
        Tags {
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }
        LOD 200
 
        Cull Off
        ZWrite Off
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
 
            #include "UnityCG.cginc"
 
            struct appdata {
                float4 vertex : POSITION;
            };
 
            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
 
            float _OutlineWidth;
            fixed4 _OutlineColor;
            sampler2D _BaseMap;
 
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy;
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;
 
                // Sample the main texture
                fixed4 col = tex2D(_BaseMap, uv);
 
                // Calculate the outline
                float2 ddx = ddx(uv);
                float2 ddy = ddy(uv);
                float edge = _OutlineWidth * (length(ddx) + length(ddy));
 
                // Apply the outline color to pixels within the outline width
                if (edge > 0.0 && edge < 1.0)
                    col = lerp(col, _OutlineColor, edge);
 
                return col;
            }
            ENDCG
        }
    }
 
    Fallback "Diffuse"
}