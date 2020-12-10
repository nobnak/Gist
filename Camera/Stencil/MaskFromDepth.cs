using nobnak.Gist.Compute.Depth;
using nobnak.Gist.Events;
using nobnak.Gist.Extensions.ScreenExt;
using nobnak.Gist.Resizable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace nobnak.Gist.Cameras {

	public class MaskFromDepth : MonoBehaviour {

		public Camera refCam;
		public Tuner tuner = new Tuner();
		public TextureEvent OnRenderDepth = new TextureEvent();

		protected ManuallyRenderCamera manualCam;
		protected CommandBuffer cmd;
		protected DepthCapture cap;
		protected Validator validator = new Validator();

		protected ResizableRenderTexture depthBuf;
		protected ResizableRenderTexture depthTex;

		#region unity
		private void OnEnable() {
			manualCam = new ManuallyRenderCamera(refCam);
			manualCam.Camera.depthTextureMode = DepthTextureMode.Depth;

			cmd = new CommandBuffer();
			cap = new DepthCapture();
			depthBuf = new ResizableRenderTexture(new FormatRT() {
				textureFormat = RenderTextureFormat.Depth,
				depth = 24
			});
			depthTex = new ResizableRenderTexture(new FormatRT() {
				textureFormat = RenderTextureFormat.ARGBFloat,
				readWrite = RenderTextureReadWrite.Linear,
				depth = 0,
			});

			validator.Validation += () => {
				Prepare();
			};
			validator.Validate();
			depthBuf.AfterCreateTexture += v => validator.Invalidate();
			depthTex.AfterCreateTexture += v => validator.Invalidate();

			manualCam.Camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, cmd);
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
			if (depthBuf != null) {
				depthBuf.Dispose();
				depthBuf = null;
			}
			if (depthTex != null) {
				depthTex.Dispose();
				depthTex = null;
			}
		}
		private void Update() {
			validator.Validate();
			manualCam.Render(depthBuf);
			OnRenderDepth.Invoke(depthTex);
		}
		private void OnValidate() {
			validator.Invalidate();
		}
		#endregion

		#region member
		private void Prepare() {
			cmd.Clear();
			manualCam.Camera.targetTexture = null;
			manualCam.Camera.cullingMask = tuner.CullingMask();

			var size = manualCam.Camera.Size();
			depthBuf.Size = size;
			depthTex.Size = size;
			cap.Add(cmd, depthTex);
		}
		#endregion

		#region definitions
		[System.Serializable]
		public class Tuner {
			public LayerMask[] masks = new LayerMask[0];
			public int CullingMask() {
				var v = 0;
				foreach (var m in masks)
					v |= m.value;
				return v;
			}
		}
		#endregion
	}
}
