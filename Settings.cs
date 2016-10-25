using UnityEngine;
using System.Collections;
using System.IO;
using DataUI;

namespace Gist {
        
    public abstract class Settings : MonoBehaviour {
        public const float WINDOW_WIDTH = 300f;
        public SettingsCore core;
        public enum ModeEnum { Normal = 0, GUI = 1, End = 2 }
        public ModeEnum mode;
        public KeyCode toggleKey = KeyCode.None;

        [System.Serializable]
        public class SettingsCore {
            public enum PathTypeEnum { StreamingAssets = 0, MyDocuments }

            public PathTypeEnum pathType;
            public string dataPath;

            #region Save/Load
            public virtual void Load<T>(T data) {
                string path;
                if (!DataPath (out path))
                    return;

                try {
                    JsonUtility.FromJsonOverwrite(File.ReadAllText(path), data);
                } catch (System.Exception e) {
                    Debug.Log (e);
                }
            }
            public virtual void Save<T>(T data) {
                string path;
                if (!DataPath (out path))
                    return;

                try {
                    File.WriteAllText(path, JsonUtility.ToJson(data, true));
                } catch (System.Exception e) {
                    Debug.Log (e);
                }
            }
            public virtual bool DataPath(out string path) {
                var dir = Application.streamingAssetsPath;
                switch (pathType) {
                case PathTypeEnum.MyDocuments:
                    dir = System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments);
                    break;
                }
                path = Path.Combine (dir, dataPath);
                return !string.IsNullOrEmpty (dataPath);
            }
            #endregion
        }
    }
    public abstract class Settings<T> : Settings {
        public T data;

        FieldEditor _dataEditor;
        Rect _window;

        protected virtual void OnEnable() {
            core.Load (data);
            _dataEditor = new FieldEditor (data);
            OnDataChange ();
        }
        protected virtual void Update() {
            if (Input.GetKeyDown (toggleKey)) {
                mode = (ModeEnum)(((int)mode + 1) % (int)ModeEnum.End);
                if (mode == ModeEnum.Normal)
                    core.Save (data);
            }

            switch (mode) {
            case ModeEnum.GUI:
                OnDataChange ();
                break;
            }
        }
        protected virtual void OnGUI() {
            if (mode == ModeEnum.GUI)
                _window = GUILayout.Window (GetInstanceID(), _window, Window, this.name, GUILayout.MinWidth (WINDOW_WIDTH));
        }

        protected virtual void OnDataChange() {}

        #region GUI
        void Window(int id) {
            _dataEditor.OnGUI ();
            GUI.DragWindow ();
        }
        #endregion
    }
}
