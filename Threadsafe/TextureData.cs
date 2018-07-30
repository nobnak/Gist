using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.ThreadSafe {

	public interface ITextureData<T> {
		T this[int x, int y] { get; }

		T this[float nx, float ny] { get; }
		T this[Vector2 uv] { get; }

		Vector2Int Size { get; }

		event System.Action<ITextureData<T>> OnLoad;
	}

	public abstract class BaseTextureData<T> : ITextureData<T> {

		public event Action<ITextureData<T>> OnLoad;

		protected Vector2Int size;
		protected T[] pixels;

		public BaseTextureData(T[] pixels, Vector2Int size) {
			Load(pixels, size);
		}

		#region abstract
		protected abstract T Bilinear(T v00, T v01, T v10, T v11, float s, float t);
		#endregion

		#region ITextureData
		public virtual T this[int x, int y] {
			get {
				lock (this) {
					Round(ref x, ref y);
					return GetPixelDirect(x, y);
				}
			}
			set {
				lock (this) {
					Round(ref x, ref y);
					SetPixelDirect(x, y, value);
				}
			}
		}

		public virtual T this[float nx, float ny] {
			get {
				lock (this) {
					int x0, y0, x1, y1;
					float t, s;
					Bridge(nx, ny, out x0, out y0, out x1, out y1, out t, out s);
					Round(ref x0, ref y0);
					Round(ref x1, ref y1);
					return Bilinear(
						GetPixelDirect(x0, y0),
						GetPixelDirect(x0, y1),
						GetPixelDirect(x1, y0),
						GetPixelDirect(x1, y1),
						s, t);
				}
			}
		}

		public virtual T this[Vector2 uv] {
			get { return this[uv.x, uv.y]; }
		}

		public virtual Vector2Int Size { get { return size; } }

		public virtual void Load(T[] pixels, Vector2Int size) {
			lock (this) {
				this.pixels = pixels;
				this.size = size;
			}
			if (OnLoad != null)
				OnLoad(this);
		}
		#endregion

		#region private
		protected T GetPixelDirect(int x, int y) {
			return pixels[GetLinearIndex(x, y)];
		}
		protected void SetPixelDirect(int x, int y, T c) {
			pixels[GetLinearIndex(x, y)] = c;
		}

		protected void Bridge(float nx, float ny, out int x0, out int y0, out int x1, out int y1, out float t, out float s) {
			var x = size.x * nx;
			var y = size.y * ny;
			x0 = Mathf.FloorToInt(x);
			y0 = Mathf.FloorToInt(y);
			x1 = x0 + 1;
			y1 = y0 + 1;
			t = x1 - x;
			s = y1 - y;
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

	public class ColorTextureData : BaseTextureData<Color> {
		public ColorTextureData(Color[] pixels, Vector2Int size) : base(pixels, size) {}

		#region BasetextureData
		protected override Color Bilinear(Color v00, Color v01, Color v10, Color v11, float s, float t) {
			return t * (s * v00 + (1f - s) * v01)
				+ (1f - t) * (s * v10 + (1f - s) * v11);
		}
		#endregion

		#region static
		public static implicit operator ColorTextureData(Texture2D tex) {
			return new ColorTextureData(
				tex.GetPixels(),
				new Vector2Int(tex.width, tex.height));
		}

		public static ColorTextureData CreateConstant(Color color) {
			return new ColorTextureData(
				new Color[] { color },
				new Vector2Int(1, 1));
		}
		#endregion
	}

	public class FloatTextureData : BaseTextureData<float> {
		public FloatTextureData(float[] pixels, Vector2Int size) : base(pixels, size) {}

		#region BasetextureData
		protected override float Bilinear(float v00, float v01, float v10, float v11, float s, float t) {
			return t * (s * v00 + (1f - s) * v01)
				+ (1f - t) * (s * v10 + (1f - s) * v11);
		}
		#endregion
	}

	public class FloatFilter : ITextureData<float> {
		public event Action<ITextureData<float>> OnLoad;

		protected int index;
		protected ColorTextureData tex;

		public FloatFilter(ColorTextureData tex, int index) {
			this.tex = tex;
			this.index = index;
		}

		public float this[Vector2 uv] {
			get { return tex[uv][index]; }
		}
		public float this[int x, int y] {
			get { return tex[x, y][index]; }
		}

		public float this[float nx, float ny] {
			get { return tex[nx, ny][index]; }
		}
		public Vector2Int Size { get { return tex.Size; } }
	}
}
