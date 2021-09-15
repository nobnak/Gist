using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Collection {

	public class ReusableIndexStorage {
		public const int DISPOSED_INDEX = -1;

		public readonly HashSet<Token> tokensInUse = new HashSet<Token>();
		public readonly SortedSet<int> unusedIndices = new SortedSet<int>();

		public int capacity = 0;

		#region interface

		#region object
		public override string ToString() {
			return $"<{GetType().Name}: capacity={capacity} use={tokensInUse.Count} reserved={unusedIndices.Count}>";
		}
		#endregion

		public int Capacity { get => capacity; }
		public int Count { get => tokensInUse.Count; }
		public Token GetToken() {
			var i = GetIndex();
			var token = new Token(this, i);
			tokensInUse.Add(token);
			return token;
		}
		#endregion

		#region methods
		protected void ReleaseToken(Token token) {
			tokensInUse.Remove(token);
			ReleaseIndex(token.index);
		}
		protected int GetIndex() {
			if (unusedIndices.Count > 0) {
				var max = unusedIndices.Max;
				unusedIndices.Remove(max);
				return max;
			}
			return capacity++;
		}
		protected void ReleaseIndex(int index) {
			unusedIndices.Add(index);
			while (capacity > 0
				&& unusedIndices.Count > 0
				&& (capacity - 1) == unusedIndices.Max) {
				capacity--;
				unusedIndices.Remove(capacity);
			}
		}
		#endregion

		#region definitions
		public class Token : System.IDisposable {

			public readonly ReusableIndexStorage reusable;
			public readonly int index;
			public bool disposed = false;

			public Token(ReusableIndexStorage reusable, int index) {
				this.reusable = reusable;
				this.index = index;
			}

			#region interface
			#region object
			public override string ToString() {
				return $"<{GetType().Name}:index={index} disposed={disposed}>";
			}
			#endregion

			#region IDisposable
			public void Dispose() {
				if (!disposed) {
					reusable.ReleaseToken(this);
					disposed = true;
				}
			}
			#endregion

			public int Index {
				get => disposed ? DISPOSED_INDEX : index;
			}
			public int Capacity {
				get => reusable.capacity;
			}

			public static implicit operator int (Token token) {
				return token.Index;
			}
			#endregion
		}
		#endregion
	}
}