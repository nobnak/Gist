using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {

    public static class MultiOctaveNoise {

        public static float Noise(float x, float y, uint octaves, float atten = .5f) {
            float wsum = 0;
            float w = 1;
            float freq = 1;
            double v = 0;


            for (uint i = 0; i < octaves; i++) {
                v += w * SimplexNoise.Noise(x * freq, y * freq);

                wsum += w;
                w *= atten;
                freq *= 2;
            }
            return (float)(v / wsum);
        }
        public static float Noise(Vector2 uv, uint octaves, float atten = .5f) {
            return Noise(uv.x, uv.y, octaves, atten);
        }

        public static float Noise(float x, float y, float z, uint octaves, float atten = .5f) {
            float wsum = 0;
            float w = 1;
            float freq = 1;
            double v = 0;

            for (uint i = 0; i < octaves; i++) {
                v += w * SimplexNoise.Noise(x * freq, y * freq, z * freq);

                wsum += w;
                w *= atten;
                freq *= 2;
            }
            return (float)(v / wsum);
        }
        public static float Noise(Vector3 uv, uint octaves, float atten = .5f) {
            return Noise(uv.x, uv.y, uv.z, octaves, atten);
        }
    }
}