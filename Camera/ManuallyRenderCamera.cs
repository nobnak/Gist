using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace nobnak.Gist {
	public class ManuallyRenderCamera : System.IDisposable {
        public event System.Action<Camera> AfterCopyFrom;

		ITracker tracker;
		Camera manualCam;
		GameObject manualCamGo;

		public ManuallyRenderCamera(ITracker tracker) {
			this.tracker = tracker;
			this.manualCamGo = new GameObject ("Manually Render Camera");
			this.manualCamGo.hideFlags = HideFlags.HideAndDontSave;
			this.manualCam = manualCamGo.AddComponent<Camera> ();
			this.manualCam.enabled = false;
		}
		public ManuallyRenderCamera(Camera referenceCam) : this(new CameraTracker(referenceCam)) {}
		public ManuallyRenderCamera(System.Action<Camera> referenceFunc) : this(new FunctionalTracker(referenceFunc)) {}

        #region IDisposable implementation
        public void Dispose () {
            manualCamGo.DestroySelf();
        }
        #endregion

		public Camera Camera { get { return manualCam; } }

		public ManuallyRenderCamera Render(RenderTexture target) {
            Profiler.BeginSample ("ManuallyRenderCamera.Render");
            PrepareForRendering (target);
			manualCam.Render ();
            PostpareForRendering ();
            Profiler.EndSample ();
			return this;
		}
        public ManuallyRenderCamera RenderWithShader(RenderTexture target, Shader shader, string tag) {
            Profiler.BeginSample ("ManuallyRenderCamera.RenderWithShader");
            PrepareForRendering (target);
            manualCam.RenderWithShader (shader, tag);
            PostpareForRendering ();
            Profiler.EndSample ();
            return this;
        }

        #region Event notification
        void NotifyAfterCopyFrom() {
            if (AfterCopyFrom != null)
                AfterCopyFrom (manualCam);
        }
        #endregion

        void PrepareForRendering(RenderTexture target) {
            tracker.Adjust (manualCam);
            NotifyAfterCopyFrom ();
            manualCam.targetTexture = target;
        }
        void PostpareForRendering() {
            manualCam.targetTexture = null;
        }

        #region Classes
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
        #endregion
	}
}
