using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.ObjectExt {
        
    public static class ObjectExtension {
        public static void Destroy(this Object obj) {
			if (obj != null) {
				if (Application.isPlaying)
					Object.Destroy(obj);
				else
					Object.DestroyImmediate(obj);
			}
        }
		public static void Destroy(this Component comp) {
			if (comp != null)
				comp.gameObject.Destroy();
		}
    }
}
