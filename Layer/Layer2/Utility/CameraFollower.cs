using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Layer2 {
    [ExecuteInEditMode]
    public class CameraFollower : MonoBehaviour {
		public enum ScaleMode { Uniform = 0, Viewport }

        [SerializeField]
        protected Camera source;
        [SerializeField]
        protected Layer target;

		[SerializeField]
		protected ScaleMode scaleMode;

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
			Vector3 next = Vector3.one;

			var curr = target.transform.localScale;
            if (source.orthographic) {
				var orthoSize = 2f * source.orthographicSize;
				next = orthoSize * Vector3.one;
				switch (scaleMode) {
					case ScaleMode.Viewport:
						var aspect = source.aspect;
						next = new Vector3(orthoSize * aspect, orthoSize, 1f);
						break;
				}
            }

			if (curr != next) {
				target.transform.localScale = next;
				return true;
			}
			return false;
        }
    }
}
