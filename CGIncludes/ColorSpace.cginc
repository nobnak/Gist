#ifndef __COLOR_SPACE__
#define __COLOR_SPACE__

static const float4 RGB2HSV_K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
static const float4 HSV2RGB_K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);

float3 RGB2HSV(float3 c) {
    float4 p = c.g < c.b ? float4(c.bg, RGB2HSV_K.wz) : float4(c.gb, RGB2HSV_K.xy);
    float4 q = c.r < p.x ? float4(p.xyw, c.r) : float4(c.r, p.yzx);

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 HSV2RGB(float3 c) {
    float3 p = abs(frac(c.xxx + HSV2RGB_K.xyz) * 6.0 - HSV2RGB_K.www);
    return c.z * lerp(HSV2RGB_K.xxx, saturate(p - HSV2RGB_K.xxx), c.y);
}

float3 HSVShift(float3 c, float3 hsvTint) { return HSV2RGB(saturate(RGB2HSV(c) + hsvTint)); }
float3 HSVShift(float3 c, float4 hsvTint) { return HSV2RGB(saturate(RGB2HSV(c) + hsvTint.rgb * hsvTint.a)); }


#define contrast_brightness_offset(c, p) (p.x * (c - p.z) + p.z + p.y)
#define contrast_brightness(c, p) contrast_brightness_offset(c, float3(p, 0.5))
#define contrast_brightness_zerobased(c, p) contrast_brightness_offset(c, float3(p, 0))

float3 ContrastBrightness3(float3 c, float2 p) {
	return contrast_brightness(c, p);
}
float4 ContrastBrightness4(float4 c, float2 p) {
	return float4(contrast_brightness(c.xyz, p), c.w);
}
#define CB4(c, p) ContrastBrightness4(c, p)
float4 CB4ZB(float4 c, float2 p) {
	return float4(contrast_brightness_zerobased(c.xyz, p), c.w);
}

float Luminance(float3 c) {
	return dot(c, float3(0.3126, 0.7152, 0.0722));
}
float Lightness(float3 c) {
	return dot(c, 1.0/3);
}
#endif