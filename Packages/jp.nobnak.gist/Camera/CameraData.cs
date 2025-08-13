using nobnak.Gist.Extensions.ScreenExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Cameras {

    public struct CameraData : System.IEquatable<Camera> {

        public readonly int cameraId;

        public readonly Matrix4x4 localToWorldMatrix;
        public readonly Matrix4x4 worldToCameraMatrix;
        public readonly Matrix4x4 projectionMatrix;

        public readonly Vector2Int screenSize;
        public readonly RenderTexture targetTexture;

        public CameraData(Camera cam) {
            if (cam == null) {
                this = default;
                return;
            }
            cameraId = cam.GetInstanceID();

            localToWorldMatrix = cam.transform.localToWorldMatrix;
            worldToCameraMatrix = cam.worldToCameraMatrix;
            projectionMatrix = cam.projectionMatrix;

            targetTexture = cam.targetTexture;
            screenSize = cam.Size();
        }

        #region interface

        #region IEquatable
        public bool Equals(Camera other) {
            return other != null
                && cameraId == other.GetInstanceID()
                && localToWorldMatrix.Equals(other.transform.localToWorldMatrix)
                && worldToCameraMatrix.Equals(other.worldToCameraMatrix)
                && projectionMatrix.Equals(other.projectionMatrix)
                && screenSize.Equals(ScreenSize(other))
                && targetTexture == other.targetTexture;
        }
        #endregion

        #endregion

        #region static
        public static Vector2Int ScreenSize(Camera cam) {
            return new Vector2Int(cam.pixelWidth, cam.pixelHeight);
        }
        public static implicit operator CameraData(Camera other) {
            return new CameraData(other);
        }
        #endregion
    }
}
