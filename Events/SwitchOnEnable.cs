using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Events {

    [ExecuteAlways]
    public class SwitchOnEnable : MonoBehaviour {

        [SerializeField]
        protected Events events = new Events();

        private void OnEnable() {
            Notify(true);
        }
        private void OnValidate() {
            Notify(enabled);
        }
        private void OnDisable() {
            Notify(false);
        }

        private void Notify(bool e) {
            events.Enabled.Invoke(e);
            events.Disabled.Invoke(!e);
        }

        [System.Serializable]
        public class Events {
            public BoolEvent Enabled = new BoolEvent();
            public BoolEvent Disabled = new BoolEvent();
        }
    }
}
