using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace nobnak.Gist.Extensions.Texture2DExt {

	public static class Texture2DExtension {

		public static Texture2D Create(this Texture2D tex,
			int width, int height, TextureFormat format,
			bool mipmap = false, bool linear = false) {

			tex.Destroy();
			tex = Create(width, height, format, mipmap, linear);
			return tex;
		}
		public static Texture2D Create(
			int width, int height, TextureFormat format,
			bool mipmap = false, bool linear = false) {

			var tex = new Texture2D(width, height, format, mipmap, linear);
			Debug.LogFormat("Create Texture2D : {0}", tex.SizeToString());

			return tex;
		}

		public static void Destroy(this Texture2D tex) {
			if (tex != null) {
				Debug.LogFormat("Destroy Texture2D : {0}", tex.SizeToString());
				ObjectExt.ObjectExtension.DestroySelf(tex);
				tex = null;
			}
		}

		public static string SizeToString(this Texture2D tex) {
			return string.Format("size={0}x{1}", tex.width, tex.height);
		}

		public static Vector2Int Size(this Texture tex) {
			return new Vector2Int(tex.width, tex.height);
		}
	}
}
