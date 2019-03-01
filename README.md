# UniWinApi Example project

UniWinApi は Unityでは本来行えない操作を Windows API 経由で行うものです。  
以下のようなことができます。  

* ウィンドウの移動
* ウィンドウサイズ変更
* ウィンドウの最大化、最小化
* **ウィンドウの透過** （枠なしで、四角形でないウィンドウにします） 
* **ファイルのドロップを受け付ける**
* **Windowsのダイアログでファイルを開く（試験実装で単一ファイルのみ）**
* マウスポインタを移動させる
* マウスのボタン操作を送出する

主にデスクトップマスコット的な用途で利用しそうな機能を取り込んでいます。

このリポジトリではそれらの機能を利用したデスクトップマスコット風のVRMビューアーのプロジェクトを置いてあります。  
[![UniWinApi VRM viewer](http://i.ytimg.com/vi/cq2g-hIGlAs/mqdefault.jpg)](https://youtu.be/cq2g-hIGlAs "UniWinApi VRM viewer v0.4.0 beta")

## Download

ビルド済みのVRMビューア―例は [Releases](https://github.com/kirurobo/UniWinApi/releases) 中の UniWinApiVrmViewer です。64ビット版と32ビット版(x86)があります。
* [Ver.0.4.0 UniWinApi本体もリリース](https://github.com/kirurobo/UniWinApi/releases/tag/v0.4.0)
* [Ver.0.4.0-beta 色々改造](https://github.com/kirurobo/UniWinApi/releases/tag/v0.4.0beta)
* [Ver.0.3.3 UniVRM 0.44に](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.3)
* [Ver.0.3.2 マウスを追う](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.2)
* [Ver.0.3.1 最初から透明化](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.1)
* [Ver.0.3.0 照明の回転と並進移動も追加](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.0)
* [Ver.0.2.3 UniVRM 0.42に。カメラFOVを10度に](https://github.com/kirurobo/UniWinApi/releases/tag/v0.2.3)
* [Ver.0.2.2 ライトを白色に](https://github.com/kirurobo/UniWinApi/releases/tag/v0.2.2)
* [Ver.0.2.1 シェーダー修正後](https://github.com/kirurobo/UniWinApi/releases/download/v0.2.1/UniWinApiVrmViewer_x64_v0.2.1.zip)
* [Ver.0.2.0 初版](https://github.com/kirurobo/UniWinApi/releases/download/v0.2.0/UniWinApiVrmViewer_x64.zip)

## License ライセンス

* CC0
   * [UniWinApi本体](http://github.com/kirurobo/UniWinApiAsset)
      * VRMビューア以外の、.unitypackageの中身は CC0 です。
      * そちらのソースは別リポジトリにて管理しています。

* MIT License
    * DWANGO Co., Ltd. [UniVRM](https://github.com/dwango/UniVRM/)
    * [えむにわ @m2wasabi](https://twitter.com/m2wasabi) [VRMLoaderUI](https://github.com/m2wasabi/VRMLoaderUI/)
    * [@setch](https://twitter.com/setchi) [uGUI-Hypertext](https://github.com/setchi/uGUI-Hypertext)

## System requirements 動作環境

* Unity 5.6 or newer
* Windows 7 or newer


## Usage 利用方法

VRMビューアを動かしてみるだけなら、ダウンロードしたビルド済み実行ファイルを展開し、その中の UniWinApiVrmViewer.exe を起動してください。  
起動後にお手元の VRM ファイルをドロップするとそのモデルが表示されます。

VRMビューアのプロジェクトではなく、.unitypackage の利用（こちらがUniWinApiの本体）については、
こちらの [チュートリアル](https://github.com/kirurobo/UniWinApi/blob/master/docs/index_jp.md) をご覧ください。

このVRMビューアのUnityプロジェクトを利用する場合は、本リポジトリのソースコードの他に下記が必要です。

* ["Unity-chan!" Model](https://assetstore.unity.com/packages/3d/characters/unity-chan-model-18705) をアセットストアからインポート
    * 必要なのは Animations 以下です。
