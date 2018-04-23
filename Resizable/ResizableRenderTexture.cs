using nobnak.Gist.ObjectExt;
using UnityEngine;

namespace nobnak.Gist.Resizable {

	[System.Serializable]
    public class ResizableRenderTexture : System.IDisposable {
        public event System.Action<RenderTexture> AfterCreateTexture;
        public event System.Action<RenderTexture> BeforeDestroyTexture;

		[SerializeField]
		protected Vector2Int size;
		[SerializeField]
		protected FormatRT format;

		protected bool valid = false;
		protected RenderTexture tex;

		public ResizableRenderTexture(FormatRT format) {
			this.format = format;
		}
		public ResizableRenderTexture() : this(new FormatRT()) { }

		#region IDisposable implementation
		public virtual void Dispose() {
			ReleaseTexture();
		}
		#endregion

		#region public
		public virtual Vector2Int Size {
			get { return size; }
			set {
				if (size != value) {
					Invalidate();
					size = value;
				}
			}
		}
		public virtual RenderTexture Texture {
			get {
				Validate();
				return tex;
			}
		}
        public virtual FilterMode FilterMode {
			get { return format.filterMode; }
			set {
				if (format.filterMode != value) {
					Invalidate();
					format.filterMode = value;
				}
			}
		}
        public virtual TextureWrapMode WrapMode {
			get { return format.wrapMode; }
			set {
				if (format.wrapMode != value) {
					Invalidate();
					format.wrapMode = value;
				}
			}
		}
		public virtual RenderTextureReadWrite ReadWrite {
			get { return format.readWrite; }
			set {
				if (format.readWrite != value) {
					Invalidate();
					format.readWrite = value;
				}
			}
		}
		public virtual int AntiAliasing {
			get { return format.antiAliasing; }
			set {
				if (format.antiAliasing != value) {
					Invalidate();
					format.antiAliasing = value;
				}
			}
		}
		public virtual FormatRT Format {
			get { return format; }
			set {
				Invalidate();
				format = value;
			}
		}
		
        public virtual void Clear(Color color, bool clearDepth = true, bool clearColor = true) {
            var active = RenderTexture.active;
            RenderTexture.active = tex;
            GL.Clear (clearDepth, clearColor, color);
            RenderTexture.active = active;
		}
		public virtual void Invalidate() {
			valid = false;
		}
		public virtual void Validate() {
			if (!valid) {
				CreateTexture(size.x, size.y);
				valid = CheckValidity();
			}
		}
		public virtual void Release() {
		}
		#endregion

		#region private
		protected virtual void CreateTexture(int width, int height) {
            ReleaseTexture();

			if (width < 2 || height < 2) {
				Debug.LogFormat("Texture size too small : {0}x{1}", width, height);
				return;
			}

            tex = format.CreateTexture(width, height);
			Debug.LogFormat("Create ResizableRenderTexture : {0}\n{1}",
				string.Format("size={0}x{1}", tex.width, tex.height),
				format);
            NotifyAfterCreateTexture ();
        }
        protected virtual void NotifyAfterCreateTexture() {
            if (AfterCreateTexture != null)
                AfterCreateTexture (tex);
        }
        protected virtual void NotifyBeforeDestroyTexture() {
            if (BeforeDestroyTexture != null)
                BeforeDestroyTexture (tex);
        }

        protected virtual void ReleaseTexture() {
            NotifyBeforeDestroyTexture ();
            tex.Destroy();
            tex = null;
        }
		protected virtual bool CheckValidity() {
			return tex != null && tex.width == size.x && tex.height == size.y;
		}
		#endregion
		
	}
}
