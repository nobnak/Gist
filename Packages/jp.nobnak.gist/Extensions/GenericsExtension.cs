using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.GenericExt {

	public static class GenericsExtension {

		public static void Swap<T>(ref T a, ref T b) {
			var tmp = a;
			a = b;
			b = tmp;
		}
	}
}
