using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {

	public static class RandomOnSphere {
		public const float AROUND_IN_DEG = 360f;
		public const float QUARTER_OF_AROUND = AROUND_IN_DEG / 4f;

		public const float ONE_OVER_PI = 1f / Mathf.PI;
		public const float TWO_OVER_PI = 2f * ONE_OVER_PI;

		public static float RandomLatitude(float rand) {
			var s = 1f - 2f * rand;
			s = Mathf.Clamp (s, -1f, 1f);

			var t = 1f - TWO_OVER_PI * Mathf.Acos (s);
			return Mathf.Clamp (t, -1f, 1f);
		}

		public static void RandomPolar(out float lat, out float lon) {
			RandomPolar (Random.value, Random.value, out lat, out lon);
		}
		public static void RandomPolar(float s, float t, out float lat, out float lon) {
			lat = QUARTER_OF_AROUND * RandomLatitude (s);
			lon = AROUND_IN_DEG * t;
		}
	}
}
