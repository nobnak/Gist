using UnityEngine;
using System.Collections;

namespace Gist {
    [RequireComponent(typeof(Camera))]
    public class CameraSettings : Settings<CameraSettings.Data> {
        Camera _c;

        void Awake() {
            _c = GetComponent<Camera> ();
        }
        protected override void Update () {
            base.Update ();
        }

        protected override void OnDataChange () {
            base.OnDataChange ();

            _c.clearFlags = data.clearFlags;
            _c.backgroundColor = data.backgroundColor;
            _c.orthographic = data.orthographic;
            _c.fieldOfView = data.fieldOfView;
            _c.orthographicSize = data.orthographicSize;
        }

        [System.Serializable]
        public class Data {
            public CameraClearFlags clearFlags = CameraClearFlags.Skybox;
            public Color backgroundColor = Color.clear;
            public bool orthographic = false;
            public float fieldOfView = 60f;
            public float orthographicSize = 10f;
            public float nearClip = 0.3f;
            public float farClip = 1000f;
        }
    }
}