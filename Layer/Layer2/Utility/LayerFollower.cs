using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Layer2 {

	public class LayerFollower : MonoBehaviour, IChangeListener<Layer> {

		[SerializeField]
		protected ScaleMode scaleMode;

		protected Validator validator = new Validator();
		protected Layer layer;

		#region Unity
		protected virtual void OnEnable() {
			validator.Reset();
			validator.Validation += () => {
				if (layer == null)
					return;

				transform.rotation = layer.transform.rotation;

				var scale = Vector3.one;
				switch (scaleMode) {
					case ScaleMode.LocalScale:
						scale = layer.transform.localScale;
						break;
				}
				transform.localScale = scale;
			};
		}
		protected virtual void OnValidate() {
			validator.Invalidate();
		}
		protected virtual void Update() {
			validator.Validate();
		}
		#endregion

		#region public
		public Layer Layer {
			get { return layer; }
			set {
				validator.Invalidate();
				layer = value;
			}
		}
		public void TargetOnChange(Layer target) {
			Layer = target;
		}
		#endregion

		#region classes
		public enum ScaleMode { Disabled = 0, LocalScale }
		#endregion
	}
}
