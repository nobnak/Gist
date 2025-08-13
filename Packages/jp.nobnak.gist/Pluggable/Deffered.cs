using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Pluggable {

	public class Deffered<T> {

		protected readonly System.Func<T> generator;

		protected bool initialized;
		protected T value;

		public Deffered(Func<T> generator) {
			this.generator = generator;
		}

		#region public
		public T Value {
			get {
				if (!initialized) {
					value = generator();
					initialized = true;
				}
				return value;
			}
		}
		public void Reset() {
			initialized = false;
		}
		#endregion

		#region static
		public static implicit operator T (Deffered<T> a) {
			return a.value;
		}
		public static implicit operator Deffered<T>(System.Func<T> a) {
			return new Deffered<T>(a);
		}
		#endregion
	}
}
