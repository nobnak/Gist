Shader "Hidden/RestoreAndMergeMask" {
    Properties {
        _MainTex ("Prev Texture", 2D) = "white" {}
		_RefTex ("Reference Texture", 2D) = "black" {}
        _ColorAdjust ("Color Adjuster", Vector) = (1,0,0,0)
    }
    SubShader {
        Cull Off ZWrite Off ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/Packages/Gist/CGIncludes/ColorSpace.cginc"

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
			sampler2D _RefTex;
			float4 _RefTex_TexelSize;

			float4 _User_Time;
			float4 _Throttle;

            float4 _ColorAdjust;

            v2f vert (appdata v) {
				float4 uv = v.uv.xyxy;
				if (_RefTex_TexelSize.y < 0)
					uv.w = 1 - uv.y;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = uv;
                return o;
            }

			float4 frag (v2f i) : SV_Target {
				float4 cprev = tex2D(_MainTex, i.uv.zw);
				float4 cref = tex2D(_RefTex, i.uv.xy);
				float dt = _User_Time.x;
                

                float4 cref_inv = saturate(1 - cref);
				float4 cnext = cprev;
                cnext = lerp(cnext, min(max(cref_inv, cnext), cnext + saturate(cref_inv * _Throttle.x * dt)), cref_inv);
                cnext = lerp(cnext, max(min(cref_inv, cnext), cnext - saturate(cref * _Throttle.y * dt)), cref);

                cnext = saturate(ContrastBrightness4(cnext, _ColorAdjust.xy));
				return cnext;
            }
            ENDCG
        }
    }
}
