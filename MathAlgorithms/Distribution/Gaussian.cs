using UnityEngine;
using System.Collections;

namespace nobnak.Gist.MathAlgorithms.Distribution {

    public static class Gaussian {
        public const float TWO_PI = 2f * Mathf.PI;

        public static Vector2 BoxMuller() {
			float z0, z1;
			BoxMuller(Random.value, Random.value, out z0, out z1);
			return new Vector2(z0, z1);
		}
		public static void BoxMuller(float r0, float r1, out float z0, out float z1) {
			var logU1 = -2f * Mathf.Log(r0);
			var sqrt = (logU1 <= 0f) ? 0f : Mathf.Sqrt(logU1);
			var theta = TWO_PI * r1;
			z0 = sqrt * Mathf.Cos(theta);
			z1 = sqrt * Mathf.Sin(theta);
		}
	}
}
