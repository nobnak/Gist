using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.Extensions.Behaviour {

    public static class BehaviourExtension {

        public static bool IsActiveAndEnabledAlsoInEditMode(this MonoBehaviour b) {
            #if UNITY_EDITOR
            return (Application.isPlaying ? b.isActiveAndEnabled : b.runInEditMode);
            #else
            return b.isActiveAndEnabled;
            #endif
        }
    }
}