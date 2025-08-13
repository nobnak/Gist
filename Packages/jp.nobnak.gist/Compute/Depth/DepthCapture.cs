using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace nobnak.Gist.Compute.Depth {

	public class DepthCapture : System.IDisposable {

		public const string PATH = "DepthCapture";
		public enum KW_OUTPUT {
			___ = default,
			STEP_FUNC,
			STEP_FUNC_INV,
		}
		public readonly int P_StepTh = Shader.PropertyToID("_StepTh");

		protected Material mat;
		protected float stepTh;

		public DepthCapture() {
			var s = Resources.Load<Shader>(PATH);
			mat = new Material(s);
			stepTh = mat.GetFloat(P_StepTh);
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

		public float StepThreashold {
			get => stepTh;
			set {
				stepTh = Mathf.Clamp01(value);
			}
		}
		public void Capture(CommandBuffer buf, RenderTexture depth, KW_OUTPUT output = default) {
			mat.shaderKeywords = null;
			if (output != default)
				mat.EnableKeyword(output.ToString());

			mat.SetFloat(P_StepTh, stepTh);

			buf.Blit(null, depth, mat);
		}
		#endregion
	}
}
