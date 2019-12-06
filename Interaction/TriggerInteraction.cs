using nobnak.Gist.Extensions.ScreenExt;
using nobnak.Gist.ObjectExt;
using nobnak.Gist.Pooling;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Interaction {

	public class TriggerInteraction : MonoBehaviour {

        [SerializeField]
        protected Camera targetCamera;
        [SerializeField]
        protected ColliderInfo fab;

        [SerializeField]
        protected bool showDebug;
        [SerializeField]
        protected float length = 100f;
        [SerializeField]
        protected float sizeScale = 0.1f;
        [SerializeField]
        protected float duration = 0.5f;

        protected List<ColliderInfo> colliders = new List<ColliderInfo>();
        protected MemoryPool<ColliderInfo> pool;
        protected Vector3 plane;
        protected Validator validator = new Validator();

        #region interface
        public void Add(Vector2 uvPos, Vector2 normSize, params object[] data) {
            var pos = targetCamera.ViewportToWorldPoint(uvPos);

            var h = targetCamera.orthographicSize * 2f;
            var aspect = targetCamera.aspect;
            var w = aspect * h;
            var worldSize = new Vector3(h * normSize.x, h * normSize.y, length);
            //worldSize = targetCamera.transform.TransformVector(worldSize);
            Debug.Log($"Touch Size in world coord: {worldSize}");

            Add(pos, worldSize, data);
        }
        public void Add(Vector3 worldPos, Vector3 worldSize, params object[] data) {
            validator.Invalidate();
            var forward = targetCamera.transform.forward;
            var near = targetCamera.nearClipPlane;
            worldPos = Vector3.ProjectOnPlane(worldPos - plane, plane) + plane;
            worldPos += forward * (2f * near * 2f + 0.5f * length);

            var c = pool.New();

            c.transform.SetParent(transform, false);
            c.transform.position = worldPos;
            c.transform.rotation = targetCamera.transform.rotation;
            c.transform.localScale = worldSize;

            c.Data = data;

            colliders.Add(c);
            c.gameObject.SetActive(true);
        }
        #endregion

        #region unity
        void OnEnable() {
            pool = new MemoryPool<ColliderInfo>(
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
                var uvSize = sizeScale * Vector2.one;
                var uvPos = Input.mousePosition.UV();
                Add(uvPos, uvSize);
            }

            validator.Validate();
        }
        #endregion

        #region member
        private void ClearColliders(bool all = false) {
            var expirationTime = CurrTime - duration;
            for (var i = 0; i < colliders.Count; ) {
                var ci = colliders[i];
                if (all || ci.birthTime < expirationTime) {
                    colliders.RemoveAt(i);
                    pool.Free(ci);
                    validator.Invalidate();
                } else {
                    i++;
                }
            }
        }

        private void SetVisibility(bool showDebug) {
            foreach (var c in colliders) {
                var r = c.GetComponent<Renderer>();
                if (r != null)
                    r.enabled = showDebug;
            }
        }
        #endregion

        #region static
        public static float CurrTime {
            get { return Time.realtimeSinceStartup; }
        }
        #endregion
    }
}
