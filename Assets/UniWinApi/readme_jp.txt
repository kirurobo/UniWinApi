UniWinApi

Copyright (c) 2014-2018 Kirurobo
Released under CC0 <http://creativecommons.org/publicdomain/zero/1.0/deed.ja>


■ 概要
・Windows API 機能を Unity 上から呼び出すためのクラスです。


■ 確認済み動作環境
・Unity 5.6.6f2, Unity 2018.2.6f1
・Windows 10 Pro x64 
・GeForce GTX980, GeForce GTX 1070


■ 内容物
リポジトリのクローンをUnityで開くと、VRMビューアを含むプロジェクトとなっています。
unitypackage の場合は、VRMビューアは含みません。

本体は下記フォルダ配下です。
・UniWinApi
	・Examples
		サンプルシーンがあります。
	・Scripts
		・WindowController.cs … 自分自身のウィンドウを操作する前提で実装したクラス
		・UniWinApi.cs … 本体
		・Wrapper
			・DwmApi … DWM API のラッパー
			・WinApi … Windows API のラッパー
		・Editor
			・WindowControllerEditor.cs … WindowControllerを利用する上でのエディタ拡張


■ 利用方法
UniWinApi が本体ですが、自分のウィンドウを操作するものとして WindowController を用意しています。

WindowController をヒエラルキーの中の適当なゲームオブジェクトにアタッチしてみてください。
インスペクタで下記を変更するか、スクリプトから変更するとウィンドウの状態を変えられます。
・IsTransparent … ウィンドウ枠を透明化
・IsTopmost … 常に最前面
・IsMaximized … ウィンドウ最大化
・IsMinimized … ウィンドウ最小化
・EnableFileDrop … ファイルドロップを受け付ける
・EnableDragMove … マウス左ボタンでウィンドウを移動できる

ファイルドロップを利用する場合、OnFilesDropped(string[] files) というイベントがありますので、そちらに処理を追加してください。
files にはドロップされたファイルのパスが入ります。

ドキュメントは整えられていないため、利用例はサンプルをご覧ください (^-^;


■ 注意点
・ウィンドウハンドルを確実に取る方法が分ってないので、別のウィンドウが
　操作対象になったりするかも知れません。
　（むしろ別のウィンドウもタイトルやクラス名で指定して操作できます。）
・閉じるボタンがなくなったり、ウィンドウを見失った場合、タスクマネージャから
　終了する必要が出るかも知れません。


■ FAQ
・エディタ上で透明にすると表示がおかしいのですが。
	・仕様です。
	・Game ウィンドウはどうも背景が塗りつぶされてしまっているようで、透明化されません。
	・透明化はビルドしたものでご確認ください。


■ ライセンス
・Kiruroboが作成したファイルは、CC0（パブリックドメイン）としています。
　著作権表示なしで修正、複製、再配布も可能です。
　好きに使っていただけますがもちろん無保証です。
　他の方のブログなど参考にしている部分はありますが、同様の機能を実装した場合は似た内容になるとして、
　著作権で保護される範囲においては基本的に独自のものとしてCC0にできると判断しています。

・UniVRM は MIT License となっています。
  https://github.com/dwango/UniVRM/ をご覧ください。

・Assets/StreamingAssets/default_vrm.vrm のVRMモデルは @cubic9com さん作の
  VRChat用超シンプル素体 を調整してVRMにしたものです。
  こちらも CC0 ですが、元モデルのサイトもご覧ください。
  https://cubic9.com/Devel/OculusRift/VRChat/


■ 更新履歴
2018/09/09 おおよそ想定通りに動作しそうなため、パッケージを公開
2018/08/24 UniWinApiとして整理し直した
2015/03/03 最小化、最大化時はその直前の状態を保存するようにした
2014/04/26 公開用初版


■ 連絡先・配布元
@kirurobo
http://twitter.com/kirurobo
http://github.com/kirurobo
