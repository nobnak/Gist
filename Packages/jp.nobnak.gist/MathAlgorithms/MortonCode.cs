using UnityEngine;
using System.Collections;

namespace nobnak.Gist {
    
    public static class MortonCode {
        public const int STRIDE_BITS = 8;
        public const int STRIDE_LIMIT = 1 << STRIDE_BITS;
        public const int STRIDE_MASK = STRIDE_LIMIT - 1;

        public const int INT_MAX = (1 << 21) - 1;
        public const int INT_MIN = 0;

        static readonly uint[] MORTON_X = X2Morton (new uint[STRIDE_LIMIT], 0);
        static readonly uint[] MORTON_Y = X2Morton (new uint[STRIDE_LIMIT], 1);
        static readonly uint[] MORTON_Z = X2Morton (new uint[STRIDE_LIMIT], 2);

        public static ulong Encode(int x, int y, int z) {
            return Encode ((uint)x, (uint)y, (uint)z);
        }
		public static ulong Encode(float x, float y, float z) {
			return Encode ((uint)(x * INT_MAX), (uint)(y * INT_MAX), (uint)(z * INT_MAX));
		}
        public static ulong Encode(uint x, uint y, uint z) {
			x = Clamp (x);
			y = Clamp (y);
			z = Clamp (z);

            ulong m = 0;
            for (var i = 2; i >= 0; i--) {
                m = (m << (3 * STRIDE_BITS))
                    | MORTON_X[(x >> (STRIDE_BITS * i)) & STRIDE_MASK]
                    | MORTON_Y[(y >> (STRIDE_BITS * i)) & STRIDE_MASK]
                    | MORTON_Z[(z >> (STRIDE_BITS * i)) & STRIDE_MASK];
            }
            return m;
        }

		static uint Clamp(uint x) {
			return (x < INT_MIN ? INT_MIN : (x <= INT_MAX ? x : INT_MAX));
		}
        static uint[] X2Morton(uint[] mortons, int offset) {
            for (var i = 0; i < mortons.Length; i++)
                mortons[i] = X2Morton (i) << offset;
            return mortons;
        }

        static uint X2Morton(int i) {
            var m = 0;
            for (var j = 0; j < STRIDE_BITS; j++) {
                var b = (i >> j) & 1;
                m |= b << (3 * j);
            }
            return (uint)m;
        }
    }
}