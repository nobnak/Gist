using nobnak.Gist;
using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {
    
    public abstract class BaseExhibitor<ArtWorkType, ExhibitInfoType, DataTransformType> : MonoBehaviour 
        where ArtWorkType : Component {

        public Layer layer;
        public Transform parent;
        public ArtWorkType nodefab;

        protected List<ArtWorkType> nodes = new List<ArtWorkType>();
        protected Validator validator = new Validator();

        public virtual Validator Validator { get { return validator; } }

        public abstract DataTransformType CurrentData { get; set; }
        public abstract void Decode(ArtWorkType node, ExhibitInfoType info);
        public abstract IEnumerable<ExhibitInfoType> IterateExhibitInfo();

        #region Unity
        protected virtual void OnEnable() {
            validator.Reset();
            validator.Validation += () => {
                Clear();
                foreach (var exhibit in IterateExhibitInfo()) {
                    var n = Instantiate(nodefab);
                    Decode(n, exhibit);
                    Add(n);
                }
            };
            validator.CheckValidation();
        }
        protected virtual void Update() {
            validator.CheckValidation();
        }
        protected virtual void OnValidate() {
            validator.Invalidate();
        }
        protected virtual void OnDisable() {
            Clear();
        }
        #endregion

        #region List
        protected virtual void Add(ArtWorkType n) {
            n.gameObject.hideFlags = HideFlags.DontSave;
            n.transform.SetParent(parent, false);
            nodes.Add(n);
            n.NotifySelf<IExhibitorListener>(l => l.ExhibitorOnParent(parent));
        }
        protected virtual void Remove(ArtWorkType n) {
            nodes.Remove(n);
            n.NotifySelf<IExhibitorListener>(l => l.ExhibitorOnUnparent(parent));
        }
        protected virtual void Clear() {
            var removelist = new List<ArtWorkType>(nodes);
            foreach (var n in removelist) {
                if (n == null)
                    continue;
                Remove(n);
                ObjectDestructor.Destroy(n.gameObject);
            }
            nodes.Clear();
        }
        #endregion

        public virtual void Invalidate() { validator.Invalidate(); }
    }
}
