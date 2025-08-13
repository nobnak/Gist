using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace nobnak.Gist.Extension.FloatArray {
    public static class FloatArrayExtension {

        public static int GreatestLowerBound(this float[] list, float value) {
            var index = GreatestLowerBound(list, 0, list.Length, value);
            return index;
        }

        private static int GreatestLowerBound(this float[] list, int leftmost, int length, float value) {
            if (length <= 1)
                return leftmost;

            var rightmost = leftmost + length - 1;
            var center = leftmost + ((length - 1) >> 1);
            var vCenter = list[center];
            var vNext = list[center + 1];

            if (value < vCenter)
                return GreatestLowerBound(list, leftmost, center - leftmost, value);
            if (vNext <= value)
                return GreatestLowerBound(list, center + 1, rightmost - center, value);
            return center;
        }

        public static void MakeCumulative(this float[] list) {
            var total = 0f;
            foreach (var v in list)
                total += v;
            var normalizer = 1f / total;

            var accum = 0f;
            for (var i = 0; i < list.Length; i++) {
                var val = list[i];
                list[i] = accum * normalizer;
                accum += val;
            }
        }

		public static IEnumerable<float> Normalize(this IEnumerable<float> values) {
			var sum = values.Sum(v => Mathf.Abs(v));
			foreach (var v in values)
				yield return v / sum;
		}

        public static float RoundBelowZero(this float v, int d = 7, System.MidpointRounding r = default(System.MidpointRounding)) {
            return (float)System.Math.Round(v, d, r);
        }
        public static Vector3 RoundBelowZero(this Vector3 v, int d = 7) {
            return new Vector3(v[0].RoundBelowZero(d), v[1].RoundBelowZero(d), v[2].RoundBelowZero(d));
        }

		public const float DX = 1e-3f;
		public static float Quantize(this float v, float dx = DX) {
			return dx * Mathf.RoundToInt(v / dx);
		}
		public static Vector3 Quantize(this Vector3 v, float dx = DX) {
			return new Vector3(v.x.Quantize(dx), v.y.Quantize(dx), v.z.Quantize(dx));
		}

		#region smoothstep
		public static float Smoothstep(this float t) {
			return t * t * (3f - 2f * t);
		}
		public static float Smoothstep(this float v, float min, float max) {
			var width = max - min;
			if (-float.Epsilon <= width && width <= float.Epsilon)
				return (v < min) ? 0f : 1f;

			return Saturate((v - min) / width).Smoothstep();
		}
		public static float Saturate(this float x) {
			return x < 0f ? 0f : (x < 1f ? x : 1f);
		}
		#endregion
	}
}
