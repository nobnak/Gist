using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Cameras {

	public class BinaryAccumulator : System.IDisposable {

		public const string PATH = "BinaryAccumulator";
		public static readonly Vector2 DEFAULT_COLOR_ADJUSTER = new Vector2(1f, 0f);

		public static readonly int P_MainTex = Shader.PropertyToID("_MainTex");
		public static readonly int P_RefTex = Shader.PropertyToID("_RefTex");
		public static readonly int P_User_Time = Shader.PropertyToID("_User_Time");
		public static readonly int P_Throttle = Shader.PropertyToID("_Throttle");
		public static readonly int P_ColorAdjust = Shader.PropertyToID("_ColorAdjust");
		public static readonly int P_BWPoints = Shader.PropertyToID("_BWPoints");

		protected Material mat;

		public BinaryAccumulator() {
			var s = Resources.Load<Shader>(PATH);
			mat = new Material(s);
		}

		#region interface

		#region IDisposable
		public void Dispose() {
			if (mat != null) {
				mat.DestroySelf();
				mat = null;
			}
		}
		#endregion

		public void Render(
			RenderTexture next, 
			Texture prev, 
			Texture refTex,
			RenderParams rparams,
			float dt
		) {
			mat.SetVector(P_User_Time, new Vector4(dt, Time.timeSinceLevelLoad, 0f, 0f));
			mat.SetVector(P_Throttle, new Vector4(rparams.light, rparams.dark));
			mat.SetVector(P_ColorAdjust, rparams.colorAdjuster);
			mat.SetVector(P_BWPoints, new Vector4(rparams.blackWhtePoints.x, rparams.blackWhtePoints.y, 0f, 1f));

			mat.SetTexture(P_RefTex, refTex);

			Graphics.Blit(prev, next, mat);
		}
		public void Render(
			RenderTexture next, 
			Texture prev, 
			Texture refTex,
			RenderParams rparams
		) {
			Render(next, prev, refTex, rparams, Time.deltaTime);
		}
		#endregion

		#region definitions
		[System.Serializable]
		public class RenderParams {
			[Tooltip("黒化速度")]
			public float dark = 0f;
			[Tooltip("白化速度")]
			public float light = 0f;
			[Tooltip("インプットのコントラスト")]
			public Vector2 colorAdjuster = new Vector2(1f, 0f);
			[Tooltip("ブラック&ホワイトポイント")]
			public Vector2 blackWhtePoints = new Vector2(0f, 1f);
		}
		#endregion
	}
}
