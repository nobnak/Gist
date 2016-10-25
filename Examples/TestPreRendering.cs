using UnityEngine;
using System.Collections;

namespace Gist {

    public class TestPreRendering : MonoBehaviour {
        public const float SEED_SCALE = 1000f;

        public GameObject primitivefab;
        public int count = 100;
        public float dist = 10f;
        public float noiseFreq = 0.1f;
        public float rotSpeed = 1f;

        Vector3 _seed;

    	void Start () {
            _seed = new Vector3 (
                Random.Range (-SEED_SCALE, SEED_SCALE), 
                Random.Range (-SEED_SCALE, SEED_SCALE), 
                Random.Range (-SEED_SCALE, SEED_SCALE));

            for (var i = 0; i < count; i++) {
                var go = Instantiate (primitivefab);
                go.transform.SetParent (transform, false);
                go.transform.localPosition = dist * Random.insideUnitSphere;
                go.transform.localRotation = Random.rotationUniform;
                go.SetActive (true);
            }
    	
    	}
    	void Update () {
            var t = Time.timeSinceLevelLoad * noiseFreq;
            var dt = Time.deltaTime * rotSpeed;
            var q = Quaternion.Euler (
                dt * Noise (t + _seed.x, _seed.y), 
                dt * Noise (t + _seed.y, _seed.z),
                dt * Noise (t + _seed.z, _seed.x));
            transform.localRotation *= q;
    	}

        float Noise(float x, float y) {
            return 2f * Mathf.PerlinNoise (x, y) - 1f;
        }
    }
}