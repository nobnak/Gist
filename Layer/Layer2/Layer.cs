using nobnak.Gist.Primitive;
using UnityEngine;

namespace nobnak.Gist.Layer2 {

    [ExecuteInEditMode]
    public class Layer : MonoBehaviour, ILayer {

        public const string MSG_CROWN_LAYER = "CrownLayer";
        public const float EPSILON = 1e-3f;
        public const float CIRCLE_INV_DEG = 1f / 360;

        public Layer() {
            LayerValidator = new Validator();

            LayerToWorld = new DefferedMatrix();
            LocalToLayer = new DefferedMatrix();
            LocalToWorld = new DefferedMatrix();
        }

        #region Unity
        protected virtual void OnEnable() {
            LayerValidator.Reset();
            LayerValidator.Validation += () => {
                transform.hasChanged = false;
                GenerateLayerData();
            };
            LayerValidator.SetCheckers(() => !transform.hasChanged);
            BroadcastCrownLayer();
        }
        protected virtual void Update() {
        }
        protected virtual void OnValidate() {
            LayerValidator.Invalidate();
        }
        protected virtual void OnDisable() {

        }
        #endregion

        #region ILayer
        public virtual Validator LayerValidator { get; protected set; }

        public DefferedMatrix LayerToWorld { get; protected set; }
        public DefferedMatrix LocalToLayer { get; protected set; }
        public DefferedMatrix LocalToWorld { get; protected set; }

        public virtual bool Raycast(Ray ray, out float distance) {
            distance = default(float);

            var n = transform.forward;
            var c = transform.position;
            var det = Vector3.Dot(n, ray.direction);
            if (-EPSILON < det && det < EPSILON)
                return false;

            distance = Vector3.Dot(n, c - ray.origin) / det;
            return true;
        }
        #endregion
        
        protected virtual void BroadcastCrownLayer() {
            BroadcastMessage(MSG_CROWN_LAYER, this, SendMessageOptions.DontRequireReceiver);
        }
        protected virtual void GenerateLayerData() {
            var layer = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            var local = Matrix4x4.Scale(transform.localScale);
            LayerToWorld.Reset(layer);
            LocalToLayer.Reset(local);
            LocalToWorld.Reset(layer, local);
        }

        public interface IMessageReceiver {
            void CrownLayer(Layer layer);
        }
    }
}
