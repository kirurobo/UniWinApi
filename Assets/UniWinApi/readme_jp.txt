UniWinApi

Copyright (c) 2014-2018 Kirurobo
Released under CC0 <https://creativecommons.org/publicdomain/zero/1.0/>


■ 概要
・Windows API のウィンドウ操作等、一部機能を Unity 上から呼び出すためのクラスです。
・デスクトップマスコット的なものに必要そうな機能を実装しています。
・DWMによるウィンドウ枠透過、マウスポインタ関連、ファイルのドロップを含みます。


■ テストした環境
・Unity 2017.4.8f1
・Windows 10 Pro 64bit


■ 内容物
・VRM … DWANGO Co., Ltd. によるVRM取り扱いアセットです。こちらはMITライセンスです。
         詳細 https://dwango.github.io/vrm/
・VRM.Samples … DWANGO Co., Ltd. によるVRM取り扱いアセットです。こちらはMITライセンスです。
         詳細 https://dwango.github.io/vrm/
・UniWinApi
	・Example
		・SampleBehaviour.cs … クラス利用のサンプル
		・SimpleSample.unity … Cubeのみのサンプル
		・UnityChanSample.uniy … ユニティちゃんに動いていただくサンプルシーン
	・Scripts
		・UniWinApi.cs … 本体
		・Wrapper
			・DwmApi … DWM API のラッパー
			・WinApi … Windows API のラッパー


■ 利用方法
・UniWinApi/Scritps 内のファイルを Assets 内の適当な場所に置いてください。
・具体的な使い方はサンプルでご覧ください。
・手順としては
	1. UniWinApi のインスタンスを作成
	2. FindHandle()で自分のウィンドウを見つける
	3. あとは SetPosition() や Maximize() などで操作
	といった流れです
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
・UniVRMフォルダ以下は、MITライセンスです。

・UniWinApiフォルダたファイルは、CC0（パブリックドメイン）としています。
　著作権表示なしで修正、複製、再配布も可能です。
　好きに使っていただけますがもちろん無保証です。


■ 更新履歴
2018/08/15 UnityWindowController から整理し直し、別名で公開
2014/04/26 公開用初版


■ 連絡先・配布元
@kirurobo
https://twitter.com/kirurobo
https://github.com/kirurobo