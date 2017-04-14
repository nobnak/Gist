using UnityEngine;
using System.Collections;

namespace Gist {
    public interface IBlock<T> {
        T Apply ();

        #region Set
        T SetColor(string name, Color value);
        T SetFloat(string name, float value);
        T SetMatrix(string name, Matrix4x4 value);
        T SetTexture(string name, Texture value);
        T SetVector(string name, Vector4 value);
        #endregion

        #region Get
        Color GetColor (string name);
        float GetFloat (string name);
        Matrix4x4 GetMatrix (string name);
        Texture GetTexture (string name);
        Vector4 GetVector (string name);
        #endregion

        #region Defaults
        Color GetDefaultColor (string name);
        float GetDefaultFloat (string name);
        Matrix4x4 GetDefaultMatrix (string name);
        Texture GetDefaultTexture (string name);
        Vector4 GetDefaultVector (string name);
        #endregion
    }

    public class MaterialPropertyBlockChanied : IBlock<MaterialPropertyBlockChanied> {
        public readonly Pair[] pairs;

        public MaterialPropertyBlockChanied(params Renderer[] rends) 
            : this(GeneratePairs(rends)) {}
        public MaterialPropertyBlockChanied(params Pair[] pairs) {
            this.pairs = pairs;
        }

        #region IBlock implementation
        public MaterialPropertyBlockChanied Apply () {
            foreach (var p in pairs)
                p.Apply ();
            return this;
        }

        public MaterialPropertyBlockChanied SetColor (string name, Color value) {
            foreach (var p in pairs)
                p.SetColor (name, value);
            return this;
        }
        public MaterialPropertyBlockChanied SetFloat (string name, float value) {
            foreach (var p in pairs)
                p.SetFloat (name, value);
            return this;
        }
        public MaterialPropertyBlockChanied SetMatrix (string name, Matrix4x4 value) {
            foreach (var p in pairs)
                p.SetMatrix (name, value);
            return this;
        }
        public MaterialPropertyBlockChanied SetTexture (string name, Texture value) {
            foreach (var p in pairs)
                p.SetTexture (name, value);
            return this;
        }
        public MaterialPropertyBlockChanied SetVector (string name, Vector4 value) {
            foreach (var p in pairs)
                p.SetVector (name, value);
            return this;
        }

        public Color GetColor (string name) { return GetColor (0, name); }
        public Color GetColor (int index, string name) { return pairs [index].GetColor (name); }
        public float GetFloat (string name) { return GetFloat (0, name); }
        public float GetFloat (int index, string name) { return pairs [index].GetFloat (name); }
        public Matrix4x4 GetMatrix (string name) { return GetMatrix (0, name); }
        public Matrix4x4 GetMatrix (int index, string name) { return pairs [index].GetMatrix (name); }
        public Texture GetTexture (string name) { return GetTexture (0, name); }
        public Texture GetTexture (int index, string name) { return pairs [index].GetTexture (name); }
        public Vector4 GetVector (string name) { return GetVector (0, name); }
        public Vector4 GetVector (int index, string name) { return pairs [index].GetVector (name); }

        public Color GetDefaultColor (string name) { return GetDefaultColor (0, name); }
        public Color GetDefaultColor (int index, string name) { return pairs [index].GetDefaultColor (name); }
        public float GetDefaultFloat (string name) { return GetDefaultFloat (0, name);  }
        public float GetDefaultFloat (int index, string name) { return pairs [index].GetDefaultFloat (name);  }
        public Matrix4x4 GetDefaultMatrix (string name) { return GetDefaultMatrix (0, name); }
        public Matrix4x4 GetDefaultMatrix (int index, string name) { return pairs [index].GetDefaultMatrix (name); }
        public Texture GetDefaultTexture (string name) { return GetDefaultTexture (0, name); }
        public Texture GetDefaultTexture (int index, string name) { return pairs [index].GetDefaultTexture (name); }
        public Vector4 GetDefaultVector (string name) { return GetDefaultVector (0, name); }
        public Vector4 GetDefaultVector (int index, string name) { return pairs [index].GetDefaultVector (name); }
        #endregion

        public static Pair[] GeneratePairs(Renderer[] renderers) {
            var pairs = new Pair[renderers.Length];
            for (var i = 0; i < renderers.Length; i++)
                pairs [i] = new Pair (renderers [i], new MaterialPropertyBlock ());
            return pairs;
        }
    }

    public class Pair : IBlock<Pair> {
        public readonly Renderer Renderer;
        public readonly MaterialPropertyBlock Block;
        bool duringSession;

        public Pair(Renderer renderer, MaterialPropertyBlock block) {
            this.duringSession = false;
            this.Renderer = renderer;
            this.Block = block;
        }

    	public Pair Apply() {
            duringSession = false;
            Renderer.SetPropertyBlock (Block);
            return this;
    	}

        #region Set
        public Pair SetColor(string name, Color value) {
            CheckLoad ();
            Block.SetColor (name, value);
            return this;
        }
        public Pair SetFloat(string name, float value) {
            CheckLoad ();
            Block.SetFloat (name, value);
            return this;
        }
        public Pair SetMatrix(string name, Matrix4x4 value) {
            CheckLoad ();
            Block.SetMatrix (name, value);
            return this;
        }
        public Pair SetTexture(string name, Texture value) {
            CheckLoad ();
            Block.SetTexture (name, value);
            return this;
        }
        public Pair SetVector(string name, Vector4 value) {
            CheckLoad ();
            Block.SetVector (name, value);
            return this;
        }
        #endregion

        #region Get
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
        #endregion

        #region Defaults
        public Color GetDefaultColor(string name) {
            CheckLoad ();
            return Renderer.sharedMaterial.GetColor (name);
        }
        public float GetDefaultFloat(string name) {
            CheckLoad ();
            return Renderer.sharedMaterial.GetFloat (name);
        }
        public Matrix4x4 GetDefaultMatrix(string name) {
            CheckLoad ();
            return Renderer.sharedMaterial.GetMatrix (name);
        }
        public Texture GetDefaultTexture(string name) {
            CheckLoad ();
            return Renderer.sharedMaterial.GetTexture (name);
        }
        public Vector4 GetDefaultVector(string name) {
            CheckLoad ();
            return Renderer.sharedMaterial.GetVector (name);
        }
        #endregion

        void CheckLoad() {
            if (!duringSession) {
                duringSession = true;
                Renderer.GetPropertyBlock (Block);
            }
        }
    }
}
