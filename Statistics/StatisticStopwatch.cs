using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Statistics {

	public class StatisticStopwatch {

		public System.Action Updated;

		public readonly System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		public readonly StatisticalMoment sm = new StatisticalMoment();

		#region interface
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