using K.Model;
using nobnak.Gist.Extensions.ScreenExt;
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

        protected List<ColliderInfo> colliders = new List<ColliderInfo>();
        protected MemoryPool<Collider> pool;
        protected Vector3 plane;
        protected Validator validator = new Validator();

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
                    c.transform.SetParent(null);
                },
                c => c.DestroyGo());

            validator.Reset();
            validator.Validation += () => {
                SetVisibility(showDebug);
            };

            if (targetCamera == null && (targetCamera = GetComponent<Camera>()) == null)
                targetCamera = Camera.main;
            plane = Vector3.Project(targetCamera.transform.position, targetCamera.transform.forward);

        }

        void OnDisable() {
            ClearColliders(true);
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

            validator.Validate();
        }
        #endregion

        #region member
        private void ClearColliders(bool all = false) {
            var expirationTime = KenkenUtil.CurrTime - duration;
            for (var i = 0; i < colliders.Count; ) {
                var ci = colliders[i];
                if (all || ci.birthTime < expirationTime) {
                    colliders.RemoveAt(i);
                    pool.Free(ci.collider);
                    validator.Invalidate();
                } else {
                    i++;
                }
            }
        }
        private void Add(Vector2 size) {
            validator.Invalidate();
            var uv = Input.mousePosition.UV();
            var pos = targetCamera.ViewportToWorldPoint(uv);
            pos = Vector3.ProjectOnPlane(pos - plane, plane) + plane;
            var h = targetCamera.orthographicSize * 2f;
            var w = targetCamera.aspect * h;

            var c = pool.New();
            c.transform.SetParent(transform, false);
            c.transform.position = pos + targetCamera.transform.forward
                * (2f * targetCamera.nearClipPlane * 2f + 0.5f * length);
            c.transform.rotation = targetCamera.transform.rotation;
            c.transform.localScale = new Vector3(h * size.x, w * size.y, length);
            c.gameObject.SetActive(true);
            var ci = new ColliderInfo(c);
            colliders.Add(ci);
        }
        private void SetVisibility(bool showDebug) {
            foreach (var c in colliders) {
                var r = c.collider.GetComponent<Renderer>();
                if (r != null)
                    r.enabled = showDebug;
            }
        }
        #endregion

        #region definition
        public struct ColliderInfo {
            public readonly float birthTime;
            public readonly Collider collider;

            public ColliderInfo(float birthTime, Collider collider) {
                this.birthTime = birthTime;
                this.collider = collider;
            }
            public ColliderInfo(Collider collider) : this(KenkenUtil.CurrTime, collider) {}
        }
        #endregion
    }
}
