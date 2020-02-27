using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.GLTools {
    public class Copy : System.IDisposable {

        public enum Pass { Default = 0 }

        public const string PATH = "Copy";
        public static readonly int P_LocalMat = Shader.PropertyToID("_LocalMat");

        protected Material mat;

        public Copy() {
            mat = new Material(Resources.Load<Shader>(PATH));
        }

        #region interface
        #region IDisposable
        public void Dispose() {
            if (mat != null) {
                mat.DestroySelf();
                mat = null;
            }
        }
        #endregion

        public void Blit(Texture src, RenderTexture dst,
            float width = 1f,
            float height = 1f,
            float woffset = 0f,
            float hoffset = 0f
            ) {
            var localMat = new Vector4(width, height, woffset, hoffset);
            mat.SetVector(P_LocalMat, localMat);

            Graphics.Blit(src, dst, mat, (int)Pass.Default);
        }

        #endregion
    }
}
