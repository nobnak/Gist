using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {

	public class Progress {

		protected float current, min, max;
		protected bool clamped;

		public Progress(float min = 0f, float max = 1f, bool clamped = true) {
			this.min = min;
			this.max = max;
			this.clamped = clamped;

			this.current = 0;
		}

		#region interface
		public float Max {
			get => max;
			set {
				if (max != value) {
					max = value;
					Update();
				}
			}
		}
		public float Min {
			get => min;
			set {
				if (min != value) {
					min = value;
					Update();
				}
			}
		}
		public float Current {
			get => current;
			set {
				if (current != value) {
					current = value;
					Update();
				}
			}
		}
		public float Rate { get; protected set; }

		public void Update() {
			var span = max - min;
			if (span <= float.Epsilon) {
				Rate = 1f;
				return;
			}

			Rate = (current - min) / span;

			if (clamped) {
				if (current <= min)
					Rate = 0f;
				if (current >= max)
					Rate = 1f;
			}
		}
		#endregion
	}
}
