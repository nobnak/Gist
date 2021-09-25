using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace nobnak.Gist.CGIncludes.Wrappers {

    public static class Wireframe {
        public const float DEF_GAIN = 1.5f;
        public readonly static int P_Wireframe_Gain = Shader.PropertyToID("_Wireframe_Gain");
    
        public static void SetupWireframe(this Material mat, float gain = DEF_GAIN) {
            mat.SetFloat(P_Wireframe_Gain, gain);
        }
        public static void SetupWifeframe(this CommandBuffer cmd, float gain = DEF_GAIN) {
            cmd.SetGlobalFloat(P_Wireframe_Gain, gain);
        }
    }
}
