using nobnak.Gist;
using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    [ExecuteInEditMode]
    public class Transform2DExhibitor
        : ListExhibitor<Transform, Transform2DExhibitor.Data> {
        [SerializeField]
        protected Data data;
        [SerializeField]
        protected Settings settings;

        #region interfaces

        #region ListExhibitor
        public override Data CurrentData {
            get { return data; }
            set {
                data = value;
                ReflectChangeOf(MVVMComponent.ViewModel);
            }
        }
        public override void ResetNodesFromData() {
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
        public Transform Decode(Transform tr, Transform2DExhibitor.TransformData ndata) {
            tr.localPosition = settings.ConvertPosition(ndata.position, 0f);
            tr.localRotation = Quaternion.Euler(settings.ConvertRotation(ndata.rotation));
            tr.localScale = settings.ConvertPosition(ndata.scale, 1f);
            return tr;
        }
        #endregion

        #region Classes
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
            public Vector2 position;
            public float rotation;
            public Vector2 scale;
        }

        [System.Serializable]
        public class Settings {
            public Vector2Int indices = new Vector2Int(0, 1);

            public Vector3 ConvertPosition(Vector2 v2, float z = 0f) {
                var v3 = Vector3.zero;
                v3[indices.x] = v2.x;
                v3[indices.y] = v2.y;
                v3[IndexOfZ] = z;
                return v3;
            }
            public Vector2 ConvertPosition(Vector3 v3, out float z) {
                z = v3[IndexOfZ];
                return new Vector2(v3[indices.x], v3[indices.y]);
            }
            public Vector2 ConvertPosition(Vector3 v3) {
                float z;
                return ConvertPosition(v3, out z);
            }
            public Vector3 ConvertRotation(float r) {
                var v3 = Vector3.zero;
                v3[IndexOfZ] = r;
                return v3;
            }

            public int IndexOfZ {
                get { return 3 - (indices.x + indices.y); }
            }
        }
        #endregion
    }
}
