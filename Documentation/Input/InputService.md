# InputService

Unity の Input System を、旧 `InputManager` のような感覚で使うためのライブラリです。

- 文字列でアクション名を指定しない（補完が効く）
- キーの追加は **Input Actions アセットを編集して保存するだけ**（コード変更なし）
- `Key` / `Down` / `Up` で押下状態を取得できる（R3 不要）
- アクションマップ（Player / UI など）単位で入力をロックできる
- R3 があればオプションで Observable として扱える

## 動作環境

| 項目 | 内容 |
|---|---|
| Unity | 6 以降（Project-wide Actions を使用） |
| 依存 | `com.unity.inputsystem` |
| 任意 | `R3`（`com.cysharp.r3`） |

## セットアップ

### 1. Project-wide Actions を設定する

`Project Settings > Input System Package` で、Project-wide Actions に Input Actions アセットを割り当てます（新規プロジェクトなら `InputSystem_Actions` が最初から割り当て済み）。

### 2. `InputService` を生成する

メニューから実行します。

```
CoreKit > Input > Generate InputService
```

`Assets/Generated/InputService.cs` が生成されます。以降は、Project-wide Actions のアセットを保存するたびに自動で再生成されます。

### 3. Active Input Handling を確認する

`Project Settings > Player > Active Input Handling` が **Input System Package (New)** か **Both** になっていることを確認してください。

## 基本的な使い方

`InputService.<マップ名>.<アクション名>` でアクセスします。

```csharp
void Update()
{
    // Vector2 の取得
    Vector2 move = InputService.Player.Move.Value;
    Vector2 look = InputService.Player.Look.Value;

    // ボタンの取得（Key=押している間 / Down=押した瞬間 / Up=離した瞬間）
    if (InputService.Player.Jump.Down)   Jump();
    if (InputService.Player.Sprint.Key)  Sprint();
    if (InputService.Player.Jump.Up)     ReleaseJump();
}
```

## 入力の種類

アクションの種類に応じて、2つのクラスが自動で割り当てられます。

| アクションの種類 | クラス | 主なメンバー |
|---|---|---|
| Button | `InputButton` | `Key` `Down` `Up`（プロパティ）、`OnPressed()` `OnReleased()` |
| Vector2 | `InputVector2` | `Value` `OnChanged()` |

上記以外（float、Vector3 など）は、現在は警告を出して生成をスキップします。

## イベント

`OnPressed` などは `IDisposable` を返します。**不要になったら必ず `Dispose` してください。**

```csharp
private IDisposable _pressed;
private IDisposable _moved;

private void OnEnable()
{
    _pressed = InputService.Player.Attack.OnPressed(Attack);
    _moved   = InputService.Player.Move.OnChanged(v => Debug.Log(v));
}

private void OnDisable()
{
    _pressed.Dispose();
    _moved.Dispose();
}
```

| メソッド | 呼ばれるタイミング |
|---|---|
| `InputButton.OnPressed(Action)` | ボタンが押された（performed） |
| `InputButton.OnReleased(Action)` | ボタンが離された（canceled） |
| `InputVector2.OnChanged(Action<Vector2>)` | 値が変化した。入力が止まると `Vector2.zero` で通知される |

`Interact` のように Input Actions 側で長押し（Hold）などの Interaction を設定している場合、`OnPressed` は**長押しが成立したとき**に発火します。`Key` は押した瞬間から true になります。

## 入力のロック

ポーズ中や会話中など、入力を一時的に止める仕組みが2種類あります。

### グループ単位（アクションマップ単位）

```csharp
// 単純な ON/OFF
InputService.Player.SetEnabled(false);
InputService.Player.SetEnabled(true);

// 参照カウント式（複数箇所から止めても競合しない）
using var block = InputService.Player.Block();
```

### 全体

```csharp
using var block = InputBlocker.Block();
```

### ロックされたときの挙動

次のいずれかに該当すると、そのアクションはロック状態になります。

