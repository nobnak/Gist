using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Pluggable {

	public class NanoCoroutine {

		public event System.Action<System.Exception> OnError;

		protected List<IEnumerator> coroutines = new List<IEnumerator>();

		public virtual void Start(IEnumerator co) {
			coroutines.Add(co);
		}

		public virtual void Update() {
			for (var i = 0; i < coroutines.Count; ) {
				try {
					var c = coroutines[i];
					if (c == null || !c.MoveNext()) {
						ReplaceWithLast(i);
						continue;
					}

					if (c.Current != null)
						Debug.LogWarning($"{GetType().Name} : Only support yield return null");
				} catch(System.Exception e) {
					OnError?.Invoke(e);
				}
				i++;
			}


		}

		#region method
		protected virtual void ReplaceWithLast(int i) {
			var ilast = coroutines.Count - 1;
			coroutines[i] = coroutines[ilast];
			coroutines.RemoveAt(ilast);
		}
		#endregion
	}
}