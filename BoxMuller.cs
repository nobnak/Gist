using UnityEngine;
using System.Collections;

namespace Gist {

    public static class BoxMuller {
        public const float TWO_PI = 2f * Mathf.PI;

        public static Vector2 Gaussian() {
			var u1 = Random.value;
			var u2 = Random.value;
			var logU1 = -2f * Mathf.Log(u1);
			var sqrt = (logU1 <= 0f) ? 0f : Mathf.Sqrt(logU1);
			var theta = TWO_PI * u2;
			var z0 = sqrt * Mathf.Cos(theta);
			var z1 = sqrt * Mathf.Sin(theta);
			return new Vector2(z0, z1);
        }
    }
}
