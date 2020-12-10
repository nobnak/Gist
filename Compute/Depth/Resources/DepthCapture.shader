Shader "Hidden/CaptureDepth" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Cull Off ZWrite Off ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
			sampler2D _CameraDepthTexture;

			float4 frag (v2f i) : SV_Target {
				float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				d = Linear01Depth(d);
				d = step(d, 0.5);
				return d;
            }
            ENDCG
        }
    }
}
