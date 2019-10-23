#ifndef __OIT__
#define __OIT__



struct OIT {
	float4 accumColor;
	float revealageAlpha;
};



#ifndef OIT_Weight
#define OIT_Weight(z, a) (1)
#endif



OIT OIT_Init() {
	OIT oit;
	oit.accumColor = 0;
	oit.revealageAlpha = 1;
	return oit;
}
OIT OIT_Accumulate(inout OIT oit, float4 color, float z) {
	float w = OIT_Weight(z, color.w);
	oit.accumColor += color * w;
	oit.revealageAlpha *= saturate(1 - color.w);
	return oit;
}
float4 OIT_Finalize(OIT oit) {
	float4 cf = float4(
		oit.accumColor.xyz / clamp(oit.accumColor.w, 1e-4, 5e4), 
		1 - oit.revealageAlpha);
	return cf;
}



#endif