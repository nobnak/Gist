using nobnak.Gist.Events;
using UnityEngine;

namespace nobnak.Gist.Scoped {

    [ExecuteAlways]
    public class ObjectModal : MonoBehaviour {
		[SerializeField]
		protected bool isActive;
		[SerializeField]
		protected EventHolder events = new EventHolder();

		protected Validator validator = new Validator();

		public ObjectModal() {
			validator.Reset();
			validator.Validation += () => {
				events.activateDuringActive.Invoke(isActive);
				events.activateDuringInactive.Invoke(!isActive);
			};
		}

		#region unity
		private void OnEnable() {
			validator.Validate();
		}
		private void Update() {
			validator.Validate();
		}
		private void OnDisable() {
			validator.Invalidate();
		}
		#endregion

		#region interface
		public bool Activity {
			get { return isActive; }
			set {
				if (isActive != value) {
					isActive = value;
					validator.Validate(true);
				}
			}
		}
		#endregion

		#region classes
		[System.Serializable]
		public class EventHolder {
			public BoolEvent activateDuringActive = new BoolEvent();
			public BoolEvent activateDuringInactive = new BoolEvent();
		}
		#endregion
	}
}
