using UnityEngine;
using System.Collections;

namespace nobnak.Gist {

    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemMaterialPropertyBlock : MonoBehaviour {
        MaterialPropertyBlockChanied block;

        #region Unity
        void Awake () {
            var ps = GetComponent<ParticleSystem>();
            var renderer = ps.GetComponent<ParticleSystemRenderer> ();
            block = new MaterialPropertyBlockChanied (renderer);
    	}
        #endregion

        public MaterialPropertyBlockChanied Block { get { return block; } }
    }
}
