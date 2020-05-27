using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace nobnak.Gist.Primitive {

	public interface IFastBoudns2D {
		FastBounds2D GetBounds();
	}

	[StructLayout(LayoutKind.Explicit, Pack = 4)]
	public struct FastBounds2D : IFastBoudns2D {
		[FieldOffset(0)]
		public Vector2 Min;
		[FieldOffset(0)]
		public float min_x;
		[FieldOffset(4)]
		public float min_y;


		[FieldOffset(8)]
		public Vector2 Max;
		[FieldOffset(8)]
		public float max_x;
		[FieldOffset(12)]
		public float max_y;

		public FastBounds2D(
			float min_x, float min_y, 
			float max_x, float max_y) {
			Min = default(Vector2);
			Max = default(Vector2);

			this.min_x = min_x;
			this.min_y = min_y;

			this.max_x = max_x;
			this.max_y = max_y;
		}
		public FastBounds2D(Vector2 min, Vector2 max) :
			this(min.x, min.y, max.x, max.y) { }

		#region interface
		#region object
		public override string ToString() {
			return $"<{GetType().Name}:min={Min},max={Max}>";
		}
		#endregion

		#region IFastBoudns2D
		public FastBounds2D GetBounds() {
			return this;
		}
		#endregion

		public Vector2 Center {
			get {
				return new Vector3(
					0.5f * (min_x + max_x),
					0.5f * (min_y + max_y));
			}
		}
		public Vector2 Size {
			get {
				return new Vector3(
					max_x - min_x,
					max_y - min_y);
			}
		}
		public Vector2 Extents {
			get {
				return new Vector3(
					0.5f * (max_x - min_x),
					0.5f * (max_y - min_y));
			}
		}

		public bool Intersects(FastBounds2D b) {
			var gap =
				(max_x < b.min_x || b.max_x < min_x) ||
				(max_y < b.min_y || b.max_y < min_y);
			return !gap;
		}
		public bool Contains(Vector3 p) {
			var gap =
				(max_x < p.x || p.x < min_x) ||
				(max_y < p.y || p.y < min_y);
			return !gap;
		}
		public void Encapsulate(float px, float py) {
			min_x = Mathf.Min(min_x, px);
			min_y = Mathf.Min(min_y, py);

			max_x = Mathf.Max(max_x, px);
			max_y = Mathf.Max(max_y, py);
		}
		public void Encapsulate(Vector2 p) {
			Encapsulate(p.x, p.y);
		}
		public void Encapsulate(FastBounds2D fb) {
			Encapsulate(fb.min_x, fb.min_y);
			Encapsulate(fb.max_x, fb.max_y);
		}
		public Vector2 Sample() {
			return new Vector2(
				Random.Range(min_x, max_x),
				Random.Range(min_y, max_y));
		}
		#endregion

		#region static
		public static implicit operator FastBounds2D(Rect b) {
			return new FastBounds2D(b.min, b.max);
		}
		public static explicit operator Rect(FastBounds2D fb) {
			return new Rect(fb.Center, fb.Size);
		}

		public static explicit operator FastBounds2D(FastBounds fb3) {
			return new FastBounds2D(fb3.min_x, fb3.min_y, fb3.max_x, fb3.max_y);
		}
		public static explicit operator FastBounds(FastBounds2D fb2) {
			return new FastBounds(fb2.min_x, fb2.min_y, 0f, fb2.max_x, fb2.max_y, 0f);
		}
		public static FastBounds2D operator +(FastBounds2D fb2, Vector2 offset) {
			var of_x = offset.x;
			var of_y = offset.y;
			return new FastBounds2D(
				fb2.min_x + of_x, fb2.min_y + of_y,
				fb2.max_x + of_x, fb2.max_y + of_y
				);
		}
		#endregion
	}
}
