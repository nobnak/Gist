using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.LogExt {

	public static class LogExtension {

		public static void EditorLog(this string text) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Debug.Log(text);
#endif
		}
		public static void EditorLogWarning(this string text) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Debug.LogWarning(text);
#endif
		}
		public static void EditorLogError(this string text) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Debug.LogError(text);
#endif
		}
	}
}
