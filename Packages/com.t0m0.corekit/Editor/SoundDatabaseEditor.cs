//AI
using CoreKit.Sound;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CoreKit.Editor
{
    /// <summary>
    /// SoundDatabaseのInspectorを拡張し、ClipData(SerializeReference)をReorderableListで表示する
    /// </summary>
    [CustomEditor(typeof(SoundDatabase))]
    public class SoundDatabaseEditor : UnityEditor.Editor
    {
        private SerializedProperty _clipDatas;
        private ReorderableList _clipList;

        private void OnEnable()
        {
            _clipDatas = serializedObject.FindProperty("_clipDatas");
            _clipList = CreateList(_clipDatas);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _clipList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// SerializeReference配列用のReorderableListを作成する。
        /// 要素の描画はPropertyFieldに任せ、型選択UIとサブクラスのフィールドを自動表示する
        /// </summary>
        /// <param name="property">配列のSerializedProperty</param>
        private ReorderableList CreateList(SerializedProperty property)
        {
            var list = new ReorderableList(serializedObject, property, true, true, true, true);

            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, property.displayName);
            };

            // 要素の中身(サブクラスごとに異なる)に応じて高さを変える
            list.elementHeightCallback = index =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true)
                       + EditorGUIUtility.standardVerticalSpacing * 2;
            };

            list.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);

                rect.y += EditorGUIUtility.standardVerticalSpacing;
                rect.height = EditorGUI.GetPropertyHeight(element, true);

                // SubclassSelectorのドロワーが型選択と子プロパティを描画する
                EditorGUI.PropertyField(rect, element, true);
            };

            // SerializeReferenceは追加時に直前の要素と参照が共有されてしまうため、nullにリセットする
            list.onAddCallback = l =>
            {
                int index = property.arraySize;
                property.arraySize++;
                property.GetArrayElementAtIndex(index).managedReferenceValue = null;
                l.index = index;
            };

            return list;
        }
    }
}