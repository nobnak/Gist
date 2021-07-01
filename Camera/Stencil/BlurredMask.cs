using nobnak.Gist.Compute.Blurring;
using nobnak.Gist.Compute.Depth;
using nobnak.Gist.Events;
using nobnak.Gist.Extensions.GenericExt;
using nobnak.Gist.Extensions.ScreenExt;
using nobnak.Gist.Extensions.Texture2DExt;
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
	public class BlurredMask : MonoBehaviour {

		[SerializeField]
		protected TempRef link = new TempRef();
		[SerializeField]
		protected Tuner tuner = new Tuner();
		[SerializeField]
		public TextureEvent OnRenderDepth = new TextureEvent();

		protected Texture depthColorTex;
		protected CameraData currCamData;
		protected Validator validator = new Validator();

		protected Blur blur;
		protected BinaryAccumulator ramm;
		protected RenderTexture bDepthTex;
		protected ResizableRenderTexture maskTex0;
		protected ResizableRenderTexture maskTex1;

		protected PIPTexture pip;

		#region unity
		private void OnEnable() {
			link.targetCam = GetComponent<Camera>();

			blur = new Blur();
			ramm = new BinaryAccumulator();
			maskTex0 = new ResizableRenderTexture(new FormatRT() {
				textureFormat = RenderTextureFormat.ARGBHalf,
				depth = 0,
			});
			maskTex1 = new ResizableRenderTexture(maskTex0.Format);

			pip = new PIPTexture();

			validator.SetCheckers(() => currCamData.Equals(link.targetCam));
			validator.Validation += () => {
				currCamData = link.targetCam;

				var size = link.targetCam.Size();
				maskTex0.Size = size;
				maskTex1.Size = size;

				pip.CurrTuner = tuner.pip;
				pip.TargetCam = link.targetCam;
			};

			System.Action<RenderTexture> fClear = tex => {
				using (new RenderTextureActivator(tex)) {
					GL.Clear(true, true, Color.white);
				}
			};

			maskTex0.AfterCreateTexture += fClear;
			maskTex1.AfterCreateTexture += fClear;

			validator.Validate();
		}
		private void OnDisable() {
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

			Texture resTex = depthColorTex;

			if (depthColorTex != null && tuner.blurSize > 0) {
				var size = depthColorTex.Size();
				var blurRes = Mathf.RoundToInt(2f * link.targetCam.orthographicSize / tuner.blurSize);
				blurRes = Mathf.Min(blurRes, size.y);

				if (blurRes >= 4) {
					blur.FindSize(size.y, blurRes, out var iter, out var lod);
					blur.Render(depthColorTex, ref bDepthTex, iter, lod);
					resTex = bDepthTex;
				}

				if (tuner.ramm.dark >= 0f) {
					ramm.Render(
						maskTex1,
						maskTex0,
						bDepthTex,
						tuner.ramm);
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

		#region public
		public Tuner CurrTuner {
			get => tuner.DeepCopy();
			set {
				tuner = value.DeepCopy();
				validator.Invalidate();
			}
		}
		public void ListenSource(Texture tex) {
			this.depthColorTex = tex;
			validator.Invalidate();
		}
		#endregion

		#region definitions
		[System.Serializable]
		public class Tuner {
			public PIPTexture.Tuner pip = new PIPTexture.Tuner();

			[Tooltip("カメラに対するブラーのサイズ")]
			public float blurSize = -1;
			[Tooltip("バイナリアキュムレータ")]
			public BinaryAccumulator.RenderParams ramm = new BinaryAccumulator.RenderParams();
		}
		[System.Serializable]
		public class TempRef {
			public Camera targetCam;
		}
		#endregion
	}
}
