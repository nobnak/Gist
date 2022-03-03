using nobnak.Gist.ObjectExt;
using UnityEngine;

namespace nobnak.Gist.Resizable {

	[System.Serializable]
    public class ResizableRenderTexture2 : System.IDisposable {
        public event System.Action<RenderTexture> AfterCreateTexture;
        public event System.Action<RenderTexture> BeforeDestroyTexture;
		public event System.Action<RenderTexture> Changed;

		[SerializeField]
		protected RenderTextureDescriptor desc;
		[SerializeField]
		FilterMode filter;
		[SerializeField]
		TextureWrapMode wrap;

		protected Validator validator;
		protected RenderTexture tex;

		public ResizableRenderTexture2(RenderTextureDescriptor desc) {
			this.desc = desc;

			validator = new Validator();

			validator.Validation += () => {
				if (desc.Equals(default) || desc.width < 4 || desc.height < 4) return;
				var tex = CreateTexture(desc);
				if (tex !=null) {
					tex.filterMode = filter;
					tex.wrapMode = wrap;
				}
			};
			validator.Validated += () => NotifyAfterCreateTexture();
			validator.SetCheckers(() =>
				tex != null && tex.descriptor.Equals(desc));
		}
		public ResizableRenderTexture2() : this(default) { }

		#region IDisposable implementation
		public virtual void Dispose() {
			ReleaseTexture();
		}
		#endregion

		#region public
		public virtual Vector2Int Size {
			get => new Vector2Int(desc.width, desc.height);
			set {
				if (desc.width != value.x || desc.height != value.y) {
					validator.Invalidate();
					desc.width = value.x;
					desc.height = value.y;
				}
			}
		}
		public virtual RenderTexture Texture {
			get {
				validator.Validate();
				return tex;
			}
		}
		public virtual RenderTextureDescriptor Format {
			get => desc;
			set {
				if (!desc.Equals(value)) {
					validator.Invalidate();
					desc = value;
				}
			}
		}
		public virtual FilterMode Filter {
			get => filter;
			set {
				if (filter != value) {
					validator.Invalidate();
					filter = value;
				}
			}
		}
		public virtual TextureWrapMode Wrap {
			get => wrap;
			set {
				if (wrap != value) {
					validator.Invalidate();
					wrap = value;
				}
			}
		}
		
        public virtual void Clear(Color color, bool clearDepth = true, bool clearColor = true) {
            var active = RenderTexture.active;
            RenderTexture.active = tex;
            GL.Clear (clearDepth, clearColor, color);
            RenderTexture.active = active;
		}
		public virtual void Invalidate() => validator.Invalidate();
		public virtual void Validate() => validator.Validate();
		public virtual void Release() => ReleaseTexture();
		#endregion

		#region private
		protected virtual RenderTexture CreateTexture(RenderTextureDescriptor desc) {
            ReleaseTexture();

			if (desc.width < 2 || desc.height < 2) {
				Debug.LogWarning($"Texture size too small : {desc}");
				return null;
			}

			tex = new RenderTexture(desc);
			tex.hideFlags = HideFlags.DontSave;
			Debug.Log($"{GetType().Name} : {tex.width}x{tex.height}");
			NotifyChanged();
			return tex;
		}
		protected virtual void ReleaseTexture() {
			NotifyBeforeDestroyTexture();
			tex.DestroySelf();
			tex = null;
			NotifyChanged();
		}
		protected virtual void NotifyAfterCreateTexture() {
            if (AfterCreateTexture != null)
                AfterCreateTexture (tex);
		}
		protected virtual void NotifyBeforeDestroyTexture() {
            if (BeforeDestroyTexture != null)
                BeforeDestroyTexture (tex);
		}
		protected virtual void NotifyChanged() {
			if (Changed != null)
				Changed(tex);
		}
		#endregion

		#region static
		public static implicit operator RenderTexture(ResizableRenderTexture2 v) {
			return (v == null) ? null : v.Texture;
		}
		#endregion
	}
}
