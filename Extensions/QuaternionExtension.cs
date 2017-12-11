using UnityEngine;
using System.Collections;

namespace nobnak.Gist.Extension.QuaternionExt {
	public static class QuaternionExtension {
		public static readonly Vector4 IDENTITY = new Vector4(0, 0, 0, 1);

		public static Vector4 Convert(this Quaternion q) {
			return new Vector4(q.x, q.y, q.z, q.w);
		}
	}
}