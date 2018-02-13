#ifndef __QUATERNION__
#define __QUATERNION__

#define HALF_DEG2RAD 8.72664625e-3

float4 quaternion(float3 normalizedAxis, float degree) {
	float rad = degree * HALF_DEG2RAD;
	return float4(normalizedAxis * sin(rad), cos(rad));
}
float4 qmultiply(float4 a, float4 b) {
	return float4(a.w * b.xyz + b.w * a.xyz + cross(a.xyz, b.xyz), a.w * b.w - dot(a.xyz, b.xyz));
}
float3 qrotate(float4 q, float3 v) {
	return qmultiply(qmultiply(q, float4(v, 0)), float4(-q.xyz, q.w)).xyz;
}
float3 qrotateinv(float4 q, float3 v) {
	return qmultiply(qmultiply(float4(-q.xyz, q.w), float4(v, 0)), q).xyz;
}

#endif
