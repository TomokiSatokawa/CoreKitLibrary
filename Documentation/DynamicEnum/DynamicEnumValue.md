# DynamicEnumValue

`DynamicEnum`（アセットで定義できる列挙型のようなもの）の**1つの値**を表す構造体です。
C# の `enum` と違い、コードを書き換えずに値を増やせます。値は名前ではなく **GUID** で識別するため、項目名を変更しても参照が壊れません。

- 名前空間: `CoreKit.DynamicEnum`
- 種類: `[Serializable]` な `struct`（`IEquatable<DynamicEnumValue>` を実装）
- 関連型: `DynamicEnumDefinition`（値の候補を定義するアセット）

---

## 概要

| メンバー | 種類 | 説明 |
|---|---|---|
| `Guid` | プロパティ | 値を識別する GUID 文字列 |
| `DynamicEnumValue(string guid)` | コンストラクタ | GUID を指定して値を生成する |
| `Equals` / `==` / `!=` | - | **GUID が同じなら等しい**と判定する |
| `GetHashCode()` | メソッド | GUID から計算する（GUID が `null` なら `0`） |
| `ToString()` | メソッド | `"定義のクラス名.項目名"` の形式で文字列化する（デバッグ用） |

---

## 基本的な使い方

### 1. フィールドとして宣言する

`MonoBehaviour` や `ScriptableObject` のフィールドとして宣言すると、Inspector で値を選べます。

```csharp
using UnityEngine;
using CoreKit.DynamicEnum;

public class Example : MonoBehaviour
{
    [SerializeField] private DynamicEnumValue _soundType;
}
```

> Inspector での選択 UI は、別途用意されているカスタムエディタの実装によります。

### 2. 値を比較する

`==` / `!=` で比較できます。比較には GUID のみが使われます。

```csharp
if (_soundType == _otherType)
{
    // 同じ値
}
```

### 3. Dictionary のキーにする

`Equals` と `GetHashCode` が実装されているので、`Dictionary` や `HashSet` のキーにできます。

```csharp
var dictionary = new Dictionary<DynamicEnumValue, ClipData>();
dictionary.TryAdd(type, clipData);
```

`SoundDatabase` も、この方法で種類からクリップを引いています。

### 4. ログで中身を確認する

```csharp
Debug.Log(_soundType); // 例: "SoundType.Jump"
```

---

## ToString の出力

`ToString()` は、内部の `_definition` の状態によって出力が変わります。

| 状態 | 出力 |
|---|---|
| 定義（`_definition`）が未設定 | `None` |
| 定義内に、GUID が一致する項目がある | `クラス名.項目名`（例: `SoundType.Jump`） |
| 定義はあるが、一致する項目がない | `定義アセット名.None` |

---

## 注意点

- **等価判定は GUID だけで行われます。**
  `_definition` は比較に使われません。
- **`default` の値は GUID が `null` です。**
  Inspector で何も選んでいない場合や、`default(DynamicEnumValue)` は `Guid == null` の状態です。`GetHashCode()` は `0` を返し、`Dictionary` のキーにもできますが、未設定の値同士が等しいとみなされる点に注意してください。
- **コードから GUID 指定で生成した値は、`ToString()` が `None` になります。**
  コンストラクタ `DynamicEnumValue(string guid)` は `_definition` を `null` にするため、`ToString()` では項目名が引けません。比較や `Dictionary` のキーとしては問題なく使えます。
- **項目名を変更しても、GUID が同じなら同じ値として扱われます。**
  名前変更でデータが壊れにくいのが、通常の文字列キーに対する利点です。一方で、項目を削除して作り直すと GUID が変わり、既存の参照は別の値になります。
- **`_definition` は Inspector 表示用の参照です。**
  コンストラクタで生成した値は `_definition` を持たないため、シリアライズして Inspector に表示しても、項目の名前は表示されない可能性があります。