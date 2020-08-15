# UniWinApi Example project

[UniWinApi](https://github.com/kirurobo/UniWinApiAsset) は Unityでは本来行えない操作を Windows API 経由で行うものです。  
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

このリポジトリではそれらの機能を利用した応用例として、デスクトップマスコット風VRMビューアーのプロジェクトを置いてあります。  
[![UniWinApi VRM viewer](http://i.ytimg.com/vi/cq2g-hIGlAs/mqdefault.jpg)](https://youtu.be/cq2g-hIGlAs "UniWinApi VRM viewer v0.57.1")

## Download (ダウンロード)

ビルド済みのVRMビューア―例は [Releases](https://github.com/kirurobo/UniWinApi/releases) 中の UniWinApiVrmViewer です。64ビット版(x64)と32ビット版(x86)があります。
* [VRMビューアでない Ver.0.5.0 UniWinApi本体はこちら](https://github.com/kirurobo/UniWinApi/releases/tag/v0.5.0)
<details>
  <summary>過去のバージョン</summary>
  
* [Ver.0.5.0 レイヤードウィンドウも選択可に](https://github.com/kirurobo/UniWinApi/releases/tag/v0.5.0)
* [Ver.0.4.0-beta 色々改造](https://github.com/kirurobo/UniWinApi/releases/tag/v0.4.0beta)
* [Ver.0.3.3 UniVRM 0.44に](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.3)
* [Ver.0.3.2 マウスを追う](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.2)
* [Ver.0.3.1 最初から透明化](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.1)
* [Ver.0.3.0 照明の回転と並進移動も追加](https://github.com/kirurobo/UniWinApi/releases/tag/v0.3.0)
* [Ver.0.2.3 UniVRM 0.42に。カメラFOVを10度に](https://github.com/kirurobo/UniWinApi/releases/tag/v0.2.3)
* [Ver.0.2.2 ライトを白色に](https://github.com/kirurobo/UniWinApi/releases/tag/v0.2.2)
* [Ver.0.2.1 シェーダー修正後](https://github.com/kirurobo/UniWinApi/releases/download/v0.2.1/UniWinApiVrmViewer_x64_v0.2.1.zip)
* [Ver.0.2.0 初版](https://github.com/kirurobo/UniWinApi/releases/download/v0.2.0/UniWinApiVrmViewer_x64.zip)

</details>

## License (ライセンス)

UniWinApi本体はCC0ですが、VRMビューアではいくつか他のプロジェクトを利用させていただいています。

* CC0
  * [UniWinApi本体](http://github.com/kirurobo/UniWinApiAsset)
    * VRMビューア以外の、.unitypackageの中身は CC0 です。
    * そちらのソースは別リポジトリにて管理しています。

* MIT License
  * [VRMコンソーシアム](https://vrm-consortium.org/) の [UniVRM](https://github.com/dwango/UniVRM/)
  * [えむにわ @m2wasabi](https://twitter.com/m2wasabi)さんの [VRMLoaderUI](https://github.com/m2wasabi/VRMLoaderUI/)

* その他（フリー）
  * [ゆず @Yuzu_Unity](https://twitter.com/Yuzu_Unity)さんの [HumanoidCollider](https://github.com/yuzu-unity/HumanoidCollider) [Qiita記事](https://qiita.com/Yuzu_Unity/items/b645ecb76816b4f44cf9)

## System requirements (動作環境)

* 2018.4.20 - 
* Windows 7 or newer


## Usage (利用方法)

VRMビューアを動かしてみるだけなら、ダウンロードしたビルド済み実行ファイルを展開し、その中の UniWinApiVrmViewer.exe を起動してください。  
起動後にお手元の VRM ファイルをドロップするとそのモデルが表示されます。

VRMビューアのプロジェクトではなく、.unitypackage の利用（こちらがUniWinApiの本体）については、
こちらの [チュートリアル](https://github.com/kirurobo/UniWinApi/blob/master/docs/index_jp.md) をご覧ください。

このVRMビューアのUnityプロジェクトを利用する場合は、本リポジトリのソースコードの他に下記が必要です。

* ["Unity-chan!" Model](https://assetstore.unity.com/packages/3d/characters/unity-chan-model-18705) をアセットストアからインポート
    * 必要なのは Animations 以下です。
