using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.CameraExt {
    
    public static class CameraExtension {
        public static Vector3 ViewportToLocalPosition(this Camera c, Vector3 viewportPos) {
            return c.transform.InverseTransformPoint (
                c.ViewportToWorldPoint (viewportPos));
        }
        public static Vector3 LocalToViewportPosition(this Camera c, Vector3 localPos) {
            return c.WorldToViewportPoint(
                c.transform.TransformPoint(localPos));
        }

		public static float GetHandleSize(this Camera cam, Vector3 worldPos) {
			if (cam.orthographic) {
				return cam.orthographicSize;
			} else {
				var z = -cam.transform.InverseTransformPoint(worldPos).z;
				return Mathf.Max(0f, Mathf.Tan(0.5f * cam.fieldOfView) * z);
			}
		}
	}
}
