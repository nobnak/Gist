using System.Collections;
using System.Collections.Generic;
using System.Text;
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

		public bool useDynamicScale;

		public int anisoLevel;

		public Format() {
			Reset();
		}

		public override string ToString() {
			var tmp = new StringBuilder();
			tmp.AppendFormat("readWrite={0}, ", readWrite);
			tmp.AppendFormat("textureFormat={0}, ", textureFormat);
			tmp.AppendFormat("wrapMode={0}, ", wrapMode);
			tmp.AppendFormat("filterMode={0}, ", filterMode);
			tmp.AppendFormat("antiAliasing={0}, ", antiAliasing);
			tmp.AppendFormat("depth={0}, ", depth);
			tmp.AppendFormat("autoGenerateMips={0}, ", autoGenerateMips);
			tmp.AppendFormat("useMipMap={0}, ", useMipMap);
			tmp.AppendFormat("useDynamicScale={0}, ", useDynamicScale);
			tmp.AppendFormat("anisoLevel={0}, ", anisoLevel);
			return tmp.ToString();
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

			useDynamicScale = false;

			anisoLevel = 0;
		}

		public RenderTexture CreateTexture(int width, int height) {
			var tex = new RenderTexture(width, height, 
				depth, textureFormat, readWrite);
			ApplyToNew(tex);
			return tex;
		}
		public RenderTexture GetTexture(int width, int height) {
			var tex = RenderTexture.GetTemporary(
				width, height, depth, 
				textureFormat, readWrite, 
				ParseAntiAliasing(antiAliasing));
			ApplyToExisting(tex);
			return tex;
		}
		public void ApplyToNew(RenderTexture tex) {
			tex.autoGenerateMips = autoGenerateMips;
			tex.useMipMap = useMipMap;
			ApplyToExisting(tex);
		}
		public  void ApplyToExisting(RenderTexture tex) {
			tex.filterMode = filterMode;
			tex.wrapMode = wrapMode;
			tex.antiAliasing = ParseAntiAliasing(antiAliasing);
			tex.useDynamicScale = useDynamicScale;
			tex.anisoLevel = anisoLevel;
		}

		public static int ParseAntiAliasing(int antiAliasing) {
			return (antiAliasing > 0 ? antiAliasing : QualitySettings.antiAliasing);
		}
	}
}