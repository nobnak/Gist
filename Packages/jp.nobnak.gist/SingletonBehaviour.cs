using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {

	public abstract class SingletonBehaviour<T> : MonoBehaviour where T:Object {

		protected static T instance;

		public static T Instance {
			get {
				if (instance == null) {
					instance = Object.FindObjectOfType<T>();
				}
				return instance;
			}
		}

		protected virtual void Awake() {
			if (this != Instance) {
				Destroy(this);
				Debug.LogFormat("Duplicate {0}", typeof(T).Name);
			}
		}
	}
}
