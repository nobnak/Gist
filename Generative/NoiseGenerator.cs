using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nobnak.Gist;

namespace nobnak.Gist.Generative {

    [System.Serializable]
    public class NoiseGenerator {
        [Range(0f, 2f)]
        public float gain = 1f;

		#region constructors
		public NoiseGenerator(float gain) {
            this.gain = gain;
        }
		public NoiseGenerator() : this(1f) { }
		public static implicit operator NoiseGenerator(float gain) {
			return new NoiseGenerator(gain);
		}
		#endregion

		public float Noise(float x, float y, float t) {
            return _Noise (gain, x, y, t, 1f, 1f);
        }
        public float Noise(float x, float y, float t, float globalGain, float globalExpo) {
			return _Noise(gain, x, y, t, globalGain, globalExpo);
        }

		#region static
		public static float _Noise(float gain, float x, float y, float t) {
			return _Noise(gain, x, y, t, 1f, 1f);
		}
		public static float _Noise(float gain, float x, float y, float t, float globalGain, float globalExpo) {
			var v = (globalGain * gain) * (float)SimplexNoise.Noise(x, y, t);
			return v;
		}
		#endregion
	}

}
