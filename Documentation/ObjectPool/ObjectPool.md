# ObjectPool

CoreKitLibrary の `CoreKit.ObjectPool` 機能の全体説明です。
Prefab 単位でオブジェクトを再利用し、生成と破棄のコストを減らします。

## 必要環境

- Unity 6 以上(`InstantiateAsync` を使用)
- UniTask

## 構成

| クラス | 役割 | 詳細 |
| --- | --- | --- |
| `PoolManager` | Prefab 単位でプールを管理し、取得と返却を行う | [PoolManager.md](PoolManager.md) |
| `PoolObject` | プールされるオブジェクトにアタッチする。有効化・返却のイベントと自動返却を担当する | [PoolObject.md](PoolObject.md) |
| `PoolPrefabDatabase` | プール対象の Prefab と、事前生成数を登録するデータ | [PoolPrefabDatabase.md](PoolPrefabDatabase.md) |

## 全体の流れ

1. `PoolManager` が `Awake` で、`PoolPrefabDatabase` に登録された数だけオブジェクトを事前に生成する
2. 呼び出し側が `PoolManager.Get(prefab)` でオブジェクトを取得する
3. 使い終わったら `PoolManager.Release(poolObject)` で返却する
4. 返却されたオブジェクトは、次回の `Get` で再利用される

## セットアップ

1. プール対象の Prefab のルートに `PoolObject` をアタッチする
2. `PoolPrefabDatabase` のアセットを作成し、Prefab と事前生成数を登録する
3. シーンに `PoolManager` を置き、Database を設定する
4. コードから `Get` と `Release` を呼ぶ

各手順の詳細は、それぞれのドキュメントを参照してください。

## 最小のサンプル

```csharp
using CoreKit.ObjectPool;
using UnityEngine;

public sealed class BulletSpawner : MonoBehaviour
{
    [SerializeField] private PoolManager _poolManager;
    [SerializeField] private PoolObject _bulletPrefab;

    public void Fire(Vector3 position, Quaternion rotation)
    {
        var bullet = _poolManager.Get(_bulletPrefab);

        if (bullet == null)
            return;

        bullet.transform.SetPositionAndRotation(position, rotation);
    }
}
```

## 状態の確認(Pool Monitor)

Play 中のプールの状態は、メニューの `CoreKit > Pool Monitor` で確認できます。

- Prefab ごとの稼働数と総数を表示します
- Prefab を開くと、各インスタンスが Active か Inactive かを確認できます
- インスタンスの名前を押すと、Hierarchy 上で選択されます
- 破棄済みのオブジェクトが残っている場合は `(Destroyed)` と表示されます

## 注意事項

- 取得したオブジェクトを `Destroy` しないでください。返却は必ず `Release` で行います
- シーン遷移でプール内のオブジェクトが破棄されると、`PoolManager` は破棄済みの参照を保持したままになります。`PoolManager` を `DontDestroyOnLoad` にするか、シーンごとに用意してください
- 複数の `PoolManager` を置く場合、プールは `PoolManager` ごとに独立します