using nobnak.Gist.Events;
using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Compute.Blurring.Test {
    [ExecuteAlways]
    public class TestBlur : MonoBehaviour {

        public TextureEvent BlurredOnCreate;

        [SerializeField]
        protected Texture image;
        [SerializeField]
        [Range(0, 16)]
        protected int lod = 0;
        [SerializeField]
        [Range(0, 10)]
        protected int iterations = 1;

        protected Blur blur;
        protected RenderTexture blurred;

        #region unity
        void OnEnable() {
            blur = new Blur();
        }
        void OnDisable() {
            if (blur != null) {
                blur.Dispose();
                blur = null;
            }
            blurred.DestroySelf();
        }
        void Update() {
            if (image == null)
                return;

            if (blur.Render(image, ref blurred, iterations, lod))
                BlurredOnCreate.Invoke(blurred);
        }
        #endregion
    }
}
