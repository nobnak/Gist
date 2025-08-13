using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.EventExt {

	public static class EventExtension {

		public static void Notify<T>(this UnityEngine.Events.UnityEvent<T> ev, T param) {
			if (ev != null)
				ev.Invoke(param);
		}
	}
}
