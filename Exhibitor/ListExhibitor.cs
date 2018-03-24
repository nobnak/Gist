using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using nobnak.Gist.ObjectExt;

namespace nobnak.Gist.Exhibitor {

    public abstract class ListExhibitor<ArtWorkType, DataTransformType> 
        : MonoBehaviour, IExhibitor
        where ArtWorkType : Component {

        public Layer layer;
        public Transform parent;
        public ArtWorkType nodefab;

        protected List<ArtWorkType> nodes = new List<ArtWorkType>();
        protected Validator validator = new Validator();

        public virtual Validator Validator { get { return validator; } }

        public abstract DataTransformType CurrentData { get; set; }
        protected abstract void Validate();

        #region Unity
        protected virtual void OnEnable() {
            validator.Reset();
            validator.Validation += () => Validate();
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
        protected virtual void AddRange(IEnumerable<ArtWorkType> niter) {
            foreach (var n in niter)
                Add(n);
        }
        protected virtual void Remove(ArtWorkType n) {
            nodes.Remove(n);
            n.NotifySelf<IExhibitorListener>(l => l.ExhibitorOnUnparent(parent));
        }
        protected virtual void Removerange(IEnumerable<ArtWorkType> niter) {
            foreach (var n in niter)
                Remove(n);
        }
        protected virtual void Clear() {
            var removelist = new List<ArtWorkType>(nodes);
            foreach (var n in removelist) {
                if (n == null)
                    continue;
                Remove(n);
				n.Destroy();
            }
            nodes.Clear();
        }
        #endregion

        #region IExhibitor
        public virtual void Invalidate() { validator.Invalidate(); }
        public virtual string SerializeToJson() {
            return JsonUtility.ToJson(CurrentData);
        }
        public virtual void DeserializeFromJson(string json) {
            CurrentData = JsonUtility.FromJson<DataTransformType>(json);
        }
		public virtual object RawData() { return CurrentData; }
        #endregion

    }
}
