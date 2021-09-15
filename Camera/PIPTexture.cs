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
		protected CameraEventRetention cmdRet;

		public PIPTexture(Camera targetCamera = null, CameraEvent cevent = CameraEvent.AfterImageEffects) {
			this.targetCam = targetCamera;
			this.CamEvent = cevent;
			this.cmdRet = new CameraEventRetention(targetCam);
			this.CamBuf = new CommandBuffer() {
				name = "PIP",
			};

			Valid.SetCheckers(() => camdata.Equals(TargetCam));
			Valid.Validation += () => {
				camdata = TargetCam;
				if (TargetCam == null || CamBuf == null)
					return;

				var destSize = TargetCam.ScaledSize();
				var offset_x = 0f;

				CamBuf.Clear();
				if (!tuner.enabled)
					return;

				var texHeight = destSize.y * tuner.sizeScale;
				foreach (var (t, m, p) in data) {
					if (t == null)
						continue;

					var texSize = new Vector2(texHeight * t.width / (float)t.height, texHeight);
					var vp = new Rect(offset_x, 0f, texSize.x, texSize.y);
					offset_x += texSize.x;

					CamBuf.SetViewport(vp);
					if (m == null)
						CamBuf.Blit(t, RTI);
					else
						CamBuf.Blit(t, RTI, m, p);
				}
				cmdRet.Set(cevent, CamBuf);
			};
		}

#region interface
		public Validator Valid { get; protected set; } = new Validator();
		public CameraEvent CamEvent { get; protected set; }
		public CommandBuffer CamBuf { get; protected set; }

		public Camera TargetCam {
			get => targetCam;
			set {
				targetCam = value;
				cmdRet.TargetCam = targetCam;
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
			if (cmdRet != null) {
				cmdRet.Reset();
				cmdRet = null;
			}
			if (CamBuf != null) {
				CamBuf = null;
			}
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
			[Tooltip("PIPの使用")]
			public bool enabled = true;
			[Tooltip("画面に占める割合")]
			public float sizeScale = 0.2f;
		}
#endregion
	}
}
