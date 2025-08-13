using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace nobnak.Gist.Timers {

	public class ContextualTimer : System.IDisposable {

		public event System.Timers.ElapsedEventHandler Elapsed;

		public SynchronizationContext context;
		public System.Timers.Timer timer;

		public ContextualTimer(double interval, SynchronizationContext context) {
			this.context = context;
			this.timer = new System.Timers.Timer(interval);

			timer.Elapsed += (o, e) => {
				context.Send(_ => {
					Elapsed?.Invoke(o, e);
				}, null);
			};
		}
		public ContextualTimer(double interval) : this(interval, SynchronizationContext.Current) { }
		public ContextualTimer() : this(0f) { }

		#region interface

		#region IDisposable
		public void Dispose() {
			if (timer != null) {
				timer.Dispose();
				timer = null;
			}
		}
		#endregion

		public double Interval {
			get => timer.Interval;
			set => timer.Interval = value;
		}
		public bool AutoReset {
			get => timer.AutoReset;
			set => timer.AutoReset = value;
		}
		public bool Enabled {
			get => timer.Enabled;
			set => timer.Enabled = value;
		}

		public void Start() {
			timer.Start();
		}
		public void Stop() {
			timer.Stop();
		}
		#endregion
	}
}
