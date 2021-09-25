using nobnak.Gist.Cameras;
using nobnak.Gist.Events.Interfaces;
using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace nobnak.Gist.Layer2 {

    [ExecuteAlways]
    public class QuadFitToCamera : MonoBehaviour {
        [SerializeField]
        protected Camera target;
        [SerializeField]
        protected Events events = new Events();

        protected CameraData cameraData;
        protected Validator validator = new Validator();

        #region unity
        private void OnEnable() {
            cameraData = default;

            validator.Reset();
            validator.SetCheckers(() => cameraData.Equals(target));
            validator.Validation += () => {
                Debug.Log($"Validate : {GetType().Name}");
                cameraData = target;
                if (target == null) return;

                var h = target.orthographicSize * 2f;
                var w = target.aspect * h;

                var s1 = new Vector3(w, h, 1f);
                var s0 = transform.localScale;
                if (s0 != s1) {
                    transform.localScale = s1;
                    this.CallbackSelf<IChangeListener<Transform>>(v => v.TargetOnChange(transform));
                }
            };
            validator.Validated += () => events?.TransformOnChange?.Invoke();

            validator.Validate();
        }
        private void Update() {
            validator.Validate();
        }
        #endregion

        #region member
        #endregion

        #region definitions
        [System.Serializable]
        public class Events {
            public UnityEvent TransformOnChange = new UnityEvent();
        }
        #endregion
    }
}
