using nobnak.Gist.Extensions.Texture2DExt;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace nobnak.Gist.Resizable {

	[System.Serializable]
	public class Format2D : BaseFormat<Texture2D> {
		
		public TextureFormat textureFormat;
		public bool useMipMap;
		public bool linear;

		public Format2D() {
			Reset();
		}
		public Format2D(
			TextureFormat textureFormat = TextureFormat.ARGB32,
			bool useMipMap = false,
			bool linear = false) {
			base.Reset();
			this.textureFormat = textureFormat;
			this.useMipMap = useMipMap;
			this.linear = linear;
		}

		public override string ToString() {
			var tmp = new StringBuilder();
			tmp.AppendFormat("textureFormat={0}, ", textureFormat);
			tmp.AppendFormat("useMipMap={0}, ", useMipMap);
			tmp.AppendFormat("linear={0}, ", linear);
			tmp.Append(base.ToString());
			return tmp.ToString();
		}

		public override void Reset() {
			base.Reset();
			textureFormat = TextureFormat.ARGB32;
			useMipMap = false;
			linear = false;
		}

		public override Texture2D CreateTexture(int width, int height) {
			var tex = Texture2DExtension.Create(width, height, textureFormat, useMipMap, linear);
			ApplyToNew(tex);
			return tex;
		}
		public override Texture2D GetTexture(int width, int height) {
			return null;
		}
		public override void ApplyToNew(Texture2D tex) {
			ApplyToExisting(tex);
		}
		public override void ApplyToExisting(Texture2D tex) {
			base.ApplyToExisting(tex);
		}
	}
}