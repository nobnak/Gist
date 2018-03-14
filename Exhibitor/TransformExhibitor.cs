using nobnak.Gist;
using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    [ExecuteInEditMode]
    public class TransformExhibitor 
        : BaseExhibitor<Transform, TransformExhibitor.Exhibit, TransformExhibitor.Data> {
        [SerializeField]
        protected Data data;

        #region BaseExhibitor
        public override Data CurrentData {
            get { return data; }
            set {
                data = value;
                Invalidate();
            }
        }
        public override void Decode(Transform node, Exhibit info) {
            node.gameObject.name = info.name;
            node.Decode(info.node);
        }
        public override IEnumerable<Exhibit> IterateExhibitInfo() {
            foreach (var exhibit in data.exhibits)
                yield return exhibit;
        }
        #endregion

        [System.Serializable]
        public class Data {
            public Exhibit[] exhibits;
        }
        [System.Serializable]
        public class Exhibit {
            public string name;
            public TransformData node;
        }
        [System.Serializable]
        public class TransformData {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
        }
    }

    public static class FieldStorageExt { 
        public static TransformExhibitor.TransformData Encode(this Transform tr) {
            return new TransformExhibitor.TransformData() {
                position = tr.localPosition,
                rotation = tr.localRotation.eulerAngles,
                scale = tr.localScale
            };
        }
        public static void Decode(this Transform tr, TransformExhibitor.TransformData ndata) {
            tr.localPosition = ndata.position;
            tr.localRotation = Quaternion.Euler(ndata.rotation);
            tr.localScale = ndata.scale;
        }
    }
}
