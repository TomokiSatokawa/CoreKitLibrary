# SingleSelectionController

選択可能な要素（`ISelectable<T>`）のうち、**常に1つだけが選択されている状態**を管理するクラスです。
タブ切り替え、キャラクター選択、難易度選択のように、ラジオボタンと同じ動きをする UI に使います。

- 名前空間: `CoreKit.UI.Selector`
- 種類: 通常の `class<T>`（`MonoBehaviour` ではありません）。`IDisposable` を実装
- 型制約: `T : ISelectable<T>`
- 関連: 複数選択できる [MultiSelectionController.md](MultiSelectionController.md)

---

## 概要

| メンバー | 種類 | 説明 |
|---|---|---|
| `SingleSelectionController(T[] selectables, int startSelectIndex)` | コンストラクタ | 要素の配列と、最初に選択する index を指定して生成する |
| `CurrentSelect` | プロパティ | 現在選択中の要素（未選択の間は `null`） |
| `SetSelect(int index)` | メソッド | コードから、index 指定で選択を切り替える |
| `Dispose()` | メソッド | 各要素の `OnClick` の購読を解除する |

---

## ISelectable<T> について

このコントローラーが扱う要素は、`ISelectable<T>` を実装している必要があります。
コントローラーの使い方から、`ISelectable<T>` には次のメンバーがあると読み取れます。

| メンバー | 内容 |
|---|---|
| `event Action<T> OnClick` | 要素がクリックされたときに、自分自身を渡して発行する |
| `void OnSelect()` | 選択されたときに、コントローラーから呼ばれる（見た目を選択状態にする） |
| `void OnDeselect()` | 選択が外れたときに、コントローラーから呼ばれる（見た目を通常状態に戻す） |

> `ISelectable<T>` の定義は未確認のため、上の表と以下のサンプルは使われ方から推測したものです。実際の定義に合わせて読み替えてください。

---

## 使い方

### 1. 選択可能な要素を用意する

```csharp
using System;
using UnityEngine;
using UnityEngine.UI;
using CoreKit.UI.Selector;

public class TabButton : MonoBehaviour, ISelectable<TabButton>
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedMark;

    public event Action<TabButton> OnClick;

    private void Awake()
    {
        _button.onClick.AddListener(() => OnClick?.Invoke(this));
    }

    public void OnSelect()
    {
        _selectedMark.SetActive(true);
    }

    public void OnDeselect()
    {
        _selectedMark.SetActive(false);
    }
}
```

### 2. コントローラーを生成する

`MonoBehaviour` ではないので、`new` で生成し、`OnDestroy` などで `Dispose` します。

```csharp
using UnityEngine;
using CoreKit.UI.Selector;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private TabButton[] _tabs;

    private SingleSelectionController<TabButton> _controller;

    private void Start()
    {
        // 先頭（index 0）を最初に選択した状態で開始する
        _controller = new SingleSelectionController<TabButton>(_tabs, 0);
    }

    private void OnDestroy()
    {
        _controller?.Dispose();
    }
}
```

### 3. 選択中の要素を取得する

```csharp
TabButton current = _controller.CurrentSelect;
```

### 4. コードから選択を切り替える

```csharp
_controller.SetSelect(2); // index 2 を選択する
```

---

## 動作

| 操作 | 動作 |
|---|---|
| 未選択の要素をクリック | 前に選択されていた要素の `OnDeselect()` を呼び、クリックされた要素の `OnSelect()` を呼ぶ |
| **選択中の要素**をクリック | 何も起きない（選択は外れない） |
| `SetSelect(index)` | クリックと同じ動作になる |
| 範囲外の index を指定 | `Debug.LogError` を出力し、選択は変わらない |

---

## 注意点

- **`Dispose` を呼んでください。**
  コンストラクタで各要素の `OnClick` を購読しているため、`Dispose` を呼ばないと、購読が残ります。要素より先にコントローラーが不要になる場合は、特に注意してください。
- **選択中の要素をクリックしても、選択は外れません。**
  常に1つが選択された状態になります。選択を外せる動作（トグル）にしたい場合は、[MultiSelectionController](MultiSelectionController.md) を使ってください。
- **初期化時、選択しない要素の `OnDeselect()` は呼ばれません。**
  開始時に選択された要素の `OnSelect()` だけが呼ばれるため、ほかの要素は、最初から通常の見た目にしておく必要があります。
- **開始 index が範囲外だと、何も選択されません。**
  コンストラクタに範囲外の index を渡すと、`Debug.LogError` が出力され、`CurrentSelect` は `null` のままになります。
- **選択が変わったことを通知するイベントは、ありません。**
  選択の変化は、各要素の `OnSelect()` / `OnDeselect()` で受け取ります。コントローラーの外で変化を知りたい場合は、`CurrentSelect` を確認するか、コントローラーにイベントを追加してください。
- **`T` は参照型（`class`）で使う想定です。**
  未選択を `null` で表現しているため、`struct` で使うと、未選択状態が正しく扱えません。
- **要素が破棄される前に `Dispose` してください。**
  `MonoBehaviour` を要素にしている場合、破棄後の要素に対して、`OnDeselect()` が呼ばれると例外が発生することがあります。
- **配列はコピーされず、そのまま保持されます。**
  生成後に配列の中身を書き換えないでください。S