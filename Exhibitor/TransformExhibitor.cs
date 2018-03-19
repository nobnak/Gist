using nobnak.Gist;
using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    [ExecuteInEditMode]
    public class TransformExhibitor : ListExhibitor<Transform, TransformExhibitor.Data> {
        [SerializeField]
        protected Data data;
        
        #region ListExhibitor
        public override Data CurrentData {
            get { return data; }
            set {
                data = value;
                Invalidate();
            }
        }
        protected override void Validate() {
            Clear();
            if (data != null && data.exhibits != null)
                AddRange(data.exhibits.Select(i => {
                    var n = Instantiate(nodefab);
                    return Decode(n, i);
                }));
        }
        #endregion

        public virtual Transform Decode(Transform node, Exhibit info) {
            node.gameObject.name = info.name;
            return Decode(node, info.node);
        }
        public static Transform Decode(Transform tr, TransformExhibitor.TransformData ndata) {
            tr.localPosition = ndata.position;
            tr.localRotation = Quaternion.Euler(ndata.rotation);
            tr.localScale = ndata.scale;
            return tr;
        }

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

}
