using UnityEngine;
using System.Collections;
using nobnak.Gist.Layers;

namespace nobnak.Gist {
    
    [ExecuteInEditMode]
    public class LayerSNTG : AbstractSimplexNoiseTextureGeenerator {

        [SerializeField]
        protected AbstractLayer layer;

        #region Unity
        protected override void OnEnable() {
            base.OnEnable();
        }
        protected override void Update () {
            SetAspect (CalcAspect());
            base.Update ();
        }
        #endregion

        #region implemented abstract members of SimplexNoiseTextureGeenerator
        protected override Vector2 WorldToViewportPoint (Vector3 worldPos) {
            return layer.ProjectOnNormalized (worldPos);
        }
        protected override Vector3 TransformDirection (Vector3 localDir) {
            return layer.transform.TransformDirection (localDir);
        }
        #endregion

        protected float CalcAspect () {
            var s = layer.transform.localScale;
            var aspect = s.x / s.y;
            return aspect;
        }
    }
}