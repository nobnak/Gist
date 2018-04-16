using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.InputDevice {

	[System.Serializable]
	public class KeycodeToggle : KeycodeStatus {
		[SerializeField]
		protected bool guiVisible;

		public event System.Action<KeycodeToggle> Toggle;

		public KeycodeToggle() {
			Down += () => {
				guiVisible = !guiVisible;
				NotifyToggle();
			};
		}

		public virtual bool Visible { get { return guiVisible; } }
		public override void Reset() {
			base.Reset();
			Toggle = null;
		}

		protected virtual void NotifyToggle() {
			if (Toggle != null)
				Toggle(this);
		}
	}
}
