using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Extensions.MathExt {

	public static class MathExtension {

		public static Rect Multiply(this Matrix4x4 m, Rect r) {
			return new Rect(
				m.MultiplyPoint3x4(r.position),
				m.MultiplyVector(r.size));
		}
	}
}
