using UnityEngine;
using System.Collections;

namespace Gist {

    public class MaterialPropertyBlockChanied {
        public readonly Renderer FirstRender;
    	public readonly Renderer[] Renderers;
    	public readonly MaterialPropertyBlock Block;

        bool _session;

    	public MaterialPropertyBlockChanied(Renderer rend) : this(rend, new MaterialPropertyBlock()) {}
        public MaterialPropertyBlockChanied(Renderer[] rends) : this(rends, new MaterialPropertyBlock()) {}
    	public MaterialPropertyBlockChanied(Renderer rend, MaterialPropertyBlock block) : this(new Renderer[]{rend}, block) {}
        public MaterialPropertyBlockChanied(Renderer[] rends, MaterialPropertyBlock block) {
            this.FirstRender = rends [0];
            this.Renderers = rends;
            this.Block = block;
            this._session = false;
        }

    	public MaterialPropertyBlockChanied Apply() {
            _session = false;
            for (var i = 0; i < Renderers.Length; i++)
                Renderers[i].SetPropertyBlock (Block);
    		return this;
    	}

        public MaterialPropertyBlockChanied SetColor(string name, Color value) {
            CheckLoad ();
            Block.SetColor (name, value);
            return this;
        }
        public MaterialPropertyBlockChanied SetFloat(string name, float value) {
            CheckLoad ();
            Block.SetFloat (name, value);
            return this;
        }
        public MaterialPropertyBlockChanied SetMatrix(string name, Matrix4x4 value) {
            CheckLoad ();
            Block.SetMatrix (name, value);
            return this;
        }
        public MaterialPropertyBlockChanied SetTexture(string name, Texture value) {
            CheckLoad ();
            Block.SetTexture (name, value);
            return this;
        }
        public MaterialPropertyBlockChanied SetVector(string name, Vector4 value) {
            CheckLoad ();
            Block.SetVector (name, value);
            return this;
        }

        public Color GetColor(string name) {
            CheckLoad ();
            return (Color)Block.GetVector (name);
        }
        public float GetFloat(string name) {
            CheckLoad ();
            return Block.GetFloat (name);
        }
        public Matrix4x4 GetMatrix(string name) {
            CheckLoad ();
            return Block.GetMatrix (name);
        }
        public Texture GetTexture(string name) {
            CheckLoad ();
            return Block.GetTexture (name);
        }
        public Vector4 GetVector(string name) {
            CheckLoad ();
            return Block.GetVector (name);
        }

        public Color GetDefaultColor(string name) {
            CheckLoad ();
            return FirstRender.sharedMaterial.GetColor (name);
        }
        public float GetDefaultFloat(string name) {
            CheckLoad ();
            return FirstRender.sharedMaterial.GetFloat (name);
        }
        public Matrix4x4 GetDefaultMatrix(string name) {
            CheckLoad ();
            return FirstRender.sharedMaterial.GetMatrix (name);
        }
        public Texture GetDefaultTexture(string name) {
            CheckLoad ();
            return FirstRender.sharedMaterial.GetTexture (name);
        }
        public Vector4 GetDefaultVector(string name) {
            CheckLoad ();
            return FirstRender.sharedMaterial.GetVector (name);
        }

        void CheckLoad() {
            if (!_session) {
                _session = true;
                FirstRender.GetPropertyBlock (Block);
            }
        }
    }
}
