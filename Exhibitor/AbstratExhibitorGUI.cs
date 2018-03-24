using nobnak.Gist.Extensions.ComponentExt;
using nobnak.Gist.Layer2;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using nobnak.Gist.ObjectExt;

namespace nobnak.Gist.Exhibitor {

	public abstract class AbstratExhibitorGUI : MonoBehaviour, IExhibitor {

		#region IExhibitor
		public abstract void Invalidate();
		public abstract string SerializeToJson();
		public abstract void DeserializeFromJson(string json);
		public abstract object RawData();
		#endregion

		#region GUI
		public abstract void Draw();
		#endregion
	}
}
