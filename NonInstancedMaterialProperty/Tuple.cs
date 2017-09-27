using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonInstancedMaterialProperty {

    public class Tuple {

        public readonly object[] values;

        protected bool codeIsValid = false;
        protected int code = 0;

        public Tuple(params object[] values) {
            this.values = values;
        }
        public override int GetHashCode() {
            if (codeIsValid)
                return code;
            codeIsValid = true;
            return code = GenerateHashCode(values);
        }
        public override bool Equals(object obj) {
            Tuple b;
            return (obj is Tuple) && (b = (Tuple)obj) != null && Equals(this, b);
        }

        public static int GenerateHashCode(object[] values) {
            var code = 0;
            foreach (var v in values)
                code ^= v.GetHashCode();
            return code;
        }
        public static bool Equals(Tuple a, Tuple b) {
            if (a == null || b == null || a.values.Length != b.values.Length)
                return false;
            for (var i = 0; i < a.values.Length; i++)
                if (!a.values[i].Equals(b.values[i]))
                    return false;
            return true;
        }
    }
}
