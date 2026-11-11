//AI
using UnityEditor;
using UnityEngine;

namespace CoreKit.DynamicEnum.Editor
{
    /// <summary>
    /// DynamicEnumValueをドロップダウン表示する
    /// </summary>
    [CustomPropertyDrawer(typeof(DynamicEnumValue))]
    public class DynamicEnumValueDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(
            SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var definition = property.FindPropertyRelative("_definition");
            var guid = property.FindPropertyRelative("_guid");

            float labelWidth = EditorGUIUtility.labelWidth;
            float spacing = 4f;

            Rect labelRect = new Rect(
                position.x, position.y,
                labelWidth, position.height);

            Rect contentRect = new Rect(
                position.x + labelWidth,
                position.y,
                position.width - labelWidth,
                position.height);

            float definitionWidth = contentRect.width * 0.4f;

            Rect definitionRect = new Rect(
                contentRect.x, contentRect.y,
                definitionWidth - spacing, contentRect.height);

            Rect popupRect = new Rect(
                contentRect.x + definitionWidth, contentRect.y,
                contentRect.width - definitionWidth, contentRect.height);

            EditorGUI.LabelField(labelRect, label);

            EditorGUI.PropertyField(
                definitionRect, definition, GUIContent.none);

            var asset = definition.objectReferenceValue
                as DynamicEnumDefinition;

            if (asset == null)
            {
                EditorGUI.Popup(popupRect, 0, new[] { "None" });
                guid.stringValue = string.Empty;
                EditorGUI.EndProperty();
                return;
            }

            var entries = asset.Entries;

            string[] options = new string[entries.Count + 1];
            options[0] = "None";

            int selectedIndex = 0;

            for (int i = 0; i < entries.Count; i++)
            {
                options[i + 1] = entries[i].Name;

                if (entries[i].Guid == guid.stringValue)
                    selectedIndex = i + 1;
            }

            int newIndex = EditorGUI.Popup(
                popupRect, selectedIndex, options);

            if (newIndex == 0)
                guid.stringValue = string.Empty;
            else
                guid.stringValue = entries[newIndex - 1].Guid;

            EditorGUI.EndProperty();
        }
    }
}