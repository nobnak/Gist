using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Compute.Edge {

    [ExecuteAlways]
    public class TestSobel : MonoBehaviour {

        public enum OutputMode { ___ = 0, Monochrome, Overlay }

        [SerializeField]
        protected Material mat;
        [SerializeField]
        protected OutputMode outputMode;

        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            mat.shaderKeywords = null;
            if (outputMode != OutputMode.___)
                mat.EnableKeyword(outputMode.ToString());

            Graphics.Blit(source, destination, mat);
        }
    }
}
