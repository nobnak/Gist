using UnityEngine;
using System.Collections;

namespace nobnak.Gist.MathAlgorithms.Sampler {

	public static class FlatRing {
		public const float TWO_PI = 2f * Mathf.PI;

		public static Vector2 InCircularRing(float innerRadius, float outerRadius) {
			var r = UniformRadiusInCircularRing (innerRadius, outerRadius);
			var theta = TWO_PI * Random.value;
			return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
		}

		public static float UniformRadiusInCircularRing (float innerRadius, float outerRadius) {
			var sqrInnerRadius = innerRadius * innerRadius;
			var a = outerRadius * outerRadius - sqrInnerRadius;
			var sqrRadius = Random.value * a + sqrInnerRadius;
			return sqrRadius <= sqrInnerRadius ? innerRadius : Mathf.Sqrt (sqrRadius);
		}
	}
}
