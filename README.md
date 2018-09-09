# UniWinApi & Example project

Unityでは本来行えない操作を Windows API 経由で行うものです。  
以下のようなことができます。  
他のアプリのウィンドウも操作可能です。

* ウィンドウの移動
* ウィンドウサイズ変更
* ウィンドウの最大化、最小化
* ウィンドウの透過
* ファイルのドロップを受け付ける
* マウスポインタを移動させる
* マウスのボタン操作を送出する

主にデスクトップマスコット的な用途で利用しそうな機能を取り込んでいます。
[![VRM viewer sample](http://img.youtube.com/vi/EETQxzzv4uY/0.jpg)](http://www.youtube.com/watch?v=EETQxzzv4uY "UniWinApi VRM viewer sample")

ビルド済みのVRMビューア―例はこちらの UniWinVrmViewer です。64ビット版と32ビット版(x86)があります。
* [Ver.0.3.1 最初から透明化](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.1)
* [Ver.0.3.0 照明の回転と並進移動も追加](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.0)
* [Ver.0.2.3 UniVRM0.42に。カメラFOVを10度に](https://github.com/kirurobo/UniWinApi/releases/tag/v0.2.3)
* [Ver.0.2.2 ライトを白色に](https://github.com/kirurobo/UniWinApi/releases/tag/v0.2.2)
* [Ver.0.2.1 シェーダー修正後](https://github.com/kirurobo/UniWinApi/releases/download/v0.2.1/UniWinApiVrmViewer_x64_v0.2.1.zip)
* [Ver.0.2.0 初版](https://github.com/kirurobo/UniWinApi/releases/download/v0.2.0/UniWinApiVrmViewer_x64.zip)


## License

[![CC0](http://i.creativecommons.org/p/zero/1.0/88x31.png "CC0")](http://creativecommons.org/publicdomain/zero/1.0/deed.ja)

## Configuration

本体は Assets/UniWinApi 以下です。  
このリポジトリには使用例として DWANGO Co., Ltd. による [UniVRM](https://github.com/dwango/UniVRM/releases) を含んでいます。


# UniVRMについて

## License

* [MIT License](Assets/VRM/LICENSE.txt)

## [VRM](https://dwango.github.io/vrm/)
###
"VRM" is a file format for using 3d humanoid avatars (and models) in VR applications.  
VRM is based on glTF2.0. And if you comply with the MIT license, you are free to use it.  
