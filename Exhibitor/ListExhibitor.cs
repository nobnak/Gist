using ModelDrivenGUISystem;
using ModelDrivenGUISystem.Factory;
using ModelDrivenGUISystem.ValueWrapper;
using ModelDrivenGUISystem.View;
using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using nobnak.Gist.ObjectExt;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    public abstract class ListExhibitor<ArtWorkType, DataTransformType> 
        : AbstractExhibitor
        where ArtWorkType : Component {

        public Layer layer;
        public Transform parent;
        public ArtWorkType nodefab;

        protected List<ArtWorkType> nodes = new List<ArtWorkType>();
        protected Validator dataValidator = new Validator();

        protected BaseView view;

        #region Unity
        protected virtual void OnEnable() {
            dataValidator.Reset();
            dataValidator.Validation += () => {
                ResetNodesFromData();
            };
            dataValidator.Validate();
        }
        protected virtual void Update() {
            dataValidator.Validate();
        }
        protected virtual void OnValidate() {
            dataValidator.Invalidate();
            ClearView();
        }
        protected virtual void OnDisable() {
            Clear();
        }
        #endregion

        #region interfaces

        #region List
        protected virtual void Add(ArtWorkType n) {
            n.gameObject.hideFlags = HideFlags.DontSave;
            n.transform.SetParent(parent, false);
            n.hideFlags = HideFlags.DontSave;
            nodes.Add(n);
            n.CallbackSelf<IExhibitorListener>(l => l.ExhibitorOnParent(parent));
        }
        protected virtual void AddRange(IEnumerable<ArtWorkType> niter) {
            foreach (var n in niter)
                Add(n);
        }
        protected virtual void Remove(ArtWorkType n) {
            nodes.Remove(n);
            n.CallbackSelf<IExhibitorListener>(l => l.ExhibitorOnUnparent(parent));
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
				n.DestroyGo();
            }
            nodes.Clear();
        }
        #endregion

        #region IExhibitor
        public override void Invalidate() { dataValidator.Invalidate(); }
        public override string SerializeToJson() {
            return JsonUtility.ToJson(CurrentData);
        }
        public override void DeserializeFromJson(string json) {
            CurrentData = JsonUtility.FromJson<DataTransformType>(json);
        }
		public override object RawData() { return CurrentData; }
        public override void Draw() {
            GetView().Draw();
        }
        #endregion

        public abstract DataTransformType CurrentData { get; set; }
        public abstract void ResetNodesFromData();

        public virtual Validator Validator { get { return dataValidator; } }
        #endregion

        public virtual BaseView GetView() {
            dataValidator.Validate();
            if (view == null) {
                var f = new SimpleViewFactory();
                view = ClassConfigurator.GenerateClassView(new BaseValue<object>(CurrentData), f);
            }
            return view;
        }
        public virtual void ClearView() {
            if(view != null) {
                view.Dispose();
                view = null;
            }
        }
    }
}
