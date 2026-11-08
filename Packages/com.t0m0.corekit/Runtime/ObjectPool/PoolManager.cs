using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CoreKit.ObjectPool
{
    /// <summary>
    /// PoolObject‚ğPrefab’PˆÊ‚Åƒv[ƒ‹‚µAæ“¾‚Æ•Ô‹p‚ğs‚¤
    /// </summary>
    public sealed class PoolManager : MonoBehaviour
    {
        [SerializeField] private PoolPrefabDatabase _prefabDatabase;
        [SerializeField] private int _maxSpawnCountPerFrame;

        private readonly Dictionary<PoolObject, List<PoolObject>> _poolObjects = new();
        public IReadOnlyDictionary<PoolObject, List<PoolObject>> PoolObjects => _poolObjects;

        private void Awake()
        {
            if (_prefabDatabase == null)
            {
                Debug.LogError("[PoolManager] PrefabDatabase‚ª–¢İ’è‚Å‚·");
                return;
            }

            InitializePoolAsync().Forget();
        }

        private async UniTaskVoid InitializePoolAsync()
        {
            var batchSize = Mathf.Max(1, _maxSpawnCountPerFrame);

            foreach (var prefabData in _prefabDatabase.PrefabDatas)
            {
                var prefab = prefabData.Prefab;

                if (prefab == null)
                {
                    Debug.LogError("[PoolManager] PrefabDatabase‚ÉPrefab‚ª–¢İ’è‚Ì—v‘f‚ª‚ ‚è‚Ü‚·");
                    continue;
                }

                if (!_poolObjects.TryGetValue(prefab, out var list))
                {
                    list = new List<PoolObject>(prefabData.Count);
                    _poolObjects.Add(prefab, list);
                }

                var remaining = prefabData.Count;

                while (remaining > 0)
                {
                    var count = Mathf.Min(remaining, batchSize);
                    var created = await InstantiateAsync(prefab, count);

                    list.AddRange(created);

                    for (var i = 0; i < created.Length; i++)
                        created[i].Release();

                    remaining -= count;
                }
            }
        }

        public PoolObject Get(PoolObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("[PoolManager] prefab‚ªnull‚Å‚·");
                return null;
            }

            if (!_poolObjects.TryGetValue(prefab, out var list))
            {
                if (!_prefabDatabase.IsPrefabContains(prefab))
                {
                    Debug.LogError($"[PoolManager] Database‚É–¢“o˜^‚ÌPrefab‚Å‚·: {prefab.name}");
                    return null;
                }

                list = new List<PoolObject>();
                _poolObjects.Add(prefab, list);
            }

            var result = FindInactive(list) ?? CreateObject(prefab, list);
            result.Activate();

            return result;
        }

        public void Release(PoolObject poolObject)
        {
            if (poolObject == null)
            {
                Debug.LogError("[PoolManager] •Ô‹p‚·‚éPoolObject‚ªnull‚Å‚·");
                return;
            }

            poolObject.Release();
        }

        private PoolObject FindInactive(List<PoolObject> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (!list[i].IsActive)
                    return list[i];
            }

            return null;
        }

        private PoolObject CreateObject(PoolObject prefab, List<PoolObject> list)
        {
            var target = Instantiate(prefab);
            list.Add(target);

            return target;
        }
    }
}