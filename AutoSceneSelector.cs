using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gist {

    public class AutoSceneSelector : MonoBehaviour {
        static readonly string[] FLAGS = new string[]{ "-scene" };

        public int defaultSceneNumber = 1;

        void OnEnable() {
            var args = System.Environment.GetCommandLineArgs ();
            int sceneNumber;
            TryFindSceneNumber (args, out sceneNumber, defaultSceneNumber);
            SceneManager.LoadScene (sceneNumber);
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
