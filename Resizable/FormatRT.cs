using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace nobnak.Gist.Resizable {

	[System.Serializable]
	public class FormatRT : BaseFormat<RenderTexture> {

		public RenderTextureReadWrite readWrite;
		public RenderTextureFormat textureFormat;
		public int antiAliasing;
		public int depth;

		public bool autoGenerateMips;
		public bool useMipMap;

		public bool useDynamicScale;

		public FormatRT() {
			Reset();
		}

		public override string ToString() {
			var tmp = new StringBuilder();
			tmp.AppendFormat("readWrite={0}, ", readWrite);
			tmp.AppendFormat("textureFormat={0}, ", textureFormat);
			tmp.AppendFormat("antiAliasing={0}, ", antiAliasing);
			tmp.AppendFormat("depth={0}, ", depth);
			tmp.AppendFormat("autoGenerateMips={0}, ", autoGenerateMips);
			tmp.AppendFormat("useMipMap={0}, ", useMipMap);
			tmp.AppendFormat("useDynamicScale={0}, ", useDynamicScale);
			tmp.Append(base.ToString());
			return tmp.ToString();
		}

		public override void Reset() {
			base.Reset();

			depth = 24;
			antiAliasing = 1;
			textureFormat = RenderTextureFormat.ARGB32;
			readWrite = RenderTextureReadWrite.Default;

			autoGenerateMips = false;
			useMipMap = false;

			useDynamicScale = false;
		}

		public override RenderTexture CreateTexture(int width, int height) {
			var tex = new RenderTexture(width, height, 
				depth, textureFormat, readWrite);
			ApplyToNew(tex);
			return tex;
		}
		public override RenderTexture GetTexture(int width, int height) {
			var tex = RenderTexture.GetTemporary(
				width, height, depth, 
				textureFormat, readWrite, 
				ParseAntiAliasing(antiAliasing));
			ApplyToExisting(tex);
			return tex;
		}
		public override void ApplyToNew(RenderTexture tex) {
			tex.autoGenerateMips = autoGenerateMips;
			tex.useMipMap = useMipMap;
			ApplyToExisting(tex);
		}
		public override void ApplyToExisting(RenderTexture tex) {
			base.ApplyToExisting(tex);
			tex.antiAliasing = ParseAntiAliasing(antiAliasing);
			tex.useDynamicScale = useDynamicScale;
		}
	}
}