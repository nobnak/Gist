using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace nobnak.Gist.Extensions.LogExt {

	public static class LogExtension {

		#region extensions
		public static void Log(
			this string text,
			[CallerMemberName] string member = "",
			[CallerFilePath] string filepath = "",
			[CallerLineNumber] int linenumber = 0
		) {
			Debug.Log(Format(text, member, filepath, linenumber));
		}
		public static void LogWarning(
			this string text,
			[CallerMemberName] string member = "",
			[CallerFilePath] string filepath = "",
			[CallerLineNumber] int linenumber = 0
		) {
			Debug.LogWarning(Format(text, member, filepath, linenumber));
		}
		public static void LogError(
			this string text,
			[CallerMemberName] string member = "",
			[CallerFilePath] string filepath = "",
			[CallerLineNumber] int linenumber = 0
		) {
			Debug.LogError(Format(text, member, filepath, linenumber));
		}

		public static void EditorLog(
			this string text,
			[CallerMemberName] string member = "",
			[CallerFilePath] string filepath = "",
			[CallerLineNumber] int linenumber = 0
		) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			text.Log(member: member, filepath: filepath, linenumber: linenumber);
#endif
		}
		public static void EditorLogWarning(
			this string text,
			[CallerMemberName] string member = "",
			[CallerFilePath] string filepath = "",
			[CallerLineNumber] int linenumber = 0
		) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			text.LogWarning(member: member, filepath: filepath, linenumber: linenumber);
#endif
		}
		public static void EditorLogError(
			this string text,
			[CallerMemberName] string member = "",
			[CallerFilePath] string filepath = "",
			[CallerLineNumber] int linenumber = 0
		) {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			text.LogError(member: member, filepath: filepath, linenumber: linenumber);
#endif
		}
		#endregion

		#region member
		private static string Format(string text, string member, string filepath, int linenumber) {
			return $"{text}\n[Log from {filepath}:{linenumber},{member}]";
		}
		#endregion
	}
}
