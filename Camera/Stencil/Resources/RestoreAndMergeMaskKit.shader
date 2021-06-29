Shader "Hidden/RestoreAndMergeMask" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
		_CharTex ("Character", 2D) = "black"
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
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _CharTex;
			float4 _CharTex_TexelSize;

			float4 _User_Time;
			float4 _Throttle;

            v2f vert (appdata v) {
				float4 uv = v.uv.xyxy;
				if (_CharTex_TexelSize.y < 0)
					uv.w = 1 - uv.y;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = uv;
                return o;
            }

			float4 frag (v2f i) : SV_Target {
				float4 cmain = tex2D(_MainTex, i.uv.zw);
				float4 cchar = tex2D(_CharTex, i.uv.xy);
				float dt = _User_Time.x;

				cmain = lerp(cmain, 1, saturate(_Throttle.x * dt));
				cmain = lerp(cmain, 0, saturate(cchar.x * _Throttle.y * dt));
				return cmain;
            }
            ENDCG
        }
    }
}
