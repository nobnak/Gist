using UnityEngine;
using System.Collections;

namespace Gist.Extensions.Easing {

	public static class EasingExtension {

		public static Easer Easing(this MonoBehaviour bhv, System.Action<float> func) {
			
		}
	}

	public class Easer {
		MonoBehaviour _bhv;
		System.Action<float> _func;

		public Easer(Component comp, System.Action<float> func) {
			
		}
	}
}
