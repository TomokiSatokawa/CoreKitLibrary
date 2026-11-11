# SoundDatabase

サウンドの**種類（`DynamicEnumValue`）と `AudioClip`・音量の対応表**を管理する `ScriptableObject` です。
`SoundSection` に割り当てて使い、種類を指定するだけでクリップと音量を引けるようにします。

- 名前空間: `CoreKit`
- 種類: `ScriptableObject`
- 関連型: `SoundSection`、`DynamicEnumValue`

---

## 概要

### SoundDatabase

| メンバー | 種類 | 説明 |
|---|---|---|
| `GetClipData(DynamicEnumValue type)` | メソッド | 種類に対応する `ClipData` を返す。未登録なら `null` を返し、エラーログを出す |

### SoundDatabase.ClipData

| メンバー | 型 | 説明 |
|---|---|---|
| `Type` | `DynamicEnumValue` | サウンドの種類（検索キー） |
| `Clip` | `AudioClip` | 再生するクリップ |
| `Volume` | `float` | 再生時の音量（0〜1） |

---

## セットアップ

### 1. アセットを作成する

Project ウィンドウで右クリックし、次のメニューから作成します。

```
Create > CoreKit > Sound > SoundDatabase
```

BGM 用、SE 用など、**セクションごとに別のデータベースを作る**と管理しやすくなります。

### 2. サウンドを登録する

作成したアセットの `Clip Datas` に要素を追加し、以下を設定します。

| 項目 | 内容 |
|---|---|
| Type | サウンドの種類（`DynamicEnumValue`） |
| Clip | 再生する `AudioClip` |
| Volume | 音量（0〜1 のスライダー） |

> **注意**
> 要素を追加した直後は `Volume` が `0` です。設定し忘れると無音で再生されるので、必ず音量を設定してください。

### 3. SoundSection に割り当てる

`SoundManager` が持つ各 `SoundSection` の `Database` に、作成したアセットを割り当てます。

```csharp
[SerializeField] private SoundSection _bgmSection; // Database に BGM 用のアセットを設定
[SerializeField] private SoundSection _seSection;  // Database に SE 用のアセットを設定
```

---

## 使い方

通常は `SoundSection` 経由で使うため、データベースを直接触る必要はありません。

```csharp
// SoundSection が内部で GetClipData を呼び、クリップと音量を取得して再生する
_soundManager.Se.PlayOneShot(seType);
_soundManager.Bgm.Play(bgmType);
```

データベースから直接引くこともできます。

```csharp
using UnityEngine;
using CoreKit;
using CoreKit.DynamicEnum;

public class SoundInfoExample : MonoBehaviour
{
    [SerializeField] private SoundDatabase _database;
    [SerializeField] private DynamicEnumValue _type;

    private void Start()
    {
        var clipData = _database.GetClipData(_type);
        if (clipData == null) return; // 未登録の場合は null

        Debug.Log($"Clip: {clipData.Clip.name}, Volume: {clipData.Volume}");
    }
}
```

---

## 内部の仕組み

- `OnEnable` のタイミングで、`Clip Datas` から `Dictionary<DynamicEnumValue, ClipData>` を作成します。
- `GetClipData` はこの辞書で検索するため、登録数が増えても高速です。

---

## 注意点

- **種類（Type）は重複させないでください。**
  重複があると、`Debug.LogError` が出力され、**最初に登録したものだけが有効**になります（後の重複分は無視されます）。
- **未登録の種類を指定すると `null` が返ります。**
  `GetClipData` は `Debug.LogError` を出力して `null` を返します。`SoundSection.Play(type)` / `PlayOneShot(type)` は返り値をそのまま使うため、未登録の種類を渡すと `NullReferenceException` になります。データベースに登録済みの種類だけを指定してください。
- **`Clip` が未設定の要素に注意してください。**
  種類は登録されていても `Clip` が空だと、再生時に鳴りません。
- **Inspector での編集は、再生中には反映されません。**
  辞書は `OnEnable` 時にだけ作られます。エディタの再生中にアセットの内容を編集しても、辞書は更新されません。内容を変えたら再生し直してください。
- **セクションごとにデータベースを分けると、種類の重複を気にせず管理できます。**
  同じ種類名を BGM と SE で使い分けたい場合も、別のアセットにしておけば衝突しません。