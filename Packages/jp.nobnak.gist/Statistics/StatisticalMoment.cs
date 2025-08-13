using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Statistics {

	public class StatisticalMoment : IReadonlyStatisticalMoment {

		public int n = 0;
		public float average = 0f;
		public float m2 = 0f;

		public StatisticalMoment() {
			Reset();
		}

		#region interface
		#region object
		public override string ToString() {
			return $"<{GetType().Name}: n={Count} average={Average} unbiased variance={UnbiasedVariance}>";
		}
		#endregion

		#region IReadonlyStatisticalMoment
		public int Count { get => n; }
		public float Average { get => average; }
		public float UnbiasedVariance { get => (n < 2 ? 0f : m2 / (n - 1)); }
		public float SD { get => Mathf.Sqrt(UnbiasedVariance); }
		#endregion

		public StatisticalMoment Add(float value) {
			var prevAvg = average;

			n++;
			average += (value - prevAvg) / n;
			m2 += (value - average) * (value - prevAvg);
			return this;
		}
		public StatisticalMoment Reset() {
			this.n = 0;
			this.average = 0f;
			this.m2 = 0f;
			return this;
		}
		#endregion
	}
}