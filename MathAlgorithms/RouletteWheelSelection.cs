using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Gist.MathAlgorithms {

	public class RouletteWheelSelection  {
		public const int DEFAULT_ITERATION_LIMIT = 100;

		protected float[] weights;
		protected float weightMax;
		protected int iterationLimit;

		public RouletteWheelSelection(
			float[] weights,
			int iterationLimit = DEFAULT_ITERATION_LIMIT) {

			System.Array.Resize(ref this.weights, weights.Length);
			System.Array.Copy(weights, this.weights, weights.Length);
			this.weightMax = MaxWeight(weights);
			this.iterationLimit = iterationLimit;
		}

		public bool TrySample(out int sampledIndex) {
			return Sample(out sampledIndex, iterationLimit, weightMax, weights);
		}

		#region static
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
		#endregion
	}
}