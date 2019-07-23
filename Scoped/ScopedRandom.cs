using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Scoped {

    public class ScopedRandom : System.IDisposable {

        protected Random.State prevState;

        public ScopedRandom(int seed) {
            Push();
            Random.InitState(seed);
        }
        public ScopedRandom(Random.State state) {
            Push();
            Random.state = state;
        }

        public void Dispose() {
            Pop();
        }

        #region mmeber
        private void Push() {
            prevState = Random.state;
        }

        private void Pop() {
            Random.state = prevState;
        }
        #endregion
    }
}
