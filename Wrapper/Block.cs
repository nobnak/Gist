using UnityEngine;
using System.Collections;

namespace nobnak.Gist.Wrapper {
    public interface IBlock<T> {
        T Apply ();
        T Clear();

        #region Set
        T SetColor(string name, Color value);
        T SetColor(int name, Color value);
        T SetFloat(string name, float value);
        T SetFloat(int name, float value);
        T SetMatrix(string name, Matrix4x4 value);
        T SetMatrix(int name, Matrix4x4 value);
        T SetTexture(string name, Texture value);
        T SetTexture(int name, Texture value);
        T SetVector(string name, Vector4 value);
        T SetVector(int name, Vector4 value);
        T SetBuffer (string name, ComputeBuffer value);
        T SetBuffer (int name, ComputeBuffer value);
        #endregion

        #region Get
        Color GetColor (string name);
        Color GetColor (int name);
        float GetFloat (string name);
        float GetFloat (int name);
        Matrix4x4 GetMatrix (string name);
        Matrix4x4 GetMatrix (int name);
        Texture GetTexture (string name);
        Texture GetTexture (int name);
        Vector4 GetVector (string name);
        Vector4 GetVector (int name);
        #endregion

        #region Defaults
        Color GetDefaultColor (string name);
        Color GetDefaultColor (int name);
        float GetDefaultFloat (string name);
        float GetDefaultFloat (int name);
        Matrix4x4 GetDefaultMatrix (string name);
        Matrix4x4 GetDefaultMatrix (int name);
        Texture GetDefaultTexture (string name);
        Texture GetDefaultTexture (int name);
        Vector4 GetDefaultVector (string name);
        Vector4 GetDefaultVector (int name);
        #endregion
    }

    public class Block : IBlock<Block> {
        public readonly Pair[] pairs;

        public Block(params Renderer[] rends) 
            : this(GeneratePairs(rends)) {}
        public Block(params Pair[] pairs) {
            this.pairs = pairs;
        }

        #region IBlock implementation
        public Block Apply () {
            foreach (var p in pairs)
                p.Apply ();
            return this;
        }
        public Block Clear() {
            foreach (var p in pairs)
                p.Clear();
            return this;
        }

        public Block SetColor (string name, Color value) {
            return SetColor (Shader.PropertyToID (name), value);
        }
        public Block SetColor(int name, Color value) {
            foreach (var p in pairs)
                p.SetColor (name, value);
            return this;
        }
        public Block SetFloat (string name, float value) {
            return SetFloat (Shader.PropertyToID (name), value);
        }
        public Block SetFloat(int name, float value) {
            foreach (var p in pairs)
                p.SetFloat (name, value);
            return this;
        }
        public Block SetMatrix (string name, Matrix4x4 value) {
            return SetMatrix (Shader.PropertyToID (name), value);
        }
        public Block SetMatrix(int name, Matrix4x4 value) {
            foreach (var p in pairs)
                p.SetMatrix (name, value);
            return this;
        }
        public Block SetTexture (string name, Texture value) {
            return SetTexture (Shader.PropertyToID (name), value);
        }
        public Block SetTexture(int name, Texture value) {
            foreach (var p in pairs)
                p.SetTexture (name, value);
            return this;
        }
        public Block SetVector (string name, Vector4 value) {
            return SetVector (Shader.PropertyToID (name), value);
        }
        public Block SetVector(int name, Vector4 value) {
            foreach (var p in pairs)
                p.SetVector (name, value);
            return this;
        }
        public Block SetBuffer(string name, ComputeBuffer value) {
            return SetBuffer (Shader.PropertyToID (name), value);
        }
        public Block SetBuffer(int name, ComputeBuffer value) {
            foreach (var p in pairs)
                p.SetBuffer (name, value);
            return this;
        }

