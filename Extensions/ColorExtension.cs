using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.ColorExt {

	public static class ColorExtension {

		public static Vector4 ToHSV(this Color c) {
			float h, s, v;
			Color.RGBToHSV(c, out h, out s, out v);
			return new Vector4(h, s, v, c.a);
		}
		public static Color FromHSV(this Vector4 v) {
			var c = Color.HSVToRGB(v.x, v.y, v.z);
			c.a = v.w;
			return c;
		}
		public static Color FromHSV(this Vector3 v) {
			return new Vector4(v.x, v.y, v.z, 1f).FromHSV();
		}
	}
}
