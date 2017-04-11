using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {
	public class ManuallyRenderCamera : System.IDisposable {
        public event System.Action<Camera> AfterCopyFrom;

		Camera referenceCam;
		Camera manualCam;
		GameObject manualCamGo;

		public ManuallyRenderCamera(Camera referenceCam) {
			this.referenceCam = referenceCam;

			manualCamGo = new GameObject ("Manually Render Camera");
			manualCamGo.transform.SetParent (referenceCam.transform, false);
			manualCamGo.transform.localPosition = Vector3.zero;
			manualCamGo.transform.localRotation = Quaternion.identity;
			manualCamGo.transform.localScale = Vector3.one;

			manualCam = manualCamGo.AddComponent<Camera> ();
			manualCam.enabled = false;
		}

		public ManuallyRenderCamera Render(RenderTexture target) {
            PrepareForRendering (target);
			manualCam.Render ();
			return this;
		}
        public ManuallyRenderCamera RenderWithShader(RenderTexture target, Shader shader, string tag) {
            PrepareForRendering (target);
            manualCam.RenderWithShader (shader, tag);
            return this;
        }

        void PrepareForRendering(RenderTexture target) {
            manualCam.CopyFrom (referenceCam);
            NotifyAfterCopyFrom ();
            manualCam.targetTexture = target;
        }
        void NotifyAfterCopyFrom() {
            if (AfterCopyFrom != null)
                AfterCopyFrom (manualCam);
        }

		#region IDisposable implementation
		public void Dispose () {
			if (Application.isPlaying)
				Object.Destroy (manualCamGo);
			else
				Object.DestroyImmediate (manualCamGo);
		}
		#endregion
	}
}
