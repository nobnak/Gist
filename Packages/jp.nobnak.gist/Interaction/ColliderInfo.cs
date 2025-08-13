using UnityEngine;

namespace nobnak.Gist.Interaction {

	[ExecuteAlways]
    [RequireComponent(typeof(Collider))]
    public class ColliderInfo : MonoBehaviour {

        [SerializeField]
        protected Collider attachedCollider = null;

        #region interface
        public float birthTime { get; protected set; }
        public object[] Data { get; set; }
        public Collider CurrCollider {
            get { return attachedCollider; }
            set { attachedCollider = value; }
        }
        #endregion

        #region unity
        private void OnEnable() {
            if (attachedCollider == null)
                attachedCollider = GetComponent<Collider>();
            birthTime = TriggerInteraction.CurrTime;
        }
        #endregion
    }
}
