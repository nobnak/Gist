using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nobnak.Gist.Events;
using nobnak.Gist.Layers;

    namespace nobnak.Gist.Examples.Layer {
        
    public class NormalizedPositionSender : MonoBehaviour {
        [SerializeField]
        AbstractLayer layer;
        [SerializeField]
        Vector2Event NormalizedPositionOnUpdate;

    	void Update () {
            NormalizedPositionOnUpdate.Invoke (layer.ProjectOnNormalized (transform.position));
    	}
    }
}
