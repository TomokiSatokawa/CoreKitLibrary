# DynamicEnumDefinition

`DynamicEnum` の**選択肢の一覧を定義する `ScriptableObject`** です。
C# の `enum` の代わりに、アセット上で項目を追加・編集できます。各項目には自動で GUID が割り振られ、`DynamicEnumValue` はこの GUID で値を識別します。

- 名前空間: `CoreKit.DynamicEnum`
- 種類: `ScriptableObject`
- 関連型: `DynamicEnumValue`（選ばれた値を表す構造体）

---

## 概要

### DynamicEnumDefinition

| メンバー | 種類 | 説明 |
|---|---|---|
| `ClassName` | プロパティ | この DynamicEnum の名前（初期値は `NewDynamicEnum`） |
| `Entries` | プロパティ | 項目（`Entry`）の読み取り専用リスト |
| `PreviousClassName` | プロパティ | 変更前のクラス名（Inspector には表示されない） |
| `SetPreviousClassName(string)` | メソッド | 変更前のクラス名を記録する |

### DynamicEnumDefinition.Entry

| メンバー | 型 | 説明 |
|---|---|---|
| `Name` | `string` | 項目名（例: `Jump`、`Attack`） |
| `Guid` | `string` | 項目を識別する GUID。**自動で割り振られる** |

---

## セットアップ

### 1. アセットを作成する

Project ウィンドウで右クリックし、次のメニューから作成します。

```
Create > CoreKit > Dynamic Enum > Definition
```

### 2. クラス名を設定する

`Class Name` に、この DynamicEnum の名前を入力します（例: `SoundType`）。
`DynamicEnumValue.ToString()` の出力（`SoundType.Jump` のような形式）にも使われます。

### 3. 項目を追加する

`Entries` に要素を追加し、`Name` を入力します。

| 項目 | 内容 |
|---|---|
| Name | 項目名（例: `Jump`） |
| Guid | **入力不要**。追加すると自動で生成される |

### 4. 値を選んで使う

`DynamicEnumValue` 型のフィールドを用意し、Inspector で定義アセットの項目を選びます。

```csharp
using UnityEngine;
using CoreKit.DynamicEnum;

public class Example : MonoBehaviour
{
    [SerializeField] private DynamicEnumValue _soundType;
}
```

> Inspector での選択 UI は、別途用意されているカスタムエディタの実装によります。

---

## 使用例

### 項目の一覧をログに出す

```csharp
using UnityEngine;
using CoreKit.DynamicEnum;

public class DefinitionLogExample : MonoBehaviour
{
    [SerializeField] private DynamicEnumDefinition _definition;

    private void Start()
    {
        Debug.Log($"[{_definition.ClassName}]");

        foreach (var entry in _definition.Entries)
        {
            Debug.Log($"{entry.Name} : {entry.Guid}");
        }
    }
}
```

### 項目名から値を作る

GUID を使えば、コードから `DynamicEnumValue` を作れます。

```csharp
foreach (var entry in _definition.Entries)
{
    if (entry.Name == "Jump")
    {
        var value = new DynamicEnumValue(entry.Guid);
        // value を Dictionary のキーなどに使える
    }
}
```

> `DynamicEnumValue(string guid)` で作った値は `ToString()` が `None` になります。比較や `Dictionary` のキーとしては問題なく使えます。

---

## GUID の自動生成

`OnValidate`（Inspector で値が変更されたとき）に、各項目の GUID を次のルールで整えます。

| 状況 | 動作 |
|---|---|
| GUID が空・空白 | 新しい GUID を生成する |
| 他の項目と GUID が重複している | 重複した側に、新しい GUID を生成する |

生成される GUID は、ハイフンなしの 32 桁の文字列です（例: `3f2504e04f8911d39a0c0305e82c3301`）。

そのため、Inspector で項目を複製しても、**複製した項目には別の GUID が付きます**。

---

## 注意点

- **`Guid` は手で編集しないでください。**
  `DynamicEnumValue` は GUID で値を識別します。変更すると、その項目を参照していた値は別の（存在しない）値になります。
- **項目を削除して作り直すと、GUID が変わります。**
  既存の参照は新しい項目とは別の値になります。名前を変えたいだけなら、削除せずに `Name` を書き換えてください。名前だけの変更なら GUID は変わらないため、参照は壊れません。
- **アセットを複製すると、GUID も複製されます。**
  複製したアセットの項目は元と同じ GUID を持ちます。`DynamicEnumValue` の等価判定は GUID のみで行われ、定義アセットは比較されないため、**別アセットの値同士が「等しい」と判定される**ことがあります。別の定義として使う場合は、各項目の GUID を作り直してください。
- **`Name` の重複は検出されません。**
  GUID の重複だけが自動で修正されます。同じ名前の項目があっても警告は出ないため、名前は重複させないようにしてください。
- **`Entries` は読み取り専用です。**
  コードから項目を追加・削除することはできません。項目の編集は Inspector で行います。
- **GUID の自動生成は `OnValidate` で行われます。**
  エディタ上で動作する処理です。実行時に項目が増えることはありません。
- **`PreviousClassName` は内部用です。**
  Inspector には表示されません。通常は意識する必要はありません。