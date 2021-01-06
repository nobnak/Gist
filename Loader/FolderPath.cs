using nobnak.Gist.Extensions.FileExt;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace nobnak.Gist.Loader {

	[System.Serializable]
	public class FolderPath {
		public const string DEFAULT_FILEPATH = @"%USERPROFILE%\Documents\";

		[SerializeField]
		protected FolderSelectionMethod method;
		[SerializeField]
		protected System.Environment.SpecialFolder specialFolder;
        [SerializeField]
        protected string folderpath = DEFAULT_FILEPATH;

		public FolderPath(
			FolderSelectionMethod method = FolderSelectionMethod.SpecialFolder,
			System.Environment.SpecialFolder specialFolder = System.Environment.SpecialFolder.MyDocuments,
			string folderpath = DEFAULT_FILEPATH) {
			this.method = method;
			this.specialFolder = specialFolder;
			this.folderpath = folderpath;
		}

		#region public
		public virtual string Folder {
			get {
                switch (method) {
                    default:
                        return System.Environment.GetFolderPath(specialFolder);
                    case FolderSelectionMethod.String:
                        return ExpandedPath(folderpath);
                }
			}
		}
		public virtual string GetFullPath(string filename) {
			return GetFullPath(Folder, filename);
		}

        public virtual bool TrySave(string filenam, string text) {
            return GetFullPath(filenam).TrySave(text);
        }
        public virtual bool TryLoad(string filename, out string text) {
            var path = GetFullPath(filename);
            if (!File.Exists(path)) {
                Debug.Log($"File not found : {path}");
                text = default;
                return false;
            }
            return path.TryLoad(out text);
        }
        #endregion

        #region static
        public static string ExpandedPath(string path) {
            return System.Environment.ExpandEnvironmentVariables(path);
        }
        public static string GetFullPath(string folder, string filename) {
            return System.IO.Path.Combine(folder, filename);
        }
        #endregion

        #region classes
        public enum FolderSelectionMethod { SpecialFolder = 0, String }
        #endregion
    }
}
