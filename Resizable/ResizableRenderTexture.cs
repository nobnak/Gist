using UnityEngine;

namespace nobnak.Gist.Resizable {

	[System.Serializable]
    public class ResizableRenderTexture : System.IDisposable {
        public const int DEFAULT_ANTIALIASING = 1;

        public event System.Action<RenderTexture> AfterCreateTexture;
        public event System.Action<RenderTexture> BeforeDestroyTexture;

		[SerializeField]
		protected Vector2Int size;
		[SerializeField]
		protected Format format;

		protected bool valid = false;
		protected RenderTexture tex;

		public ResizableRenderTexture(Format format) {
			this.format = format;
		}
		public ResizableRenderTexture() : this(new Format()) { }

		#region IDisposable implementation
		public void Dispose() {
			ReleaseTexture();
		}
		#endregion

		#region public
		public Vector2Int Size {
			get { return size; }
			set {
				if (size != value) {
					Invalidate();
					size = value;
				}
			}
		}
		public RenderTexture Texture {
			get {
				Validate();
				return tex;
			}
		}
        public FilterMode FilterMode {
			get { return format.filterMode; }
			set {
				if (format.filterMode != value) {
					Invalidate();
					format.filterMode = value;
				}
			}
		}
        public TextureWrapMode WrapMode {
			get { return format.wrapMode; }
			set {
				if (format.wrapMode != value) {
					Invalidate();
					format.wrapMode = value;
				}
			}
		}
		public RenderTextureReadWrite ReadWrite {
			get { return format.readWrite; }
			set {
				if (format.readWrite != value) {
					Invalidate();
					format.readWrite = value;
				}
			}
		}
		public int AntiAliasing {
			get { return format.antiAliasing; }
			set {
				if (format.antiAliasing != value) {
					Invalidate();
					format.antiAliasing = value;
				}
			}
		}
		public Format Format {
			get { return format; }
			set {
				Invalidate();
				format = value;
			}
		}
		
        public void Clear(Color color, bool clearDepth = true, bool clearColor = true) {
            var active = RenderTexture.active;
            RenderTexture.active = tex;
            GL.Clear (clearDepth, clearColor, color);
            RenderTexture.active = active;
        }
		#endregion

		#region private
		protected void CreateTexture(int width, int height) {
            ReleaseTexture();

			if (width < 2 || height < 2) {
				Debug.LogFormat("Texture size too small : {0}x{1}", width, height);
				return;
			}

            tex = format.CreateTexture(width, height);
			Debug.LogFormat("Create ResizableRenderTexture : {0}\n{1}",
				string.Format("size={0}x{1}", tex.width, tex.height),
				string.Format("depth={0} format={1} readWrite={2} filter={3} wrap={4} antiAliasing={5}",
				tex.depth, tex.format, (tex.sRGB ? "sRGB" : "Linear"),
				tex.filterMode, tex.wrapMode, tex.antiAliasing));
            NotifyAfterCreateTexture ();
        }
        protected void NotifyAfterCreateTexture() {
            if (AfterCreateTexture != null)
                AfterCreateTexture (tex);
        }
        protected void NotifyBeforeDestroyTexture() {
            if (BeforeDestroyTexture != null)
                BeforeDestroyTexture (tex);
        }

        protected void ReleaseTexture() {
            NotifyBeforeDestroyTexture ();
            Release(tex);
            tex = null;
        }
		protected virtual void Invalidate() {
			valid = false;
		}
		protected virtual void Validate() {
			if (!valid) {
				CreateTexture(size.x, size.y);
				valid = CheckValidity();
			}
		}
		protected virtual bool CheckValidity() {
			return tex != null && tex.width == size.x && tex.height == size.y;
		}
		#endregion

		public static void Release(Object obj) {
			if (Application.isPlaying)
				Object.Destroy(obj);
			else
				Object.DestroyImmediate(obj);
		}
	}
}
