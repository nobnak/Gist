using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Statistics {

	public class StatisticStopwatch : IReadonlyStatisticalMoment {

		public System.Action Updated;

		public readonly System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		public readonly StatisticalMoment sm = new StatisticalMoment();

		#region interface

		#region IReadonlyStatisticalMoment
		public float Average => sm.Average;
		public int Count => sm.Count;
		public float SD => sm.SD;
		public float UnbiasedVariance => sm.UnbiasedVariance;
		#endregion

		public StatisticalMoment Stat { get => sm; }

		public void Start() {
			sw.Restart();
		}
		public void Stop() {
			sw.Stop();
			var elapsed = (float)sw.Elapsed.TotalSeconds;
			sm.Add(elapsed);
			Updated?.Invoke();
		}
		public void Reset() {
			sm.Reset();
		}
		#endregion
	}
}