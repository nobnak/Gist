using nobnak.Gist.Extensions.FileExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Loader {

	[System.Serializable]
	public class FilePath {
		public const string DEFAULT_FILEPATH_PATTERN = @"%USERPROFILE%\Documents\{0}";
		public const string DEFAULT_FILENAME = @"ChangeThis.txt";
		public static readonly string DEFAULT_FILEPATH = string.Format(DEFAULT_FILEPATH_PATTERN, DEFAULT_FILENAME);

		public string filepath = DEFAULT_FILEPATH;

		public FilePath(string filepath) {
			Path = filepath;
		}
		public FilePath() : this(DEFAULT_FILEPATH) { }

		#region public
		public virtual string Path {
			get {
				return filepath;
			}
			set {
				filepath = value;
			}
		}
		public virtual string ExpandedPath {
			get {
				return System.Environment.ExpandEnvironmentVariables(filepath);
			}
		}
		public virtual string FullPath {
			get { return GetFullPath(ExpandedPath); }
		}

		public virtual bool TryLoad<Data>(out Data data) {
			string json;
			var result = FullPath.TryLoad(out json);
			data = (result ? JsonUtility.FromJson<Data>(json) : default(Data));
			return result;
		}
		public virtual bool TryLoadOverwrite<Data>(ref Data data) {
			string json;
			var result = FullPath.TryLoad(out json);
			if (result)
				JsonUtility.FromJsonOverwrite(json, data);
			return result;
		}

		public virtual bool TrySave<Data>(Data data) {
			var json = JsonUtility.ToJson(data, true);
			var result = FullPath.TrySave(json);
			return result;
		}

		public static string GetFullPath(string path) {
			if (!System.IO.Path.IsPathRooted(path))
				path = System.IO.Path.Combine(Application.dataPath, path);
			return path;
		}
		#endregion
	}
}
