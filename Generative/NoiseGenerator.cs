using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nobnak.Gist;

namespace nobnak.Gist.Generative {

    [System.Serializable]
    public class NoiseGenerator {
        [Range(0f, 2f)]
        public float gain = 1f;

        public NoiseGenerator(float gain) {
            this.gain = gain;
        }

        public float Noise(float x, float y, float t) {
            return Noise (x, y, t, 1f, 1f);
        }
        public float Noise(float x, float y, float t, float globalGain, float globalExpo) {
            var v = (globalGain * gain) * (float)SimplexNoise.Noise (x, y, t);
            return v;
        }
    }

}
