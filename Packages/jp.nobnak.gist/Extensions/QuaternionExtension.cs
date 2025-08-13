using UnityEngine;
using System.Collections;

namespace nobnak.Gist.Extension.QuaternionExt {
	public static class QuaternionExtension {
        public const float ROTATION_DEG = 360f;
        public const float ROTATION_RAD = 2f * Mathf.PI;

		public static readonly Vector4 IDENTITY = new Vector4(0, 0, 0, 1);

		public static Vector4 ToVector4(this Quaternion q) {
			return new Vector4(q.x, q.y, q.z, q.w);
		}

        public static float UnitToDeg(this float u) {
            return u * ROTATION_DEG;
        }
        public static float UnitToRad(this float u) {
            return u * ROTATION_RAD;
        }
	}
}