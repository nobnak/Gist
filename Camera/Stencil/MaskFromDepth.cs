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
		protected CameraEventRetention commander;
		protected DepthCapture cap;
		protected CameraData currCamData;
		protected Validator validator = new Validator();

		protected ResizableRenderTexture depthTex;
		protected ResizableRenderTexture depthColorTex;

		protected PIPTexture pip;

		#region unity
		private void OnEnable() {
			link.targetCam = GetComponent<Camera>();

			manualCam = new ManuallyRenderCamera(link.targetCam);
			commander = new CameraEventRetention(manualCam.Camera);

			cmd = new CommandBuffer();
			cap = new DepthCapture();
			depthTex = new ResizableRenderTexture(new FormatRT() {
				textureFormat = RenderTextureFormat.Depth,
				depth = 24
			});
			depthColorTex = new ResizableRenderTexture(new FormatRT() {
				textureFormat = RenderTextureFormat.ARGBHalf,
				readWrite = RenderTextureReadWrite.Linear,
				depth = 0,
			});

			pip = new PIPTexture();

			validator.SetCheckers(() => currCamData.Equals(link.targetCam));
			validator.Validation += () => {
				currCamData = link.targetCam;

				cmd.Clear();

				var size = link.targetCam.Size();
				depthTex.Size = size;
				depthColorTex.Size = size;

				cmd.Clear();
				cap.StepThreashold = tuner.stepThreashold;
				cap.Capture(cmd, depthColorTex, tuner.depthOutputMode);
				commander.Set(tuner.captureEvent, cmd);

				pip.CurrTuner = tuner.pip;
				pip.TargetCam = link.targetCam;
			};
			manualCam.AfterCopyFrom += v => {
				manualCam.Camera.depthTextureMode = DepthTextureMode.Depth;
				manualCam.Camera.cullingMask = tuner.CullingMask();
			};

			validator.Validate();

			depthTex.AfterCreateTexture += v => validator.Invalidate();
			depthColorTex.AfterCreateTexture += v => {
				using (new RenderTextureActivator(depthColorTex))
					GL.Clear(true, true, Color.white);
				validator.Invalidate();
			};
		}
		private void OnDisable() {
			if (commander != null) {
				commander.Reset();
				commander = null;
			}
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
			if (pip != null) {
				pip.Dispose();
				pip = null;
			}
		}
		private void Update() {
			validator.Validate();
			manualCam.Render(depthTex);

			OnRenderDepth.Invoke(depthColorTex);

			pip.Clear();
			pip.Add(depthColorTex);
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
			public CameraEvent captureEvent = CameraEvent.AfterForwardOpaque;
			public LayerMask mask = default;
			public DepthCapture.KW_OUTPUT depthOutputMode = DepthCapture.KW_OUTPUT.STEP_FUNC_INV;
			public float stepThreashold = 0.5f;

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
