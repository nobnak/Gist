#ifndef __NOBNAK_NORMAL_CGINC__
#define __NOBNAK_NORMAL_CGINC__



#define TANGENT_SPACE_ROTATION_VAR(normal, tangent) \
    float3 binormal = cross( normalize(normal), normalize(tangent.xyz) ) * tangent.w; \
    float3x3 rotation = float3x3( tangent.xyz, binormal, normal )

#define TangentToObjectNormal(normal) normalize(mul(normal, rotation))
#define ObjectToViewNormal(normal) normalize(mul(UNITY_MATRIX_IT_MV, normal))



void WorldTangentSpace(float3 objNormal, float4 objTangent,
	out float3 ts0, out float3 ts1, out float3 ts2) {

	float3 worldNormal = UnityObjectToWorldNormal(objNormal);
	fixed3 worldTangent = UnityObjectToWorldDir(objTangent.xyz);
	fixed tangentSign = objTangent.w * unity_WorldTransformParams.w;
	fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
	ts0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
	ts1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
	ts2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);
}
float3x3 TangentToWorldNormalMatrix(float3 ts0, float3 ts1, float3 ts2) {
	return float3x3(ts0, ts1, ts2);
}
float3 TangentToWorldNormal(float3x3 ts, float3 normal) {
	return normalize(mul(ts, normal));
}

#endif