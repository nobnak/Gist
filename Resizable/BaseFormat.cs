using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace nobnak.Gist.Resizable {

	[System.Serializable]
	public abstract class BaseFormat {
		public TextureWrapMode wrapMode;
		public FilterMode filterMode;
		public int anisoLevel;
	}

	[System.Serializable]
	public abstract class BaseFormat<TextureType> : BaseFormat where TextureType : Texture {

		public BaseFormat() {
			Reset();
		}

		public override string ToString() {
			var tmp = new StringBuilder();
			tmp.AppendFormat("wrapMode={0}, ", wrapMode);
			tmp.AppendFormat("filterMode={0}, ", filterMode);
			tmp.AppendFormat("anisoLevel={0}, ", anisoLevel);
			return tmp.ToString();
		}

		public virtual void Reset() {
			filterMode = FilterMode.Bilinear;
			wrapMode = TextureWrapMode.Clamp;
			anisoLevel = 0;
		}

		public abstract TextureType CreateTexture(int width, int height);
		public abstract TextureType GetTexture(int width, int height);
		public virtual void ApplyToNew(TextureType tex) {
			ApplyToExisting(tex);
		}
		public virtual void ApplyToExisting(TextureType tex) {
			tex.filterMode = filterMode;
			tex.wrapMode = wrapMode;
			tex.anisoLevel = anisoLevel;
		}

		public static int ParseAntiAliasing(int antiAliasing) {
			if (antiAliasing > 0)
				return antiAliasing;
			else if (antiAliasing == 0)
				return 1;
			else
				return QualitySettings.antiAliasing;
		}
	}
}