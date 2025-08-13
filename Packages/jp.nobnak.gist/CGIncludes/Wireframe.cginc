#ifndef __WIREFRAME_CGINC__
#define __WIREFRAME_CGINC__



static const float CUTOFF = 0.5;
float _Wireframe_Gain;

float wireframe(float4 bary) {
	float4 d = fwidth(bary);
	float4 l = smoothstep(0, _Wireframe_Gain * d, bary);
	float lmin = min(min(l.x, l.y), min(l.z, l.w));
    return saturate(1 - lmin) > CUTOFF;
}
float wireframe(float3 b) { return wireframe(float4(b, 1)); }
float wireframe(float2 b) { return wireframe(float4(b, 1, 1)); }
float wireframe(float b) { return wireframe(float4(b, 1, 1, 1)); }


#endif
