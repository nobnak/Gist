using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Events {

	[ExecuteAlways]
	public class MonoBehaviourNotifier : MonoBehaviour {

		[SerializeField]
		protected BoolEvent ActiveAndEnabled = new BoolEvent();

		#region unity
		private void Awake() {
			NotifyActiveAndEnabled();
		}
		private void OnEnable() {
			NotifyActiveAndEnabled();
		}
		private void OnDisable() {
			NotifyActiveAndEnabled();
		}
		#endregion

		#region member
		private void NotifyActiveAndEnabled() {
			ActiveAndEnabled.Invoke(isActiveAndEnabled);
		}
		#endregion
	}
}
