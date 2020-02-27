Shader "Hidden/Copy" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
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

			sampler2D _MainTex;
			float4 _LocalMat;

			v2f vert(appdata v) {
				float3 vertex = v.vertex;
				vertex.xy = vertex.xy * _LocalMat.xy + _LocalMat.zw;

				v2f o;
				o.vertex = UnityObjectToClipPos(vertex);
				o.uv = v.uv;
				return o;
			}
		ENDCG

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			float4 frag (v2f IN) : SV_Target {
				float4 cmain = tex2D(_MainTex, IN.uv);
				return cmain;
            }
            ENDCG
        }
    }
}
