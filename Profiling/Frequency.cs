using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace nobnak.Gist.Profiling {

	public class Frequency {
		public const float EPSILON = 1e-6f;

		protected float duration;
		protected int minimum;

		protected Queue<DateTime> timestamps = new Queue<DateTime>();

		protected Validator validator = new Validator();
		protected DateTime lastTimestamp;

		protected float outputFrequency;
		protected int outputCount;
		protected float outputTimeSpan;

		public Frequency(float duration = 60f, int minimum = 10) {
			this.duration = Mathf.Max(duration, 0.01f);
			this.minimum = Mathf.Max(minimum, 2);

			validator.Reset();
			validator.Validation += () => {
				var expiration = DateTime.Now.AddSeconds(-duration);
				while (timestamps.Count > 0 && timestamps.Peek() < expiration)
					timestamps.Dequeue();

				outputFrequency = 0f;
				outputTimeSpan = 0f;

				outputCount = timestamps.Count;
				if (outputCount < 2)
					return;

				outputTimeSpan = (float)((lastTimestamp - timestamps.Peek()).TotalSeconds);
				if (outputTimeSpan <= EPSILON)
					return;

				outputFrequency = (outputCount - 1) / outputTimeSpan;
			};
		}

		public float CurrentFrequency {
			get {
				validator.Validate();
				return outputFrequency;
			}
		}
		public float CurrentTimeSpan {
			get {
				validator.Validate();
				return outputTimeSpan;
			}
		}
		public int CurrentCount {
			get {
				validator.Validate();
				return outputCount;
			}
		}

		public Frequency Increment() {
			validator.Invalidate();
			timestamps.Enqueue(lastTimestamp = DateTime.Now);
			return this;
		}
	}
}
