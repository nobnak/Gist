using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.CameraExtension {
    
    public static class CameraTransformExtension {
        public static Vector3 ViewportToLocalPosition(this Camera c, Vector3 viewportPos) {
            return c.transform.InverseTransformPoint (
                c.ViewportToWorldPoint (viewportPos));
        }
        public static Vector3 LocalToViewportPosition(this Camera c, Vector3 localPos) {
            return c.WorldToViewportPoint(
                c.transform.TransformPoint(localPos));
        }
	}
}
