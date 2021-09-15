using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace nobnak.Gist.Loader {

	public class AbstractConfig {

		public AbstractConfig(string filename) {
			try {
				var path = Path.Combine(Application.streamingAssetsPath, filename);

				#if UNITY_EDITOR
				var fullpath_example = path + ".example";
				File.WriteAllText(fullpath_example, JsonUtility.ToJson(this, true));
				Debug.LogWarning($"{GetType().Name} : write to {fullpath_example}");
				#endif

				if (File.Exists(path)) {
					var text = File.ReadAllText(path);
					JsonUtility.FromJsonOverwrite(text, this);
					Debug.Log($"{GetType().Name} : Load Config, path={path}");
				}
			}catch(System.Exception e) {
				Debug.LogWarning(e);
			}
		}
	}
}
