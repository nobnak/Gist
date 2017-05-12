using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gist.Editor {

    public class CopyPasteComponentsWindow : EditorWindow {
        GameObject objectFrom;
        GameObject objectTo;

        [MenuItem("Custom/Window/CopyPastComponents")]
        static void OpenWindow() {
            EditorWindow.GetWindow (typeof(CopyPasteComponentsWindow));
        }

        void OnGUI() {
            objectFrom = ObjectField("From", objectFrom, true);
            objectTo = ObjectField ("To", objectTo, true);

            GUI.enabled = (objectFrom != null && objectTo != null);
            if (GUILayout.Button ("Copy"))
                Copy (objectFrom, objectTo);
            GUI.enabled = true;
        }

        static void Copy(GameObject objectFrom, GameObject objectTo) {
            foreach (var comp in objectFrom.GetComponents<Component>()) {
                if (UnityEditorInternal.ComponentUtility.CopyComponent (comp)) {
                    if (!UnityEditorInternal.ComponentUtility.PasteComponentAsNew (objectTo)) {                        
                        var duplicatedComponent = objectTo.GetComponent (comp.GetType ());
                        UnityEditorInternal.ComponentUtility.PasteComponentValues (duplicatedComponent);
                    }
                }
            }
        }
        static T ObjectField<T>(string label, T obj, bool allowSceneObject) where T:Object {
            return (T)EditorGUILayout.ObjectField (label, obj, typeof(T), allowSceneObject);
        }
    }

}