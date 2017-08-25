using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gist.Layers {

    [ExecuteInEditMode]
    public class CameraLookatLayer : AbstractLayer {
        public enum ScaleModeEnum { Fixed = 0, Scale }

        public Camera targetCamera;

        [SerializeField]
        protected ScaleModeEnum scaleMode;

        ScaleModeEnum prevScaleMode;

        protected override void InitLayer () {
            prevScaleMode = scaleMode;
        }
        protected override bool UpdateLayer () {
            var changed = transform.hasChanged;
            transform.hasChanged = false;

            var cam = (targetCamera == null ? Camera.main : targetCamera);
            var targetRotation = cam.transform.rotation;
            if (targetRotation != transform.rotation) {
                changed = true;
                transform.rotation = targetRotation;
            }

            var z = Vector3.Dot (cam.transform.forward, (cacheTr.position - cam.transform.position));
            var size = cam.transform.InverseTransformDirection (
                cam.ViewportToWorldPoint (new Vector3 (1f, 1f, z)) - cam.ViewportToWorldPoint (new Vector3 (0f, 0f, z)));
            var targetField = new Rect(-0.5f * size.x, -0.5f * size.y, size.x, size.y);
            if (targetField != field || scaleMode != prevScaleMode) {
                changed = true;
                field = targetField;
                prevScaleMode = scaleMode;
                switch (scaleMode) {
                case ScaleModeEnum.Scale:
                    transform.localScale = new Vector3 (size.x, size.y, 1f);
                    break;
                }
            }

            return changed;
    	}
    }
}
