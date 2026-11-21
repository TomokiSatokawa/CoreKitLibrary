using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreKit.SceneLoading
{
    /// <summary>
    /// Sceneの事前非同期読み込みと、読み込み完了後のScene移動を担当するstaticクラス
    /// </summary>
    public static class SceneLoader
    {
        // allowSceneActivation = false の間、progressはこの値で止まる
        private const float LoadedProgress = 0.9f;

        private static AsyncOperation _operation;

        /// <summary>読み込み中(未完了)か</summary>
        public static bool IsLoading => _operation != null && !IsLoaded;

        /// <summary>読み込みが完了し、移動可能な状態か</summary>
        public static bool IsLoaded => _operation != null && _operation.progress >= LoadedProgress;

        /// <summary>
        /// Sceneを非同期で読み込む。読み込みのみで、Sceneの移動は行わない
        /// </summary>
        public static async UniTask LoadAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, CancellationToken token = default)
        {
            if (_operation != null)
            {
                Debug.LogError($"[SceneLoader] 既に読み込み中または読み込み済みです。sceneName={sceneName}");
                return;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, mode);
            if (operation == null)
            {
                Debug.LogError($"[SceneLoader] Sceneの読み込みを開始できません。sceneName={sceneName}");
                return;
            }

            operation.allowSceneActivation = false;
            _operation = operation;

            await UniTask.WaitUntil(() => operation.progress >= LoadedProgress, cancellationToken: token);
        }

        /// <summary>
        /// 読み込みが完了していればSceneを移動する。未完了ならfalseを返す
        /// </summary>
        public static async UniTask<bool> TryMoveSceneAsync(CancellationToken token = default)
        {
            if (!IsLoaded)
            {
                return false;
            }

            AsyncOperation operation = _operation;
            _operation = null;

            operation.allowSceneActivation = true;
            await UniTask.WaitUntil(() => operation.isDone, cancellationToken: token);

            return true;
        }
    }
}