using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Pluggable {

	public class LambdaQueue : IEnumerable {

		protected Queue<System.Action> lambdas = new Queue<System.Action>();

		#region interface

		#region IEnumerable
		public IEnumerator GetEnumerator() {
			while (lambdas.Count > 0)
				yield return lambdas.Dequeue();
		}
		#endregion

		public LambdaQueue Enqueue(System.Action a) {
			lambdas.Enqueue(a);
			return this;
		}
		public LambdaQueue Update() {
			if (lambdas.Count > 0)
				lambdas.Dequeue()();
			return this;
		}
		#endregion
	}
}
