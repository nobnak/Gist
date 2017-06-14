using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.Events {
        
    public class ListenerMaterial : MonoBehaviour {
        [SerializeField]
        protected string defaultTextureName = "_MainTex";

        protected MaterialPropertyBlockChanied block;

        #region Unity
        protected virtual void OnEnable() {
            Renderer rend;
            if ((rend = GetComponent<Renderer> ()) != null)
                block = new MaterialPropertyBlockChanied (rend);
        }
        #endregion

        public virtual void SetTexture(Texture tex) {
            SetTexture(defaultTextureName, tex);
        }
        public virtual void SetTexture(string textureName, Texture tex) {
            if (block != null)
                block.SetTexture (textureName, tex).Apply ();
        }

        public virtual Texture GetTexture(string textureName) {
            return (block != null ? block.GetTexture (textureName) : null);
        }
        public virtual Texture GetTexture() {
            return GetTexture (defaultTextureName);
        }
    }
}
