# CoreKitLibrary

Unity開発で汎用的に使用できる機能をまとめたライブラリです。

## Installation

Unity Package Managerから以下のURLを指定してインストールできます。

```text
https://github.com/TomokiSatokawa/CoreKitLibrary.git?path=/Packages/com.t0m0.corekit
```

## Features

### FloatRange
[FloatRangeの使い方](Documentation/Common/FloatRange.md)
float型の範囲値を扱う構造体です。範囲判定、値の制限、範囲内のランダム値取得に対応しています。


### FlagRegistry
複数の要求元からのフラグ登録を管理するクラスです。トークンを使用して登録元ごとに状態を管理し、1つでも登録があればフラグを `true` にします。
