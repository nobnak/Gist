using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.Int {
    
    public static class IntExtension {

		public static int SmallestPowerOfTwoGreaterThan(this int n) {
			if (n <= 0)
				return 0;

			--n;
			n |= n >> 1;
			n |= n >> 2;
			n |= n >> 4;
			n |= n >> 8;
			n |= n >> 16;
			return ++n;
		}
		public static uint SmallestPowerOfTwoGreaterThan(this uint n) {
			if (n <= 0)
				return 0;

			--n;
			n |= n >> 1;
			n |= n >> 2;
			n |= n >> 4;
			n |= n >> 8;
			n |= n >> 16;
			return ++n;
		}

		public static long SmallestPowerOfTwoGreaterThan(this long n) {
			if (n <= 0)
				return 0;

			--n;
			n |= n >> 1;
			n |= n >> 2;
			n |= n >> 4;
			n |= n >> 8;
			n |= n >> 16;
			n |= n >> 32;
			return ++n;
		}
		public static ulong SmallestPowerOfTwoGreaterThan(this ulong n) {
			if (n <= 0)
				return 0;

			--n;
			n |= n >> 1;
			n |= n >> 2;
			n |= n >> 4;
			n |= n >> 8;
			n |= n >> 16;
			n |= n >> 32;
			return ++n;
		}
	}
}
