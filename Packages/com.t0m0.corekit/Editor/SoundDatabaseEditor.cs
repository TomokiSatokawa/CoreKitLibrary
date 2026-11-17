//AI
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CoreKit.Editor
{
    /// <summary>
    /// SoundDatabaseのInspectorを拡張し、ClipDataとRandomClipDataをReorderableListで表示する
    /// </summary>
    [CustomEditor(typeof(SoundDatabase))]
    public class SoundDatabaseEditor : UnityEditor.Editor
    {
        private const int ElementLineCount = 3;

        private SerializedProperty _clipDatas;
        private SerializedProperty _randomClipDatas;
        private ReorderableList _clipList;
        private ReorderableList _randomClipList;

        private void OnEnable()
        {
            _clipDatas = serializedObject.FindProperty("_clipDatas");
            _randomClipDatas = serializedObject.FindProperty("_randomClipData");

            _clipList = CreateList(_clipDatas, "_clip");
            _randomClipList = CreateList(_randomClipDatas, "_audioResource");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _clipList.DoLayoutList();
            EditorGUILayout.Space();
            _randomClipList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Type / 音源 / Volume の3行を表示するリストを作成する
        /// </summary>
        /// <param name="property">配列のSerializedProperty</param>
        /// <param name="sourcePropertyName">2行目に表示する音源プロパティ名</param>
        private ReorderableList CreateList(SerializedProperty property, string sourcePropertyName)
        {
            var list = new ReorderableList(serializedObject, property, true, true, true, true);

            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, property.displayName);
            };

            list.elementHeightCallback = index =>
            {
                return EditorGUIUtility.singleLineHeight * ElementLineCount
                       + EditorGUIUtility.standardVerticalSpacing * (ElementLineCount + 1);
            };

            list.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);

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
                    element.FindPropertyRelative(sourcePropertyName)
                );

                rect.y += height + spacing;

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, height),
                    element.FindPropertyRelative("_volume")
                );
            };

            return list;
        }
    }
}