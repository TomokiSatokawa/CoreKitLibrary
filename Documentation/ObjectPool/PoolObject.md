# PoolObject

プールされるオブジェクトにアタッチする `MonoBehaviour` です。
有効化と返却のイベント、時間経過による自動返却を担当します。
全体の説明は [ObjectPool.md](ObjectPool.md) を参照してください。

## セットアップ

プール対象の Prefab のルートに `PoolObject` をアタッチします。
`PoolManager.Get` に渡す Prefab は、この `PoolObject` の参照です。

## インスペクター

| 項目 | 内容 |
| --- | --- |
| `On Activate` | `Get` で有効化されたときに呼ばれるイベント |
| `On Release` | 返却されたときに呼ばれるイベント |
| `Is Auto Release` | 有効化から一定時間後に、自動で返却するか |
| `Auto Release Duration` | 自動返却までの時間(秒) |
| `Custom Component` | `GetCustomComponent` で取得したいコンポーネントを登録する配列 |

## API

### `bool IsActive`

オブジェクトが使用中かどうかを返します。
`PoolManager` は、この値が `false` のオブジェクトを未使用として再利用します。

生成直後は `true` です。事前生成では、生成後に `Release` が呼ばれて `false` になります。

### `void Activate()`

オブジェクトを使用中にして、有効化します。通常は `PoolManager.Get` が呼ぶので、直接呼ぶ必要はありません。

1. `IsActive` を `true` にする
2. `SetActive(true)` で有効化する
3. 自動返却がオンなら、返却する時刻を設定する
4. `On Activate` を呼ぶ

### `void Release()`

オブジェクトを返却します。`PoolManager.Release` から呼ばれるほか、直接呼んでも構いません。

1. `IsActive` を `false` にする
2. `On Release` を呼ぶ
3. `SetActive(false)` で非アクティブにする

すでに返却済みの場合は、何もしません。`On Release` が二重に呼ばれることはありません。

### `T GetCustomComponent<T>() where T : MonoBehaviour`

`Custom Component` に登録したコンポーネントのうち、型 `T` に一致するものを返します。
`Get` で取得した `PoolObject` から、`GetComponent` を使わずに目的のコンポーネントへアクセスできます。

```csharp
var poolObject = _poolManager.Get(_bulletPrefab);

if (poolObject == null)
    return;

var bullet = poolObject.GetCustomComponent<Bullet>();

if (bullet == null)
    return;

bullet.Launch(direction);
```

- 登録されていない型を指定した場合は、エラーログを出して `null` を返します。使う前に `null` チェックをしてください
- 同じ型が複数登録されている場合は、配列の先頭にあるものを返します
- 派生クラスも取得できます。たとえば `HomingBullet`(`Bullet` を継承)を登録していれば、`GetCustomComponent<Bullet>()` で取得できます
- 検索は配列を先頭から順に調べるので、登録数は数個程度に抑えてください

## 自動返却

エフェクトや弾など、一定時間で消えるオブジェクトは `Is Auto Release` をオンにします。
呼び出し側で `Release` する必要がなくなります。

- 時間は `Time.time` 基準です。`Time.timeScale = 0` の間はカウントが進みません
- 自動返却を使わないオブジェクトでは、`Update` は動きません

## 注意事項

- `On Activate` の時点では、位置や回転は `Get` の呼び出し側でまだ設定されていません
- 取得したオブジェクトを `Destroy` しないでください。返却は必ず `Release` で行います
- `IsActive` の初期値(`true`)を変えると、事前生成したオブジェクトが返却されなくなります
- `GetCustomComponent` で取得するコンポーネントは、Prefab のインスペクターで `Custom Component` に登録しておく必要があります