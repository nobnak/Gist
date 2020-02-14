using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nobnak.Gist.ObjectExt;
using UnityEngine.Rendering;

namespace nobnak.Gist {

	public class GLMaterial : System.IDisposable {

        public enum ZTestEnum {
            NEVER = 1, LESS = 2, EQUAL = 3, LESSEQUAL = 4,
            GREATER = 5, NOTEQUAL = 6, GREATEREQUAL = 7, ALWAYS = 8
        };

        public const string LINE_SHADER = "Hidden/Internal-Colored";

        public static readonly int PROP_COLOR =  Shader.PropertyToID("_Color");
        public static readonly int PROP_SRC_BLEND = Shader.PropertyToID("_SrcBlend");
        public static readonly int PROP_DST_BLEND = Shader.PropertyToID("_DstBlend");
        public static readonly int PROP_ZWRITE = Shader.PropertyToID("_ZWrite");
        public static readonly int PROP_ZTEST = Shader.PropertyToID("_ZTest");
        public static readonly int PROP_CULL = Shader.PropertyToID("_Cull");
        public static readonly int PROP_ZBIAS = Shader.PropertyToID("_ZBias");

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

        public Color Color {
            get { return LineMat.color; }
            set { LineMat.color = value; }
        }
        public BlendMode SrcBlend {
            set { SetBlendMode(PROP_SRC_BLEND, value); }
            get { return GetBlendMode(PROP_SRC_BLEND); }
        }
        public BlendMode DstBlend {
            set { SetBlendMode(PROP_DST_BLEND, value); }
            get { return GetBlendMode(PROP_DST_BLEND); }
        }

        public void SetPass(int pass = 0) {
            LineMat.SetPass(pass);
        }
        public BlendMode GetBlendMode(int p) {
            return (BlendMode)LineMat.GetInt(p);
        }
        public void SetBlendMode(int p, BlendMode v) {
            LineMat.SetInt(p, (int)v);
        }

        #region IDisposable implementation
        public bool IsDisposed { get { return LineMat == null; } }
		public void Dispose () {
			LineMat.DestroySelf();
		}
        #endregion

        #region static
        public static implicit operator Material (GLMaterial v) {
            return v.LineMat;
        }
        #endregion
    }
}