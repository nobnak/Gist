using nobnak.Gist.Extensions.ComponentExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.Behaviour {

    public static class BehaviourExtension {

        public static bool IsActiveAndEnabledAlsoInEditMode(this MonoBehaviour b) {
            var result = b != null && b.isActiveAndEnabled;
            #if UNITY_EDITOR
            result = result && (Application.isPlaying || b.runInEditMode);
            #endif
            return result;
        }
        public static bool CanRender(this MonoBehaviour b) {
            return b.IsActiveAndEnabledAlsoInEditMode() && b.IsVisibleLayer();
        }
    }
}