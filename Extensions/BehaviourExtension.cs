using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.Extensions.Behaviour {

    public static class BehaviourExtension {

        public static bool IsActiveAndEnabledAlsoInEditMode(this MonoBehaviour b) {
            var result = b.isActiveAndEnabled;
            #if UNITY_EDITOR
            result = result && (Application.isPlaying || b.runInEditMode);
            #endif
            return result;
        }
    }
}