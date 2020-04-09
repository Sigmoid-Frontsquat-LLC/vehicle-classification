using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LangAttribute))]
public class LangAttributeDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		//base.OnGUI(position, property, label);
		int selected = System.Array.FindIndex(Localization.Supported, i => string.Compare(i, property.stringValue) == 0);

		if(selected == -1) selected = 0;

		selected = EditorGUI.Popup(position, label.text, selected, Localization.Supported);

		property.stringValue = Localization.Supported[selected];
	}
}