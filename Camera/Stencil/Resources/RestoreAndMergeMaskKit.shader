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
                
                //float lum = saturate(dot(cref.xyz, float3(0.2, 0.7, 0.1)));
                float lum = saturate(dot(cref.xyz, 1));
                cref = saturate(ContrastBrightness4(cref, _ColorAdjust.xy));

				float4 cnext = cprev;
                cnext = saturate(cnext + saturate(_Throttle.x * dt));
                cnext = lerp(cnext, min(cnext, 1 - cref), saturate(cref * _Throttle.y * dt));
				return cnext;
            }
            ENDCG
        }
    }
}
