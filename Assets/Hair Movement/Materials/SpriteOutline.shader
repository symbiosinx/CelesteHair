Shader "Unlit/SpriteOutline"
{
    Properties
    {
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}
        [Toggle] _Outline("Outline", int) = 1
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        [Toggle] _DarkenEdges("Darken Edges", int) = 1
        _DarkenOffsets("Edge HSV Offsets", Vector) = (0.075, .2, -.2, 0)
    }
    SubShader
    {
        Tags {
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}

	Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off
		Cull Off

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
            bool _Outline;
			float4 _OutlineColor;
            bool _DarkenEdges;
            float3 _DarkenOffsets;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3 rgb2hsv(float3 c) {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsv2rgb(float3 c) {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

			float colordist(float4 col, float4 col2) {
				return length(col-col2);
			}

			float4 samecolor(float4 col, float4 col2) {
				// return (
				// 	col.r == col2.r &&
				// 	col.g == col2.g &&
				// 	col.b == col2.b &&
				// 	col.a == col2.a
				// );
				return colordist(col, col2) < .1;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float3 t = float3(_MainTex_TexelSize.xy, 0);

                fixed4 col = tex2D(_MainTex, i.uv);

				float4 UP = tex2D(_MainTex, i.uv + t.zy);
				float4 DOWN = tex2D(_MainTex, i.uv - t.zy);
				float4 LEFT = tex2D(_MainTex, i.uv - t.xz);
				float4 RIGHT = tex2D(_MainTex, i.uv + t.xz);

				bool empty = col.a == 0;
				bool neighbouring = (
					(UP.a != 0 && !samecolor(UP, _OutlineColor)) ||
					(DOWN.a != 0 && !samecolor(DOWN, _OutlineColor)) ||
					(LEFT.a != 0 && !samecolor(LEFT, _OutlineColor)) ||
					(RIGHT.a != 0 && !samecolor(RIGHT, _OutlineColor))
				);

                int neighbours =
                    (UP.a != 0) +
                    (DOWN.a != 0) +
                    (LEFT.a != 0) +
                    (RIGHT.a != 0);

                bool corner = neighbours < 3;

                if (_Outline)
				    col = (empty && neighbouring) ? _OutlineColor : col;
                if (_DarkenEdges && corner && !empty) {
                    float3 hsv = rgb2hsv(col.rgb);
                    col.rgb = hsv2rgb(hsv + float3(_DarkenOffsets.x, _DarkenOffsets.y, _DarkenOffsets.z));
                }
                return col;
            }
            ENDCG
        }
    }
}
