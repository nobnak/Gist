using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.ScreenExt {

	public static class ScreenExtension {
		public const int MAX_RESOLUTION = 1 << 14;
		public const int MIN_RESOLUTION = 1;

		public static Vector2Int ClampScreenSize(this Vector2Int screen) {
			screen.x = (screen.x < MIN_RESOLUTION ? MIN_RESOLUTION :
				(screen.x <= MAX_RESOLUTION ? screen.x : MAX_RESOLUTION));
			screen.y = (screen.y < MIN_RESOLUTION ? MIN_RESOLUTION :
				(screen.y <= MAX_RESOLUTION ? screen.y : MAX_RESOLUTION));
			return screen;
		}
	}
}
