#ifndef __BLEND_MODE_CGINC__
#define __BLEND_MODE_CGINC__



#define UNARY(cond, ctrue, cfalse) lerp(cfalse, ctrue, cond)



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



#define blend0(t, b)			blend_darken(t, b)
#define blend1(t, b)			blend_multiply(t, b)
#define blend2(t, b)			blend_color_burn(t, b)
#define blend3(t, b)			blend_lienar_burn(t, b)
#define blend4(t, b)			blend_lighten(t, b)
#define blend5(t, b)			blend_screen(t, b)
#define blend6(t, b)			blend_color_dodge(t, b)
#define blend7(t, b)			blend_linear_dodge(t, b)
#define blend8(t, b)			blend_overlay(t, b)
#define blend9(t, b)			blend_soft_light(t, b)
#define blend10(t, b)		blend_hard_light(t, b)
#define blend11(t, b)		blend_vivid_light(t, b)
#define blend12(t, b)		blend_linear_light(t, b)
#define blend13(t, b)		blend_pin_light(t, b)
#define blend14(t, b)		blend_difference(t, b)
#define blend15(t, b)		blend_exclusion(t, b)



#define blend_mode3(_target, _blend, _mode) (blend##_mode((_target).rgb, (_blend).rgb))
#define blend_mode4(_target, _blend, _mode) \
	lerp(_target, float4(blend_mode3((_target).rgb, (_blend).rgb, _mode).rgb, 1), (_blend).a)



#endif