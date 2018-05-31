using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {

	public static class PlanetaricRandom {
		public const float CIRCLE_DEG = 360f;
		public const float QUARTER_DEG_OF_CIRCLE = CIRCLE_DEG / 4f;

		public const float ONE_OVER_PI = 1f / Mathf.PI;
		public const float TWO_OVER_PI = 2f * ONE_OVER_PI;

		public static void Next(out float lat, out float lon) {
			lat = QUARTER_DEG_OF_CIRCLE * SymmetricSemicircle(Random.value);
			lon = CIRCLE_DEG * Random.value;
		}
		public static void NextRangeBetweenLongitudes(float lonFrom, float lonTo, out float lat, out float lon) {
			lat = QUARTER_DEG_OF_CIRCLE * SymmetricSemicircle(Random.value);
			lon = (lonTo - lonFrom) * Random.value + lonFrom;
		}
	
		public static float SymmetricSemicircle(float rand) {
			var s = 1f - 2f * rand;
			s = Mathf.Clamp(s, -1f, 1f);

			var t = 1f - TWO_OVER_PI * Mathf.Acos(s);
			return Mathf.Clamp(t, -1f, 1f);
		}
	}
}
