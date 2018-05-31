using UnityEngine;

namespace nobnak.Gist.IMGUI.Scope {

	public class FoldoutScope : System.IDisposable {
        public const char CHAR_OPEN = '▼';
        public const char CHAR_CLOSE = '▶';

        protected GUIStyle foldoutStyle;

        public FoldoutScope(ref bool visible, string title = "") {

            if (foldoutStyle == null)
                foldoutStyle = new GUIStyle(UnityEngine.GUI.skin.label) {
                    alignment = TextAnchor.MiddleLeft
                };
            var foldoutTitle = (visible ? CHAR_OPEN : CHAR_CLOSE) + title;
            visible = GUILayout.Toggle(visible, foldoutTitle, foldoutStyle, GUILayout.ExpandWidth(false));
        }

        public void Dispose() {
        }
    }
}
