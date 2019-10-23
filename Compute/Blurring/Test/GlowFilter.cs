using nobnak.Gist.Extensions.GPUExt;
using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Compute.Blurring {

    [ExecuteAlways]
    public class GlowFilter : MonoBehaviour {

        [SerializeField]
        protected int blurIterations = 0;
        [SerializeField]
        protected int blurLod = 2;
        [SerializeField]
        [Range(0f, 1f)]
        protected float glowThreshold = 0.5f;
        [SerializeField]
        protected float glowIntensity = 1f;

        protected Blur blur;
        protected Glow glow;
        protected RenderTexture blurred;

        #region unity
        void OnEnable() {
            blur = new Blur();
            glow = new Glow();
        }
        void OnDisable() {
            if (blur != null) {
                blur.Dispose();
                blur = null;
            }
            if (glow != null) {
                glow.Dispose();
                glow = null;
            }
            blurred.DestroySelf();
        }
        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            var w = source.width;
            var h = source.height;
            var threshTex = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGBHalf);

            glow.Threshold(source, threshTex, glowThreshold);
            blur.Render(threshTex, ref blurred, blurIterations, blurLod);
            glow.Apply(source, blurred, destination, glowIntensity);

            RenderTexture.ReleaseTemporary(threshTex);
        }
        #endregion
    }
}
