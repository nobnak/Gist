using UnityEngine;
using System.Collections;

namespace nobnak.Gist {

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraFit : MonoBehaviour {
        public bool copyMatrix = true;
        public bool local = true;
        public Camera[] targetCameras;

        Camera _attachedCam;

        void OnEnable() {
            _attachedCam = GetComponent<Camera> ();
        }
    	void Update () {
            if (targetCameras != null)
                foreach (var cam in targetCameras)
                    CopySettings (_attachedCam, cam);
        }

        public void CopySettings(Camera src, Camera dst) {
            dst.orthographic = src.orthographic;
            dst.orthographicSize = src.orthographicSize;

            dst.fieldOfView = src.fieldOfView;
            dst.nearClipPlane = src.nearClipPlane;
            dst.farClipPlane = src.farClipPlane;

            if (local) {
                dst.transform.localPosition = src.transform.localPosition;
                dst.transform.localRotation = src.transform.localRotation;
            } else {
                dst.transform.position = src.transform.position;
                dst.transform.rotation = src.transform.rotation;
            }

            if (copyMatrix)
                dst.projectionMatrix = src.projectionMatrix;
        }
    }
}
