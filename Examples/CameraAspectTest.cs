using UnityEngine;
using System.Collections;

namespace Gist {
        
    [ExecuteInEditMode]
    public class CameraAspectTest : MonoBehaviour {
        public Camera targetCam;
        public Positioner[] positions;

        public float mouseSize = 1f;
        public float mouseDepth = 10f;
        public Color mouseColor = Color.white;

        GLFigure _fig;

        void Awake() {
            _fig = new GLFigure ();
        }
        void OnDestroy() {
            if (_fig != null)
                _fig.Dispose ();
        }
        void Update() {
            if (positions == null || targetCam == null || _fig == null)
                return;

            for (var i = 0; i < positions.Length; i++) {
                var pos = positions [i];
                if (pos == null || pos.target == null)
                    continue;

                var vp = pos.viewportPos;
                vp.z = vp.z * (targetCam.farClipPlane - targetCam.nearClipPlane) + targetCam.nearClipPlane;
                pos.target.position = targetCam.ViewportToWorldPoint (vp);
            }
        }
        void OnRenderObject() {
            if (_fig == null || targetCam == null)
                return;
            
            var mousePosScreen = Input.mousePosition;
            mousePosScreen.z = mouseDepth;
            var mousePos = targetCam.ScreenToWorldPoint (mousePosScreen);
            _fig.FillCircle (mousePos, targetCam.transform.rotation, mouseSize * Vector2.one, mouseColor);
        }

        [System.Serializable]
        public class Positioner {
            public Transform target;
            public Vector3 viewportPos;
        }
    }
}