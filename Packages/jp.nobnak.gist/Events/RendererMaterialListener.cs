using nobnak.Gist.Wrapper;
using UnityEngine;

namespace nobnak.Gist.Events {

    [ExecuteAlways]
    public class RendererMaterialListener : BaseMaterialListener {

        protected Block block;

        #region unity
        void OnEnable() {
            if (block != null)
                block.Apply();
        }
        void OnDisable() {
            if (block != null)
                block.Clear();
        }
        #endregion

        public virtual Block Block {
            get { 
                if (block == null)
                    block = new Block (GetComponent<Renderer> ());
                return block;
            }
        }

        public override void Set(string colorName, Color value) {
            Block.SetColor (colorName, value).Apply ();
        }
        public override void Set(string textureName, float value) {
            Block.SetFloat (textureName, value).Apply ();
        }
        public override void Set(string textureName, Matrix4x4 value) {
            Block.SetMatrix (textureName, value).Apply ();
        }
        public override void Set(string textureName, Texture value) {
            Block.SetTexture (textureName, value).Apply ();
        }
        public override void Set(string textureName, Vector4 value) {
            Block.SetVector (textureName, value).Apply ();
        }

        public override Color GetColor(string name) { return Block.GetColor (name); }
        public override float GetFloat(string name) { return Block.GetFloat (name); }
        public override Matrix4x4 GetMatrix(string name) { return Block.GetMatrix (name); }
        public override Texture GetTexture(string name) { return Block.GetTexture (name); }
        public override Vector4 GetVector(string name) { return Block.GetVector (name); }
    }
}
