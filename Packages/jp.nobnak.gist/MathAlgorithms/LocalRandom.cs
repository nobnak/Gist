using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms {

	public class LocalRandom {

		protected Random.State prev;

		public LocalRandom(int seed) {
			Push();
			Random.InitState(seed);
			Pop();
		}

		#region properties
		public Random.State mine { get; protected set; }
		#endregion

		public float Value {
			get {
				Push();
				var v = Random.value;
				Pop();
				return v;
			}
		}
		public int Range(int min, int max) {
			Push();
			var i = Random.Range(min, max);
			Pop();
			return i;
		}

		#region methods
		private void Pop() {
			mine = Random.state;
			Random.state = prev;
		}

		private void Push() {
			prev = Random.state;
		}
		#endregion
	}
}