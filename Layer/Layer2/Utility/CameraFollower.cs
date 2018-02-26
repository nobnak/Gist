using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Layer2 {
    [ExecuteInEditMode]
    public class CameraFollower : MonoBehaviour {
        [SerializeField]
        protected Camera source;
        [SerializeField]
        protected Layer target;

        void Update() {
            if (source == null || target == null)
                return;

            if (FollowScale() | FollowRotation()) {
                target.LayerValidator.Invalidate();
            }
        }

        private bool FollowRotation() {
            var curr = target.transform.localRotation;
            var next = source.transform.localRotation;
            if (curr == next)
                return false;
            target.transform.localRotation = next;
            return true;
        }

        private bool FollowScale() {
            var curr = target.transform.localScale;
            if (source.orthographic) {
                var orthoSize = 2f * source.orthographicSize;
                var next = new Vector3(orthoSize, orthoSize, 1f);
                if (curr != next) {
                    target.transform.localScale = next;
                    return true;
                }
            }
            return false;
        }
    }
}
