using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.Performance {

    public class FrameRateDisplay : MonoBehaviour {
        public KeyCode guiDisplayKey = KeyCode.F;
		public FrameRateJob frameRate;

        protected bool visible;
        protected GUIStyle style;
        protected Rect windowRect;
		protected Coroutine job;

        #region Unity
        private void OnEnable() {
            windowRect = new Rect(10f, 10f, 10f, 10f);
            job = StartCoroutine(frameRate.GetEnumerator());
        }
        private void Update() {
            if (Input.GetKeyDown(guiDisplayKey)) {
                visible = !visible;
            }
        }
        private void OnGUI() {
            if (visible)
                windowRect = GUILayout.Window(GetInstanceID(), windowRect, Window, GetType().Name);
        }
		private void OnDisable() {
			if (job != null) {
				StopCoroutine(job);
				job = null;
			}
		}
		#endregion

		protected virtual void Window(int id) {
            if (style == null) {
                style = new GUIStyle(GUI.skin.label);
                style.stretchWidth = true;
                style.wordWrap = false;
            }

            using (new GUILayout.VerticalScope()) {
                using (new GUILayout.HorizontalScope()) {
                    GUILayout.Label(frameRate.ToString(), style);
                }
            }

            GUI.DragWindow();
        }
    }
}
