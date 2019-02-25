using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Timers {

	public class CoroutineBasedTimer : System.IDisposable {

		public event System.Action Started;
		public event System.Action Ended;

		public event System.Action Completed;
		public event System.Action Aborted;

		protected MonoBehaviour target;
		protected Coroutine current;

		public CoroutineBasedTimer(MonoBehaviour target) {
			this.target = target;
		}

		#region public
		public CoroutineBasedTimer Start(float duration) {
			Abort();
			current = target.StartCoroutine(GetTimer(duration));
			if (Started != null)
				Started.Invoke();
			return this;
		}
		public CoroutineBasedTimer Abort() {
			if (current != null) {
				target.StopCoroutine(current);
				current = null;
				if (Aborted != null)
					Aborted.Invoke();
				if (Ended != null)
					Ended.Invoke();
			}
			return this;
		}
		#endregion

		#region private
		protected IEnumerator GetTimer(float duration) {
			yield return new WaitForSeconds(duration);
			Complete();
		}
		protected void Complete() {
			current = null;
			if (Completed != null)
				Completed.Invoke();
			if (Ended != null)
				Ended.Invoke();
		}
		#endregion

		#region IDisposable
		public void Dispose() {
			Abort();
		}
		#endregion
	}
}
