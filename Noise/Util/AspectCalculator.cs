using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist.Events;

namespace Gist { 

    public class AspectCalculator : MonoBehaviour {
        public FloatEvent OnUpdate;

        float prevAspect;

        #region Unity
        void OnEnable() {
            prevAspect = -1f;
            UpdateAspect ();
        }
        void Update() {
            UpdateAspect ();
        }
        #endregion

        public void UpdateAspect(bool force = false) {
            var aspect = Calculate ();
            if (prevAspect != aspect) {
                prevAspect = aspect;
                OnUpdate.Invoke (aspect);
            }
        }

        float Calculate () {
            var s = transform.localScale;
            var aspect = s.x / s.y;
            return aspect;
        }
    }
}
