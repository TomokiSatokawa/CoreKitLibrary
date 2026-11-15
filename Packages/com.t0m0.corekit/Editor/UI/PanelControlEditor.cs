using CoreKit.UI;
using UnityEditor;
using UnityEngine;

namespace CoreKit.Editor.UI
{
    /// <summary>
    /// PanelControl‚ÌInspectorŠg’£
    /// </summary>
    [CustomEditor(typeof(PanelControl))]
    public class PanelControlEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(5);

            PanelControl panel = (PanelControl)target;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("•\Ž¦"))
                    SetVisible(panel, true);

                if (GUILayout.Button("”ñ•\Ž¦"))
                    SetVisible(panel, false);
            }
        }

        private void SetVisible(PanelControl panel, bool visible)
        {
            Undo.RecordObject(panel, "Change Panel Visibility");

            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                return;

            Undo.RecordObject(canvasGroup, "Change Panel Visibility");

            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;

            EditorUtility.SetDirty(canvasGroup);
        }
    }
}