        public Color GetColor (string name) { return GetColor (Shader.PropertyToID(name)); }
        public Color GetColor(int name) { return GetColor(0, name); }
        public Color GetColor (int index, int name) { return pairs [index].GetColor (name); }
        public float GetFloat (string name) { return GetFloat (Shader.PropertyToID(name)); }
        public float GetFloat (int name) { return GetFloat (0, name); }
        public float GetFloat (int index, int name) { return pairs [index].GetFloat (name); }
        public Matrix4x4 GetMatrix (string name) { return GetMatrix (Shader.PropertyToID(name)); }
        public Matrix4x4 GetMatrix (int name) { return GetMatrix (0, name); }
        public Matrix4x4 GetMatrix (int index, int name) { return pairs [index].GetMatrix (name); }
        public Texture GetTexture (string name) { return GetTexture (Shader.PropertyToID(name)); }
        public Texture GetTexture (int name) { return GetTexture (0, name); }
        public Texture GetTexture (int index, int name) { return pairs [index].GetTexture (name); }
        public Vector4 GetVector (string name) { return GetVector (Shader.PropertyToID(name)); }
        public Vector4 GetVector (int name) { return GetVector (0, name); }
        public Vector4 GetVector (int index, int name) { return pairs [index].GetVector (name); }

        public Color GetDefaultColor (string name) { return GetDefaultColor (Shader.PropertyToID(name)); }
        public Color GetDefaultColor (int name) { return GetDefaultColor (0, name); }
        public Color GetDefaultColor (int index, int name) { return pairs [index].GetDefaultColor (name); }
        public float GetDefaultFloat (string name) { return GetDefaultFloat (Shader.PropertyToID(name));  }
        public float GetDefaultFloat (int name) { return GetDefaultFloat (0, name);  }
        public float GetDefaultFloat (int index, int name) { return pairs [index].GetDefaultFloat (name);  }
        public Matrix4x4 GetDefaultMatrix (string name) { return GetDefaultMatrix (Shader.PropertyToID(name)); }
        public Matrix4x4 GetDefaultMatrix (int name) { return GetDefaultMatrix (0, name); }
        public Matrix4x4 GetDefaultMatrix (int index, int name) { return pairs [index].GetDefaultMatrix (name); }
        public Texture GetDefaultTexture (string name) { return GetDefaultTexture (Shader.PropertyToID(name)); }
        public Texture GetDefaultTexture (int name) { return GetDefaultTexture (0, name); }
        public Texture GetDefaultTexture (int index, int name) { return pairs [index].GetDefaultTexture (name); }
        public Vector4 GetDefaultVector (string name) { return GetDefaultVector (Shader.PropertyToID(name)); }
        public Vector4 GetDefaultVector (int name) { return GetDefaultVector (0, name); }
        public Vector4 GetDefaultVector (int index, int name) { return pairs [index].GetDefaultVector (name); }
        #endregion

        #region Static
        public static Pair[] GeneratePairs(Renderer[] renderers) {
            var pairs = new Pair[renderers.Length];
            for (var i = 0; i < renderers.Length; i++)
                pairs [i] = new Pair (renderers [i], new MaterialPropertyBlock ());
            return pairs;
        }
        public static Block operator+ (Block a, Block b) {
            var pairs = new Pair[a.pairs.Length + b.pairs.Length];
            System.Array.Copy (a.pairs, pairs, a.pairs.Length);
            System.Array.Copy (b.pairs, 0, pairs, a.pairs.Length, b.pairs.Length);
            return new Block (pairs);
        }
        #endregion
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
            if (Renderer != null)
                Renderer.SetPropertyBlock (Block);
            return this;
    	}
        public Pair Clear() {
            duringSession = false;
            if (Renderer != null)
                Renderer.SetPropertyBlock(null);
            return this;
        }

