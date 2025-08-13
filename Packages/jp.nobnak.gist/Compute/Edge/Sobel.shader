Shader "Hidden/Sobel" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
		_Blend ("Blend", Range(0, 1)) = 0
    }
    SubShader {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#pragma multi_compile ___ Monochrome

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			float _Blend;

            float4 frag (v2f i) : SV_Target {
				float2 dtx = _MainTex_TexelSize.xy;

                float4 c00 = tex2D(_MainTex, i.uv + float2(-1, -1) * dtx);
                float4 c01 = tex2D(_MainTex, i.uv + float2(-1,  0) * dtx);
                float4 c02 = tex2D(_MainTex, i.uv + float2(-1,  1) * dtx);

                float4 c10 = tex2D(_MainTex, i.uv + float2( 0, -1) * dtx);
                float4 c12 = tex2D(_MainTex, i.uv + float2( 0,  1) * dtx);

                float4 c20 = tex2D(_MainTex, i.uv + float2( 1, -1) * dtx);
                float4 c21 = tex2D(_MainTex, i.uv + float2( 1,  0) * dtx);
                float4 c22 = tex2D(_MainTex, i.uv + float2( 1,  1) * dtx);

				float4 fx = (c00 + 2 * c01 + c02 - c20 - 2 * c21 - c22) / 8.0;
				float4 fy = (c00 + 2 * c10 + c20 - c02 - 2 * c12 - c22) / 8.0;

                float4 sobel = sqrt(fx * fx + fy * fy);



				#ifdef Monochrome
				sobel = dot(sobel, 1) / 4;
				#endif

				float4 cmain = tex2D(_MainTex, i.uv);
				return lerp(cmain, sobel, _Blend);
            }
            ENDCG
        }
    }
}
