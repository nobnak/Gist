using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace nobnak.Gist.IMGUI {

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(LayerAttribute))]
	public class LayerAttributeEditor : PropertyDrawer {
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {
			EditorGUI.BeginProperty(pos, label, prop);
			int index = Mathf.Clamp(prop.intValue, 0, 31);
			prop.intValue = EditorGUI.LayerField(pos, label, index);
			EditorGUI.EndProperty();
		}
	}
#endif

	public class LayerAttribute : PropertyAttribute {}
}
