using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Events {
        
    public class MaterialListener : BaseMaterialListener {
		[SerializeField]
		protected Material targetMaterial;

        #region unity
        private void OnEnable() {
            if (targetMaterial == null) {
                var r = GetComponent<Renderer>();
                if (r != null)
                    targetMaterial = r.sharedMaterial;
            }
        }
        #endregion

        public override void Set(string name, Color value) { targetMaterial.SetColor(name, value); }
		public override void Set(string name, float value) { targetMaterial.SetFloat(name, value); }
		public override void Set(string name, Matrix4x4 value) { targetMaterial.SetMatrix(name, value); }
		public override void Set(string name, Texture value) { targetMaterial.SetTexture(name, value); }
		public override void Set(string name, Vector4 value) { targetMaterial.SetVector(name, value); }

		public override Color GetColor(string name) { return targetMaterial.GetColor(name); }
		public override float GetFloat(string name) { return targetMaterial.GetFloat(name); }
		public override Matrix4x4 GetMatrix(string name) { return targetMaterial.GetMatrix(name); }
		public override Texture GetTexture(string name) { return targetMaterial.GetTexture(name); }
		public override Vector4 GetVector(string name) { return targetMaterial.GetVector(name); }
    }
}
