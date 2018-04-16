using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.InputDevice {

	[System.Serializable]
	public class KeycodeStatus {
		[SerializeField]
		protected KeyCode key = KeyCode.None;

		public event System.Action Down;
		public event System.Action Up;
		public event System.Action Hold;

		public virtual void Update() {
			if (Down != null && Input.GetKeyDown(key))
				Down();
			if (Up != null && Input.GetKeyUp(key))
				Up();
			if (Hold != null && Input.GetKey(key))
				Hold();
		}
		public virtual void Reset() {
			Down = null;
			Up = null;
			Hold = null;
		}
	}
}
