#ifndef __BLEND_MODE_CGINC__
#define __BLEND_MODE_CGINC__



#define UNARY(cond, ctrue, cfalse) lerp(cfalse, ctrue, cond)


#define blend_normal(t, b)			(b)
#define blend_darken(t, b)			min(t, b)
#define blend_multiply(t, b)			(t * b)
#define blend_color_burn(t, b)		(1.0 - (1.0 - t) / b)
#define blend_lienar_burn(t, b)		(t + b - 1)
#define blend_lighten(t, b)			max(t, b)
#define blend_screen(t, b)			(1.0 - (1.0 - t) * (1.0 - b))
#define blend_color_dodge(t, b)		(t / (1.0 - b))
#define blend_linear_dodge(t, b)		(t + b)
#define blend_overlay(t, b) \
	UNARY(t > 0.5, 1 - (1 - 2 * (t - 0.5)) * (1 - b), 2 * t * b)
#define blend_soft_light(t, b) \
	UNARY(b > 0.5, 1 - (1 - t) * (1 - (b - 0.5)), t * (b + 0.5))
#define blend_hard_light(t, b) \
	UNARY(b > 0.5, 1 - (1 - t) * (1 - 2 * (b - 0.5)), 2 * t * b)
#define blend_vivid_light(t, b)	\
	UNARY(b > 0.5, 1 - (1 - t) / (2 * (b - 0.5)), t / (1 - 2 * b))
#define blend_linear_light(t, b)		UNARY(b > 0.5, 	t + 2 * (b - 0.5), t + 2 * b - 1)
#define blend_pin_light(t, b)		UNARY(b > 0.5, max(t, 2 * (b - 0.5)), min(t, 2 * b))
#define blend_difference(t, b)		abs(t - b)
#define blend_exclusion(t, b)		(0.5 - 2.0 * (t - 0.5) * (b - 0.5))



inline float3 blend_mode3(float3 t, float3 b, int mode) {
	switch (mode) {
	default:
		return blend_normal(t, b);
	case 1:
		return blend_darken(t, b);
	case 2:
		return blend_multiply(t, b);
	case 3:
		return blend_color_burn(t, b);
	case 4:
		return blend_lienar_burn(t, b);
	case 5:
		return blend_lighten(t, b);
	case 6:
		return blend_screen(t, b);
	case 7:
		return blend_color_dodge(t, b);
	case 8:
		return blend_linear_dodge(t, b);
	case 9:
		return blend_overlay(t, b);
	case 10:
		return blend_soft_light(t, b);
	case 11:
		return blend_hard_light(t, b);
	case 12:
		return blend_vivid_light(t, b);
	case 13:
		return blend_linear_light(t, b);
	case 14:
		return blend_pin_light(t, b);
	case 15:
		return blend_difference(t, b);
	case 16:
		return blend_exclusion(t, b);
	}
}
inline float4 blend_mode4(float4 _target, float4 _blend, int _mode) {
	return lerp(_target, float4(blend_mode3(_target.rgb, _blend.rgb, _mode), _target.a), _blend.a);
}

#endif