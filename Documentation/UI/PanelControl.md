# PanelControl

UI のウィンドウ（パネル）の**表示 / 非表示を切り替える** `MonoBehaviour` です。
`CanvasGroup` の `alpha` と `blocksRaycasts` を切り替えるため、GameObject を非アクティブにせずに、パネルを出し入れできます。表示・非表示のタイミングで、`UnityEvent` を呼び出すこともできます。

- 名前空間: `CoreKit.UI`
- 種類: `MonoBehaviour`（`[RequireComponent(typeof(CanvasGroup))]`）

---

## 概要

| メンバー | 種類 | 説明 |
|---|---|---|
| `IsActive` | プロパティ | 現在表示中なら `true` |
| `Rect` | プロパティ | パネルの `RectTransform` |
| `OnActive(float duration = 0)` | メソッド（`virtual`） | `_activeAction` を発行し、パネルを**表示**する |
| `OnHidden(float duration = 0)` | メソッド（`virtual`） | `_hiddenAction` を発行し、パネルを**非表示**にする |

### Inspector の設定項目

| 項目 | 内容 |
|---|---|
| `Rect Transform` | パネルの `RectTransform`（自動で設定される） |
| `Canvas Group` | パネルの `CanvasGroup`（自動で設定される） |
| `Active Action` | 表示するときに呼ばれる `UnityEvent` |
| `Hidden Action` | 非表示にするときに呼ばれる `UnityEvent` |

---

## セットアップ

### 1. コンポーネントを追加する

パネルにしたい UI オブジェクトに `PanelControl` を追加します。`CanvasGroup` は `[RequireComponent]` により、自動で追加されます。

`Rect Transform` と `Canvas Group` は、コンポーネントを追加したときと Inspector で値が変更されたときに、同じオブジェクトのコンポーネントが自動で設定されます。

### 2. 必要に応じて UnityEvent を設定する

表示時・非表示時に実行したい処理があれば、`Active Action` / `Hidden Action` に登録します（Button の `OnClick` と同じ要領です）。

---

## 使い方

### 表示する / 隠す

```csharp
using UnityEngine;
using CoreKit.UI;

public class Example : MonoBehaviour
{
    [SerializeField] private PanelControl _panel;

    public void Open()
    {
        _panel.OnActive();
    }

    public void Close()
    {
        _panel.OnHidden();
    }
}
```

### Button から直接呼ぶ

ボタンの `OnClick` に `PanelControl.OnActive` / `PanelControl.OnHidden` を登録しても使えます。

> `OnActive` / `OnHidden` は引数に `float duration = 0` を持つため、Inspector の `OnClick` には引数なしのメソッドとして表示されない場合があります。その場合は、上のようにスクリプトから呼ぶ形にしてください。

### 表示状態を確認する

```csharp
if (_panel.IsActive)
{
    // 表示中
}
```

### 表示時・非表示時に処理を実行する

`Active Action` / `Hidden Action` に登録した処理は、`OnActive` / `OnHidden` が呼ばれたときに、表示切り替えの**直前**に実行されます。

### 継承して拡張する

`OnActive` / `OnHidden` は `virtual` なので、派生クラスで処理を追加できます。

```csharp
using UnityEngine;
using CoreKit;
using CoreKit.DynamicEnum;
using CoreKit.UI;

public class SoundPanelControl : PanelControl
{
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private DynamicEnumValue _openSound;

    public override void OnActive(float duration = 0)
    {
        base.OnActive(duration);
        _soundManager.Se.PlayOneShot(_openSound);
    }
}
```

---

## 仕組み

| 状態 | `alpha` | `blocksRaycasts` | `IsActive` |
|---|---|---|---|
| 表示 | `1` | `true` | `true` |
| 非表示 | `0` | `false` | `false` |

- **`Awake` で非表示にします。** シーン開始時、パネルは非表示の状態から始まります（このとき `Hidden Action` は発行されません）。
- GameObject の `SetActive` は使いません。パネルの GameObject は、アクティブなままです。

---

## 注意点

- **シーン開始時は、必ず非表示になります。**
  `Awake` で `SetVisible(false)` が呼ばれます。最初から表示しておきたい場合は、`Start` などで `OnActive()` を呼んでください。
- **`duration` は、現状では使われていません。**
  基底クラスの `OnActive` / `OnHidden` は、`duration` を受け取るだけで、フェードなどの処理は行いません。表示・非表示は即座に切り替わります。フェードなどを行う場合は、派生クラスで `duration` を使う処理を実装してください。
- **派生クラスから `CanvasGroup` を操作するには、工夫が必要です。**
  `_canvasGroup` と `SetVisible` は `private` のため、派生クラスからは直接触れません。フェードなどを実装する場合は、派生クラス側で `GetComponent<CanvasGroup>()` を取得するか、基底クラスで `protected` に変更してください。
- **非表示中も、GameObject はアクティブです。**
  `Update` やコルーチンは動き続けます。また、`alpha = 0` と `blocksRaycasts = false` で、見えず・クリックもできませんが、`interactable` は変更されないため、キーボードやゲームパッドによる UI ナビゲーションでは、非表示のパネルのボタンが選択される可能性があります。
- **`OnActive` を表示中に呼ぶと、`Active Action` が再度発行されます。**
  すでに表示中かどうかのチェックはありません。必要なら、`IsActive` を確認してから呼んでください。`OnHidden` も同様です。
- **`OnActive` / `OnHidden` は、「表示する」「隠す」の命令です。**
  イベントを受け取るコールバックではないので、注意してください。