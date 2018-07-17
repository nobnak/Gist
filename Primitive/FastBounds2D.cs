using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace nobnak.Gist.Primitive {

	public struct FastBounds2D {
		public float min_x;
		public float min_y;

		public float max_x;
		public float max_y;

		public FastBounds2D(
			float min_x, float min_y, 
			float max_x, float max_y) {
			this.min_x = min_x;
			this.min_y = min_y;

			this.max_x = max_x;
			this.max_y = max_y;
		}
		public FastBounds2D(Vector2 min, Vector2 max) :
			this(min.x, min.y, max.x, max.y) { }

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
		public Vector2 Min {
			get { return new Vector2(min_x, min_y); }
		}
		public Vector2 Max {
			get { return new Vector2(max_x, max_y); }
		}

		public bool Intersects(FastBounds b) {
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

		public static implicit operator FastBounds2D(Rect b) {
			return new FastBounds2D(b.min, b.max);
		}
		public static explicit operator Rect(FastBounds2D fb) {
			return new Rect(fb.Center, fb.Size);
		}
	}
}
