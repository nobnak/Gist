using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {
        
    public static class ObjectDestructor {
        public static void Destroy(Object obj) {
            if (Application.isPlaying)
                Object.Destroy (obj);
            else
                Object.DestroyImmediate (obj);
        }
    }
}
