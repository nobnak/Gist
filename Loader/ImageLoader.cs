using NonInstancedMaterialProperty;
using System.IO;
using UnityEngine;

namespace nobnak.Gist.Loader {

	[System.Serializable]
	public class ImageLoader : System.IDisposable {
		[SerializeField]
		FilePath file;
		[SerializeField]
		protected TextureFormat format = TextureFormat.ARGB32;
		[SerializeField]
		protected bool mipmap = true;

		protected Reactive<string> reactivePath = new Reactive<string>();
		protected Reactive<TextureFormat> reactiveFormat = new Reactive<TextureFormat>();
		protected Reactive<bool> reactiveMipmap = new Reactive<bool>();

		protected Texture2D target;
		protected FileSystemWatcher watcher = new FileSystemWatcher();
		protected Validator validator = new Validator();

		public ImageLoader() {
			validator.Reset();
			validator.Validation += () => {
				ApplyDataToReactive();
				LoadTarget();
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
		}

		public Texture2D Target {
			get {
				validator.Validate();
				return target;
			}
		}

		public void Dispose() {
			ClearTarget();
		}

		#region Hidden
		protected void ClearTarget() {
			if (target != null) {
				ObjectDestructor.Destroy(target);
				target = null;
			}
		}
		protected void ApplyDataToReactive() {
			reactivePath.Value = file.Path;
			reactiveFormat.Value = format;
			reactiveMipmap.Value = mipmap;
		}
		protected void LoadTarget() {
			try {
				if (target == null)
					target = new Texture2D(2, 2, format, mipmap);
				var path = file.Path;
				if (!string.IsNullOrEmpty(path) && target.LoadImage(File.ReadAllBytes(path)))
					Debug.LogFormat("Load Image : {0}", path);
			} catch { }
		}

		#endregion
	}
}
