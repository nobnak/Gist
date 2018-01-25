using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Gist {

	public class GLMaterial : System.IDisposable {

        public enum ZTestEnum {
            NEVER = 1, LESS = 2, EQUAL = 3, LESSEQUAL = 4,
            GREATER = 5, NOTEQUAL = 6, GREATEREQUAL = 7, ALWAYS = 8
        };

        public const string PROP_COLOR = "_Color";
        public const string PROP_SRC_BLEND = "_SrcBlend";
        public const string PROP_DST_BLEND = "_DstBlend";
        public const string PROP_ZWRITE = "_ZWrite";
        public const string PROP_ZTEST = "_ZTest";
        public const string PROP_CULL = "_Cull";
        public const string PROP_ZBIAS = "_ZBias";
		public const string LINE_SHADER = "Hidden/Internal-Colored";

        protected Material _lineMat;

		public GLMaterial() {
			var lineShader = Shader.Find (LINE_SHADER);
			if (lineShader == null)
				Debug.LogErrorFormat ("Line Shader not found : {0}", LINE_SHADER);
			_lineMat = new Material (lineShader);
            Reset();
		}

        public void Reset() {
            ZWriteMode = false;
            ZTestMode = ZTestEnum.LESSEQUAL;
            ZOffset = 0f;
        }
        public bool ZWriteMode {
            get { return _lineMat.GetInt (PROP_ZWRITE) == 1; }
            set { _lineMat.SetInt (PROP_ZWRITE, value ? 1 : 0); }
        }
        public ZTestEnum ZTestMode {
            get { return (ZTestEnum)_lineMat.GetInt (PROP_ZTEST); }
            set { _lineMat.SetInt (PROP_ZTEST, (int)value); }
        }
        public float ZOffset {
            get { return _lineMat.GetFloat(PROP_ZBIAS); }
            set { _lineMat.SetFloat(PROP_ZBIAS, value); }
        }

        public void SetPass(int pass = 0) {
            _lineMat.SetPass(pass);
        }
        public bool Color (UnityEngine.Color color) {
            if (_lineMat == null)
                return false;
            GL.Color (color);
            return true;
        }

		#region IDisposable implementation
        public bool IsDisposed { get { return _lineMat == null; } }
		public void Dispose () {
			ObjectDestructor.Destroy(_lineMat);
		}
		#endregion
	}
}