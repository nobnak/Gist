using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Sampler {

	public static class Sphere {
		public const float CIRCLE_DEG = 360f;
		public const float QUARTER_DEG_OF_CIRCLE = CIRCLE_DEG / 4f;

		public const float TWO_PI = 2f * Mathf.PI;
		public const float TWO_PI_INVERSE = 1f / TWO_PI;

		public static void Sample(out float lat, out float lon) {
			lat = QUARTER_DEG_OF_CIRCLE * SymmetricSemicircle(Random.value);
			lon = CIRCLE_DEG * Random.value;
		}
		public static void RangeBetweenLongitudes(float lonFrom, float lonTo, out float lat, out float lon) {
			lat = QUARTER_DEG_OF_CIRCLE * SymmetricSemicircle(Random.value);
			lon = (lonTo - lonFrom) * Random.value + lonFrom;
		}
	
		public static float SymmetricSemicircle(float rand) {
			var s = 1f - 2f * rand;
			s = Mathf.Clamp(s, -1f, 1f);

			var t = 1f - 2f * TWO_PI_INVERSE * Mathf.Acos(s);
			return Mathf.Clamp(t, -1f, 1f);
		}

		public static float AreaOfConialArc(float halfangle) {
			return TWO_PI * (1f - Mathf.Cos(halfangle * Mathf.Deg2Rad));
		}
		public static float AngleOfConialArc(float halfAngle, float r) {
			var area = AreaOfConialArc(halfAngle);
			return Mathf.Rad2Deg * Mathf.Acos(1f - area * r * TWO_PI_INVERSE);
		}
		public static float AngleOfConialArc(float rangeAngle) {
			var r = Random.value;
			return AngleOfConialArc(rangeAngle, r);
		}

		public static Quaternion RotationOfConialArc(float halfangle, Vector3 forward) {
			float lat, lon;
			var right = new Vector3(forward.z, forward.x, forward.y);
			RotationOfConialArc(halfangle, out lat, out lon);
			return Quaternion.AngleAxis(lon, forward)
				* Quaternion.AngleAxis(lat, right);
		}
		public static void RotationOfConialArc(float halfangle, out float lat, out float lon) {
			lat = AngleOfConialArc(halfangle);
			lon = CIRCLE_DEG * Random.value;
		}
	}
}
