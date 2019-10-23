Shader "Hidden/GlowFilter" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
		_BlurredTex ("Blurred", 2D) = "black" {}
		_Intensity ("Intensity", Float) = 0
		_Threshold ("Threshold", Vector) = (0, 1, 0, 0)
    }
    SubShader {
        Cull Off ZWrite Off ZTest Always

			CGINCLUDE
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
			sampler2D _BlurredTex;
			float _Intensity;
			float4 _Threshold;

			float4 fragThreshold(v2f i) : SV_Target {
				float4 cmain = tex2D(_MainTex, i.uv);
				float4 cthresh = _Threshold.y * max(cmain - _Threshold.x, 0) + _Threshold.z;
				return cthresh;
			}
            float4 fragAdditive (v2f i) : SV_Target {
                float4 cmain = tex2D(_MainTex, i.uv);
				float4 cblur = tex2D(_BlurredTex, i.uv);
                return cmain + cblur * _Intensity;
            }
			ENDCG

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragThreshold
            ENDCG
        }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragAdditive
            ENDCG
        }
    }
}
