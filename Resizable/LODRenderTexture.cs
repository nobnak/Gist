using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Resizable {

	[System.Serializable]
    public class LODRenderTexture : System.IDisposable {
        public event System.Action<LODRenderTexture> AfterCreateTexture;
        public event System.Action<LODRenderTexture> BeforeDestroyTexture;

		[SerializeField]
		protected int lod;
		[SerializeField]
		protected Vector2Int size;

		protected Validator validator = new Validator();
		protected ResizableRenderTexture tex;
		
		public LODRenderTexture() {
			tex = new ResizableRenderTexture();

			validator.Validation += () => {
				var sizeLod = size;
				if (lod > 0) {
					sizeLod.x = sizeLod.x >> Lod;
					sizeLod.y = sizeLod.y >> Lod;
				}
				tex.Size = sizeLod;
			};
		}

		#region IDisposable implementation
		public void Dispose() {
			if (tex != null) {
				tex.Dispose();
				tex = null;
			}
		}
		#endregion

		#region public
		public int Lod {
			get { return lod; }
			set {
				value = Mathf.Max(0, value);
				if (lod != value) {
					validator.Invalidate();
					lod = value;
				}
			}
		}
		public Vector2Int Size {
			get { return size; }
			set {
				if (size != value) {
					validator.Invalidate();
					size = value;
				}
			}
		}
		public Format Format {
			get { return tex.Format; }
			set {
				tex.Format = value;
			}
		}
		public RenderTexture Texture {
			get {
				validator.Validate();
				return tex.Texture;
			}
		}

        public void Clear(Color color, bool clearDepth = true, bool clearColor = true) {
			tex.Clear(color, clearDepth);
        }
		#endregion

		#region private
        protected void NotifyAfterCreateTexture() {
            if (AfterCreateTexture != null)
                AfterCreateTexture (this);
        }
        protected void NotifyBeforeDestroyTexture() {
            if (BeforeDestroyTexture != null)
                BeforeDestroyTexture (this);
        }
		
		protected static int ParseAntiAliasing(int antiAliasing) {
			return (antiAliasing > 0 ? antiAliasing : QualitySettings.antiAliasing);
		}
		#endregion
	}
}
