using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {
    public class LODRenderTexture : System.IDisposable {
        public const int DEFAULT_ANTIALIASING = -1;
        public delegate void TargetSize(out int width, out int height);
        public event System.Action<LODRenderTexture> AfterCreateTexture;

        int lod;
        int depth;
        RenderTextureReadWrite readWrite;
        RenderTextureFormat format;
        RenderTexture tex;
        TargetSize size;
        int antiAliasing;

        public LODRenderTexture(TargetSize size, int lod, int depth, RenderTextureFormat format,
            RenderTextureReadWrite readWrite, int antiAliasing) {
            this.size = size;
            this.lod = Mathf.Max(0, lod);
            this.depth = depth;
            this.size = size;
            this.format = format;
            this.readWrite = readWrite;
            this.antiAliasing = ParseAntiAliasing (antiAliasing);
        }
        public LODRenderTexture(Camera cam, int lod = 0, int depth = 24, 
            RenderTextureFormat format = RenderTextureFormat.ARGB32, 
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default,
            int antiAliasing = DEFAULT_ANTIALIASING)
            : this((out int w, out int h) => { w = cam.pixelWidth; h = cam.pixelHeight; }, 
                lod, depth, format, readWrite, antiAliasing) { }
        public LODRenderTexture(Texture tex, int lod = 0, int depth = 24, 
            RenderTextureFormat format = RenderTextureFormat.ARGB32,
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default,
            int antiAliasing = DEFAULT_ANTIALIASING)
            : this((out int w, out int h) => { w = tex.width; h = tex.height; }, 
                lod, depth, format, readWrite, antiAliasing) { }

        public RenderTexture Texture { get { return tex; } }
        public FilterMode FilterMode { get; set; }
        public TextureWrapMode WrapMode { get; set; }
        public bool UpdateTexture () {
            int w, h;
            size (out w, out h);
            w >>= lod;
            h >>= lod;
            return UpdateTexture (w, h);
        }
        public void Clear(Color color, bool clearDepth = true, bool clearColor = true) {
            var active = RenderTexture.active;
            RenderTexture.active = tex;
            GL.Clear (clearDepth, clearColor, color);
            RenderTexture.active = active;
        }

        bool UpdateTexture(int width, int height) {
            if (tex == null || tex.width != width || tex.height != height) {
                Release(ref tex);
                tex = new RenderTexture (width, height, depth, format, readWrite);
                tex.filterMode = FilterMode;
                tex.wrapMode = WrapMode;
                tex.antiAliasing = antiAliasing;
                NotifyAfterCreateTexture ();
                return true;
            }
            return false;
        }
        void NotifyAfterCreateTexture() {
            if (AfterCreateTexture != null)
                AfterCreateTexture (this);
        }
        static int ParseAntiAliasing (int antiAliasing) {
            return (antiAliasing > 0 ? antiAliasing : (QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing));
        }

        #region IDisposable implementation
        public void Dispose () {
            Release(ref tex);
        }
        #endregion

        void Release<T>(ref T data) where T : Object {
            if (Application.isPlaying)
                Object.Destroy (data);
            else
                Object.DestroyImmediate (data);
        }
    }
}
