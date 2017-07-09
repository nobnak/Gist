using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.Events {
        
    public class ListenerMaterial : MonoBehaviour {
        [SerializeField]
        protected string defaultColorName = "_Color";
        [SerializeField]
        protected string defaultFloatName = "_Float";
        [SerializeField]
        protected string defaultMatrixName = "_Matrix";
        [SerializeField]
        protected string defaultTextureName = "_MainTex";
        [SerializeField]
        protected string defaultVectorName = "_Vector";

        protected MaterialPropertyBlockChanied block;

        public virtual MaterialPropertyBlockChanied Block {
            get { 
                if (block == null)
                    block = new MaterialPropertyBlockChanied (GetComponent<Renderer> ());
                return block;
            }
        }

        public virtual void Set(string colorName, Color value) {
            Block.SetColor (colorName, value).Apply ();
        }
        public virtual void Set(string textureName, float value) {
            Block.SetFloat (textureName, value).Apply ();
        }
        public virtual void Set(string textureName, Matrix4x4 value) {
            Block.SetMatrix (textureName, value).Apply ();
        }
        public virtual void Set(string textureName, Texture value) {
            Block.SetTexture (textureName, value).Apply ();
        }
        public virtual void Set(string textureName, Vector4 value) {
            Block.SetVector (textureName, value).Apply ();
        }

        public virtual void Set(Color value) { Set(defaultColorName, value); }
        public virtual void Set(float value) { Set(defaultFloatName, value); }
        public virtual void Set(Matrix4x4 value) { Set(defaultMatrixName, value); }
        public virtual void Set(Texture value) { Set(defaultTextureName, value); }
        public virtual void Set(Vector4 value) { Set(defaultVectorName, value); }

        public virtual Color GetColor(string name) { return Block.GetColor (name); }
        public virtual float GetFloat(string name) { return Block.GetFloat (name); }
        public virtual Matrix4x4 GetMatrix(string name) { return Block.GetMatrix (name); }
        public virtual Texture GetTexture(string name) { return Block.GetTexture (name); }
        public virtual Vector4 GetVector(string name) { return Block.GetVector (name); }

        public virtual Color GetColor() { return Block.GetColor (defaultColorName); }
        public virtual float GetFloat() { return Block.GetFloat (defaultFloatName); }
        public virtual Matrix4x4 GetMatrix() { return Block.GetMatrix (defaultMatrixName); }
        public virtual Texture GetTexture() { return Block.GetTexture (defaultTextureName); }
        public virtual Vector4 GetVector() { return Block.GetVector (defaultVectorName); }
    }
}
