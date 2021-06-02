using nobnak.Gist.Extensions.ScreenExt;
using nobnak.Gist.Extensions.Texture2DExt;
using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace nobnak.Gist.Cameras {

	public class PIPTexture : System.IDisposable, IValidator {

		public readonly static RenderTargetIdentifier RTI = BuiltinRenderTextureType.CurrentActive;

		protected List<(Texture, Material, int)> data = new List<(Texture, Material, int)>();
		protected Tuner tuner = new Tuner();

		protected Camera targetCam;
		protected CameraData camdata;

		public PIPTexture(Camera targetCamera = null, CameraEvent cevent = CameraEvent.AfterEverything) {
			this.TargetCam = targetCamera;
			this.CamEvent = cevent;

			Valid.SetCheckers(() => camdata.Equals(TargetCam));
			Valid.Validation += () => {
				camdata = TargetCam;
				if (TargetCam == null || CamBuf == null)
					return;

				var screenSize = TargetCam.Size();
				var offset_x = 0f;

				RemoveCommandBuffer();
				CamBuf.Clear();
				foreach (var (t, m, p) in data) {
					var texSize = (Vector2)t.Size() * tuner.sizeScale;
					var vp = new Rect(offset_x, 0f, texSize.x, texSize.y);
					CamBuf.SetViewport(vp);

					if (m == null)
						CamBuf.Blit(t, RTI);
					else
						CamBuf.Blit(t, RTI, m, p);
				}
				TargetCam.AddCommandBuffer(CamEvent, CamBuf);
			};
		}

		#region interface
		public Validator Valid { get; protected set; } = new Validator();
		public CameraEvent CamEvent { get; protected set; }
		public CommandBuffer CamBuf { get; protected set; } = new CommandBuffer();

		public Camera TargetCam {
			get => targetCam;
			set {
				RemoveCommandBuffer();
				targetCam = value;
				Valid.Invalidate();
			}
		}

		public Tuner CurrTuner {
			get { return tuner.DeepCopy(); }
			set {
				tuner = value.DeepCopy();
				Valid.Invalidate();
			}
		}

		public PIPTexture Add(Texture t, Material m = null, int pass = -1) {
			data.Add((t, m, pass));
			Valid.Invalidate();
			return this;
		}
		public PIPTexture Clear() {
			data.Clear();
			Valid.Invalidate();
			return this;
		}

		#region IDisposable
		public void Dispose() {
			if (CamBuf != null) {
				RemoveCommandBuffer();
				CamBuf = null;
			}
		}
		#endregion

		#region member
		private void RemoveCommandBuffer() {
			if (targetCam != null)
				targetCam.RemoveCommandBuffer(CamEvent, CamBuf);
		}
		#endregion

		#region IValidator
		public bool IsValid => Valid.IsValid;
		public void Invalidate() => Valid.Invalidate();
		public bool Validate(bool force = false) => Valid.Validate(force);
		#endregion

		#endregion

		#region definition
		[System.Serializable]
		public class Tuner {
			public float sizeScale = 0.2f;
		}
		#endregion
	}
}
