using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace nobnak.Gist.Layers {

    [ExecuteInEditMode]
    public abstract class BaseLookatLayer : AbstractLayer {
        public enum ScaleModeEnum { Fixed = 0, Scale }

        [SerializeField]
        protected ScaleModeEnum scaleMode;

        protected ScaleModeEnum prevScaleMode;

        protected abstract IMaster GetMaster ();

        protected override void InitLayer () {
            prevScaleMode = scaleMode;
        }
        protected override bool UpdateLayer () {
            var changed = transform.hasChanged;
            transform.hasChanged = false;

            var master = GetMaster ();

            var targetRotation = master.Rotation ();
            if (targetRotation != transform.rotation) {
                changed = true;
                transform.rotation = targetRotation;
            }

            var size = master.Size ();
            if (SetSize(size) || scaleMode != prevScaleMode) {
                changed = true;
                prevScaleMode = scaleMode;
                switch (scaleMode) {
                case ScaleModeEnum.Scale:
                    transform.localScale = new Vector3 (size.x, size.y, 1f);
                    break;
                }
            }

            return changed;
    	}

        public interface IMaster {
            Quaternion Rotation();
            Vector3 Size();
        }
    }
}
