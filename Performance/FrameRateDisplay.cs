using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.Performance {

    public class FrameRateDisplay : MonoBehaviour {
        public KeyCode guiDisplayKey = KeyCode.F;
        public float updateFreq = 0.5f;

        protected float cachedFrameRate;

        protected bool visible;
        protected GUIStyle style;
        protected Rect windowRect;

        #region Unity
        private void OnEnable() {
            windowRect = new Rect(10f, 10f, 10f, 10f);
            StartCoroutine(Job());
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
        #endregion

        protected virtual IEnumerator Job() {
            while (true) {
                cachedFrameRate = 1.0f / Time.smoothDeltaTime;
                yield return new WaitForSeconds(updateFreq);
            }
        }
        protected virtual void Window(int id) {
            if (style == null) {
                style = new GUIStyle(GUI.skin.label);
                style.stretchWidth = true;
                style.wordWrap = false;
            }

            using (new GUILayout.VerticalScope()) {
                using (new GUILayout.HorizontalScope()) {
                    GUILayout.Label(string.Format("Frame-rate : {0:f1} (fps)", cachedFrameRate), style);
                }
            }

            GUI.DragWindow();
        }
    }
}
