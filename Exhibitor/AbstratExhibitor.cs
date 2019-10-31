using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using nobnak.Gist.ObjectExt;

namespace nobnak.Gist.Exhibitor {

	public abstract class AbstractExhibitor : MonoBehaviour, IExhibitor {

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
