using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Events {

    [ExecuteAlways]
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyListener : MonoBehaviour {
        [SerializeField]
        protected ColliderEvent TriggerEnter = new ColliderEvent();
        [SerializeField]
        protected ColliderEvent TriggerStay = new ColliderEvent();
        [SerializeField]
        protected ColliderEvent TriggerExit = new ColliderEvent();

        private void OnTriggerEnter(Collider other) {
            TriggerEnter.Invoke(other);
        }
        private void OnTriggerStay(Collider other) {
            TriggerStay.Invoke(other);
        }
        private void OnTriggerExit(Collider other) {
            TriggerExit.Invoke(other);
        }

        #region classes
        [System.Serializable]
        public class ColliderEvent : UnityEngine.Events.UnityEvent<Collider> { }
        #endregion
    }
}
