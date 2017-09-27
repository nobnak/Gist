using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonInstancedMaterialProperty {

    [System.Serializable]
    public class Property {
        public enum ValueTypeEnum { Texture = 0 }

        public string name;
        public ValueTypeEnum valueType;

        public Texture[] textureValues;

        public void Set(Material m, int valueIndex) {
            m.SetTexture(name, textureValues[valueIndex]);
        }
        public int CountValues {
            get {
                return textureValues.Length;
            }
        }
    }
}
