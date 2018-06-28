using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		#endregion

		#region override
		public override int GetHashCode() {
			var hashCode = -1924324773;
			foreach(var v in Values)
				hashCode = hashCode * -1521134295 + v.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(Tuple<T> tuple1, Tuple<T> tuple2) {
			return tuple1.Equals(tuple2);
		}

		public static bool operator !=(Tuple<T> tuple1, Tuple<T> tuple2) {
			return !(tuple1 == tuple2);
		}
		#endregion
	}
}