- `InputBlocker.Block()` 中
- 所属グループが `SetEnabled(false)`
- 所属グループが `Block()` 中

ロック状態のアクションは、`Key` / `Down` / `Up` が `false`、`Value` が `Vector2.zero` を返し、イベントも発火しません。

### `SetEnabled` と `Block()` の使い分け

| 用途 | おすすめ |
|---|---|
| 1か所で管理するON/OFF | `SetEnabled` |
| ポーズ・会話・演出など、複数の仕組みが重なる | `Block()`（参照カウント） |

`Block()` は全員が `Dispose` して初めて解除されます。

```csharp
var pause = InputService.Player.Block();   // カウント1
var talk  = InputService.Player.Block();   // カウント2

talk.Dispose();                            // カウント1（まだロック中）
pause.Dispose();                           // カウント0（解除）
```

| 拡張メソッド | 対象 | 内容 |
|---|---|---|
| `PressedAsObservable()` | `InputButton` | 押下の通知 |
| `ReleasedAsObservable()` | `InputButton` | 離しの通知 |
| `HeldAsReactiveProperty()` | `InputButton` | 押している間 true |
| `ChangedAsObservable()` | `InputVector2` | 値の変化の通知 |

R3 が無い環境では、この拡張は自動でコンパイル対象から外れます。

## Project-wide Actions 以外のアセットを使う

別の Input Actions アセットを使いたい場合は、最初に一度だけ呼びます。

```csharp
InputSource.Use(myInputActionAsset);
```

## 注意点

- **無効化中の「離し」は通知されません**
  押したままロックして、ロック中にキーを離しても `OnReleased` は発火しません。チャージ攻撃の解除のように重要な処理は、ロック時に自前でリセットするか、`Key` の状態を見てください。
- **ロック解除直後の押しっぱなし**
  ロック中に押されたキーは、解除した直後に `Key` が true に見えます（キーの物理状態は変わっているため）。
- **`Look` の感度**
  マウスは「フレームあたりの移動量」、スティックは「傾き」でスケールが違います。感度は使う側で調整してください。
- **生成されたコードは手編集しない**
  保存のたびに上書きされます。独自の処理は別クラスに書いてください。
- **名前に使えない文字**
  アクション名やマップ名に空白や記号が含まれる場合は `_` に置き換えられます。
- **`InputService` が見つからない**
  生成直後は、コンパイルが走るまで見つかりません。見つからない場合は `CoreKit > Input > Generate InputService` を手動で実行してください。
- **ドメインリロード無効**
  再生開始時にキャッシュとロック状態が自動でリセットされます。ただしイベントは自動で解除されないので、`Dispose` は必ず行ってください。

## トラブルシューティング

| 症状 | 原因と対処 |
|---|---|
| `Project-wide Actionsが未設定です` | `Project Settings > Input System Package` でアセットを割り当てる |
| `InputAction 'Player/Xxx' が見つかりません` | アクション名を変更した後は、`InputService` を再生成する |
| `InputAction` の型が解決できない | asmdef の参照に `Unity.InputSystem` を追加する |
| ゲーム側 asmdef から `InputService` が見えない | ゲーム側 asmdef に `CoreKit.Inputs` を追加する |
| `GameInputs` がライブラリから見えない | asmdef で分けたアセンブリは `Assembly-CSharp` を参照できない。ライブラリ内で `GameInputs` を使わない |
| キー入力が反応しない | Gameビューにフォーカスがあるか、Active Input Handling の設定を確認する |

## ファイル構成

```
Runtime/
  CoreKit.Inputs.asmdef
  InputBlocker.cs      全体ロック（参照カウント）
  InputSource.cs       使用する Input Actions アセットの解決
  InputGroup.cs        アクションマップ単位のロック
  InputBase.cs         InputButton / InputVector2 の共通処理
  InputButton.cs       ボタン入力
  InputVector2.cs      Vector2 入力
Editor/
  CoreKit.Inputs.Editor.asmdef
  InputServiceGenerator.cs   InputService の自動生成
```
