using UnityEngine;
using System.Collections;

namespace nobnak.Gist {
    
    public static class MortonCodeInt {
        public const int STRIDE_BITS = 10;
        public const int STRIDE_LIMIT = 1 << STRIDE_BITS;
        public const int STRIDE_MASK = STRIDE_LIMIT - 1;

		public const int INT_MAX = (1 << STRIDE_BITS) - 1;
        public const int INT_MIN = 0;

        static readonly int[] MORTON_X = X2Morton (new int[STRIDE_LIMIT], 0);
        static readonly int[] MORTON_Y = X2Morton (new int[STRIDE_LIMIT], 1);
        static readonly int[] MORTON_Z = X2Morton (new int[STRIDE_LIMIT], 2);

		public static int Encode(float x, float y, float z) {
			return Encode ((int)(x * INT_MAX), (int)(y * INT_MAX), (int)(z * INT_MAX));
		}
		public static int Encode(int x, int y, int z) {
			x = Clamp (x);
			y = Clamp (y);
			z = Clamp (z);
			return MORTON_X[x & STRIDE_MASK] | MORTON_Y[y & STRIDE_MASK] | MORTON_Z[z & STRIDE_MASK];
        }

		static int Clamp(int x) {
			return (x < INT_MIN ? INT_MIN : (x <= INT_MAX ? x : INT_MAX));
		}
        static int[] X2Morton(int[] mortons, int offset) {
            for (var i = 0; i < mortons.Length; i++)
                mortons[i] = X2Morton (i) << offset;
            return mortons;
        }
        static int X2Morton(int i) {
            var m = 0;
            for (var j = 0; j < STRIDE_BITS; j++) {
                var b = (i >> j) & 1;
                m |= b << (3 * j);
            }
            return (int)m;
        }
    }
}