using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Variations {

    [ExecuteAlways]
    public class MeshVariation : MonoBehaviour {

        [SerializeField]
        protected MeshInfo[] infos = new MeshInfo[0];

        #region unity
        private void OnEnable() {
            var filter = GetComponent<MeshFilter>();

            if (infos.Length > 0 && filter != null) {
                var picked = infos[Random.Range(0, infos.Length)];
                filter.sharedMesh = picked.mesh;
            }
        }
        private void OnDisable() {
            
        }
        #endregion

        [System.Serializable]
        public class MeshInfo {
            public Mesh mesh;
        }
    }
}
