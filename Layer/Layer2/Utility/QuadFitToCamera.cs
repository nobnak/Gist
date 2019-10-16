using nobnak.Gist.Cameras;
using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Layer2 {

    [ExecuteAlways]
    public class QuadFitToCamera : MonoBehaviour {
        [SerializeField]
        protected Camera target;

        protected CameraData cameraData;

        #region interface
        private void Update() {
            if (target != null && !cameraData.Equals(target)) {
                cameraData = target;

                var h = target.orthographicSize * 2f;
                var w = target.aspect * h;

                var s1 = new Vector3(w, h, 1f);
                var s0 = transform.localScale;
                if (s0 != s1) {
                    transform.localScale = s1;
                    this.CallbackSelf<IChangeListener<Transform>>(v => v.TargetOnChange(transform));
                }
            }
        }
        #endregion
    }
}
