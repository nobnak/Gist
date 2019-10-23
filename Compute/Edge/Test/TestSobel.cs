using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Compute.Edge {

    [ExecuteAlways]
    public class TestSobel : MonoBehaviour {

        public enum OutputMode { ___ = 0, Monochrome }

        public static readonly int P_BLEND = Shader.PropertyToID("_Blend");

        [SerializeField]
        protected Material mat;
        [SerializeField]
        protected OutputMode outputMode;
        [SerializeField]
        [Range(0f, 1f)]
        protected float blend = 0f;

        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            mat.shaderKeywords = null;
            if (outputMode != OutputMode.___)
                mat.EnableKeyword(outputMode.ToString());

            mat.SetFloat(P_BLEND, blend);

            Graphics.Blit(source, destination, mat);
        }
    }
}
