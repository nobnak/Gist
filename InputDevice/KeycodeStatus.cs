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

        protected int lastUpdateFrame = -1;
        protected KeyFlag flags;
		
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

                if (Down != null && IsUp)
                    Down();
                if (Up != null && IsDown)
                    Up();
                if (Hold != null && IsHold)
                    Hold();
            }
        }
        public virtual void Reset() {
			Down = null;
			Up = null;
			Hold = null;
		}
        public virtual bool IsUp { get { Update();  return (flags & KeyFlag.Down) != 0; } }
        public virtual bool IsDown { get { Update(); return (flags & KeyFlag.Up) != 0; } }
        public virtual bool IsHold { get { Update(); return (flags & KeyFlag.Hold) != 0; } }
        #endregion

        #region member
        KeyFlag GetFlags() {
            return (KeyFlag)(
                (Input.GetKeyDown(key) ? (int)KeyFlag.Down : 0)
                + (Input.GetKeyUp(key) ? (int)KeyFlag.Up : 0)
                + (Input.GetKey(key) ? (int)KeyFlag.Hold : 0));
        }
        #endregion
    }
}
