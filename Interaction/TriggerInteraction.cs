using K.Model;
using nobnak.Gist.ObjectExt;
using nobnak.Gist.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Interaction {

    public class TriggerInteraction : MonoBehaviour {

        [SerializeField]
        protected Camera targetCamera;
        [SerializeField]
        protected Collider fab;

        [SerializeField]
        protected bool showDebug;
        [SerializeField]
        protected float length = 100f;
        [SerializeField]
        protected float sizeScale = 0.1f;
        [SerializeField]
        protected float duration = 0.5f;

        protected List<Collider> colliders = new List<Collider>();
        protected MemoryPool<Collider> pool;
        protected Vector3 plane;

        #region unity
        void OnEnable() {
            pool = new MemoryPool<Collider>(
                () => {
                    var c = Instantiate(fab);
                    var r = c.GetComponent<Renderer>();
                    if (r != null)
                        r.enabled = showDebug;
                    c.gameObject.SetActive(false);
                    return c;
                },
                c => {
                    c.gameObject.SetActive(false);
                },
                c => c.DestroyGo());
        

            if (targetCamera == null && (targetCamera = GetComponent<Camera>()) == null)
                targetCamera = Camera.main;
            plane = Vector3.Project(targetCamera.transform.position, targetCamera.transform.forward);

        }
        void OnDisable() {
            ClearColliders();
            if (pool != null) {
                pool.Dispose();
                pool = null;
            }
        }
        void Update() {
            ClearColliders();

            if (Input.GetMouseButtonDown(0)) {
                var size = sizeScale * Vector2.one;
                Add(size);
            }
        }
        #endregion

        #region member
        private void ClearColliders() {
            foreach (var c in colliders)
                pool.Free(c);
            colliders.Clear();
        }
        private void Add(Vector2 size) {
            var pos = targetCamera.ScreenToWorldPoint(Input.mousePosition);
            pos = Vector3.ProjectOnPlane(pos - plane, plane) + plane;
            var uv = (Vector2)targetCamera.ScreenToViewportPoint(Input.mousePosition);
            var h = targetCamera.orthographicSize * 2f;
            var w = targetCamera.aspect * h;

            var c = pool.New();
            c.transform.SetParent(transform, false);
            c.transform.position = pos + targetCamera.transform.forward
                * (2f * targetCamera.nearClipPlane * 2f + 0.5f * length);
            c.transform.rotation = targetCamera.transform.rotation;
            c.transform.localScale = new Vector3(h * size.x, w * size.y, length);
            c.gameObject.SetActive(true);
            colliders.Add(c);
        }
        #endregion

        #region definition
        public struct CollisionInfo {
            public readonly float birthTime;
            public readonly Collider collider;

            public CollisionInfo(float birthTime, Collider collider) {
                this.birthTime = birthTime;
                this.collider = collider;
            }
            public CollisionInfo(Collider collider) : this(KenkenUtil.CurrTime, collider) {}
        }
        #endregion
    }
}
