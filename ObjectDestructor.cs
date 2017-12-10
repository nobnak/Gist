using UnityEngine;

namespace Blending.Gist {

    public static class ObjectDestructor {
        public static void Destroy(Object obj) {
            if (Application.isPlaying)
                Object.Destroy (obj);
            else
                Object.DestroyImmediate (obj);
        }
    }
}
