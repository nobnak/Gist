using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace nobnak.Gist.Extensions.FileExt {

	public static class FileExtension {

		public static bool TrySave(this string path, string text) {
			try {
				File.WriteAllText(path, text);
				return true;
			} catch {
				return false;
			}
		}
		public static bool TryLoad(this string path, out string text) {
			try {
				text = File.ReadAllText(path);
				return true;
			} catch {
				text = default(string);
				return false;
			}
		}

		public static void Save(this string path, string text) {
			path.TrySave(text);
		}
		public static string Load(this string path) {
			string text;
			path.TryLoad(out text);
			return text;
		}

        public static string SanitizeFilename(this string filename, char rep = '_') {
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                filename = filename.Replace(c, rep);
            return filename;
        }
	}
}
