using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataUI;

namespace Gist {
    [RequireComponent(typeof(Camera))]
    public class PreRendering : MonoBehaviour {
        public Data data;

        public bool uiVisibility;
        public KeyCode uiToggle = KeyCode.P;

        LinkedList<Frame> _frames;
        FieldEditor _editor;
        Rect _window = new Rect(10, 10, 200, 200);

    	void OnEnable() {
            _frames = new LinkedList<Frame> ();
            _editor = new FieldEditor (data);
    	}
        void OnDisable() {
            if (_frames != null) {
                foreach (var f in _frames)
                    f.Dispose ();
                _frames.Clear ();
            }                
        }
        void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (_frames == null || data.passthrough) {
                Graphics.Blit (src, dst);
                return;
            }

            var tnow = Time.timeSinceLevelLoad;
            if (_frames.Count == 0 || 1f <= (data.maxFramesPerSec * (tnow - _frames.Last.Value.time))) {
                var f = Frame.Create (src);
                _frames.AddLast (f);
            }

            var first = _frames.First;
            var next = first.Next;
            while (next != null && next.Value.time < (tnow - data.delay)) {
                _frames.RemoveFirst ();
                first.Value.Dispose ();
                first = next;
                next = first.Next;
            }
            Graphics.Blit (first.Value.rt, dst);
        }
        void Update() {
            if (Input.GetKeyDown (uiToggle))
                uiVisibility = !uiVisibility;
        }
        void OnGUI() {
            if (uiVisibility && _editor != null)
                _window = GUILayout.Window (GetInstanceID (), _window, Window, name);
        }

        void Window(int id) {
            GUILayout.BeginVertical ();
            _editor.OnGUI ();
            GUILayout.EndVertical ();
            GUI.DragWindow ();
        }

        public class Frame : System.IDisposable {
            public readonly RenderTexture rt;
            public readonly float time;

            bool _disposed = false;

            private Frame(RenderTexture rt, float time) {
                this.rt = rt;
                this.time = time;
            }

            public static Frame Create(RenderTexture src) {
                var rt = RenderTexture.GetTemporary (src.width, src.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
                Graphics.Blit (src, rt);
                var f = new Frame (rt, Time.timeSinceLevelLoad);
                return f;
            }

            #region IDisposable implementation
            public void Dispose () {
                if (!_disposed) {
                    _disposed = true;
                    RenderTexture.ReleaseTemporary (rt);
                }
            }
            #endregion
        }

        [System.Serializable]
        public class Data {
            public bool passthrough = false;
            public float delay = 0.1f;
            public int maxFramesPerSec = 60;
        }
    }
}
