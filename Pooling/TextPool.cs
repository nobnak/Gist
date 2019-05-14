using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Pooling {
    
    public class TextPool : MonoBehaviour {
        [SerializeField] protected TextMesh fab;

        protected MemoryPool<TextMesh> pool;

        #region Unity
        protected virtual void OnEnable() {
            pool = new MemoryPool<TextMesh>(
                () => Instantiate(fab, transform),
                (tm) => tm.gameObject.SetActive(false),
                (tm) => tm.DestroyGo());
        }
        protected virtual void OnDisable() {
            if (pool != null) {
                pool.Dispose();
                pool = null;
            }
        }
        #endregion
    }
}
