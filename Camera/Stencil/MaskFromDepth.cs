using nobnak.Gist.Compute.Blurring;
using nobnak.Gist.Compute.Depth;
using nobnak.Gist.Events;
using nobnak.Gist.Extensions.GenericExt;
using nobnak.Gist.Extensions.ScreenExt;
using nobnak.Gist.ObjectExt;
using nobnak.Gist.Resizable;
using nobnak.Gist.Scoped;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace nobnak.Gist.Cameras {

	[ExecuteAlways]
	[RequireComponent(typeof(Camera))]
	public class MaskFromDepth : MonoBehaviour {

		[SerializeField]
		protected TempRef link = new TempRef();
		[SerializeField]
		public Tuner tuner = new Tuner();
		[SerializeField]
		public TextureEvent OnRenderDepth = new TextureEvent();

		protected ManuallyRenderCamera manualCam;
		protected CommandBuffer cmd;
		protected DepthCapture cap;
		protected CameraData currCamData;
		protected Validator validator = new Validator();

		protected ResizableRenderTexture depthTex;
		protected ResizableRenderTexture depthColorTex;

		protected Blur blur;
		protected RestoreAndMergeMaskKit ramm;
		protected RenderTexture bDepthTex;
		protected ResizableRenderTexture maskTex0;
		protected ResizableRenderTexture maskTex1;

		protected PIPTexture pip;

		#region unity
		private void OnEnable() {
			link.targetCam = GetComponent<Camera>();

			manualCam = new ManuallyRenderCamera(link.targetCam);

			cmd = new CommandBuffer();
			cap = new DepthCapture();
			depthTex = new ResizableRenderTexture(new FormatRT() {
				textureFormat = RenderTextureFormat.Depth,
				depth = 24
			});
			depthColorTex = new ResizableRenderTexture(new FormatRT() {
				textureFormat = RenderTextureFormat.ARGBFloat,
				readWrite = RenderTextureReadWrite.Linear,
				depth = 0,
			});

			blur = new Blur();
			ramm = new RestoreAndMergeMaskKit();
			maskTex0 = new ResizableRenderTexture(new FormatRT() {
				depth = 0,
			});
			maskTex1 = new ResizableRenderTexture(maskTex0.Format);

			pip = new PIPTexture();

			validator.SetCheckers(() => currCamData.Equals(link.targetCam));
			validator.Validation += () => {
				currCamData = link.targetCam;

				cmd.Clear();

				var size = link.targetCam.Size();
				depthTex.Size = size;
				depthColorTex.Size = size;
				maskTex0.Size = size;
				maskTex1.Size = size;

				var evt = CameraEvent.AfterEverything;
				manualCam.Camera.RemoveCommandBuffer(evt, cmd);
				cmd.Clear();
				cap.StepThreashold = 1e-2f;
				cap.Capture(cmd, depthColorTex, DepthCapture.KW_OUTPUT.STEP_FUNC);
				manualCam.Camera.AddCommandBuffer(evt, cmd);

				pip.CurrTuner = tuner.pip;
				pip.TargetCam = link.targetCam;
			};
			manualCam.AfterCopyFrom += v => {
				manualCam.Camera.depthTextureMode = DepthTextureMode.Depth;
				manualCam.Camera.cullingMask = tuner.CullingMask();
			};

			validator.Validate();

			System.Action<RenderTexture> fClear = tex => {
				using (new RenderTextureActivator(tex)) {
					GL.Clear(true, true, Color.white);
				}
			};

			depthTex.AfterCreateTexture += v => validator.Invalidate();
			depthColorTex.AfterCreateTexture += v => validator.Invalidate();
			maskTex0.AfterCreateTexture += fClear;
			maskTex1.AfterCreateTexture += fClear;
		}
		private void OnDisable() {
			if (manualCam != null) {
				manualCam.Dispose();
				manualCam = null;
			}
			if (cap != null) {
				cap.Dispose();
				cap = null;
			}
			if (cmd != null) {
				cmd.Dispose();
				cmd = null;
			}
			if (depthTex != null) {
				depthTex.Dispose();
				depthTex = null;
			}
			if (depthColorTex != null) {
				depthColorTex.Dispose();
				depthColorTex = null;
			}

			if (blur != null) {
				blur.Dispose();
				blur = null;
			}
			if (bDepthTex != null) {
				bDepthTex.DestroySelf();
				bDepthTex = null;
			}
			if (maskTex0 != null) {
				maskTex0.Dispose();
				maskTex0 = null;
			}
			if (maskTex1 != null) {
				maskTex1.Dispose();
				maskTex1 = null;
			}
			if (ramm != null) {
				ramm.Dispose();
				ramm = null;
			}
			if (pip != null) {
				pip.Dispose();
				pip = null;
			}
		}
		private void Update() {
			validator.Validate();
			manualCam.Render(depthTex);

			var size = depthColorTex.Size;
			Texture resTex = depthColorTex;

			if (tuner.blurSize > 0) {
				var blurRes = Mathf.RoundToInt(2f * manualCam.Camera.orthographicSize / tuner.blurSize);
				blurRes = Mathf.Clamp(blurRes, 4, size.y);

				var iter = 0;
				var lod = 4;
				blur.FindSize(size.y, blurRes, out iter, out lod);
				blur.Render(depthColorTex, ref bDepthTex, iter, lod);
				resTex = bDepthTex;

				if (tuner.blurMergeSpeed > 0f) {
					ramm.Render(
						maskTex1,
						maskTex0,
						bDepthTex,
						tuner.blurRestoreSpeed,
						tuner.blurMergeSpeed);
					GenericsExtension.Swap(ref maskTex0, ref maskTex1);
					resTex = maskTex0;
				}
			}

			OnRenderDepth.Invoke(resTex);

			pip.Clear();
			pip.Add(resTex);
			pip.Validate();
		}
		private void OnValidate() {
			validator.Invalidate();
		}
#endregion

		#region definitions
		[System.Serializable]
		public class Tuner {
			public PIPTexture.Tuner pip = new PIPTexture.Tuner();
			public LayerMask mask = default;

			public float blurSize = -1;
			public float blurRestoreSpeed = 0.01f;
			public float blurMergeSpeed = 0.1f;

			public int CullingMask() {
				return mask.value;
			}
		}
		[System.Serializable]
		public class TempRef {
			public Camera targetCam;
		}
		#endregion
	}
}
