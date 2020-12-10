using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace nobnak.Gist.Compute.Depth {

	public class DepthCapture : System.IDisposable {

		public const string PATH = "DepthCapture";

		protected Material mat;

		public DepthCapture() {
			var s = Resources.Load<Shader>(PATH);
			mat = new Material(s);
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

		public void Add(CommandBuffer buf, RenderTexture depth) {
			buf.Blit(null, depth, mat);
		}
		#endregion
	}
}
