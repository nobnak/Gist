using nobnak.Gist.Extensions.FileExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Loader {

	[System.Serializable]
	public class FilePath {

		public string filepath = @"%USERPROFILE%\Documents\ChangeThis.txt";

		public virtual string Path {
			get {
				return System.Environment.ExpandEnvironmentVariables(filepath);
			}
			set {
				filepath = value;
			}
		}
		public virtual string FullPath {
			get { return GetFullPath(Path); }
		}

		public virtual bool TryLoad<Data>(out Data data) {
			string json;
			var result = Path.TryLoad(out json);
			data = (result ? JsonUtility.FromJson<Data>(json) : default(Data));
			return result;
		}

		public virtual bool TrySave<Data>(Data data) {
			var json = JsonUtility.ToJson(data, true);
			var result = Path.TrySave(json);
			return result;
		}

		public static string GetFullPath(string path) {
			if (!System.IO.Path.IsPathRooted(path))
				path = System.IO.Path.Combine(Application.dataPath, path);
			return path;
		}

	}
}
