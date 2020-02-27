using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.GLTools {
    public static class Copy {

        public enum Pass { Default = 0 }

        public const string PATH = "Hidden_Copy";
        public static readonly int P_LocalMat = Shader.PropertyToID("_LocalMat");

        public static Material mat;

        #region interface
        public static Material LoadedMat {
            get {
                if (mat == null)
                    mat = Resources.Load<Material>(PATH);
                return mat;
            }
        }
        public static void DrawTexture(Texture src, RenderTexture dst,
            float width = 1f,
            float height = 1f,
            float woffset = 0f,
            float hoffset = 0f
            ) {
            var localMat = new Vector4(width, height, woffset, hoffset);
            LoadedMat.SetVector(P_LocalMat, localMat);

            Graphics.Blit(src, dst, LoadedMat, (int)Pass.Default);
        }

        #endregion
    }
}
