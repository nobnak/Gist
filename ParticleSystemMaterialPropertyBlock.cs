using nobnak.Gist.Wrapper;
using UnityEngine;

namespace nobnak.Gist {

    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemMaterialPropertyBlock : MonoBehaviour {
        Block block;

        #region Unity
        void Awake () {
            var ps = GetComponent<ParticleSystem>();
            var renderer = ps.GetComponent<ParticleSystemRenderer> ();
            block = new Block (renderer);
    	}
        #endregion

        public Block Block { get { return block; } }
    }
}
