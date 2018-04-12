using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Resizable {
    public class LODRenderTexture : System.IDisposable {
        public const int DEFAULT_ANTIALIASING = 1;

        public event System.Action<LODRenderTexture> AfterCreateTexture;
        public event System.Action<LODRenderTexture> BeforeDestroyTexture;

        int depth;
		int lod;
        RenderTextureReadWrite readWrite;
        RenderTextureFormat format;
        RenderTexture tex;
        System.Func<Vector2Int> fsize;
        int antiAliasing;

        public LODRenderTexture(System.Func<Vector2Int> fsize, int depth = 24, 
			RenderTextureFormat format = RenderTextureFormat.ARGB32,
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, 
			int antiAliasing = 0) {
            this.fsize = fsize;
            this.Lod = 0;
            this.depth = depth;
            this.fsize = fsize;
            this.format = format;
            this.readWrite = readWrite;
            this.antiAliasing = ParseAntiAliasing (antiAliasing);
        }

		#region IDisposable implementation
		public void Dispose() {
			ReleaseTexture();
		}
		#endregion

		#region public
		public int Lod {
			get { return lod; }
			set { lod = Mathf.Max(0, value); }
		}
		public RenderTexture Texture { get { return tex; } }
        public FilterMode FilterMode { get; set; }
        public TextureWrapMode WrapMode { get; set; }

        public bool Update () {
            var size = fsize();
            var w = size.x >> Lod;
            var h = size.y >> Lod;
            return UpdateTexture (w, h);
        }
        public void Clear(Color color, bool clearDepth = true, bool clearColor = true) {
            var active = RenderTexture.active;
            RenderTexture.active = tex;
            GL.Clear (clearDepth, clearColor, color);
            RenderTexture.active = active;
        }
		#endregion

		#region private
		protected bool UpdateTexture(int width, int height) {
            if (tex == null || tex.width != width || tex.height != height) {
                ReleaseTexture();
                tex = new RenderTexture (width, height, depth, format, readWrite);
                tex.filterMode = FilterMode;
                tex.wrapMode = WrapMode;
                tex.antiAliasing = antiAliasing;
				Debug.LogFormat("Create LOD RTex : {0}x{1}", width, height);
                NotifyAfterCreateTexture ();
                return true;
            }
            return false;
        }
        protected void NotifyAfterCreateTexture() {
            if (AfterCreateTexture != null)
                AfterCreateTexture (this);
        }
        protected void NotifyBeforeDestroyTexture() {
            if (BeforeDestroyTexture != null)
                BeforeDestroyTexture (this);
        }

        protected void ReleaseTexture() {
            NotifyBeforeDestroyTexture ();
            tex.Destroy();
            tex = null;
        }
		protected static int ParseAntiAliasing(int antiAliasing) {
			return (antiAliasing > 0 ? antiAliasing : QualitySettings.antiAliasing);
		}
		#endregion
	}
}
