using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace nobnak.Gist.IMGUI {

	[CustomPropertyDrawer(typeof(LayerAttribute))]
	public class LayerAttributeEditor : PropertyDrawer {
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {
			EditorGUI.BeginProperty(pos, label, prop);
			int index = Mathf.Clamp(prop.intValue, 0, 31);
			prop.intValue = EditorGUI.LayerField(pos, label, index);
			EditorGUI.EndProperty();
		}
	}

	public class LayerAttribute : PropertyAttribute {}
}
