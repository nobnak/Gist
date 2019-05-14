using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nobnak.Gist.ObjectExt;

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

        public Material LineMat { get; protected set; }

		public GLMaterial() {
			var lineShader = Shader.Find (LINE_SHADER);
			if (lineShader == null)
				Debug.LogErrorFormat ("Line Shader not found : {0}", LINE_SHADER);
			LineMat = new Material (lineShader);
            Reset();
		}

        public void Reset() {
            ZWriteMode = false;
            ZTestMode = ZTestEnum.LESSEQUAL;
            ZOffset = 0f;
        }
        public bool ZWriteMode {
            get { return LineMat.GetInt (PROP_ZWRITE) == 1; }
            set { LineMat.SetInt (PROP_ZWRITE, value ? 1 : 0); }
        }
        public ZTestEnum ZTestMode {
            get { return (ZTestEnum)LineMat.GetInt (PROP_ZTEST); }
            set { LineMat.SetInt (PROP_ZTEST, (int)value); }
        }
        public float ZOffset {
            get { return LineMat.GetFloat(PROP_ZBIAS); }
            set { LineMat.SetFloat(PROP_ZBIAS, value); }
        }

        public void SetPass(int pass = 0) {
            LineMat.SetPass(pass);
        }
        public bool Color (UnityEngine.Color color) {
            if (LineMat == null)
                return false;
			//GL.Color (color);
			LineMat.color = color;
			return true;
        }

		#region IDisposable implementation
        public bool IsDisposed { get { return LineMat == null; } }
		public void Dispose () {
			LineMat.DestroySelf();
		}
		#endregion
	}
}