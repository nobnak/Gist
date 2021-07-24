using nobnak.Gist.Extensions.Texture2DExt;
using nobnak.Gist.Resizable;
using System.IO;
using UnityEngine;

namespace nobnak.Gist.Loader {

	[System.Serializable]
	public class ImageLoader : System.IDisposable {
		public const string DEFAULT_FILEPATH = @"%USERPROFILE%\Documents\ChangeThis.png";

		public event System.Action<Texture2D> Changed;

		[SerializeField]
		protected FilePath file;
		[SerializeField]
		protected Format2D format = new Format2D(TextureFormat.ARGB32, true, false);
        [SerializeField]
        protected bool markNonReadable = true;
        [SerializeField]
        protected Texture2D defaultTexture;

        protected Reactive<string> reactivePath = new Reactive<string>();

		protected Texture2D target;
		protected FileSystemWatcher watcher;
		protected Validator validator = new Validator();

		public ImageLoader() : this(TextureFormat.ARGB32, true, false) { }
		public ImageLoader(TextureFormat format, bool mipmap, bool linear) {
			file = new FilePath(DEFAULT_FILEPATH);

			validator.Reset();
			validator.Validation += () => LoadTarget();
			validator.Validated += () => NotifyChanged();

			watcher = new FileSystemWatcher();
			watcher.IncludeSubdirectories = false;
			watcher.Changed += ListenFilesystemEvent;
			watcher.Created += ListenFilesystemEvent;
			watcher.Deleted += ListenFilesystemEvent;
			//watcher.Renamed += (s, e) => validator.Invalidate();

			reactivePath.Changed += v => {
				try {
					file.Path = v;
					watcher.Path = Path.GetDirectoryName(file.FullPath);
					validator.Invalidate();
				} catch { }
			};

			watcher.EnableRaisingEvents = true;
		}

		#region public
		public virtual Texture2D Target {
			get {
				Validate();
				return target == null ? defaultTexture : target;
			}
		}

		public virtual bool Validate() {
			ApplyDataToReactive();
			return validator.Validate();
		}

		public virtual string CurrentFilePath {
			get { return file.Path; }
			set { reactivePath.Value = value; }
		}

		public virtual void Dispose() {
			ClearTarget();
		}
		#endregion

		#region private
		protected virtual void ListenFilesystemEvent(object s, FileSystemEventArgs e) {
			if (file.FullPath == e.FullPath) {
				Debug.LogFormat("File changed : path={0} changed={1}", e.FullPath, e.ChangeType);
				validator.Invalidate();
			}
		}
		protected virtual void NotifyChanged() {
            if (Changed != null)
			    Changed(Target);
		}
		protected virtual void ClearTarget() {
			if (target != null) {
				target.Destroy();
				target = null;
			}
		}
		protected virtual void ApplyDataToReactive() {
			reactivePath.Value = file.Path;
		}
		protected virtual bool LoadTarget() {
			var result = false;
			var path = file.FullPath;
			try {
				result = (!string.IsNullOrEmpty(path)
					&& File.Exists(path)
					&& (target == null
					? (target = format.CreateTexture(2, 2))
					: target).LoadImage(File.ReadAllBytes(path), markNonReadable));

				if (result) {
					Debug.LogFormat("Load Image : {0}", path);
					target.name = path;
					target.hideFlags = HideFlags.DontSave;
				} else {
					target.Destroy();
					target = null;
				}
			} catch (System.Exception e) {
				Debug.LogErrorFormat("Exception at loading : {0}\n{1}", path, e);
			}
			return result;
		}

		#endregion
	}
}
