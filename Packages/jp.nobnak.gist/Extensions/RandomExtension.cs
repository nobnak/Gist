using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.RandomExt {

    public static class RandomFunction {

        public static float RandomSNorm() {
            return Random.Range(-1f, 1f);
        }
        public static float Perturb(this float scale) {
            return Random.Range(-scale, scale);
        }
    }
}
