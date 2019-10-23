using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Pooling {

    public class RenderTexturePool : IMemoryPool<RenderTexture>, System.IDisposable {

        public event System.Action<RenderTexture> OnCreate;

        protected MemoryPool<RenderTexture> pool;
        protected Vector3Int size = new Vector3Int(4, 4, 24);
        protected RenderTextureFormat format;
        protected RenderTextureReadWrite linear;

        public RenderTexturePool(
            RenderTextureFormat format = RenderTextureFormat.ARGB32,  
            RenderTextureReadWrite linear = RenderTextureReadWrite.Linear) {
            this.format = format;
            this.linear = linear;
        }

        public void SetSize(Vector3Int newSize) {
            if (size != newSize) {
                size = newSize;
                ReleasePool();
                CreatePool();
            }
        }

        #region IMemoryPool
        public int Count => pool.Count;

        public IMemoryPool<RenderTexture> Free(RenderTexture used) {
            return pool.Free(used);
        }

        public RenderTexture New() {
            return pool.New();
        }
        #endregion

        #region IDisposable
        public void Dispose() {
            ReleasePool();
        }

        private void ReleasePool() {
            if (pool != null) {
                pool.Dispose();
                pool = null;
            }
        }
        private void CreatePool() {
            pool = new MemoryPool<RenderTexture>(
                () => {
                    var tex = new RenderTexture(size.x, size.y, size.z, format, linear);
                    if (OnCreate != null)
                        OnCreate.Invoke(tex);
                    return tex;
                },
                t => { },
                t => t.DestroySelf()
                );
        }
        #endregion
    }
}
