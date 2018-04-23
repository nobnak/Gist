using nobnak.Gist.Extensions.Texture2DExt;
using nobnak.Gist.Resizable;
using NonInstancedMaterialProperty;
using System.IO;
using UnityEngine;

namespace nobnak.Gist.Loader {

	[System.Serializable]
	public class ImageLoader : System.IDisposable {
		public event System.Action<Texture2D> Changed;

		[SerializeField]
		FilePath file;
		[SerializeField]
		protected Format2D format = new Format2D(TextureFormat.ARGB32, true, false);

		protected Reactive<string> reactivePath = new Reactive<string>();
		protected Reactive<TextureFormat> reactiveFormat = new Reactive<TextureFormat>();
		protected Reactive<bool> reactiveMipmap = new Reactive<bool>();
		protected Reactive<bool> reactiveLinear = new Reactive<bool>();

		protected Texture2D target;
		protected FileSystemWatcher watcher = new FileSystemWatcher();
		protected Validator validator = new Validator();

		public ImageLoader() : this(TextureFormat.ARGB32, true, false) { }
		public ImageLoader(TextureFormat format, bool mipmap, bool linear) {
			validator.Reset();
			validator.Validation += () => {
				LoadTarget();
			};
			validator.Validated += () => {
				NotifyChanged();
			};

			watcher.Changed += (s, e) => {
				if (file.Path == e.FullPath)
					validator.Invalidate();
			};
			
			reactivePath.Changed += v => {
				validator.Invalidate();
				watcher.Path = Path.GetDirectoryName(v.Value);
			};
			reactiveFormat.Changed += v => {
				validator.Invalidate();
				ClearTarget();
			};
			reactiveMipmap.Changed += v => {
				validator.Invalidate();
				ClearTarget();
			};
			reactiveLinear.Changed += v => {
				validator.Invalidate();
				ClearTarget();
			};
		}

		#region public
		public virtual Texture2D Target {
			get {
				validator.Validate();
				return target;
			}
		}
		public virtual string CurrentFilePath {
			get { return file.Path; }
			set {
				if (file.Path != value) {
					validator.Invalidate();
					file.Path = value;
				}
			}
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
			reactivePath.Value = file.Path;
			reactiveFormat.Value = format.textureFormat;
			reactiveMipmap.Value = format.useMipMap;
			reactiveLinear.Value = format.linear;
		}
		protected virtual bool LoadTarget() {
			var result = false;
			try {
				var path = file.Path;
				result = (!string.IsNullOrEmpty(path)
					&& (target == null
					? (target = format.CreateTexture(2, 2))
					: target).LoadImage(File.ReadAllBytes(path)));
				if (result)
					Debug.LogFormat("Load Image : {0}", path);
			} catch { }
			return result;
		}

		#endregion
	}
}
