using UnityEngine;
using System.Collections;
using nobnak.Gist.Layers;

namespace nobnak.Gist {
    
    [ExecuteInEditMode]
    public class CameraSNTG : AbstractSimplexNoiseTextureGeenerator {

        [SerializeField]
        protected Camera targetCam;

        #region Unity
        protected override void OnEnable() {
            base.OnEnable();
        }
        protected override void Update () {
            SetAspect (targetCam.aspect);
            base.Update ();
        }
        #endregion

        #region implemented abstract members of SimplexNoiseTextureGeenerator
        protected override Vector2 WorldToViewportPoint (Vector3 worldPos) {
            return targetCam.WorldToViewportPoint (worldPos);
        }
        protected override Vector3 TransformDirection (Vector3 localDir) {
            return targetCam.transform.TransformDirection (localDir);
        }
        #endregion

		public Camera TargetCamera {
			set {
				targetCam = value;
			}
			get {
				return targetCam;
			}
		}
    }
}