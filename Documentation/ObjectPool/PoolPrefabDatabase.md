# PoolPrefabDatabase

プール対象の Prefab と、事前生成数を登録するデータです。
全体の説明は [ObjectPool.md](ObjectPool.md) を参照してください。

## 役割

- `PoolManager` が事前生成する Prefab と、その数を決める
- `PoolManager` が、プール対象として有効な Prefab かどうかを判定する

## 登録する内容

`PrefabDatas` に、次の内容を持つ要素を並べます。

| 項目 | 内容 |
| --- | --- |
| `Prefab` | プール対象の `PoolObject`(Prefab) |
| `Count` | 事前に生成する数 |

## セットアップ

1. `PoolPrefabDatabase` のアセットを作成する
2. `PrefabDatas` に、プールしたい Prefab と事前生成数を登録する
3. [PoolManager](PoolManager.md) の `Prefab Database` に、このアセットを設定する

## API

### `bool IsPrefabContains(PoolObject prefab)`

Database に Prefab が登録されているかを返します。
`PoolManager.Get` が、初めて取得する Prefab に対して判定します。

## 動作

- `PoolManager.Get` に渡した Prefab が未登録の場合は、エラーログを出して `null` を返します
- `Count` を 0 にした Prefab は、事前生成されません。`Get` したときに初めて生成されます
- Prefab が未設定(`null`)の要素は、エラーログを出してスキップされます
- 同じ Prefab を複数の要素に登録すると、事前生成数は合計されます

## 注意事項

- `Count` には、同時に使う最大数を目安に設定してください。足りない場合は `Get` のときに生成されますが、そのフレームの負荷が増えます
- 多すぎると、起動時の生成と待機中のメモリが増えます。Pool Monitor で稼働数の最大値を見ながら調整してください