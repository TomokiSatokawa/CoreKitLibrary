# SceneLoader

Scene の**事前の非同期読み込み**と、**読み込み完了後の Scene 移動**を担当する `static` クラスです。
読み込みと移動を分けて呼べるため、「裏で次の Scene を読み込んでおき、演出やボタン操作のタイミングで切り替える」といった使い方ができます。

- 名前空間: `CoreKit.SceneLoading`
- 種類: `static class`
- 依存: [UniTask](https://github.com/Cysharp/UniTask)（`Cysharp.Threading.Tasks`）

---

## 概要

| メンバー | 種類 | 説明 |
|---|---|---|
| `IsLoading` | プロパティ | 読み込み中（未完了）なら `true` |
| `IsLoaded` | プロパティ | 読み込みが完了し、移動できる状態なら `true` |
| `LoadAsync(string sceneName, LoadSceneMode mode = Single, CancellationToken token = default)` | メソッド | Scene を非同期で読み込む。**読み込みのみで、Scene の移動はしない** |
| `TryMoveSceneAsync(CancellationToken token = default)` | メソッド | 読み込みが完了していれば Scene を移動し、`true` を返す。未完了なら何もせず `false` を返す |

### 状態の遷移

| 状態 | `IsLoading` | `IsLoaded` |
|---|---|---|
| 読み込みを開始していない | `false` | `false` |
| 読み込み中（進捗 0.9 未満） | `true` | `false` |
| 読み込み完了・移動待ち | `false` | `true` |
| 移動後 | `false` | `false` |

---

## 使い方

### 基本の流れ

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;
using CoreKit.SceneLoading;

public class TitleController : MonoBehaviour
{
    private async void Start()
    {
        // タイトル画面の表示中に、次の Scene を裏で読み込んでおく
        await SceneLoader.LoadAsync("Game");
    }

    public async void OnStartButtonClicked()
    {
        // 読み込みが終わっていれば、Scene を移動する
        bool moved = await SceneLoader.TryMoveSceneAsync();

        if (!moved)
        {
            Debug.Log("まだ読み込み中です");
        }
    }
}
```

### 読み込み完了を待ってから移動する

```csharp
await SceneLoader.LoadAsync("Game");

// 演出などを挟んでから移動
await UniTask.Delay(1000);

await SceneLoader.TryMoveSceneAsync();
```

### ボタンを、読み込み完了まで押せなくする

```csharp
private void Update()
{
    _startButton.interactable = SceneLoader.IsLoaded;
}
```

### CancellationToken を渡す

```csharp
await SceneLoader.LoadAsync("Game", token: destroyCancellationToken);
await SceneLoader.TryMoveSceneAsync(destroyCancellationToken);
```

> キャンセル時の動作は、「注意点」を参照してください。

---

## 仕組み

### LoadAsync

1. すでに読み込み中、または読み込み済みなら、`Debug.LogError` を出力して終了します。
2. `SceneManager.LoadSceneAsync` で読み込みを開始します。開始できなければ、`Debug.LogError` を出力して終了します。
3. `allowSceneActivation = false` にして、Scene が自動で有効化されないようにします。
4. 進捗が `0.9` 以上になるまで待ちます。

`allowSceneActivation = false` の間、`AsyncOperation.progress` は `0.9` で止まります。このクラスは、`0.9` に達した時点を「読み込み完了」として扱います。

### TryMoveSceneAsync

1. `IsLoaded` が `false` なら、`false` を返します。
2. 保持している読み込み情報を破棄し、`allowSceneActivation = true` にして、Scene を有効化します。
3. 有効化が完了する（`isDone`）まで待ち、`true` を返します。

---

## 注意点

- **`LoadAsync` が完了しても、読み込みが成功したとは限りません。**
  すでに読み込み中・読み込み済みの場合や、読み込みを開始できなかった場合は、エラーログを出力して、そのまま完了します（戻り値で成否は分かりません）。移動の前には、`IsLoaded` か `TryMoveSceneAsync` の戻り値で確認してください。
- **同時に、読み込めるのは1つだけです。**
  読み込み中、または読み込み済みで移動していない状態で `LoadAsync` を呼ぶと、エラーになります。
- **`LoadAsync` をキャンセルすると、状態が残ります。**
  キャンセルされても、読み込みの情報（内部の `_operation`）は破棄されません。そのため、`IsLoading` / `IsLoaded` の状態が残り、別の Scene を `LoadAsync` しようとすると、「既に読み込み中または読み込み済み」のエラーになります。読み込みを取り消す・やり直す手段（リセット用のメソッド）は、ありません。
  特に、`destroyCancellationToken` を渡している場合、呼び出し元のオブジェクトが破棄されると、読み込み途中でもキャンセルされ、この状態になる点に注意してください。
- **`TryMoveSceneAsync` のキャンセルは、移動を取り消しません。**
  `allowSceneActivation = true` にした後で待機をキャンセルしても、Scene の有効化は進みます。
- **`LoadSceneMode.Additive` の場合、「移動」は有効化だけです。**
  `TryMoveSceneAsync` が行うのは、読み込んだ Scene の有効化までです。アクティブな Scene の切り替えや、元の Scene のアンロードは行いません。必要な場合は、呼び出し側で行ってください。
- **読み込み中は、ほかの Scene の非同期読み込みが進まないことがあります。**
  `allowSceneActivation = false` の間は、Unity の仕様で、後から開始した別の非同期読み込みが待たされる場合があります。
- **読み込みの進捗（0〜1）を取得する手段は、ありません。**
  公開されているのは、`IsLoading` と `IsLoaded` の状態だけです。ローディングゲージを出したい場合は、進捗を返すプロパティの追加が必要です。
- **Scene 名は、Build Settings に登録しておく必要があります。**
  登録されていない Scene を指定すると、読み込みを開始できず、エラーになります。
- **「Reload Domain」を無効にしている場合は、状態が残ります。**
  Enter Play Mode Settings で Domain Reload を無効にしていると、`static` の `_operation` が、再生を止めても残ります。次の再生で、「既に読み込み中」のエラーになることがあります。
- **メインスレッドから呼んでください。**
  スレッドセーフではありません。