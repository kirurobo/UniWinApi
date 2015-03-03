WindowController

Copyright (c) 2014-2015 Kirurobo
Released under Unlicense <http://unlicense.org/>


■ 概要
・Windows API のウィンドウ操作機能を Unity 上から呼び出すためのクラスです。


■ 必要環境
・Unity 4.6 以降（おそらく）
・Windows Vista 以降（おそらく）


■ 内容物
・UnityChan … ユニティちゃん関連（Unity Technologies Japan 合同会社さん配布物の一部）
・WindowController
	・Example
		・SampleBehaviour.cs … クラス利用のサンプル
		・SimpleSample.unity … Cubeのみのサンプル
		・UnityChanSample.uniy … ユニティちゃんに動いていただくサンプルシーン
	・Scripts
		・WindowController … 本体
		・Wrapper
			・DwmApi … DWM API のラッパー（@ru__enさんによる）
			・WinApi … Windows API のラッパー（@ru__enさんによる）


■ 利用方法
・WindowController/Scritps 内のファイルを Assets 内の適当な場所に置いてください。
・具体的な使い方はサンプルでご覧ください…。
・手順としては
	1. WindowController のインスタンスを作成
	2. FindHandle()でウィンドウを見つける
	3. あとは SetPosition() や Maximize() などが利用できます
・SampleBehaviour.cs を適当なオブジェクトにアタッチするだけでも一応動きます。
・ウィンドウを透過させる場合の設定
	・カメラのBackgroundはRGBAすべてゼロにしておく
	・透過させたくない部分はアルファ値を255になるようにする
	　（シェーダーによっては半透明になります）


■ 注意点
・ウィンドウハンドルを確実に取る方法が分ってないので、別のウィンドウが
　操作対象になったりするかも知れません。（むしろ別のウィンドウも操作できます）
・閉じるボタンがなくなったり、ウィンドウを見失った場合、タスクマネージャから
　終了する必要が出るかも知れません。


■ ライセンス
・UnityChanフォルダ以下は『ユニティちゃんライセンス条項』に従います。

・Kiruroboが作成したファイルは、Unlicense（パブリックドメイン）としています。
　著作権表示なしで修正、複製、再配布も可能です。
　好きに使っていただけますがもちろん無保証です。

・WinApi.cs, DwmApi.cs は @ru__en さんによりますが
　こちらも利用に制限はありません。
　ただしいずれも Microsoft の仕様書やソフトウェアに基づいています
　また DwmApi.cs は http://msdn.microsoft.com/ja-jp/magazine/cc163435.aspx
　に基づいています。


■ 更新履歴
2015/03/03 最小化、最大化時はその直前の状態を保存するようにした
2014/04/26 公開用初版


■ 連絡先・配布元
@kirurobo
http://twitter.com/kirurobo
http://github.com/kirurobo