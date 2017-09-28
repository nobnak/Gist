using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonInstancedMaterialProperty {

    public class MaterialHolder {

        protected Dictionary<Tuple, Retainer<Material>> retainedMaterials;

        public MaterialHolder() {
            retainedMaterials = new Dictionary<Tuple, Retainer<Material>>();
        }


    }
}
