using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Cameras {

	public class RestoreAndMergeMaskKit : System.IDisposable {

		public const string PATH = "RestoreAndMergeMaskKit";

		public static readonly int P_MainTex = Shader.PropertyToID("_MainTex");
		public static readonly int P_CharTex = Shader.PropertyToID("_CharTex");
		public static readonly int P_User_Time = Shader.PropertyToID("_User_Time");
		public static readonly int P_Throttle = Shader.PropertyToID("_Throttle");

		protected Material mat;

		public RestoreAndMergeMaskKit() {
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
			RenderTexture dst, 
			Texture src, 
			Texture charMaskTex, 
			float thRestore,
			float thMerge,
			float dt
		) {
			mat.SetVector(P_User_Time, new Vector4(dt, Time.timeSinceLevelLoad, 0f, 0f));
			mat.SetVector(P_Throttle, new Vector4(thRestore, thMerge));

			mat.SetTexture(P_CharTex, charMaskTex);

			Graphics.Blit(src, dst, mat);
		}
		public void Render(
			RenderTexture dst, 
			Texture src, 
			Texture charMaskTex,
			float thRestore,
			float thMerge
		) {
			Render(dst, src, charMaskTex, thRestore, thMerge, Time.deltaTime);
		}
		#endregion
	}
}
