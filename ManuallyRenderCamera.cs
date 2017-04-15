using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {
	public class ManuallyRenderCamera : System.IDisposable {
        public event System.Action<Camera> AfterCopyFrom;

		ITracker tracker;
		Camera manualCam;
		GameObject manualCamGo;

		public ManuallyRenderCamera(ITracker tracker) {
			this.tracker = tracker;
			this.manualCamGo = new GameObject ("Manually Render Camera");
			this.manualCamGo.hideFlags = HideFlags.DontSave;
			this.manualCam = manualCamGo.AddComponent<Camera> ();
			this.manualCam.enabled = false;
		}
		public ManuallyRenderCamera(Camera referenceCam) : this(new CameraTracker(referenceCam)) {}
		public ManuallyRenderCamera(System.Action<Camera> referenceFunc) : this(new FunctionalTracker(referenceFunc)) {}

		public Camera Camera { get { return manualCam; } }

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
			tracker.Adjust (manualCam);
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

		public interface ITracker {
			void Adjust(Camera cam);
		}

		public class CameraTracker : ITracker {
			protected Camera referenceCam;

			public CameraTracker(Camera referenceCam) {
				this.referenceCam = referenceCam;
			}

			#region ITracker
			public void Adjust(Camera cam) {
				cam.CopyFrom (referenceCam);
			}
			#endregion
		}

		public class FunctionalTracker : ITracker {
			protected System.Action<Camera> adjust;

			public FunctionalTracker(System.Action<Camera> adjust) {
				this.adjust = adjust;
			}

			#region ITracker implementation
			public void Adjust (Camera cam) {
				adjust (cam);
			}
			#endregion
		}
	}
}
