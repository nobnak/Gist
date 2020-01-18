using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace nobnak.Gist.ObjectExt {

    public static class ObjectExtension {
        public static void DestroySelf(this Object obj, float t = 0f) {
			if (obj != null) {
				if (Application.isPlaying)
					Object.Destroy(obj, t);
				else
					Object.DestroyImmediate(obj);
			}
        }
		public static void DestroyGo(this Component comp, float t = 0f) {
			if (comp != null)
				comp.gameObject.DestroyGo(t);
		}
        public static void DestroyGo(this GameObject go, float t = 0f) {
            if (go != null)
                go.DestroySelf(t);
        }
        public static T DeepCopy<T>(this T src) {
			var json = JsonUtility.ToJson(src);
			return JsonUtility.FromJson<T>(json);
		}

        public static void Destroy(this System.IDisposable obj) {
            if (obj != null) {
                obj.Dispose();
            }
        }

        public static bool IsDefault<T>(this T v) {
            return EqualityComparer<T>.Default.Equals(v, default(T));
        }

#if UNITY_EDITOR
        public static string AssetFolderName(this Object obj) {
			string folder = null;
			try {
				folder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(obj));
				while (!Directory.Exists(folder)) {
					if (string.IsNullOrEmpty(folder)) {
						folder = "Assets";
						break;
					}
					folder = Directory.GetParent(folder).FullName;
				}
			} catch {
				folder = "Assets";
			}

			return folder;
		}
		public static IEnumerable<T> GetAssets<T>(this Object obj) where T : Object {
			var path = AssetDatabase.GetAssetPath(obj);
			foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path)) {
				if (asset is T)
					yield return (T)asset;
			}
		}
#endif

		public static bool IsPrefab(this GameObject go) {
			return go.scene.rootCount == 0;
		}
	}
}
