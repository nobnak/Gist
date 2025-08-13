using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.InputDevice {

	[System.Serializable]
	public class KeycodeStatus {
        [System.Flags]
        public enum KeyFlag {
            None    = 0,
            Down    = 1 << 0,
            Up      = 1 << 1,
            Hold    = 1 << 2
        }

		[SerializeField]
		protected KeyCode key;
		[SerializeField]
		protected CombinationKey combinationKey;

        protected int lastUpdateFrame = -1;
        protected KeyFlag flags;
		protected bool combination;
		
		public KeycodeStatus(KeyCode key = KeyCode.None) {
			this.key = key;
		}

		public event System.Action Down;
		public event System.Action Up;
		public event System.Action Hold;

        #region interface
        public virtual void Update() {
            if (lastUpdateFrame != Time.frameCount) {
                lastUpdateFrame = Time.frameCount;
                flags = GetFlags();
				combination = FilterCombinationKey();

                if (IsDown) Down?.Invoke();
                if (IsUp) Up?.Invoke();
                if (IsHold) Hold?.Invoke();
            }
        }
        public virtual void Reset() {
			Down = null;
			Up = null;
			Hold = null;
		}
        public virtual bool IsDown { get { Update();  return (flags & KeyFlag.Down) != 0 && combination; } }
        public virtual bool IsUp { get { Update(); return (flags & KeyFlag.Up) != 0 && combination; } }
        public virtual bool IsHold { get { Update(); return (flags & KeyFlag.Hold) != 0 && combination; } }
        #endregion

        #region member
        KeyFlag GetFlags() {
            return (KeyFlag)(
                (Input.GetKeyDown(key) ? (int)KeyFlag.Down : 0)
                + (Input.GetKeyUp(key) ? (int)KeyFlag.Up : 0)
                + (Input.GetKey(key) ? (int)KeyFlag.Hold : 0));
        }
		bool FilterCombinationKey() {
			var filter = true;
			if (combinationKey != CombinationKey.None) {
				if ((combinationKey & CombinationKey.Shift) != 0)
					filter &= Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
				if ((combinationKey & CombinationKey.Control) != 0)
					filter &= Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				if ((combinationKey & CombinationKey.Alt) != 0)
					filter &= Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			}
			return filter;
		}
		#endregion

		#region declarations
		[System.Flags]
		public enum CombinationKey {
			None = 0,
			Shift = 1 << 0,
			Control = 1 << 1,
			Alt = 1 << 2,
		}
		#endregion
	}
}
