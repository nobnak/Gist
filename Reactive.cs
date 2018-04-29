using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {
	[System.Serializable]
	public abstract class BaseReactive<T> {

		public event System.Action<BaseReactive<T>> Changed;

		protected T data;

		public BaseReactive(T initialData) {
			this.data = initialData;
		}
		public BaseReactive() : this(default(T)) { }

		#region public
		public abstract bool AreEqual(T a, T b);
		public T Value {
			get { return data; }
			set {
				if (!AreEqual(data, value)) {
					data = value;
					ForceNotifyChanged();
				}
			}
		}
		public void ForceNotifyChanged() {
			if (Changed != null)
				Changed(this);
		}
		#endregion

		#region static
		public static implicit operator T(BaseReactive<T> reactive) {
			return reactive.data;
		}
		#endregion
	}
	[System.Serializable]
    public class Reactive<T> : BaseReactive<T> where T : System.IComparable {

		public Reactive(T data) : base(data) { }
		public Reactive() : base() { }

		public override bool AreEqual(T a, T b) {
			return a != null && a.CompareTo(b) == 0;
        }
        public static implicit operator Reactive<T>(T data) {
            return new Reactive<T>(data);
        }
	}
	[System.Serializable]
	public class ReactiveObject<T> : BaseReactive<T> where T : class {

		public ReactiveObject(T data) : base(data) { }
		public ReactiveObject() : base() { }

		public override bool AreEqual(T a, T b) {
			return a != null && a.Equals(b);
		}
		public static implicit operator ReactiveObject<T>(T data) {
			return new ReactiveObject<T>(data);
		}
	}
}
