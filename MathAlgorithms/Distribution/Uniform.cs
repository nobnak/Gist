using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Distribution.Uniform {

    public static class UniformExtension {

        public static float SUniform(this float d) {
            return Random.Range(-d, d);
        }

        public static float UUniform(this float d) {
            return Random.Range(0f, d);
        }
    }
}
