# Sound

BGM や SE の再生を管理する機能です。
**サウンドの種類（`DynamicEnumValue`）を指定するだけで、クリップと音量をデータベースから引いて再生**できます。

- 名前空間: `CoreKit`

---

## 構成

| 型 | 役割 | 詳細 |
|---|---|---|
| `SoundDatabase` | 種類 → クリップ・音量の対応表（`ScriptableObject`） | [SoundDatabase.md](SoundDatabase.md) |
| `SoundSection` | 1つの `AudioSource` での再生・音量フェードを担当（BGM 用、SE 用など） | [SoundSection.md](SoundSection.md) |
| `Sound3DPlayer` | 3D 空間上の位置で SE を再生する（`AudioSource` をプールして再利用） | [Sound3DPlayer.md](Sound3DPlayer.md) |

### 関連する機能

| 型 | 役割 | 詳細 |
|---|---|---|
| `DynamicEnumValue` / `DynamicEnumDefinition` | サウンドの種類をアセットで管理する仕組み | [DynamicEnum.md](../DynamicEnum/DynamicEnum.md) |

---

## 全体像

```
DynamicEnumDefinition（種類の一覧: SoundType など）
        │  種類を選ぶ
        ▼
SoundDatabase（種類 → AudioClip・音量）
        │  引く
        ├──────────────► SoundSection（BGM / SE など。位置なしで再生）
        │                  └─ AudioSource
        │
        └──────────────► Sound3DPlayer（位置を指定して再生）
                           └─ AudioSource（プールで再利用）
```

- **種類の定義**は `DynamicEnumDefinition`、**種類ごとのクリップ・音量**は `SoundDatabase` に登録します。
- 再生側は、どちらも `DynamicEnumValue` を渡すだけです。クリップや音量を直接指定して再生することもできます。

---

## どれを使うか

| 再生したい音 | 使うもの |
|---|---|
| BGM（ループ、フェードあり） | `SoundSection` の `Play` / `ChangeVolume` |
| UI 音など、位置を持たない SE | `SoundSection` の `PlayOneShot` |
| 足音、銃声、爆発音など、位置を持つ SE | `Sound3DPlayer` の `Play3D` |

---

## クイックスタート

### 1. サウンドの種類を定義する

`Create > CoreKit > Dynamic Enum > Definition` で定義アセットを作り、種類（例: `Jump`、`Explosion`）を追加します。

### 2. SoundDatabase を作って登録する

`Create > CoreKit > Sound > SoundDatabase` でアセットを作り、種類・クリップ・音量を登録します。
BGM 用、SE 用など、用途ごとに分けると管理しやすくなります。

### 3. 再生する

```csharp
// 位置を持たない音（SoundSection 経由）
_soundManager.Bgm.Play(bgmType);
_soundManager.Se.PlayOneShot(seType);

// 位置を持つ音（Sound3DPlayer）
_sound3DPlayer.Play3D(explosionType, transform.position);
```

> `SoundSection` は `SoundManager` が保持し、毎フレーム `Update()` を呼ぶ構成を想定しています。詳しくは [SoundSection.md](SoundSection.md) を参照してください。

---

## 依存関係

| 型 | 必要なもの |
|---|---|
| `SoundDatabase` / `SoundSection` | `CoreKit.DynamicEnum` |
| `Sound3DPlayer` | `CoreKit.DynamicEnum`、[UniTask](https://github.com/Cysharp/UniTask) |

---

## 共通の注意点

- **種類指定の再生は、`SoundDatabase` への登録が前提です。**
  未登録の種類を渡すと、`GetClipData` が `null` を返し、`SoundSection` / `Sound3DPlayer` で `NullReferenceException` になります。
- **`SoundDatabase` の `Volume` は、追加直後は `0` です。**
  設定し忘れると無音になります。
- **`SoundDatabase` の辞書は `OnEnable` で作られます。**
  再生中にアセットを編集しても反映されません。