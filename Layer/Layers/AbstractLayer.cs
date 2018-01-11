using nobnak.Gist.Extensions.ComponentExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace nobnak.Gist.Layers {

    public abstract class AbstractLayer : MonoBehaviour {
        const float EPSILON = 1e-3f;
        public UnityEvent Changed;

        [SerializeField]
        protected Color gizmoFieldColor = Color.grey;

        protected TransformCache cacheTr;
        protected Rect field;
        protected float aspect;

        #region Unity
        protected virtual void OnEnable() {
            InitLayer ();
            UpdateAll ();
        }
        protected virtual void Update() {
            UpdateAll ();
        }
        protected virtual void OnDrawGizmos() {
            if (!isActiveAndEnabled || !cacheTr.initialized || !this.IsVisibleLayer())
                return;
            
            var size = field.size;

            Gizmos.color = gizmoFieldColor;
            Gizmos.matrix = Matrix4x4.TRS (cacheTr.position, cacheTr.rotation, Vector3.one);
            Gizmos.DrawWireCube (field.center, new Vector3(size.x, size.y, 0f));

            Gizmos.matrix = Matrix4x4.identity;
        }
        #endregion

        public virtual Rect Field { get { return field; } }
        public virtual float Aspect { get { return aspect; } }
        public virtual TransformCache Cache { get { return cacheTr; } }

        public virtual bool SetSize(Vector2 size) {
            var targetField = new Rect(-0.5f * size.x, -0.5f * size.y, size.x, size.y);
            var result = (targetField != field);
            field = targetField;
            aspect = size.x / size.y;
            return result;
        }
        public virtual bool Raycast(Ray ray, out Vector3 position, out float t) {
            position = default(Vector3);
            t = default(float);

            var n = transform.forward;
            var dn = Vector3.Dot (ray.direction, n);
            if (-EPSILON < dn && dn < EPSILON)
                return false;

            t = Vector3.Dot (transform.position - ray.origin, n) / dn;
            position = ray.GetPoint (t);
            return true;
        }
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
            var localPos = transform.InverseTransformPoint (p);
            return new Vector2 (localPos.x + 0.5f, localPos.y + 0.5f);
        }

        protected virtual void InitLayer () {
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
            public readonly Vector3 localScale;

            public TransformCache(Transform tr) {
                this.initialized = true;
                this.position = tr.position;
                this.rotation = tr.rotation;
                this.localScale = tr.localScale;
            }

            public Transform CopyTo(Transform tr) {
                tr.position = position;
                tr.rotation = rotation;
                tr.localScale = localScale;
                return tr;
            }
        }
        #endregion
    }
}
