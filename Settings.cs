using UnityEngine;
using System.Collections;
using System.IO;

namespace Gist {
    
    public abstract class Settings<T> : MonoBehaviour {
        public const float WINDOW_WIDTH = 300f;

        public UnityEngine.Events.UnityEvent OnDataChange;
        public event System.Action EventOnDataChange;

        public enum PathTypeEnum { StreamingAssets = 0, MyDocuments }

        public T data;
        public PathTypeEnum pathType;
        public string dataPath;

        #region Unity
        protected virtual void OnEnable() {
            Load (data);
            NotifyOnDataChange ();
        }
        #endregion

        #region Save/Load
        #region Static
        public static bool DataPath(PathTypeEnum pathType, string dataPath, out string path) {
            var dir = Application.streamingAssetsPath;
            switch (pathType) {
            case PathTypeEnum.MyDocuments:
                dir = System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments);
                break;
            }
            path = Path.Combine (dir, dataPath);
            return !string.IsNullOrEmpty (dataPath);
        }
        public static void Load<S>(PathTypeEnum pathType, string dataPath, S data) {
            string path;
            if (!DataPath (pathType, dataPath, out path))
                return;

            try {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(path), data);
            } catch (System.Exception e) {
                Debug.Log (e);
            }
        }
        public static void Save<S>(PathTypeEnum pathType, string dataPath, S data) {
            string path;
            if (!DataPath (pathType, dataPath, out path))
                return;

            try {
                File.WriteAllText(path, JsonUtility.ToJson(data, true));
            } catch (System.Exception e) {
                Debug.Log (e);
            }
        }
        #endregion
        public virtual void Load(T data) {
            Load (pathType, dataPath, data);
        }
        public virtual void Save(T data) {
            Save (pathType, dataPath, data);
        }
        public virtual bool DataPath(out string path) {
            return DataPath (pathType, dataPath, out path);
        }
        #endregion

        protected virtual void NotifyOnDataChange () {
            OnDataChange.Invoke ();
            if (EventOnDataChange != null)
                EventOnDataChange ();
        }
    }
}
