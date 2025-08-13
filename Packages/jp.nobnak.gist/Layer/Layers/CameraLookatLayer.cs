using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nobnak.Gist.Layers {

    [ExecuteInEditMode]
    public class CameraLookatLayer : BaseLookatLayer {

        public Camera targetCamera;

        protected Master master;

        #region implemented abstract members of BaseLookatLayer
        protected override IMaster GetMaster () {
            master.Update (targetCamera);
            return master;
        }
        #endregion

        protected override void InitLayer () {
            base.InitLayer ();
            master = new Master (this);
        }

        public class Master : IMaster {
            public Camera TargetCamera { get; protected set; }

            protected CameraLookatLayer parent;

            public Master(CameraLookatLayer parent) {
                this.parent = parent;
            }

            #region IMaster implementation
            public Quaternion Rotation () {
                return TargetCamera.transform.rotation;
            }
            public Vector3 Size () {
                var z = Vector3.Dot (TargetCamera.transform.forward, 
                    (parent.cacheTr.position - TargetCamera.transform.position));
                var size = TargetCamera.transform.InverseTransformDirection (
                    TargetCamera.ViewportToWorldPoint (new Vector3 (1f, 1f, z))
                    - TargetCamera.ViewportToWorldPoint (new Vector3 (0f, 0f, z)));
                return size;
            }
            #endregion

            public void Update(Camera nextCam) {
                TargetCamera = (nextCam == null ? Camera.main : nextCam);
            }
        }
    }
}
