using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CoreKit.Editor
{
    [CustomEditor(typeof(SoundDatabase))]
    public class SoundDatabaseEditor : UnityEditor.Editor
    {
        private SerializedProperty _clipDatas;
        private ReorderableList _list;

        private void OnEnable()
        {
            _clipDatas = serializedObject.FindProperty("_clipDatas");

            _list = new ReorderableList(
                serializedObject,
                _clipDatas,
                true,
                true,
                true,
                true
            );

            _list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, _clipDatas.displayName);
            };

            _list.elementHeightCallback = index =>
            {
                return EditorGUIUtility.singleLineHeight * 3
                       + EditorGUIUtility.standardVerticalSpacing * 4;
            };

            _list.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element =
                    _clipDatas.GetArrayElementAtIndex(index);

                rect.y += 2;

                float height = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, height),
                    element.FindPropertyRelative("_type")
                );

                rect.y += height + spacing;

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, height),
                    element.FindPropertyRelative("_clip")
                );

                rect.y += height + spacing;

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, height),
                    element.FindPropertyRelative("_volume")
                );
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}