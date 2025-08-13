using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Wrapper {

	public class CachedValue<T> {

		protected int cachedFrame = -1;
		protected T currValue;
		protected System.Func<T> evaluator;

		public CachedValue(System.Func<T> evaluator = null) {
			Evaluator = evaluator;
		}

		#region interface
		public T Value {
			get {
				if (cachedFrame != Time.frameCount) {
					cachedFrame = Time.frameCount;
					currValue = evaluator();
				}
				return currValue;
			}
		}
		public System.Func<T> Evaluator {
			get => evaluator;
			set => evaluator = value ?? DefaultValue;
		}
		#endregion

		#region member
		protected T DefaultValue() => default;
		#endregion

		#region static
		public static implicit operator T(CachedValue<T> cv) => cv.Value;
		#endregion
	}
}
