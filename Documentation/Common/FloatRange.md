# FloatRange

`float` の最小値・最大値をひとまとめに扱う範囲値構造体です。Inspector 上で範囲を設定し、判定・補正・ランダム取得に使えます。

- 名前空間: `CoreKit`
- 種類: `struct`（`[System.Serializable]`）

---

## 概要

| メンバー | 種類 | 説明 |
|---|---|---|
| `Min` | プロパティ | 範囲の最小値 |
| `Max` | プロパティ | 範囲の最大値 |
| `IsRange(float value)` | メソッド | 値が範囲内か判定する（**最小値を含み、最大値を含まない**） |
| `Clamp(float value)` | メソッド | 値を範囲内に収める（**両端を含む**） |
| `GetRandom()` | メソッド | 範囲内のランダムな値を取得する |

---

## 基本的な使い方

### 1. フィールドとして宣言する

`MonoBehaviour` や `ScriptableObject` に `FloatRange` を宣言すると、Inspector に `Min` / `Max` の入力欄が表示されます。

```csharp
using UnityEngine;
using CoreKit;

public class Enemy : MonoBehaviour
{
    [SerializeField] private FloatRange _attackDistance;
    [SerializeField] private FloatRange _spawnInterval;
}
```

> **注意**
> `_min` / `_max` は `private` フィールドで、コンストラクタも用意されていません。
> そのため **値の設定は Inspector から行う** 想定です。コード上で `new FloatRange()` すると `Min = 0, Max = 0` になります。

### 2. 範囲内か判定する（`IsRange`）

```csharp
float distance = Vector3.Distance(transform.position, target.position);

if (_attackDistance.IsRange(distance))
{
    Attack();
}
```

**境界の扱い**

`Min = 2, Max = 5` の場合:

| value | 結果 |
|---|---|
| `1.9f` | `false` |
| `2.0f` | `true`（最小値は含む） |
| `4.99f` | `true` |
| `5.0f` | `false`（最大値は含まない） |

### 3. 範囲内に収める（`Clamp`）

```csharp
float speed = _speedRange.Clamp(inputSpeed);
```

`Min = 2, Max = 5` の場合:

| value | 結果 |
|---|---|
| `1.0f` | `2.0f` |
| `3.5f` | `3.5f` |
| `8.0f` | `5.0f`（最大値も含む） |

### 4. ランダムな値を取得する（`GetRandom`）

```csharp
float wait = _spawnInterval.GetRandom();
yield return new WaitForSeconds(wait);
```

内部で `UnityEngine.Random.Range(float, float)` を使用しています。

---

## 使用例

### 敵のスポーン間隔をランダムにする

```csharp
using System.Collections;
using UnityEngine;
using CoreKit;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private FloatRange _spawnInterval; // 例: Min=1, Max=3

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval.GetRandom());
            Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        }
    }
}
```

### 距離によって行動を切り替える

```csharp
using UnityEngine;
using CoreKit;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private FloatRange _chaseRange;  // 例: Min=5,  Max=15
    [SerializeField] private FloatRange _attackRange; // 例: Min=0,  Max=5

    public void Think(float distanceToPlayer)
    {
        if (_attackRange.IsRange(distanceToPlayer))
        {
            Attack();
        }
        else if (_chaseRange.IsRange(distanceToPlayer))
        {
            Chase();
        }
    }

    private void Attack() { /* ... */ }
    private void Chase() { /* ... */ }
}
```

> `_attackRange` の Max と `_chaseRange` の Min を同じ値にしておくと、`IsRange` が「最大値を含まない」ため境界で判定が重複しません。

### 入力値を安全な範囲に制限する

```csharp
[SerializeField] private FloatRange _zoomRange; // 例: Min=2, Max=10

public void SetZoom(float value)
{
    _camera.orthographicSize = _zoomRange.Clamp(value);
}
```

---

## 注意点

- **`IsRange` と `Clamp` は境界の扱いが異なります。**
  `IsRange` は最大値を含みませんが、`Clamp` は最大値を含みます。
  そのため `Clamp` した結果を `IsRange` に渡すと、最大値のときに `false` になる場合があります。
- **`Min` ≤ `Max` になるように設定してください。**
  入れ替わっている場合、`IsRange` は常に `false` を返します。また `Clamp` / `GetRandom` は意図しない値になります。
- **`GetRandom` は `UnityEngine.Random.Range(float, float)` を使うため、最大値も出る可能性があります**（`IsRange` の「最大値を含まない」とは異なります）。
- 値の設定は Inspector 経由が前提です（コード上から `Min` / `Max` を変更する手段はありません）。