//AI
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreKit.ObjectPool
{
    /// <summary>
    /// PoolManagerが保持するPoolの状態(Prefab別の稼働数・総数・各インスタンス)を確認するEditorウィンドウ
    /// </summary>
    public sealed class PoolMonitorWindow : EditorWindow
    {
        private readonly Dictionary<PoolObject, bool> _foldouts = new();

        private PoolManager _poolManager;
        private Vector2 _scrollPosition;

        private void OnInspectorUpdate()
        {
            if (!Application.isPlaying)
                return;

            if (_poolManager == null)
                _poolManager = FindFirstObjectByType<PoolManager>();

            Repaint();
        }

        private void OnGUI()
        {
            _poolManager = (PoolManager)EditorGUILayout.ObjectField("Pool Manager", _poolManager, typeof(PoolManager), true);

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Play中のみPoolの状態を表示できます", MessageType.Info);
                return;
            }

            if (_poolManager == null)
            {
                EditorGUILayout.HelpBox("シーン内にPoolManagerが見つかりません", MessageType.Warning);
                return;
            }

            using var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition);
            _scrollPosition = scrollView.scrollPosition;

            DrawSummary(_poolManager.PoolObjects);

            foreach (var pair in _poolManager.PoolObjects)
                DrawPool(pair.Key, pair.Value);
        }

        [MenuItem("CoreKit/Pool Monitor")]
        private static void Open()
        {
            GetWindow<PoolMonitorWindow>("Pool Monitor");
        }

        private static int CountActive(IReadOnlyList<PoolObject> instances)
        {
            var count = 0;

            for (var i = 0; i < instances.Count; i++)
            {
                if (instances[i] != null && instances[i].IsActive)
                    count++;
            }

            return count;
        }

        private void DrawSummary(IReadOnlyDictionary<PoolObject, List<PoolObject>> pools)
        {
            var totalCount = 0;
            var activeCount = 0;

            foreach (var pair in pools)
            {
                totalCount += pair.Value.Count;
                activeCount += CountActive(pair.Value);
            }

            EditorGUILayout.LabelField("Prefab種類", pools.Count.ToString());
            EditorGUILayout.LabelField("Active / Total", $"{activeCount} / {totalCount}");
            EditorGUILayout.Space();
        }

        private void DrawPool(PoolObject prefab, IReadOnlyList<PoolObject> instances)
        {
            _foldouts.TryGetValue(prefab, out var isOpen);

            using (new EditorGUILayout.HorizontalScope())
            {
                isOpen = EditorGUILayout.Foldout(isOpen, prefab.name, true);

                if (GUILayout.Button("Ping", EditorStyles.miniButton, GUILayout.Width(40f)))
                    EditorGUIUtility.PingObject(prefab);
            }

            _foldouts[prefab] = isOpen;

            DrawUsageBar(CountActive(instances), instances.Count);

            if (!isOpen)
                return;

            EditorGUI.indentLevel++;

            for (var i = 0; i < instances.Count; i++)
                DrawInstance(instances[i]);

            EditorGUI.indentLevel--;
        }

        private void DrawUsageBar(int activeCount, int totalCount)
        {
            var ratio = totalCount == 0 ? 0f : (float)activeCount / totalCount;

            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), ratio, $"Active {activeCount} / Total {totalCount}");
        }

        private void DrawInstance(PoolObject poolObject)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (poolObject == null)
                {
                    EditorGUILayout.LabelField("(Destroyed)");
                    return;
                }

                var defaultColor = GUI.color;
                GUI.color = poolObject.IsActive ? Color.green : Color.gray;
                EditorGUILayout.LabelField(poolObject.IsActive ? "● Active" : "○ Inactive", GUILayout.Width(80f));
                GUI.color = defaultColor;

                if (GUILayout.Button(poolObject.name, EditorStyles.miniButton))
                    Selection.activeGameObject = poolObject.gameObject;
            }
        }
    }
}