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

		protected Reactive<string> reactivePath = new Reactive<string>();

		protected Texture2D target;
		protected FileSystemWatcher watcher;
		protected Validator validator = new Validator();

		public ImageLoader() : this(TextureFormat.ARGB32, true, false) { }
		public ImageLoader(TextureFormat format, bool mipmap, bool linear) {
			file = new FilePath(DEFAULT_FILEPATH);

			validator.Reset();
			validator.Validation += () => {
				LoadTarget();
			};
			validator.Validated += () => {
				NotifyChanged();
			};

			watcher = new FileSystemWatcher();
			watcher.Changed += (s, e) => {
				validator.Invalidate();
			};
			
			reactivePath.Changed += v => {
				validator.Invalidate();
				file.Path = v;
				watcher.Path = Path.GetDirectoryName(file.FullPath);
			};
			
			watcher.EnableRaisingEvents = true;
		}

		#region public
		public virtual Texture2D Target {
			get {
				Validate();
				return target;
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
		protected virtual void NotifyChanged() {
			if (Changed != null)
				Changed(target);
		}
		protected virtual void ClearTarget() {
			if (target != null) {
				target.Destroy();
				target = null;
			}
		}
		protected virtual void ApplyDataToReactive() {
			reactivePath.Value = file.FullPath;
		}
		protected virtual bool LoadTarget() {
			var result = false;
			try {
				var path = file.FullPath;
				result = (!string.IsNullOrEmpty(path)
					&& File.Exists(path)
					&& (target == null
					? (target = format.CreateTexture(2, 2))
					: target).LoadImage(File.ReadAllBytes(path)));

				if (result) {
					Debug.LogFormat("Load Image : {0}", path);
				} else {
					target.Destroy();
					target = null;
				}
			} catch { }
			return result;
		}

		#endregion
	}
}
