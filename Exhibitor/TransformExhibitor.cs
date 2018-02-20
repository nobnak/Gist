using nobnak.Gist;
using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    [ExecuteInEditMode]
    public class TransformExhibitor : MonoBehaviour {

        public Layer layer;
        public Transform parent;
        public Transform nodefab;

        [SerializeField] protected Data data;

        protected List<Transform>nodes = new List<Transform>();
        protected Validator validator = new Validator();

        #region Unity
        private void OnEnable() {
            validator.Reset();
            validator.Validation += () => {
                Clear();
                foreach (var exhibit in data.exhibits) {
                    var n = Instantiate(nodefab);
                    n.gameObject.name = exhibit.name;
                    n.Decode(exhibit.node);
                    Add(n);
                }
            };
            validator.CheckValidation();
        }
        private void Update() {
            validator.CheckValidation();
        }
        private void OnValidate() {
            validator.Invalidate();
        }
        private void OnDisable() {
            Clear();
        }
        #endregion

        #region List
        private void Add(Transform n) {
            n.gameObject.hideFlags = HideFlags.DontSave;
            n.SetParent(parent, false);
            nodes.Add(n);
            n.NotifySelf<IExhibitorListener>(l => l.ExhibitorOnParent(parent));
        }
        private void Remove(Transform n) {
            nodes.Remove(n);
            n.NotifySelf<IExhibitorListener>(l => l.ExhibitorOnUnparent(parent));
        }
        private void Clear() {
            var removelist = new List<Transform>(nodes);
            foreach (var n in removelist) {
                if (n == null)
                    continue;
                Remove(n);
                ObjectDestructor.Destroy(n.gameObject);
            }
            nodes.Clear();
        }
        #endregion

        [System.Serializable]
        public class Data {
            public Exhibit[] exhibits;
        }
        [System.Serializable]
        public struct Exhibit {
            public string name;
            public TransformData node;
        }
        [System.Serializable]
        public struct TransformData {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
        }
    }

    public static class FieldStorageExt { 
        public static TransformExhibitor.TransformData Encode(this Transform tr) {
            return new TransformExhibitor.TransformData() {
                position = tr.localPosition,
                rotation = tr.localRotation,
                scale = tr.localScale
            };
        }
        public static void Decode(this Transform tr, TransformExhibitor.TransformData ndata) {
            tr.localPosition = ndata.position;
            tr.localRotation = ndata.rotation;
            tr.localScale = ndata.scale;
        }
    }
}
