# UI

UI まわりで汎用的に使える機能をまとめたものです。

- 名前空間: `CoreKit.UI`

---

## 機能一覧

| 機能 | 概要 | 詳細 |
|---|---|---|
| `HoverChecker` | UI 上のポインター（マウス・タッチ）の Enter / Exit を検知し、ホバー状態・イベント・そのフレームでの Enter / Exit 判定を提供する | [HoverChecker.md](HoverChecker.md) |

---

## 共通の前提

UI 機能の多くは、Unity の `EventSystem` を使います。次の条件がそろっていることを確認してください。

| 必要なもの | 内容 |
|---|---|
| `EventSystem` | シーンに存在すること |
| `Canvas` と `GraphicRaycaster` | 対象の UI が Canvas の下にあること |
| Raycast を受ける `Graphic` | `Image` などの `Raycast Target` がオンのコンポーネントがあること |

---