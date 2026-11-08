using System.Collections.Generic;
using CoreKit.ObjectPool;
using UnityEngine;

namespace CoreKit
{
    [CreateAssetMenu(fileName = "PoolPrefabDatabase", menuName = "CoreKit/ObjectPool/PoolPrefabDatabase")]
    public class PoolPrefabDatabase : ScriptableObject
    {
        [SerializeField] private List<PoolPrefabData> _prefabDatas;

        public IReadOnlyList<PoolPrefabData> PrefabDatas => _prefabDatas;

        [System.Serializable]
        public class PoolPrefabData
        {
            [SerializeField] private PoolObject _prefab;
            [SerializeField] private int _count;

            public PoolObject Prefab => _prefab;
            public int Count => _count;
        }

        public bool IsPrefabContains(PoolObject prefab)
        {
            return _prefabDatas.Find(x => x.Prefab == prefab) != null;
        }
    }
}
