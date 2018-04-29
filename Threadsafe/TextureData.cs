using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.ThreadSafe {

	public class TextureData {

		protected Vector2Int size;
		protected Color[] pixels;

		#region public
		public Color GetPixel(int x, int y) {
			lock (this) {
				Round(ref x, ref y);
				return GetPixelDirect(x, y);
			}
		}
		public Color GetPixelBilinear(float nx, float ny) {
			lock (this) {
				var x = size.x * nx;
				var y = size.y * ny;
				var x0 = Mathf.FloorToInt(x);
				var y0 = Mathf.FloorToInt(y);
				var x1 = x0 + 1;
				var y1 = y0 + 1;
				var t = x1 - x;
				var s = y1 - y;
				Round(ref x0, ref y0);
				Round(ref x1, ref y1);
				return t * (s * GetPixelDirect(x0, y0) + (1f - s) * GetPixelDirect(x0, y1))
					+ (1f - t) * (s * GetPixelDirect(x1, y0) + (1f - s) * GetPixelDirect(x1, y1));
			}
		}
		public void SetPixels(Color[] pixels, int width, int height) {
			lock (this) {
				this.pixels = pixels;
				this.size = new Vector2Int(width, height);
			}
		}
		#endregion

		#region static
		public static implicit operator TextureData(Texture2D tex) {
			return new TextureData() {
				size = new Vector2Int(tex.width, tex.height),
				pixels = tex.GetPixels()
			};
		}

		public static TextureData CreateConstant(Color color) {
			return new TextureData() {
				size = new Vector2Int(1, 1),
				pixels = new Color[] { color }
			};
		}
		#endregion

		#region private
		protected Color GetPixelDirect(int x, int y) {
			return pixels[GetLinearIndex(x, y)];
		}
		protected void Round(ref int x, ref int y) {
			x = (x < 0 ? 0 : (x < size.x ? x : size.x - 1));
			y = (y < 0 ? 0 : (y < size.y ? y : size.y - 1));
		}
		protected int GetLinearIndex(int x, int y) {
			return x + y * size.x;
		}
		protected int GetLinearIndex(Vector2Int index) {
			return GetLinearIndex(index.x, index.y);
		}
		#endregion
	}
}
