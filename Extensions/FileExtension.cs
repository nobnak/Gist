using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace nobnak.Gist.Extensions.FileExt {

	public static class FileExtension {

		public static System.Exception lastErrot = null;

		public static bool TrySave(this string path, string text) {
			try {
                path.Save(text);
				return true;
			} catch (System.Exception e) {
				lastErrot = e;
                Debug.Log($"Failed to save file : path={path}, readon={e.Message}");
				return false;
			}
		}
		public static bool TryLoad(this string path, out string text) {
			try {
                text = path.Load();
				return true;
			} catch (System.Exception e) {
				lastErrot = e;
				Debug.Log($"Faild to Load file : path={path}, readon={e.Message}");
				text = default(string);
				return false;
			}
		}

		public static void Save(this string path, string text) {
            File.WriteAllText(path, text);
        }
		public static string Load(this string path) {
            return File.ReadAllText(path);
		}

        public static string SanitizeFilename(this string filename, char rep = '_') {
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                filename = filename.Replace(c, rep);
            return filename;
        }
	}
}
