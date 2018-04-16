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

		public bool autoGenerateMips;
		public bool useMipMap;

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

			autoGenerateMips = false;
			useMipMap = false;
		}

		public RenderTexture CreateTexture(int width, int height) {
			var tex = new RenderTexture(width, height, 
				depth, textureFormat, readWrite);
			ApplyTo(tex);
			return tex;
		}
		public void ApplyTo(RenderTexture tex) {
			tex.filterMode = filterMode;
			tex.wrapMode = wrapMode;
			tex.antiAliasing = ParseAntiAliasing(antiAliasing);
			tex.autoGenerateMips = autoGenerateMips;
			tex.useMipMap = useMipMap;

		}

		public static int ParseAntiAliasing(int antiAliasing) {
			return (antiAliasing > 0 ? antiAliasing : QualitySettings.antiAliasing);
		}
	}
}