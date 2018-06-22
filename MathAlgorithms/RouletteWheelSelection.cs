using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Gist.MathAlgorithms {

	public static class RouletteWheelSelection  {
        public static bool Sample(
			out int sampledIndex, int iterationLimit, float weightMax, params float[] weights) {

            var invWeightMax = 1f / weightMax;
            for (var i = 0; i < iterationLimit; i++) {
                sampledIndex = Random.Range (0, weights.Length);
                if (Random.value < (weights [sampledIndex] * invWeightMax))
                    return true;
            }

            sampledIndex = -1;
            return false;
        }
        public static int Sample(float weightMax, params float[] weights) {
            int sampledIndex = -1;
            Sample (out sampledIndex, int.MaxValue, weightMax, weights);
            return sampledIndex;
        }
		public static bool Sample(out int sampledIndex,
			int iterationLimit, float weightMax, System.Func<int, float> Weight, int weightCount) {

			var invWeightMax = 1f / weightMax;
			for (var i = 0; i < iterationLimit; i++) {
				sampledIndex = Random.Range(0, weightCount);
				if (Random.value < Weight(sampledIndex) * invWeightMax)
					return true;
			}

			sampledIndex = -1;
			return false;
		}

		public static float MaxWeight<T>(IList<T> weights, System.Func<T, float> Evaluate) {
            var max = 0f;
            for (var i = 0; i < weights.Count; i++) {
                var value = Evaluate (weights [i]);
                max = (max > value ? max : value);
            }
            return max;
        }
        public static float MaxWeight(IList<float> weights) {
            var max = 0f;
            for (var i = 0; i < weights.Count; i++) {
                var value = weights [i];
                max = (max > value ? max : value);
            }
            return max;
        }
    }
}