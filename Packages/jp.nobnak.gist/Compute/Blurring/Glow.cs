using nobnak.Gist.Extensions.GPUExt;
using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Compute.Blurring {

    public class Glow : System.IDisposable {

        public enum ShaderPass { Threshold = 0, Additive }

        public const string PATH = "GlowFilter";

        public static readonly int P_MAIN_TEX = Shader.PropertyToID("_MainTex");
        public static readonly int P_BLUR_TEX = Shader.PropertyToID("_BlurredTex");
        public static readonly int P_INTENSITY = Shader.PropertyToID("_Intensity");
        public static readonly int P_THRESHOLD = Shader.PropertyToID("_Threshold");

        protected Material mat;

        public Glow() {
            mat = new Material(Resources.Load<Shader>(PATH));
        }

        #region interface
        #region IDisposable
        public void Dispose() {
            mat.DestroySelf();
        }
        #endregion

        public void Threshold(Texture src, RenderTexture dst, float threshold) {
            var vthresh = new Vector4(threshold, 1f / Mathf.Clamp(1f - threshold, 0.1f, 1f), 0f, 0f);
            mat.SetVector(P_THRESHOLD, vthresh);
            Graphics.Blit(src, dst, mat, (int)ShaderPass.Threshold);
        }
        public void Apply(Texture src, Texture blurred, RenderTexture dst, float intensity) {
            mat.SetTexture(P_BLUR_TEX, blurred);
            mat.SetFloat(P_INTENSITY, intensity);
            Graphics.Blit(src, dst, mat, (int)ShaderPass.Additive);
        }
        #endregion
    }
}
