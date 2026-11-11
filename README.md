# CoreKitLibrary

Unity開発で汎用的に使用できる機能をまとめたライブラリです。

## Installation

Unity Package Managerから以下のURLを指定してインストールできます。

```text
https://github.com/TomokiSatokawa/CoreKitLibrary.git?path=/Packages/com.t0m0.corekit
```

## Features

### InputService

Input SystemをInputManagerのように扱いやすくする機能です。

- [詳細・使い方を見る →](Documentation/Input/InputService.md)

### FloatRange

float型の範囲値を扱う構造体です。範囲判定、値の制限、範囲内のランダム値取得に対応しています。

- [詳細・使い方を見る →](Documentation/Common/FloatRange.md)

### FlagRegistry

複数の要求元からのフラグ登録を管理するクラスです。トークンを使用して登録元ごとに状態を管理し、1つでも登録があればフラグを `true` にします。

- [詳細・使い方を見る →](Documentation/Flag/FlagRegistry.md)

### ObjectPool

Prefab単位でオブジェクトを再利用するオブジェクトプールです。事前生成、自動返却、Editorウィンドウでの状態確認に対応しています。

- [詳細・使い方を見る →](Documentation/ObjectPool/ObjectPool.md)

### DynamicEnum

コードを書き換えずに、アセット上で選択肢を追加・編集できる列挙型のような仕組みです。値はGUIDで識別されるため、項目名を変更しても参照が壊れません。

- [詳細・使い方を見る →](Documentation/DynamicEnum/DynamicEnum.md)

### SoundSection

BGMやSEなど、サウンドの種類ごとの再生を管理するクラスです。種類（DynamicEnum）指定またはAudioClip指定での再生、ワンショット再生、音量のフェード変更に対応しています。

- [詳細・使い方を見る →](Documentation/Sound/SoundSection.md)
