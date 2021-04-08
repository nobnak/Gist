using UnityEngine;

namespace nobnak.Gist.IMGUI.Scope {

	public class FoldoutScope : System.IDisposable {
        public const char CHAR_OPEN = '▼';
        public const char CHAR_CLOSE = '▶';

        protected GUIStyle foldoutStyle;

        public FoldoutScope(ref bool visible, string title = "", string tooltip = "") {

			if (foldoutStyle == null) {
				foldoutStyle = new GUIStyle(UnityEngine.GUI.skin.label) {
					alignment = TextAnchor.MiddleLeft
				};
				var coff = foldoutStyle.normal.textColor;
				var con = Color.white;

				foldoutStyle.onNormal.textColor
					= foldoutStyle.onHover.textColor
					= foldoutStyle.active.textColor
					= con;

				foldoutStyle.normal.textColor
					= foldoutStyle.hover.textColor 
					= foldoutStyle.onActive.textColor 
					= coff;
			}
			var foldoutTitle = (visible ? CHAR_OPEN : CHAR_CLOSE) + title;
            visible = GUILayout.Toggle(
				visible, 
				new GUIContent(foldoutTitle, tooltip), 
				foldoutStyle, 
				GUILayout.ExpandWidth(false));
        }

        public void Dispose() {
        }
    }
}
