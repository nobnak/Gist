using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    public interface IExhibitorListener {

        void ExhibitorOnParent(Transform parent);
        void ExhibitorOnUnparent(Transform parent);

    }
}
