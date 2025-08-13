using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nobnak.Gist.Pooling {

	public struct Tuple<T> : System.IEquatable<Tuple<T>> {
		public readonly T[] Values;

		public Tuple(params T[] values) {
			this.Values = values;
		}
		
		#region IEquatable
		public bool Equals(Tuple<T> other) {
			if (Values == null || other.Values == null)
				return false;

			if (Values.Length != other.Values.Length)
				return false;

			return Values.SequenceEqual(other.Values);
		}
		public override bool Equals(object obj) {
			return (obj is Tuple<T>) && Equals((Tuple<T>)obj);
		}
		#endregion

		#region override
		public override int GetHashCode() {
			var hashCode = -1924324773;
			foreach(var v in Values)
				hashCode = hashCode * -1521134295 + v.GetHashCode();
			return hashCode;
		}
		public override string ToString() {
			var buf = new StringBuilder(string.Format("<{0}:", typeof(Tuple<T>).Name));
			foreach (var v in Values)
				buf.AppendFormat("{0},", v);
			buf.Append(">");
			return buf.ToString();
		}

		public static bool operator ==(Tuple<T> tuple1, Tuple<T> tuple2) {
			return tuple1.Equals(tuple2);
		}
		public static bool operator !=(Tuple<T> tuple1, Tuple<T> tuple2) {
			return !(tuple1 == tuple2);
		}
		#endregion

		public T this[int index] {
			get { return Values[index]; }
		}
		public int Count {
			get { return Values.Length; }
		}
	}
}
