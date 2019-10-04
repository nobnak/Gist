#pragma warning disable 0067

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace nobnak.Gist.ThreadSafe {

	public interface ITextureData<T> : System.IDisposable {
		T this[int x, int y] { get; }

		T this[float nx, float ny] { get; }
		T this[Vector2 uv] { get; }

		Vector2Int Size { get; }

		System.Func<float, float, T> Interpolation { get; set; }

		event System.Action<ITextureData<T>> OnLoad;
	}

	public abstract class BaseTextureData<T> : ITextureData<T> {
		public delegate T BilinearFunc(T v00, T v01, T v10, T v11, float s, float t);
		public event System.Action<ITextureData<T>> OnLoad;

		protected Vector2Int size;
		protected Vector2 uvToIndex;
		protected System.Func<float, float, T> interpolation;

		public System.Func<float, float, T> Interpolation {
			get { return interpolation; }
			set {
				interpolation = (value ?? PointInterpolation);
			}
		}

		public BaseTextureData(Vector2Int size, System.Func<float, float, T> interpolation) {
			this.Size = size;
			this.Interpolation = interpolation;
		}
		public BaseTextureData(Vector2Int size) : this(size, null) { }

		#region abstract
		protected abstract T GetPixelDirect(int x, int y);
		protected abstract void SetPixelDirect(int x, int y, T c);
		#endregion

		#region static
		public static Color Bilinear(Color v00, Color v01, Color v10, Color v11, float s, float t) {
			return t * (s * v00 + (1f - s) * v01)
				+ (1f - t) * (s * v10 + (1f - s) * v11);
		}
		public static float Bilinear(float v00, float v01, float v10, float v11, float s, float t) {
			return t * (s * v00 + (1f - s) * v01)
				+ (1f - t) * (s * v10 + (1f - s) * v11);
		}
		public T PointInterpolation(float nx, float ny) {
			var x = Mathf.RoundToInt(nx * uvToIndex.x);
			var y = Mathf.RoundToInt(ny * uvToIndex.y);
			ClampPixelPos(ref x, ref y);
			return GetPixelDirect(x, y);
		}
		public System.Func<float, float, T> GenerateInterpolation(BilinearFunc bilinear) {
			return (float nx, float ny) => {
				int x0, y0, x1, y1;
				float t, s;
				Bridge(nx, ny, out x0, out y0, out x1, out y1, out t, out s);
				ClampPixelPos(ref x0, ref y0);
				ClampPixelPos(ref x1, ref y1);
				return bilinear(
					GetPixelDirect(x0, y0),
					GetPixelDirect(x0, y1),
					GetPixelDirect(x1, y0),
					GetPixelDirect(x1, y1),
					s, t);
			};
		}
		#endregion
		#region IDisposable
		public abstract void Dispose();
		#endregion

		#region ITextureData
		public virtual T this[int x, int y] {
			get {
				lock (this) {
					ClampPixelPos(ref x, ref y);
					return GetPixelDirect(x, y);
				}
			}
			set {
				lock (this) {
					ClampPixelPos(ref x, ref y);
					SetPixelDirect(x, y, value);
				}
			}
		}

		public virtual T this[float nx, float ny] {
			get {
				lock (this) {
					return interpolation(nx, ny);
				}
			}
		}

		public virtual T this[Vector2 uv] {
			get { return this[uv.x, uv.y]; }
		}

		public virtual Vector2Int Size {
			get { return size; }
			set {
				size = value;
				uvToIndex = new Vector2(value.x - 1, value.y - 1);
			}
		}
		#endregion

		#region private
		protected void NotifyOnLoad() {
			if (OnLoad != null)
				OnLoad.Invoke(this);
		}

		protected void Bridge(float nx, float ny, out int x0, out int y0, out int x1, out int y1, out float t, out float s) {
			var x = uvToIndex.x * nx;
			var y = uvToIndex.y * ny;
			x0 = Mathf.FloorToInt(x);
			y0 = Mathf.FloorToInt(y);
			x1 = x0 + 1;
			y1 = y0 + 1;
			t = x1 - x;
			s = y1 - y;
		}
		protected void ClampPixelPos(ref int x, ref int y) {
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
	public class ListTextureData<T> : BaseTextureData<T> {
		protected IList<T> pixels;

		public ListTextureData(IList<T> pixels, Vector2Int size) : base(size) {
			if (pixels.Count < (size.x * size.y))
				Debug.LogWarningFormat("Size mismatch : pixels={0} size={1}", pixels.Count, size);
			Load(pixels);
		}

		#region public
		public virtual void Load(IList<T> pixels) {
			lock (this) {
				this.pixels = pixels;
			}
			NotifyOnLoad();
		}
		#endregion
		#region IDisposable
		public override void Dispose() {
			if (pixels != null) {
				pixels = null;
			}
		}
		#endregion
		#region private
		protected override T GetPixelDirect(int x, int y) {
			var i = GetLinearIndex(x, y);
			return pixels[i];
		}
		protected override void SetPixelDirect(int x, int y, T c) {
			pixels[GetLinearIndex(x, y)] = c;
		}
		#endregion
	}

	public class ColorTextureData : ListTextureData<Color> {
		public ColorTextureData(Color[] pixels, Vector2Int size) : base(pixels, size) {
			this.Interpolation = GenerateInterpolation(Bilinear);
		}

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

	public class FloatTextureData : ListTextureData<float> {
		public FloatTextureData(float[] pixels, Vector2Int size) : base(pixels, size) {
			Interpolation = GenerateInterpolation(Bilinear);
		}

		#region static
		public static FloatTextureData CreateConstant(float v) {
			return new FloatTextureData(
				new float[] { v },
				new Vector2Int(1, 1));
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

		#region public
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
		public Func<float, float, float> Interpolation { get; set; }
		#endregion
		#region IDisposable
		public void Dispose() {
			if (tex != null) {
				tex.Dispose();
				tex = null;
			}
		}
		#endregion
	}

    public class FuncFilter<T> : ITextureData<T> {
        protected ITextureData<T> source;
        protected System.Func<T, T> func;

        public FuncFilter(ITextureData<T> source, System.Func<T, T> func) {
            this.func = func;
            this.source = source;
        }

        #region interface
        public T this[Vector2 uv] => func(source[uv]);

        public T this[int x, int y] => func(source[x, y]);

        public T this[float nx, float ny] => func(source[nx, ny]);

        public Vector2Int Size => source.Size;

        public Func<float, float, T> Interpolation {
            get => source.Interpolation;
            set => source.Interpolation = value;
        }

        public event Action<ITextureData<T>> OnLoad;

        public void Dispose() {
        }
        #endregion
    }
}
