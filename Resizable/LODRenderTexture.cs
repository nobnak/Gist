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
		protected ResizableRenderTexture tex;

		public LODRenderTexture() {
			tex = new ResizableRenderTexture();
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
			set { lod = Mathf.Max(0, value); }
		}
		public Vector2Int Size {
			get { return tex.Size; }
			set {
				var size = value;
				if (lod > 0) {
					size.x = size.x >> Lod;
					size.y = size.y >> Lod;
				}
				tex.Size = size;
			}
		}
		public Format Format {
			get { return tex.Format; }
			set {
				tex.Format = value;
			}
		}
		public RenderTexture Texture {
			get { return tex.Texture; }
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
