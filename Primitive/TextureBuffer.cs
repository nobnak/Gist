using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Primitive {

    public interface ITextureBuffer<out T> {
		IBuffer2D<T> Buffer { get; }
		Vector2Int Size { get; }
		T this[int x, int y] { get; }
		T this[Vector2 uv] { get; }
	}

	public class ConstantBuffer<T> : ITextureBuffer<T> {
		public readonly T value;

		public ConstantBuffer(T value) {
			this.value = value;
		}

		#region interface
		public T this[Vector2 uv] => value;
		public T this[int x, int y] => value;
		public IBuffer2D<T> Buffer => throw new System.NotImplementedException();
		public Vector2Int Size => Vector2Int.one;
		#endregion
	}

	public abstract class TextureBuffer<T> : ITextureBuffer<T> {

		public delegate T InterpolateFunc(T v00, T v01, T v10, T v11, float s, float t);

		protected IBuffer2D<T> buffer;

		public TextureBuffer(IBuffer2D<T> buffer) {
			this.buffer = buffer;
		}

		public virtual IBuffer2D<T> Buffer { get => buffer; }
		public virtual T this[int x, int y] {
			get => buffer[x, y];
		}

		public virtual Vector2Int Size {
			get => buffer.Size;
		}

		public abstract T this[Vector2 uv] { get; }
		public virtual T Interpolate(Vector2 uv, InterpolateFunc f) {
			CrossScale(
				uv.x, uv.y, 
				out int x0, out int y0, out int x1, out int y1, 
				out float t, out float s);
			return f(
				buffer[x0, y0],
				buffer[x0, y1],
				buffer[x1, y0],
				buffer[x1, y1],
				s, t);
		}
		public virtual void CrossScale(float uvx, float uvy, out int x0, out int y0, out int x1, out int y1, out float t, out float s) {
			var size = buffer.Size;
			var x = size.x * uvx;
			var y = size.y * uvy;
			x0 = Mathf.FloorToInt(x);
			y0 = Mathf.FloorToInt(y);
			x1 = x0 + 1;
			y1 = y0 + 1;
			t = x1 - x;
			s = y1 - y;
		}
	}

	public class FloatTexture : TextureBuffer<float> {

		protected InterpolateFunc f;

		public FloatTexture(IBuffer2D<float> buffer, InterpolateFunc f)
			: base(buffer) {
			this.f = f;
		}
		public FloatTexture(IBuffer2D<float> buffer)
			: this(buffer, TextureBufferUtil.Bilinear) { }

		public override float this[Vector2 uv] {
			get => Interpolate(uv, f);
		}
	}

	public static class TextureBufferUtil {
		public static Color Bilinear(Color v00, Color v01, Color v10, Color v11, float s, float t) {
			return t * (s * v00 + (1f - s) * v01)
				+ (1f - t) * (s * v10 + (1f - s) * v11);
		}
		public static Vector4 Bilinear(Vector4 v00, Vector4 v01, Vector4 v10, Vector4 v11, float s, float t) {
			return t * (s * v00 + (1f - s) * v01)
				+ (1f - t) * (s * v10 + (1f - s) * v11);
		}
		public static float Bilinear(float v00, float v01, float v10, float v11, float s, float t) {
			return t * (s * v00 + (1f - s) * v01)
				+ (1f - t) * (s * v10 + (1f - s) * v11);
		}
	}
}
