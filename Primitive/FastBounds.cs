using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace nobnak.Gist.Primitive {

	public struct FastBounds {
		public float min_x;
		public float min_y;
		public float min_z;

		public float max_x;
		public float max_y;
		public float max_z;

		public FastBounds(
			float min_x, float min_y, float min_z, 
			float max_x, float max_y, float max_z) {
			this.min_x = min_x;
			this.min_y = min_y;
			this.min_z = min_z;

			this.max_x = max_x;
			this.max_y = max_y;
			this.max_z = max_z;
		}
		public FastBounds(Vector3 min, Vector3 max) :
			this(min.x, min.y, min.z, max.x, max.y, max.z) { }

		public Vector3 Center {
			get {
				return new Vector3(
					0.5f * (min_x + max_x),
					0.5f * (min_y + max_y),
					0.5f * (min_z + max_z));
			}
		}
		public Vector3 Size {
			get {
				return new Vector3(
					max_x - min_x,
					max_y - min_y,
					max_z - min_z);
			}
		}
		public Vector3 Extents {
			get {
				return new Vector3(
					0.5f * (max_x - min_x),
					0.5f * (max_y - min_y),
					0.5f * (max_z - min_z));
			}
		}
		public Vector3 Min {
			get { return new Vector3(min_x, min_y, min_z); }
		}
		public Vector3 Max {
			get { return new Vector3(max_x, max_y, max_z); }
		}

		public bool Intersects(FastBounds b) {
			var gap =
				(max_x < b.min_x || b.max_x < min_x) ||
				(max_y < b.min_y || b.max_y < min_y) ||
				(max_z < b.min_z || b.max_z < min_z);
			return !gap;
		}
		public bool Contains(Vector3 p) {
			var gap =
				(max_x < p.x || p.x < min_x) ||
				(max_y < p.y || p.y < min_y) ||
				(max_z < p.z || p.z < min_z);
			return !gap;
		}
		public void Encapsulate(float px, float py, float pz) {
			min_x = Mathf.Min(min_x, px);
			min_y = Mathf.Min(min_y, py);
			min_z = Mathf.Min(min_z, pz);

			max_x = Mathf.Max(max_x, px);
			max_y = Mathf.Max(max_y, py);
			max_z = Mathf.Max(max_z, pz);
		}
		public void Encapsulate(Vector3 p) {
			Encapsulate(p.x, p.y, p.z);
		}
		public void Encapsulate(FastBounds fb) {
			Encapsulate(fb.min_x, fb.min_y, fb.min_z);
			Encapsulate(fb.max_x, fb.max_y, fb.max_z);
		}

		public static implicit operator FastBounds(Bounds b) {
			return new FastBounds(b.min, b.max);
		}
		public static explicit operator Bounds(FastBounds fb) {
			return new Bounds(fb.Center, fb.Size);
		}
	}
}
