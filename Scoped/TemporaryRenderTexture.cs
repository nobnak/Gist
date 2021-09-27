using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Scoped {

	public class TemporaryRenderTexture : System.IDisposable {

		protected RenderTexture rtex;

		public TemporaryRenderTexture(RenderTexture rtex) {
			this.rtex = rtex;
		}
		public TemporaryRenderTexture(RenderTextureDescriptor desc)
			: this(RenderTexture.GetTemporary(desc)) { }

		public void Dispose() {
			if (rtex != null) {
				RenderTexture.ReleaseTemporary(rtex);
				rtex = null;
			}
		}

		public static implicit operator RenderTexture(TemporaryRenderTexture trt) => trt.rtex;
		public static explicit operator TemporaryRenderTexture(RenderTexture rtex) 
			=> new TemporaryRenderTexture(rtex);
	}
}