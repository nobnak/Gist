using nobnak.Gist.Pluggable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace nobnak.Gist.Events.Hooks {

	[ExecuteAlways]
	public class HookOnEnable : MonoBehaviour {

		[SerializeField]
		protected Events events = new Events();

		#region unity
		private void OnEnable() {
			Notify(gameObject);
		}
		private void OnDisable() {
			Notify(gameObject);
		}
		#endregion

		#region interface
		public void Notify(GameObject g) {
			events.OnSetEnabled.Invoke(g);

			events.EnabledOnEnable.Invoke(g.activeSelf);
			events.DisabledOnEnable.Invoke(!g.activeSelf);
		}
		#endregion

		#region definitions
		[System.Serializable]
		public class HookEvent : UnityEngine.Events.UnityEvent<GameObject, bool> { }
		[System.Serializable]
		public class Events {
			public GoEvent OnSetEnabled = new GoEvent();

			public BoolEvent EnabledOnEnable = new BoolEvent();
			public BoolEvent DisabledOnEnable = new BoolEvent();
		}
		#endregion
	}
}
