# UI

UI まわりで汎用的に使える機能をまとめたものです。

- 名前空間: `CoreKit.UI`

---

## 機能一覧

| 機能 | 概要 | 詳細 |
|---|---|---|
| `HoverChecker` | UI 上のポインター（マウス・タッチ）の Enter / Exit を検知し、ホバー状態・イベント・そのフレームでの Enter / Exit 判定を提供する | [HoverChecker.md](HoverChecker.md) |
| `PanelControl` | UI のウィンドウ（パネル）の表示 / 非表示を `CanvasGroup` で切り替え、表示・非表示時に `UnityEvent` を発行する | [PanelControl.md](PanelControl.md) |
| `SingleSelectionController<T>` | 選択可能な要素のうち、常に1つだけが選択されている状態（ラジオボタン形式）を管理する。名前空間: `CoreKit.UI.Selector` | [SingleSelectionController.md](SingleSelectionController.md) |
| `MultiSelectionController<T>` | 選択可能な要素の複数選択（クリックごとに選択 / 解除が切り替わるトグル形式）を管理する。名前空間: `CoreKit.UI.Selector` | [MultiSelectionController.md](MultiSelectionController.md) |

---

## 共通の前提

UI 機能の多くは、Unity の `EventSystem` を使います。次の条件がそろっていることを確認してください。

| 必要なもの | 内容 |
|---|---|
| `EventSystem` | シーンに存在すること |
| `Canvas` と `GraphicRaycaster` | 対象の UI が Canvas の下にあること |
| Raycast を受ける `Graphic` | `Image` などの `Raycast Target` がオンのコンポーネントがあること |

---