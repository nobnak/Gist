using UnityEngine;
using System.Collections;

namespace nobnak.Gist {
    
    public static class LensShift {
        
        public static void NearPlane(this Camera cam, out float left, out float right, out float bottom, out float top) {
            NearPlane(cam.nearClipPlane, cam.aspect, cam.fieldOfView, out left, out right, out bottom, out top);
        }
        public static void NearPlane(float near, float aspect, float fieldOfView, out float left, out float right, out float bottom, out float top) {
            var tan = Mathf.Tan (0.5f * fieldOfView * Mathf.Deg2Rad);
            top = near * tan;
            bottom = -top;
            right = top * aspect;
            left = -right;
        }
        
        public static void Shift(float shiftX, float shiftY, ref float left, ref float right, ref float top, ref float bottom) {
			var offset_y = top * shiftY;
			var offset_x = right * shiftX;
			top += offset_y; bottom += offset_y;
			right += offset_x; left += offset_x;
		}
        public static void Perspective(this Camera cam, float left, float right, float bottom, float top, float near, float far) {
            cam.projectionMatrix = Perspective (left, right, bottom, top, near, far);
        }
		public static Matrix4x4 Perspective(float left, float right, float bottom, float top, float near, float far) {
			float x = 2.0F * near / (right - left);
			float y = 2.0F * near / (top - bottom);
			float a = (right + left) / (right - left);
			float b = (top + bottom) / (top - bottom);
			float c = -(far + near) / (far - near);
			float d = -(2.0F * far * near) / (far - near);
			float e = -1.0F;

			Matrix4x4 m = Matrix4x4.zero;
			m[ 0]=x;          m[ 8]=a;
			         m[ 5]=y; m[ 9]=b;
			                  m[10]=c; m[14]=d;
			                  m[11]=e;

			return m;
		}
	}
}
