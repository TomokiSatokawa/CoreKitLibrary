using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InlineAttribute))]
public class InlineDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        var iter = property.Copy();
        var end = property.GetEndProperty();
        bool enter = true;

        while (iter.NextVisible(enter) && !SerializedProperty.EqualContents(iter, end))
        {
            height += EditorGUI.GetPropertyHeight(iter, true)
                    + EditorGUIUtility.standardVerticalSpacing;
            enter = false;
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var iter = property.Copy();
        var end = property.GetEndProperty();
        bool enter = true;

        while (iter.NextVisible(enter) && !SerializedProperty.EqualContents(iter, end))
        {
            float h = EditorGUI.GetPropertyHeight(iter, true);
            position.height = h;
            EditorGUI.PropertyField(position, iter, true);
            position.y += h + EditorGUIUtility.standardVerticalSpacing;
            enter = false;
        }

        EditorGUI.EndProperty();
    }
}