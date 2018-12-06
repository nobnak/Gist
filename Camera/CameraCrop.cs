using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {

    public static class CameraCrop {
        public static void Crop (Camera worldView, Camera[] localViews, Data data) {
            var totalSize = data.totalSize;
            var occupancy = Mathf.Max (0.01f, data.occupancy);
            var normCropX = occupancy / totalSize.x;
            var normCropY = occupancy / totalSize.y;
            var normOffsetX = 2f * (data.offset.x + 0.5f * occupancy) / totalSize.x - 1f;
            var normOffsetY = 2f * (data.offset.y + 0.5f * occupancy) / totalSize.y - 1f;
            var totalAspect = worldView.aspect * data.totalSize.x / totalSize.y;

            float left, right, bottom, top;
            LensShift.NearPlane (worldView.nearClipPlane, totalAspect, worldView.fieldOfView, out left, out right, out bottom, out top);
            var cropRight = right * (normCropX + normOffsetX);
            var cropLeft = right * (-normCropX + normOffsetX);
            var cropTop = top * (normCropY + normOffsetY);
            var cropBottom = top * (-normCropY + normOffsetY);
            worldView.Perspective (cropLeft, cropRight, cropBottom, cropTop,
                worldView.nearClipPlane, worldView.farClipPlane);

            Apply (worldView, localViews, totalAspect);
        }

        public static void Apply(Camera worldView, Camera[] localViews, float totalAspect) {
            if (localViews == null || localViews.Length == 0)
                return;

            var pos = worldView.transform.position;
            var rot = worldView.transform.rotation;
            for (var i = 0; i < localViews.Length; i++) {
                var totalCam = localViews [i];
                if (totalCam == null)
                    continue;
                totalCam.transform.position = pos;
                totalCam.transform.rotation = rot;
                totalCam.ResetProjectionMatrix ();
                totalCam.orthographic = totalCam.orthographic;
                totalCam.fieldOfView = worldView.fieldOfView;
                totalCam.nearClipPlane = worldView.nearClipPlane;
                totalCam.farClipPlane = worldView.farClipPlane;
                totalCam.aspect = totalAspect;

                var fixWidthFlexHeight = (float)worldView.pixelWidth / (worldView.pixelHeight * totalAspect);
                var flexWidthFixHeight = (float)totalAspect * worldView.pixelHeight / worldView.pixelWidth;
                totalCam.rect = (fixWidthFlexHeight < 1f) ?
                    new Rect(0f, 0f, 1f, fixWidthFlexHeight) :
                    new Rect(0f, 0f, flexWidthFixHeight, 1f);
            }
        }
        [System.Serializable]
        public class Data {
            public Vector2 totalSize = new Vector2(1f, 1f);
            public Vector2 offset = Vector2.zero;
            public float occupancy = 1f;
        }
    }
}
