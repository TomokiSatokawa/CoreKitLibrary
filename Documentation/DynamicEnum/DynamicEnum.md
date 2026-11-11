# DynamicEnum

コードを書き換えずに、**アセット上で選択肢を追加・編集できる列挙型のような仕組み**です。
C# の `enum` の代わりに使うことで、データ駆動の設計がしやすくなります。

- 名前空間: `CoreKit.DynamicEnum`

---

## 構成

DynamicEnum は、2つの型で構成されます。

| 型 | 役割 | 詳細 |
|---|---|---|
| `DynamicEnumDefinition` | 選択肢の一覧を定義する `ScriptableObject` | [DynamicEnumDefinition.md](DynamicEnumDefinition.md) |
| `DynamicEnumValue` | 選択肢のうち1つを表す `struct` | [DynamicEnumValue.md](DynamicEnumValue.md) |

```
DynamicEnumDefinition（アセット）          DynamicEnumValue（フィールド）
 ├─ ClassName: "SoundType"                  └─ GUID を持ち、定義内の
 └─ Entries                                     1項目を指す
     ├─ Name: "Jump"   Guid: 3f25...
     ├─ Name: "Attack" Guid: 9a0c...
     └─ Name: "Damage" Guid: b71e...
```

- **定義側**: 項目名（`Name`）と、自動生成される GUID のペアを並べたアセットです。
- **値側**: 項目の GUID を保持します。比較・`Dictionary` のキー・シリアライズに使えます。

---

## enum との違い

| | C# の `enum` | DynamicEnum |
|---|---|---|
| 項目の追加 | コードを書き換えて再コンパイル | アセットを編集するだけ |
| 識別方法 | 整数値（または名前） | GUID |
| 名前の変更 | 参照側の修正が必要になる場合がある | GUID が同じなので参照が壊れない |
| 定義の分割 | 1つの enum は1ファイル | 用途ごとにアセットを分けられる |
| `switch` での分岐 | 使える | 使えない（`==` や `Dictionary` で比較する） |
| 型安全性 | 型ごとに別の型になる | すべて `DynamicEnumValue` 型（型では区別されない） |

### 向いている用途

- サウンドの種類、エフェクトの種類など、**項目が増えやすいもの**
- プランナーやサウンド担当者が、**コードを触らずに項目を追加したい**場合
- データベース（`ScriptableObject`）で「種類 → データ」を引く場合

### 向いていない用途

- 項目が固定で、`switch` 文で網羅的に分岐したいもの
- 型でコンパイル時に区別したいもの（`DynamicEnumValue` は、どの定義の値でも同じ型）

---

## 基本的な使い方

### 1. 定義アセットを作る

Project ウィンドウで右クリックし、次のメニューから作成します。

```
Create > CoreKit > Dynamic Enum > Definition
```

`Class Name` に名前（例: `SoundType`）を入力し、`Entries` に項目名を追加します。GUID は自動で生成されます。

### 2. 値を持つフィールドを宣言する

```csharp
using UnityEngine;
using CoreKit.DynamicEnum;

public class Example : MonoBehaviour
{
    [SerializeField] private DynamicEnumValue _soundType;
}
```

Inspector で、定義アセットの項目から値を選びます。

> Inspector での選択 UI は、別途用意されているカスタムエディタの実装によります。

### 3. 値を比較する

```csharp
if (_soundType == _otherType)
{
    // 同じ値
}
```

### 4. Dictionary のキーにする

```csharp
private Dictionary<DynamicEnumValue, ClipData> _dictionary;
```

`switch` の代わりに、`Dictionary` で「値 → データ」を引く形が基本になります。

---

## CoreKit での使用例

`SoundDatabase` が、サウンドの種類をキーにしてクリップと音量を引く用途で使っています。

```csharp
// SoundDatabase.ClipData の一部
[SerializeField] private DynamicEnumValue _type;
[SerializeField] private AudioClip _clip;
```

```csharp
// 種類を指定して再生する
_soundManager.Se.PlayOneShot(seType); // seType は DynamicEnumValue
```

関連: [SoundDatabase.md](SoundDatabase.md)、[SoundSection.md](SoundSection.md)

---

## 注意点

- **値の識別は GUID で行われます。**
  `DynamicEnumValue` の等価判定は GUID のみで行われ、どの定義アセットの値かは比較されません。
- **`Guid` は手で編集しないでください。**
  変更すると、その項目を参照していた値は別の値になります。
- **項目を削除して作り直すと、GUID が変わります。**
  名前を変えたいだけなら、削除せずに `Name` を書き換えてください。
- **定義アセットを複製すると、GUID も複製されます。**
  別の定義として使う場合は、各項目の GUID を作り直してください。
- **`Entries` は読み取り専用です。**
  項目の追加・削除は、Inspector（エディタ上）で行います。
- **何も選んでいない値（`default`）は GUID が `null` です。**
  未設定の値として扱えますが、未設定同士は等しいと判定されます。