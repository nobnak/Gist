using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nobnak.Gist.Layers {

    [ExecuteInEditMode]
    public class TransformLookatLayer : BaseLookatLayer {

        public Transform target;

        protected Master master;

        #region implemented abstract members of BaseLookatLayer
        protected override IMaster GetMaster () {
            master.Update (target);
            return master;
        }
        #endregion

        protected override void InitLayer () {
            base.InitLayer ();
            master = new Master (this);
        }

        public class Master : IMaster {
            public Transform Target { get; protected set; }

            protected TransformLookatLayer parent;

            public Master(TransformLookatLayer parent) {
                this.parent = parent;
            }

            #region IMaster implementation
            public Quaternion Rotation () {
                return Target.rotation;
            }
            public Vector3 Size () {
                var size = Target.localScale;
                return size;
            }
            #endregion

            public void Update(Transform next) {
                Target = next;
            }
        }
    }
}
