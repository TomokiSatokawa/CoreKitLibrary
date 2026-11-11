# SoundSection

`SoundManager` が持つ「セクション（BGM用、SE用など）」1つ分の再生を担当するクラスです。
1つの `AudioSource` と `SoundDatabase` を組み合わせ、**種類（`DynamicEnumValue`）指定での再生**、**直接 `AudioClip` 指定での再生**、**音量のフェード変更**ができます。

- 名前空間: `CoreKit`
- 種類: `[System.Serializable]` な通常の `class`（`MonoBehaviour` ではありません）
- 関連型: `IReadOnlySoundSection`（再生操作だけを公開するインターフェース）、`SoundDatabase`、`VolumeController`、`DynamicEnumValue`

---

## 全体像

```
SoundManager
 ├─ SoundSection（例: BGM）  ── AudioSource / SoundDatabase
 └─ SoundSection（例: SE）   ── AudioSource / SoundDatabase
```

`SoundSection` 自体は `MonoBehaviour` ではないため、**`SoundManager` のフィールドとして保持し、`SoundManager` から `Update()` を呼びます。** セクションごとに `AudioSource` と `SoundDatabase` を割り当てることで、BGM と SE の音量などを独立して扱えます。

---

## セットアップ

### 1. SoundManager にセクションを持たせる

`[System.Serializable]` なので、`SoundManager` のフィールドとして宣言すると Inspector に表示されます。

```csharp
using UnityEngine;
using CoreKit;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundSection _bgmSection;
    [SerializeField] private SoundSection _seSection;

    // 外部には再生操作だけを公開する
    public IReadOnlySoundSection Bgm => _bgmSection;
    public IReadOnlySoundSection Se => _seSection;

    private void Update()
    {
        // 音量フェードを進めるため、毎フレーム呼ぶ
        _bgmSection.Update();
        _seSection.Update();
    }
}
```

### 2. Inspector で設定する

各セクションに以下を設定します。

| フィールド | 内容 |
|---|---|
| `Database` | そのセクションで使う `SoundDatabase`（種類 → クリップ・音量の対応表） |
| `Audio Source` | 再生に使う `AudioSource` |

BGM 用と SE 用で、それぞれ別の `AudioSource` を割り当ててください。

---

## メソッド一覧

| メソッド | 説明 |
|---|---|
| `Play(DynamicEnumValue type)` | データベースから `type` に対応するクリップと音量を取得し、**通常再生**する |
| `Play(AudioClip clip, float volume)` | クリップと音量を直接指定して**通常再生**する |
| `PlayOneShot(DynamicEnumValue type)` | データベースから取得したクリップを**ワンショット再生**する |
| `PlayOneShot(AudioClip clip, float volume)` | クリップと音量を直接指定して**ワンショット再生**する |
| `ChangeVolume(float volume, float duration)` | 音量を `duration` 秒かけて `volume` に変更する |
| `Update()` | 音量フェードを進める。**毎フレーム呼ぶ必要がある** |

### Play と PlayOneShot の違い

| | `Play` | `PlayOneShot` |
|---|---|---|
| 再生方法 | `AudioSource.clip` に設定して `Play()` | `AudioSource.PlayOneShot()` |
| 再生中の音 | 前の音は**止まって切り替わる** | 前の音を止めず**重ねて鳴る** |
| 音量の扱い | 音量を即座に設定し、フェード状態もリセットする | 引数の `volume` を、現在の `AudioSource.volume` に掛け合わせて鳴らす |
| 向いている用途 | BGM、ループ音 | SE（足音、銃声、UI 音など） |

---

## 使用例

### BGM を再生する（種類指定）

```csharp
// DynamicEnumValue は、データベースに登録した種類
_soundManager.Bgm.Play(bgmType);
```

### SE を鳴らす（種類指定）

```csharp
_soundManager.Se.PlayOneShot(seType);
```

### AudioClip を直接指定して再生する

データベースに登録していないクリップを、その場で鳴らしたいときに使います。

```csharp
[SerializeField] private AudioClip _clip;

_soundManager.Se.PlayOneShot(_clip, 0.8f);
_soundManager.Bgm.Play(_clip, 0.5f);
```

### BGM をフェードアウト／フェードインする

```csharp
// 2秒かけて無音にする
_soundManager.Bgm.ChangeVolume(0f, 2f);

// 1秒かけて音量 0.6 に戻す
_soundManager.Bgm.ChangeVolume(0.6f, 1f);
```

### ポーズ中に BGM を小さくする

```csharp
private void OnPause()
{
    _soundManager.Bgm.ChangeVolume(0.2f, 0.3f);
}

private void OnResume()
{
    _soundManager.Bgm.ChangeVolume(1f, 0.3f);
}
```

---

## IReadOnlySoundSection について

`IReadOnlySoundSection` は再生・音量変更の操作だけを公開するインターフェースです。`Update()` は含まれません。
`SoundManager` 以外のクラスにはこのインターフェースだけを渡すと、**`Update()` を外部から呼ばれる事故を防げます**。

```csharp
public class Gun : MonoBehaviour
{
    // SoundManager から受け取った SE セクション（再生操作のみ可能）
    private IReadOnlySoundSection _se;

    public void Initialize(IReadOnlySoundSection se)
    {
        _se = se;
    }

    public void Fire()
    {
        _se.PlayOneShot(_fireSoundType);
    }
}
```

---

## 注意点

- **`Update()` を毎フレーム呼んでください。**
  `ChangeVolume` のフェードは `Update()` の中で進みます。呼ばないと音量が変化しません。
- **`Play` は前の音を止めます。**
  同じセクションで `Play` を呼ぶと、再生中のクリップは切り替わります。重ねて鳴らしたい音は `PlayOneShot` を使ってください。
- **`PlayOneShot` の音量は、セクションの現在音量と掛け合わさります。**
  `Update()` が毎フレーム `AudioSource.volume` をセクションの音量に更新するため、`ChangeVolume` でフェードアウト中の SE セクションに `PlayOneShot` すると、その分小さく鳴ります。
- **`Play` で音量をリセットします。**
  `Play` は指定した音量を即座に適用し、フェード中の状態もその音量に置き換えます。
- **種類指定の再生は、データベースの登録が前提です。**
  `Play(type)` / `PlayOneShot(type)` は `SoundDatabase` から情報を取得するため、対象の種類がデータベースに登録されている必要があります。
- **`Database` と `Audio Source` は Inspector で必ず設定してください。**
  未設定のまま再生すると `NullReferenceException` になります。
