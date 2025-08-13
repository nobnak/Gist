using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.InputDevice {

	[System.Serializable]
	public class KeycodeToggle : KeycodeStatus {
		[SerializeField]
		protected bool guiVisible;

		public event System.Action<KeycodeToggle> Toggle;

        protected Validator validator = new Validator();
        protected bool lastGuiVisible;

		public KeycodeToggle(KeyCode key = KeyCode.None) : base(key) {
            Reset();
		}

		#region interface
		public virtual bool Visible { get { return guiVisible; } }
		public override void Reset() {
			base.Reset();
			Toggle = null;

            Down += () => {
				lastGuiVisible = guiVisible;
				guiVisible = !guiVisible;
				validator.Invalidate();
            };
            validator.Reset();
            validator.Validation += () => {
				if (lastGuiVisible != guiVisible) NotifyToggle();
				lastGuiVisible = guiVisible;
			};
            validator.SetCheckers(() => lastGuiVisible == guiVisible);
        }
        public override void Update() {
            base.Update();
            validator.Validate();
        }
		#endregion
		#region member
		protected virtual void NotifyToggle() => Toggle?.Invoke(this);
		#endregion
		#region static
		public static implicit operator bool(KeycodeToggle k) {
			return k.guiVisible;
		}
		#endregion
	}
}
