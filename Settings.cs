using UnityEngine;
using System.Collections;
using System.IO;
using DataUI;

namespace Gist {
        
    public abstract class Settings : MonoBehaviour {
        public SettingsCore core;


        [System.Serializable]
        public class SettingsCore {
            public const float WINDOW_WIDTH = 300f;
            public enum ModeEnum { Normal = 0, GUI = 1, End = 2 }

            public UnityEngine.Events.UnityEvent OnDataChange;

            public ModeEnum mode;
            public KeyCode toggleKey = KeyCode.None;

            public enum PathTypeEnum { StreamingAssets = 0, MyDocuments }

            public PathTypeEnum pathType;
            public string dataPath;

            #region Behaviour Wrapper
            FieldEditor _dataEditor;
            Rect _window;

            public virtual void OnEnable<T>(T data) {
                Load (data);
                _dataEditor = new FieldEditor (data);
                NotifyOnDataChange ();
            }
            public virtual void Update<T>(T data) {
                if (Input.GetKeyDown (toggleKey)) {
                    mode = (ModeEnum)(((int)mode + 1) % (int)ModeEnum.End);
                    if (mode == ModeEnum.Normal)
                        Save (data);
                }

                switch (mode) {
                case ModeEnum.GUI:
                    NotifyOnDataChange ();
                    break;
                }
            }
            public virtual void OnGUI(MonoBehaviour b) {
                if (mode == ModeEnum.GUI)
                    _window = GUILayout.Window (GetHashCode(), _window, Window, b.name, GUILayout.MinWidth (WINDOW_WIDTH));
            }

            void NotifyOnDataChange () {
                OnDataChange.Invoke ();
            }

            void Window(int id) {
                _dataEditor.OnGUI ();
                GUI.DragWindow ();
            }
            #endregion

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
        public event System.Action EventOnDataChange;

        public T data;

        protected virtual void Awake() {
            core.OnDataChange.AddListener (new UnityEngine.Events.UnityAction (NotifyOnDataChange));
        }
        protected virtual void OnEnable() {
            core.OnEnable (data);
        }
        protected virtual void Update() {
            core.Update (data);
        }
        protected virtual void OnGUI() {
            core.OnGUI (this);
        }

        protected virtual void NotifyOnDataChange() {
            if (EventOnDataChange != null)
                EventOnDataChange ();
            OnDataChange ();
        }
        protected virtual void OnDataChange() {}
    }
}
