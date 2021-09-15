using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace nobnak.Gist.Extensions.ScreenExt {

	public static class ScreenExtension {
		public const int MAX_RESOLUTION = 1 << 14;
		public const int MIN_RESOLUTION = 1;
		private const int DEF_MAX_LOD = 10;
		public static readonly int P_ScreenParams = Shader.PropertyToID("_ScreenParams");

		public static Vector2Int ClampScreenSize(this Vector2Int screen) {
			screen.x = (screen.x < MIN_RESOLUTION ? MIN_RESOLUTION :
				(screen.x <= MAX_RESOLUTION ? screen.x : MAX_RESOLUTION));
			screen.y = (screen.y < MIN_RESOLUTION ? MIN_RESOLUTION :
				(screen.y <= MAX_RESOLUTION ? screen.y : MAX_RESOLUTION));
			return screen;
		}

        public static Vector2 UV(this Vector3 mousePosition) {
            var uv = new Vector2(
                (float)mousePosition.x / Screen.width, 
                (float)mousePosition.y / Screen.height);
            return uv;
        }
		public static int LOD(this int size, int lod = 0,
			int maxLod = DEF_MAX_LOD, int minLod = 0) {
			lod = Mathf.Clamp(lod, minLod, maxLod);
			if (lod > 0) {
				size >>= lod;
			} else if (lod < 0) {
				size <<= -lod;
			}
			return size;
		}
		public static Vector2Int LOD(int width, int height, int lod = 0,
			int maxLod = DEF_MAX_LOD, int minLod = 0) {
			return new Vector2Int(width.LOD(lod), height.LOD(lod));
		}
		public static Vector2Int LOD(this Vector2Int size, int lod = 0, int maxLod = DEF_MAX_LOD, int minLod = 0) {
			return LOD(size.x, size.y, lod, maxLod, minLod);
		}
		public static Vector2Int LOD(this Camera c, int lod = 0, int maxLod = DEF_MAX_LOD, int minLod = 0) {
			return LOD(c.pixelWidth, c.pixelHeight, lod, maxLod, minLod);
		}
		public static Vector2Int LOD(this Texture tex, int lod = 0, int maxLod = DEF_MAX_LOD, int minLod = 0) {
			return LOD(tex.width, tex.height, lod, maxLod, minLod);
		}

		public static Vector2Int Size(this Camera c) {
			return new Vector2Int(c.pixelWidth, c.pixelHeight);
		}
		public static Vector2Int ScaledSize(this Camera c) {
			return new Vector2Int(c.scaledPixelWidth, c.scaledPixelHeight);
		}

		public static void SetGlobalScreenParams(int width, int height) {
			var s = new Vector4(width, height, 1f / width, 1f / height);
			Shader.SetGlobalVector(P_ScreenParams, s);
		}
		public static void SetGlobalScreenParams(this Vector2Int s) {
			SetGlobalScreenParams(s.x, s.y);
		}
		public static void SetGlobalScreenParams(this Camera c) {
			var s = c.Size();
			SetGlobalScreenParams(s);
		}

		public static float AspectRatio(this Vector2 v) {
			return v.x / v.y;
		}

		public static void ScaleGUIBasedOnDpi(float targetDpi = 96f) {
			var scale = Screen.dpi / targetDpi;
#if UNITY_EDITOR
			var typeGameView = System.Type.GetType("UnityEditor.GameView,UnityEditor");
			var gameView = EditorWindow.GetWindow(typeGameView);
			var propLowRes = typeGameView.GetProperty("lowResolutionForAspectRatios");
			var isLowRes = (bool)propLowRes.GetValue(gameView);
			if (isLowRes)
				scale *= 0.5f;
#endif
			GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), Vector2.zero);
		}
	}
}
