using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace nobnak.Gist.ObjectExt {

    public static class ObjectExtension {
        public static void Destroy(this Object obj, float t = 0f) {
			if (obj != null) {
				if (Application.isPlaying)
					Object.Destroy(obj, t);
				else
					Object.DestroyImmediate(obj);
			}
        }
		public static void Destroy(this Component comp, float t = 0f) {
			if (comp != null)
				comp.gameObject.Destroy(t);
		}
		public static T DeepCopy<T>(this T src) {
			var json = JsonUtility.ToJson(src);
			return JsonUtility.FromJson<T>(json);
		}
    }
}
