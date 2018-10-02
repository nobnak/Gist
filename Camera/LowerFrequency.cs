using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Cameras {

	public class LowerFrequency : MonoBehaviour {
		[SerializeField]
		protected Data data = new Data();

		protected Validator validator = new Validator();
		protected Timer timer = new Timer(0f, Timer.StateEnum.Completed);

		protected RenderTexture captured;

		#region unity
		protected void Awake() {
			validator.Validation += () => {
				data.interval = Mathf.Max(data.interval, 0f);
				timer = new Timer(data.interval, Timer.StateEnum.Completed);
			};
		}
		protected void OnRenderImage(RenderTexture source, RenderTexture destination) {
			validator.Validate();
			timer.Update();

			if (timer.Completed) {
				ReleaseTemporary(ref captured);
				captured = CaptureInTemporary(source);
				timer.Start();
			}

			Graphics.Blit(captured == null ? source : captured, destination);
		}
		protected void OnValidate() {
			validator.Invalidate();
		}
		protected void OnDisable() {
			ReleaseTemporary(ref captured);
		}
		#endregion

		#region private
		#endregion

		#region static
		public static void ReleaseTemporary(ref RenderTexture tmp) {
			if (tmp != null) {
				RenderTexture.ReleaseTemporary(tmp);
				tmp = null;
			}
		}
		public static RenderTexture CaptureInTemporary(RenderTexture source) {
			var captured = RenderTexture.GetTemporary(source.descriptor);
			Graphics.Blit(source, captured);
			return captured;
		}
		#endregion

		#region interface
		public Data CurrentData {
			get {
				return data.DeepCopy();
			}
			set {
				validator.Invalidate();
				data = value;
			}
		}
		#endregion

		[System.Serializable]
		public class Data {
			public float interval = 1f;
		}
	}
}
