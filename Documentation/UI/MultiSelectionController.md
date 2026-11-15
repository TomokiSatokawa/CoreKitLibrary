# MultiSelectionController

選択可能な要素（`ISelectable<T>`）の**複数選択**を管理するクラスです。
各要素は、クリックするたびに**選択 / 選択解除が切り替わります（トグル）**。フィルターの ON / OFF、複数アイテムの選択、オプションの切り替えなどに使います。

- 名前空間: `CoreKit.UI.Selector`
- 種類: 通常の `class<T>`（`MonoBehaviour` ではありません）。`IDisposable` を実装
- 型制約: `T : ISelectable<T>`
- 関連: 常に1つだけ選択する [SingleSelectionController.md](SingleSelectionController.md)

---

## 概要

| メンバー | 種類 | 説明 |
|---|---|---|
| `MultiSelectionController(T[] selectables, params int[] startSelectIndex)` | コンストラクタ | 要素の配列と、最初に選択しておく index（複数可）を指定して生成する |
| `SetSelect(int index, bool setValue)` | メソッド | コードから、index 指定で選択 / 解除を設定する |
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

public class FilterButton : MonoBehaviour, ISelectable<FilterButton>
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _checkMark;

    public event Action<FilterButton> OnClick;

    private void Awake()
    {
        _button.onClick.AddListener(() => OnClick?.Invoke(this));
    }

    public void OnSelect()
    {
        _checkMark.SetActive(true);
    }

    public void OnDeselect()
    {
        _checkMark.SetActive(false);
    }
}
```

### 2. コントローラーを生成する

`MonoBehaviour` ではないので、`new` で生成し、`OnDestroy` などで `Dispose` します。
開始時に選択しておく index は、**0個以上いくつでも**渡せます。

```csharp
using UnityEngine;
using CoreKit.UI.Selector;

public class FilterGroup : MonoBehaviour
{
    [SerializeField] private FilterButton[] _filters;

    private MultiSelectionController<FilterButton> _controller;

    private void Start()
    {
        // index 0 と 2 を選択した状態で開始する
        _controller = new MultiSelectionController<FilterButton>(_filters, 0, 2);

        // 何も選択せずに開始する場合
        // _controller = new MultiSelectionController<FilterButton>(_filters);
    }

    private void OnDestroy()
    {
        _controller?.Dispose();
    }
}
```

### 3. コードから選択状態を設定する

```csharp
_controller.SetSelect(1, true);  // index 1 を選択する
_controller.SetSelect(1, false); // index 1 の選択を解除する
```

---

## 動作

| 操作 | 動作 |
|---|---|
| 未選択の要素をクリック | 選択状態になり、`OnSelect()` が呼ばれる |
| 選択中の要素をクリック | 選択が解除され、`OnDeselect()` が呼ばれる |
| `SetSelect(index, value)` | 現在の状態と `value` が同じなら、何もしない。違うときだけ `OnSelect()` / `OnDeselect()` が呼ばれる |
| 範囲外の index を指定 | `Debug.LogError` を出力し、状態は変わらない |

### 生成時の動作

1. すべての要素の `OnDeselect()` を呼び、通常の見た目に揃えます。
2. `startSelectIndex` で指定された要素の `OnSelect()` を呼び、選択状態にします。

---

## 注意点

- **`Dispose` を呼んでください。**
  コンストラクタで各要素の `OnClick` を購読しているため、`Dispose` を呼ばないと、購読が残ります。
- **選択状態を取得するメンバーは、ありません。**
  どの要素が選択中かは、コントローラーの内部（`private`）にしか保持されていません。「現在の選択を一覧で取得したい」「選択数を知りたい」「選択が変わったときに通知を受けたい」場合は、プロパティやイベントを追加してください。
- **選択数の制限は、ありません。**
  「最大2つまで」「最低1つは必須」といった制限が必要な場合は、コントローラー側の拡張が必要です。
- **同じ要素を、配列に2回含めないでください。**
  内部で要素をキーにした `Dictionary` を作るため、重複があると `ArgumentException` になります。
- **開始 index が重複していても、エラーにはなりません。**
  ただし、同じ index を2回渡すと、その要素の `OnSelect()` が2回呼ばれます。
- **範囲外の開始 index は、無視されます。**
  `Debug.LogError` を出力して、その index だけスキップされます。ほかの index は、通常どおり選択されます。
- **生成時に、すべての要素の `OnDeselect()` が呼ばれます。**
  [SingleSelectionController](SingleSelectionController.md) と違い、選択しない要素にも `OnDeselect()` が呼ばれます。`OnDeselect()` に、生成時に実行されて困る処理（SE の再生など）を入れないでください。
- **配列はコピーされず、そのまま保持されます。**
  生成後に配列の中身を書き換えないでください。