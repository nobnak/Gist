using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Collection {

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
		public T Value { get => target; }

		public Token GetToken() { return new Token(this); }
		#endregion

		#region definition
		public class Token : System.IDisposable {

			protected RefCounter<T> counter;
			protected bool disposed = false;

			public Token(RefCounter<T> counter) {
				this.counter = counter;
				lock (counter)
					counter.count++;
			}

			#region interface

			public static implicit operator T (Token h) {
				return h.counter.target;
			}

			#region object
			public override string ToString() {
				return $"<{GetType().Name}:counter={counter} disposed={disposed}>";
			}
			#endregion

			#region IDisposable
			public void Dispose() {
				lock (counter) {
					if (!disposed)
						counter.count--;
					disposed = true;
				}
			}
			#endregion

			~Token() {
				Dispose();
			}
			#endregion
		}
		#endregion
	}
}
