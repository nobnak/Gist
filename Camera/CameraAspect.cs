using UnityEngine;
using System.Collections;

namespace nobnak.Gist {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraAspect : MonoBehaviour {
        public float aspect = -1f;

        Camera _attachedCam;

        void AssureInit() {
            if (_attachedCam == null)
                _attachedCam = GetComponent<Camera>();
        }
    	void Update () {
            AssureInit ();

            if (aspect <= 0f) {
                _attachedCam.ResetAspect ();
                _attachedCam.rect = new Rect (0f, 0f, 1f, 1f);
                return;
            }

            var screenWidth = (float)Screen.width;
            var screenHeight = (float)Screen.height;
            var targetTex = _attachedCam.targetTexture;
            if (targetTex != null) {
                screenWidth = targetTex.width;
                screenHeight = targetTex.height;
            }
            var screenAspect = screenWidth / screenHeight;
            if (aspect < screenAspect) {
                var offset = 0.5f * (1f - aspect / screenAspect);
                _attachedCam.aspect = aspect;
                _attachedCam.rect = new Rect (offset, 0f, 1f - 2f * offset, 1f);
            } else {
                var offset = 0.5f * (1f - screenAspect / aspect);
                _attachedCam.aspect = aspect;
                _attachedCam.rect = new Rect (0f, offset, 1f, 1f - 2f * offset);
            }

            Debug.LogFormat ("Size {0}x{1}", _attachedCam.pixelWidth, _attachedCam.pixelHeight);
    	}
    }
}
