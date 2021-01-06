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
					NotifyModelChanged();
					break;
				case MVVMComponent.ViewModel:
					NotifyViewModelChanged();
					break;
				case MVVMComponent.View:
					NotifyViewChanged();
					break;
			}
        }

		public virtual void NotifyModelChanged() {
			ResetViewModelFromModel();
			ResetView();
		}
		public virtual void NotifyViewModelChanged() {
			ApplyViewModelToModel();
			ResetView();
		}
		public virtual void NotifyViewChanged() {
			ApplyViewModelToModel();
		}
		#endregion
	}
}