        #region Set
        public Pair SetColor(string name, Color value) {
            return SetColor (Shader.PropertyToID (name), value);
        }
        public Pair SetColor(int name, Color value) {
            CheckLoad ();
            Block.SetColor (name, value);
            return this;
        }
        public Pair SetFloat(string name, float value) {
            return SetFloat (Shader.PropertyToID (name), value);
        }
        public Pair SetFloat(int name, float value) {
            CheckLoad ();
            Block.SetFloat (name, value);
            return this;
        }
        public Pair SetMatrix(string name, Matrix4x4 value) {
            return SetMatrix (Shader.PropertyToID (name), value);
        }
        public Pair SetMatrix(int name, Matrix4x4 value) {
            CheckLoad ();
            Block.SetMatrix (name, value);
            return this;
        }
        public Pair SetTexture(string name, Texture value) {
            return SetTexture (Shader.PropertyToID (name), value);
        }
        public Pair SetTexture(int name, Texture value) {
            CheckLoad ();
			if(value != null)
				Block.SetTexture (name, value);
            return this;
        }
        public Pair SetVector(string name, Vector4 value) {
            return SetVector (Shader.PropertyToID (name), value);
        }
        public Pair SetVector(int name, Vector4 value) {
            CheckLoad ();
            Block.SetVector (name, value);
            return this;
        }
        public Pair SetBuffer(string name, ComputeBuffer value) {
            return SetBuffer (Shader.PropertyToID (name), value);
        }
        public Pair SetBuffer(int name, ComputeBuffer value) {
            CheckLoad ();
            Block.SetBuffer (name, value);
            return this;
        }
        #endregion

        #region Get
        public Color GetColor(string name) {
            return GetColor (Shader.PropertyToID (name));
        }
        public Color GetColor(int name) {
            CheckLoad ();
            return (Color)Block.GetVector (name);
        }
        public float GetFloat(string name) {
            return GetFloat (Shader.PropertyToID (name));
        }
        public float GetFloat(int name) {
            CheckLoad ();
            return Block.GetFloat (name);
        }
        public Matrix4x4 GetMatrix(string name) {
            return GetMatrix (Shader.PropertyToID (name));
        }
        public Matrix4x4 GetMatrix(int name) {
            CheckLoad ();
            return Block.GetMatrix (name);
        }
        public Texture GetTexture(string name) {
            return GetTexture (Shader.PropertyToID (name));
        }
        public Texture GetTexture(int name) {
            CheckLoad ();
            return Block.GetTexture (name);
        }
        public Vector4 GetVector(string name) {
            return GetVector (Shader.PropertyToID (name));
        }
        public Vector4 GetVector(int name) {
            CheckLoad ();
            return Block.GetVector (name);
        }
        #endregion

        #region Defaults
        public Color GetDefaultColor(string name) {
            return GetDefaultColor (Shader.PropertyToID (name));
        }
        public Color GetDefaultColor(int name) {
            CheckLoad ();
            return Renderer.sharedMaterial.GetColor (name);
        }
        public float GetDefaultFloat(string name) {
            return GetDefaultFloat (Shader.PropertyToID (name));
        }
        public float GetDefaultFloat(int name) {
            CheckLoad ();
            return Renderer.sharedMaterial.GetFloat (name);
        }
        public Matrix4x4 GetDefaultMatrix(string name) {
            return GetDefaultMatrix (Shader.PropertyToID (name));
        }
        public Matrix4x4 GetDefaultMatrix(int name) {
            CheckLoad ();
            return Renderer.sharedMaterial.GetMatrix (name);
        }
        public Texture GetDefaultTexture(string name) {
            return GetDefaultTexture (Shader.PropertyToID (name));
        }
        public Texture GetDefaultTexture(int name) {
            CheckLoad ();
            return Renderer.sharedMaterial.GetTexture (name);
        }
        public Vector4 GetDefaultVector(string name) {
            return GetDefaultVector (Shader.PropertyToID (name));
        }
        public Vector4 GetDefaultVector(int name) {
            CheckLoad ();
            return Renderer.sharedMaterial.GetVector (name);
        }
        #endregion

        void CheckLoad() {
            if (!duringSession && Renderer != null) {
                duringSession = true;
                Renderer.GetPropertyBlock (Block);
            }
        }
    }
}
