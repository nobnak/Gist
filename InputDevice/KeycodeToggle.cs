using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.InputDevice {

	[System.Serializable]
	public class KeycodeToggle : KeycodeStatus {
		[SerializeField]
		protected bool guiVisible;

		public event System.Action<KeycodeToggle> Toggle;

		public KeycodeToggle(KeyCode key = KeyCode.None) : base(key) {
			Down += () => {
				guiVisible = !guiVisible;
				NotifyToggle();
			};
		}

		#region interface
		public virtual bool Visible { get { return guiVisible; } }
		public override void Reset() {
			base.Reset();
			Toggle = null;
		}
		#endregion
		#region member
		protected virtual void NotifyToggle() {
			if (Toggle != null)
				Toggle(this);
		}
		#endregion
		#region static
		public static implicit operator bool(KeycodeToggle k) {
			return k.guiVisible;
		}
		#endregion
	}
}
