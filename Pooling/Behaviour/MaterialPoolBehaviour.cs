using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Pooling {

	public class MaterialPoolBehaviour : SingletonBehaviour<MaterialPoolBehaviour> {
		protected MaterialPool pool = new MaterialPool();

		#region unity
		protected void OnDestroy() {
			pool.Dispose();
		}
		#endregion

		#region public
		public MaterialPool CurrentPool {
			get { return pool; }
		}
		#endregion
	}
}
