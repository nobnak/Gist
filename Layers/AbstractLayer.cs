using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gimp.Layers {

    public abstract class AbstractLayer : MonoBehaviour {
        public UnityEvent Changed;

        [SerializeField]
        protected Color gizmoFieldColor = Color.grey;

        protected TransformCache cacheTr;
        protected Rect field;

        #region Unity
        protected virtual void OnEnable() {
            UpdateAll ();
        }
        protected virtual void Update() {
            UpdateAll ();
        }
        protected virtual void OnDrawGizmos() {
            if (!isActiveAndEnabled || !cacheTr.initialized)
                return;
            
            var size = field.size;

            Gizmos.color = gizmoFieldColor;
            Gizmos.matrix = Matrix4x4.TRS (cacheTr.position, cacheTr.rotation, Vector3.one);
            Gizmos.DrawWireCube (field.center, new Vector3(size.x, size.y, 0f));

            Gizmos.matrix = Matrix4x4.identity;
        }
        #endregion

        public virtual Rect Field { get { return field; } }

        public virtual Vector3 ProjectOn(Vector3 p, out Vector3 distance) {
            var arrow = p - cacheTr.position;
            distance = Vector3.Dot (transform.forward, arrow) * transform.forward;
            return p - distance;
        }
        public virtual Vector3 ProjectOn(Vector3 p) {
            Vector3 distance;
            return ProjectOn (p, out distance);
        }
        public virtual Vector3 Offset(float xNormalized, float yNormalized) {
            var x = Mathf.LerpUnclamped (field.xMin, field.xMax, xNormalized);
            var y = Mathf.LerpUnclamped (field.yMin, field.yMax, yNormalized);
            return x * transform.right + y * transform.up;
        }
        public virtual Vector3 Position(float xNormalized, float yNormalized) {
            return cacheTr.position + Offset (xNormalized, yNormalized);
        }
        public virtual Vector2 ProjectOnNormalized(Vector3 p) {
            var arrow = p - cacheTr.position;
            var size = field.size;
            var offset = field.min;
            var xNormalized = (Vector2.Dot (arrow, transform.right) - offset.x) / size.x;
            var yNormalized = (Vector2.Dot (arrow, transform.up) - offset.y) / size.y;
            return new Vector2 (xNormalized, yNormalized);
        }
            
        protected abstract bool UpdateLayer ();
        protected virtual void UpdateCache() {
            cacheTr = new TransformCache (transform);
        }
        protected virtual void NotifyOnChanged() {
            if (Changed != null)
                Changed.Invoke ();
        }
        protected virtual void UpdateAll () {
            if (UpdateLayer ()) {
                UpdateCache ();
                NotifyOnChanged ();
            }
        }

        #region Classes
        public struct TransformCache {
            public readonly bool initialized;
            public readonly Vector3 position;
            public readonly Quaternion rotation;

            public TransformCache(Transform tr) {
                this.initialized = true;
                this.position = tr.position;
                this.rotation = tr.rotation;
            }
        }
        #endregion
    }
}
