using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.WindowSystem {

	[System.Serializable]
    public class FrameRateJob : IEnumerable {
		[SerializeField]
        protected float updateFreq = 0.5f;

        protected float currFramerate;
		protected int currRefreshrate;

		#region public
		public float UpdateFreq {
			get { return updateFreq; }
			set {
				updateFreq = Mathf.Max(0f, value);
			}
		}
		public float CurrentFrameRate {
			get { return currFramerate; }
		}
		public int CurrentRefreshRate {
			get { return currRefreshrate; }
		}

		public override string ToString() {
			return string.Format(
				"Frame-rate : {0:f1} (fps) / {1}",
				currFramerate, currRefreshrate);
		}
		#endregion

		#region IEnumerable
		public IEnumerator GetEnumerator() {
			while (true) {
				try {
					currFramerate = 1.0f / Time.smoothDeltaTime;
					var currResolution = Screen.currentResolution;
					currRefreshrate = currResolution.refreshRate;
				} catch { }
				yield return new WaitForSeconds(updateFreq);
			}
		}
		#endregion
	}
}
