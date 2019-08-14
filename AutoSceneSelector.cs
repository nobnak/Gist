using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace nobnak.Gist {

    public class AutoSceneSelector : MonoBehaviour {
        static readonly string[] FLAGS = new string[]{ "-scene" };

        public int defaultSceneNumber = 1;
        public float sleep = 3f;

        void OnEnable() {
            StartCoroutine(LaunchCo());
        }

        IEnumerator LaunchCo() {
            if (sleep > 0f) {
                Debug.LogFormat("Wait for {0} seconds", sleep);
                yield return new WaitForSeconds(sleep);
            }

            var args = System.Environment.GetCommandLineArgs();
            int targetSceneIndex;
            TryFindSceneNumber(args, out targetSceneIndex, defaultSceneNumber);

            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (targetSceneIndex < 0 || targetSceneIndex == currentSceneIndex)
                targetSceneIndex = default;

            var loader = SceneManager.LoadSceneAsync(targetSceneIndex);
            while (!loader.isDone) {
                Debug.LogFormat("Loading : {0:f1}/100", loader.progress * 100f);
                yield return null;
            }
        }

        static bool TryFindSceneNumber (string[] args, out int sceneNumber, int defaultSceneNumber = -1) {
            foreach (var f in FLAGS)
                for (var i = 1; i < args.Length; i++)
                    if (args [i] == f && (i + 1) < args.Length && int.TryParse (args [i + 1], out sceneNumber))
                        return true;
            sceneNumber = defaultSceneNumber;
            return false;
        }
    }
}
