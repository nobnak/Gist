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
		public virtual T Value {
			get { return data; }
			set {
				if (!AreEqual(data, value)) {
					SetData(value);
				}
			}
		}
		public virtual void ForceNotifyChanged() {
			if (Changed != null)
				Changed(this);
		}
		#endregion

		#region member
		protected virtual void SetData(T value) {
			data = value;
			ForceNotifyChanged();
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
	[System.Serializable]
	public class ReactiveValue<T> : BaseReactive<T> where T : struct {
		public ReactiveValue(T data) : base(data) { }
		public ReactiveValue() : base() { }

		public override bool AreEqual(T a, T b) {
			return a.Equals(b);
		}
		public static implicit operator ReactiveValue<T>(T data) {
			return new ReactiveValue<T>(data);
		}
	}
}
