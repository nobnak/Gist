using nobnak.Gist.Extensions.GPUExt;
using nobnak.Gist.ObjectExt;
using nobnak.Gist.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Compute.Blurring {

    public class Blur : System.IDisposable {

        public const string PATH = "GaussianDownsample";
        public const int k_MaxPyramidSize = 16; // Just to make sure we handle 64k screens... Future-proof!

        public static readonly int P_SOURCE_TEX = Shader.PropertyToID("_Source");
        public static readonly int P_RESULT_TEX = Shader.PropertyToID("_Result");
        public static readonly int P_SIZE = Shader.PropertyToID("_Size");

        public enum Pass { Downsample13 = 0, UpsampleTent, Blit }

        protected readonly int K_MAIN;
        protected int iterations = 1;
        protected RenderTexturePool pool;

        public Blur() {
            pool = new RenderTexturePool(RenderTextureFormat.ARGBHalf);
            CS = Resources.Load<ComputeShader>(PATH);
            K_MAIN = CS.FindKernel("KMain");
        }

        #region interface
        #region IDisposable
        public void Dispose() {
        }
        #endregion

        public ComputeShader CS { get; protected set; }
        public void Render(Texture src, RenderTexture dst) {

            var w = dst.width;
            var h = dst.height;
            var size = new Vector4(w, h, 1f / w, 1f / h);
            CS.SetVector(P_SIZE, size);

            CS.SetTexture(K_MAIN, P_SOURCE_TEX, src);
            CS.SetTexture(K_MAIN, P_RESULT_TEX, dst);

            var ds = CS.DispatchSize(K_MAIN, w, h);
            CS.Dispatch(K_MAIN, ds.x, ds.y, ds.z);
        }
        public void Render(Texture src, RenderTexture dst, int iterations, int lod = 0) {
            iterations = Mathf.Max(0, iterations);
            lod = Mathf.Clamp(lod, 0, 16);

            var size = new Vector2Int(src.width, src.height);
            var lodSize = LoD(size, lod);

            var last = src;
            RenderTexture lastTmp = null;
            for (var l = 1; l <= lod; l++) {
                var nextSize = LoD(size, l);
                var nextTmp = CreateTempRT(nextSize);
                var next = nextTmp;
                Render(last, next);

                if (lastTmp != null)
                    ReleaseTempRT(lastTmp);
                last = lastTmp = nextTmp;
            }

            for (var i = 0; i < iterations; i++) {
                var nextTmp = CreateTempRT(lodSize);
                var next = nextTmp;
                Render(last, next);

                if (lastTmp != null)
                    ReleaseTempRT(lastTmp);
                last = lastTmp = nextTmp;
            }

            Graphics.Blit(last, dst);
            if (lastTmp != null)
                ReleaseTempRT(lastTmp);
        }
        public bool Render(Texture src, ref RenderTexture dst, int iterations, int lod = 0) {
            iterations = Mathf.Max(0, iterations);
            lod = Mathf.Clamp(lod, 0, 16);

            var size = new Vector2Int(src.width, src.height);
            var lodSize = LoD(size, lod);
            var dstResized = dst == null 
                || dst.width != lodSize.x || dst.height != lodSize.y
                || !dst.enableRandomWrite;
            if (dstResized) {
                dst.DestroySelf();
                dst = CreateRT(lodSize);
            }
            Render(src, dst, iterations, lod);

            return dstResized;
        }
        public void FindSize(int sourceRes, int blurRes, out int iterations, out int lod) {
            // Cascaded Gaussians : sigma^2 = sigma_0^2 + sigma_1^2
            // See : http://www.cse.psu.edu/~rtc12/CSE486/lecture10.pdf
            var flod = Mathf.Max(0, Mathf.Log(sourceRes, 2) - Mathf.Log(blurRes, 2));
            lod = Mathf.FloorToInt(flod);
            iterations = Mathf.RoundToInt(Mathf.Lerp(0, 4, flod - lod));
        }
        #endregion

        #region static
        public static RenderTexture CreateRT(Vector2Int lodSize) {
            RenderTexture dst = new RenderTexture(lodSize.x, lodSize.y, 0, RenderTextureFormat.ARGBHalf);
            dst.filterMode = FilterMode.Bilinear;
            dst.wrapMode = TextureWrapMode.Clamp;
            dst.enableRandomWrite = true;
            dst.Create();
            return dst;
        }
        public static RenderTexture CreateRT(Texture src, int lod) {
            var size = new Vector2Int(src.width, src.height);
            var lodSize = LoD(size, lod);
            return CreateRT(lodSize);
        }
        public static RenderTexture CreateTempRT(Vector2Int nextSize) {
            var desc = new RenderTextureDescriptor(nextSize.x, nextSize.y, RenderTextureFormat.ARGBHalf, 0);
            desc.enableRandomWrite = true;

            var nextTmp = RenderTexture.GetTemporary(desc);
            nextTmp.filterMode = FilterMode.Bilinear;
            nextTmp.wrapMode = TextureWrapMode.Clamp;
            nextTmp.Create();
            return nextTmp;
        }
        public static void ReleaseTempRT(RenderTexture tex) {
            RenderTexture.ReleaseTemporary(tex);
        }
        public static void Swap(ref RenderTexture tmp0, ref RenderTexture tmp1) {
            var t = tmp0;
            tmp0 = tmp1;
            tmp1 = t;
        }
        public static Vector2Int LoD(int width, int height, int lod = 0) {
            return new Vector2Int(Mathf.Max(1, width >> lod), Mathf.Max(1, height >> lod));
        }
        public static Vector2Int LoD(Vector2Int size, int lod = 0) {
            return LoD(size.x, size.y, lod);
        }
        #endregion
    }
}
