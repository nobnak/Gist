using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nobnak.Gist.Resizable {

	[System.Serializable]
	public class Format {

		public RenderTextureReadWrite readWrite;
		public RenderTextureFormat textureFormat;
		public TextureWrapMode wrapMode;
		public FilterMode filterMode;
		public int antiAliasing;
		public int depth;

		public Format() {
			Reset();
		}

		public void Reset() {
			depth = 24;
			antiAliasing = 0;
			filterMode = FilterMode.Bilinear;
			wrapMode = TextureWrapMode.Clamp;
			textureFormat = RenderTextureFormat.ARGB32;
			readWrite = RenderTextureReadWrite.Default;
		}
	}
}