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
        public virtual Vector3 Offset(float x, float y) {
            x = Mathf.LerpUnclamped (field.xMin, field.xMax, x);
            y = Mathf.LerpUnclamped (field.yMin, field.yMax, y);
            return x * transform.right + y * transform.up;
        }
        public virtual Vector3 Position(float x, float y) {
            return cacheTr.position + Offset (x, y);
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
