using UnityEngine;
using System.Collections;

namespace nobnak.Gist {

    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemMaterialPropertyBlock : MonoBehaviour {
        MaterialPropertyBlockMethodChain block;

        #region Unity
        void Awake () {
            var ps = GetComponent<ParticleSystem>();
            var renderer = ps.GetComponent<ParticleSystemRenderer> ();
            block = new MaterialPropertyBlockMethodChain (renderer);
    	}
        #endregion

        public MaterialPropertyBlockMethodChain Block { get { return block; } }
    }
}
