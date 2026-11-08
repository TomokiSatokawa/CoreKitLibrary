# PoolManager

Prefab 単位でプールを管理し、オブジェクトの取得と返却を行う `MonoBehaviour` です。
全体の説明は [ObjectPool.md](ObjectPool.md) を参照してください。

## セットアップ

1. シーンに空の GameObject を作り、`PoolManager` をアタッチする
2. `Prefab Database` に [PoolPrefabDatabase](PoolPrefabDatabase.md) のアセットを設定する
3. `Max Spawn Count Per Frame` を設定する(事前生成で 1 回に生成する数。1 以上)

`Prefab Database` が未設定の場合は、エラーログを出して事前生成を行いません。

## インスペクター

| 項目 | 内容 |
| --- | --- |
| `Prefab Database` | プール対象の Prefab と事前生成数を登録したデータ |
| `Max Spawn Count Per Frame` | 事前生成で、1 回の生成にまとめる数 |

## API

### `PoolObject Get(PoolObject prefab)`

Prefab に対応するオブジェクトを取得して、有効化します。

- 未使用のオブジェクトがあれば再利用し、なければ新しく生成します
- 取得したオブジェクトは自動で有効化(`Activate`)されます
- `prefab` が `null` の場合、または Database に未登録の Prefab の場合は、エラーログを出して `null` を返します

```csharp
var bullet = _poolManager.Get(_bulletPrefab);

if (bullet == null)
    return;

bullet.transform.SetPositionAndRotation(position, rotation);
```

位置や回転は `Get` のあとに設定します。`On Activate` の時点では、まだ設定されていません。

### `void Release(PoolObject poolObject)`

オブジェクトを返却します。`poolObject.Release()` を呼ぶのと同じ動作です。

```csharp
_poolManager.Release(bullet);
```

`null` を渡した場合は、エラーログを出して何もしません。

### `IReadOnlyDictionary<PoolObject, List<PoolObject>> PoolObjects`

Prefab ごとのプールの中身を読み取るためのプロパティです。Pool Monitor が使用します。
値の `List` は書き換え可能ですが、外部から変更しないでください。

## 事前生成

`Awake` で、Database に登録された数だけオブジェクトを事前に生成します。

- `Max Spawn Count Per Frame` の数ずつ、複数フレームに分けて生成します
- 生成したオブジェクトはすぐに返却され、非アクティブな状態で待機します
- `PoolManager` が破棄されると、事前生成は中断されます
- 事前生成の完了前に `Get` を呼んでも動作します。その場合は新しく生成されます

## 注意事項

- シーン遷移でプール内のオブジェクトが破棄されると、破棄済みの参照が残ります。`PoolManager` を `DontDestroyOnLoad` にするか、シーンごとに用意してください
- 複数の `PoolManager` を置く場合、プールは `PoolManager` ごとに独立します
- 未使用のオブジェクトは先頭から順に探します。プールが大きく、使用中のものが多いと `Get` のコストが増えます