using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Primitive { 

	public interface IBuffer2D<out T> : IEnumerable<T> {
		T this[int x, int y] { get; }
		Vector2Int Size { get; }
	}

	public class Buffer2D<T> : IBuffer2D<T> {

		public const int MIN_LENGTH = 4;
		public static readonly Vector2Int DEFAULT_SIZE = new Vector2Int(MIN_LENGTH, MIN_LENGTH);

		protected Vector2Int size;
		protected T[] values;

		public Buffer2D(Vector2Int size) {
			Resize(size);
		}
		public Buffer2D() : this(DEFAULT_SIZE) { }

		#region interface

		#region IEnumerable
		public IEnumerator<T> GetEnumerator() {
			return ((IEnumerable<T>)values).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion

		public virtual T this[int x, int y] {
			get =>  values[Linearize(x, y)];
			set => values[Linearize(x, y)] = value;
		}

		public virtual Vector2Int Size {
			get { return size; }
			set { Resize(value); }
		}

		public virtual void Resize(Vector2Int size) {
			if (size.x < MIN_LENGTH || size.y < MIN_LENGTH) {
				Debug.Log($"Size is too small : {size}");
				size.x = Mathf.Max(MIN_LENGTH, size.x);
				size.y = Mathf.Max(MIN_LENGTH, size.y);
			}
			if (this.size == size)
				return;

			this.size = size;
			this.values = new T[size.x * size.y];
		}
		public virtual void Clear(T defaultValue) {
			for (var i = 0; i < values.Length; i++)
				values[i] = defaultValue;
		}

		public virtual int ClampX(int x) {
			return (x < 0 ? 0 : (x < size.x ? x : size.x - 1));
		}
		public virtual int ClampY(int y) {
			return (y < 0 ? 0 : (y < size.y ? y : size.y - 1));
		}
		public virtual void ClampIndices(ref int x, ref int y) {
			x = ClampX(x);
			y = ClampY(y);
		}
		public virtual int Linearize(int x, int y) {
			return ClampX(x) + ClampY(y) * size.x;
		}

		#endregion
	}
}
