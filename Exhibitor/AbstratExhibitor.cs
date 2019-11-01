using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    public abstract class AbstractExhibitor : MonoBehaviour {

		#region IExhibitor
		public abstract string SerializeToJson();
		public abstract void DeserializeFromJson(string json);
		public abstract object RawData();

		public virtual void Draw() { }
        public virtual void ApplyViewModelToModel() { }
        public virtual void ResetViewModelFromModel() { }

        public virtual void Invalidate() {
            ApplyViewModelToModel();
        }
        #endregion
    }
}
