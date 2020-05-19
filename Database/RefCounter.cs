using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Database {

	public class RefCounter<T> {

		protected T target;
		protected int count;

		public RefCounter(T target) {
			this.target = target;
		}

		#region interface
		public static implicit operator RefCounter<T> (T target) {
			return new RefCounter<T>(target);
		}

		public int Count { get { return count; } }
		public bool IsUsed {
			get {
				lock (target)
					return count > 0;
			}
		}

		public Handler Get() { return new Handler(this); }
		#endregion

		#region definition
		public class Handler : System.IDisposable {

			protected RefCounter<T> counter;
			protected bool disposed = false;

			public Handler(RefCounter<T> counter) {
				this.counter = counter;
				lock (counter)
					counter.count++;
			}

			#region interface

			public static implicit operator T (Handler h) {
				return h.counter.target;
			}

			#region IDisposable
			public void Dispose() {
				lock (counter) {
					if (!disposed)
						counter.count--;
					disposed = true;
				}
			}
			#endregion

			~Handler() {
				Dispose();
			}
			#endregion
		}
		#endregion
	}
}
