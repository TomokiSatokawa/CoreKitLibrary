# FlagRegistry

複数の要求元からのフラグ登録を管理し、**1つでも登録があれば `Flag` が `true`** になるクラスです。
「入力を無効化する」「ポーズ扱いにする」「カーソルを表示する」など、複数の箇所が同じフラグを独立して ON/OFF したい場面で使います。

- 名前空間: `CoreKit`
- 種類: 通常の `class`（`MonoBehaviour` ではありません）
- 関連型: `FlagToken`（登録を識別するトークン）

---

## 概要

### FlagRegistry

| メンバー | 種類 | 説明 |
|---|---|---|
| `Flag` | プロパティ | 登録が1つ以上あれば `true` |
| `RegisterFlag()` | メソッド | フラグを登録し、解除用の `FlagToken` を返す |
| `UnregisterFlag(FlagToken token)` | メソッド | 登録時のトークンを渡して登録を解除する |

### FlagToken

| メンバー | 種類 | 説明 |
|---|---|---|
| `IsValid` | プロパティ | 有効なトークンなら `true`（`default` のトークンは `false`） |
| `Create()` | 静的メソッド | 新しいトークンを生成する（通常は `RegisterFlag()` が内部で呼ぶので直接使う必要はありません） |
| `==` / `!=` / `Equals` | - | ID が同じなら等しいと判定 |

---

## 基本的な使い方

```csharp
using CoreKit;

var registry = new FlagRegistry();

Debug.Log(registry.Flag); // false

// 登録（Flag が true になる）
FlagToken token = registry.RegisterFlag();
Debug.Log(registry.Flag); // true

// 解除（登録が0件になるので Flag が false に戻る）
registry.UnregisterFlag(token);
Debug.Log(registry.Flag); // false
```

### 複数の要求元が登録した場合

`Flag` は **全員が解除するまで `true` のまま** です。

```csharp
var registry = new FlagRegistry();

var tokenA = registry.RegisterFlag(); // 要求元A
var tokenB = registry.RegisterFlag(); // 要求元B

registry.UnregisterFlag(tokenA);
Debug.Log(registry.Flag); // true（Bがまだ登録中）

registry.UnregisterFlag(tokenB);
Debug.Log(registry.Flag); // false
```

---

## 使用例

### プレイヤー入力のロック管理

メニュー、ダイアログ、カットシーンなど複数の処理がそれぞれ入力ロックを要求する例です。
どれか1つでもロックを要求していれば入力を無効化します。

```csharp
using UnityEngine;
using CoreKit;

public class InputLock : MonoBehaviour
{
    private readonly FlagRegistry _lockRegistry = new FlagRegistry();

    public bool IsLocked => _lockRegistry.Flag;

    public FlagToken Lock() => _lockRegistry.RegisterFlag();

    public void Unlock(FlagToken token) => _lockRegistry.UnregisterFlag(token);
}
```

```csharp
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private InputLock _inputLock;

    private CoreKit.FlagToken _lockToken;

    private void OnEnable()
    {
        _lockToken = _inputLock.Lock();
    }

    private void OnDisable()
    {
        _inputLock.Unlock(_lockToken);
    }
}
```

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputLock _inputLock;

    private void Update()
    {
        if (_inputLock.IsLocked) return;

        // 通常の入力処理
    }
}
```

### 登録済みかどうかをトークンで管理する

トークンは `struct` なので、未登録状態は `default` で表せます。`IsValid` で登録中か判定できます。

```csharp
private FlagToken _token; // default の間は未登録

public void Begin()
{
    if (_token.IsValid) return; // 二重登録を防ぐ
    _token = _registry.RegisterFlag();
}

public void End()
{
    if (!_token.IsValid) return; // 未登録なら何もしない
    _registry.UnregisterFlag(_token);
    _token = default;            // 解除済みにする
}
```

---

## 注意点

- **必ず解除してください。**
  解除し忘れると `Flag` が `true` のまま戻らなくなります。`OnEnable` / `OnDisable` や `OnDestroy` など、対になるタイミングで登録・解除すると安全です。
- **同じトークンを2回解除するとエラーになります。**
  未登録のトークンを渡すと `Debug.LogError` が出力され、何も変更されません。上記の「`_token = default` に戻す」パターンで防げます。
- **`default` のトークンは解除できません。**
  `IsValid == false` のトークンを渡した場合も同様にエラーログが出ます。
- **別の `FlagRegistry` のトークンは解除できません。**
  トークンは全体で一意の ID を持ちますが、登録した `FlagRegistry` 以外に渡すと「未登録」扱いでエラーになります。
- **スレッドセーフではありません。**
  `FlagToken` の ID 採番（`static`）も `FlagRegistry` 内のリスト操作も排他制御していないため、Unity のメインスレッドからのみ使用してください。
- `FlagRegistry` は `MonoBehaviour` ではないため Inspector には表示されません。`new FlagRegistry()` で生成して、フィールドとして保持してください。