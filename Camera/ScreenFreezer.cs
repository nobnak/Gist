using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Cameras {

	public class ScreenFreezer : MonoBehaviour {

		[SerializeField]
		protected Data data = new Data();

		protected RenderTexture captured;

		#region unity
		protected void Update() {
			if (Input.GetKey(data.captureKey))
				ReleaseCapture(ref captured);
		}
		protected void OnRenderImage(RenderTexture source, RenderTexture destination) {
			if (captured == null) {
				captured = Capture(source);
			}
			Graphics.Blit(captured, destination);
		}
		protected void OnDisable() {
			ReleaseCapture(ref captured);
		}
		#endregion
		#region static
		public static RenderTexture Capture(RenderTexture source) {
			var captured = RenderTexture.GetTemporary(source.descriptor);
			Graphics.Blit(source, captured);
			return captured;
		}
		#endregion
		#region private
		public static void ReleaseCapture(ref RenderTexture captured) {
			if (captured != null) {
				RenderTexture.ReleaseTemporary(captured);
				captured = null;
			}
		}
		#endregion

		[System.Serializable]
		public class Data {
			public KeyCode captureKey;
		}
	}
}
