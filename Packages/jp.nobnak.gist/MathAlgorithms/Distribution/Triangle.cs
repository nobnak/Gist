using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Distribution {

	public static class Triangle {

		public static float Sample(float min, float max, float center) {
			if (!(min <= center && center <= max))
				throw new System.ArgumentException($"center={center} should be in [{min}, {max}]");

			var f = Mathf.Clamp01((center - min) / (max - min));
			var r = Random.value;
			if (r < f)
				return min + Mathf.Sqrt(r * (max - min) * (center - min));
			else
				return max - Mathf.Sqrt((1 - r) * (max - min) * (max - center));
		}

		public static float UpperHalf(float center, float width) {
			return Sample(center, center + width, center);
		}
	}
}