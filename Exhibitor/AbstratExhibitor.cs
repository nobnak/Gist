using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    public enum MVVMComponent { Model = 0, ViewModel, View }

    public abstract class AbstractExhibitor : MonoBehaviour {

		#region IExhibitor
		public abstract string SerializeToJson();
		public abstract void DeserializeFromJson(string json);
		public abstract object RawData();

		public virtual void Draw() { }
        public virtual void ResetView() { }
        public virtual void ApplyViewModelToModel() { }
        public virtual void ResetViewModelFromModel() { }

        public virtual void ReflectChangeOf(MVVMComponent latestOne) {
            switch (latestOne) {
                case MVVMComponent.Model:
                    ResetViewModelFromModel();
                    ResetView();
                    break;
                case MVVMComponent.ViewModel:
                    ApplyViewModelToModel();
                    ResetView();
                    break;
                case MVVMComponent.View:
                    ApplyViewModelToModel();
                    break;
            }
        }
        #endregion
    }
}
