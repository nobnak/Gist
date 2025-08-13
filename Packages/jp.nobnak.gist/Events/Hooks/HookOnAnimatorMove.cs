using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Events.Hooks {

	public class HookOnAnimatorMove : MonoBehaviour {

		public event System.Action<Animator> AnimatorMove;

		protected Animator anim;

		protected virtual void OnAnimatorMove() {
			if (AnimatorMove != null)
				AnimatorMove(CurrentAnimator);
		}
		protected virtual Animator CurrentAnimator {
			get {
				if (anim == null)
					anim = GetComponent<Animator>();
				return anim;
			}
		}
	}
}