using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Events {
        
    public abstract class BaseMaterialListener : MonoBehaviour {
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

		public abstract void Set(string name, Color value);
		public abstract void Set(string name, float value);
		public abstract void Set(string name, Matrix4x4 value);
		public abstract void Set(string name, Texture value);
		public abstract void Set(string name, Vector4 value);

        public virtual void Set(Color value) { Set(defaultColorName, value); }
        public virtual void Set(float value) { Set(defaultFloatName, value); }
        public virtual void Set(Matrix4x4 value) { Set(defaultMatrixName, value); }
        public virtual void Set(Texture value) { Set(defaultTextureName, value); }
        public virtual void Set(Vector4 value) { Set(defaultVectorName, value); }

		public abstract Color GetColor(string name);
		public abstract float GetFloat(string name);
		public abstract Matrix4x4 GetMatrix(string name);
		public abstract Texture GetTexture(string name);
		public abstract Vector4 GetVector(string name);

        public virtual Color GetColor() { return GetColor (defaultColorName); }
        public virtual float GetFloat() { return GetFloat (defaultFloatName); }
        public virtual Matrix4x4 GetMatrix() { return GetMatrix (defaultMatrixName); }
        public virtual Texture GetTexture() { return GetTexture (defaultTextureName); }
        public virtual Vector4 GetVector() { return GetVector (defaultVectorName); }
    }
}
