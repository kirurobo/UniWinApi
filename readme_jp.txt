UniWinApi

Copyright (c) 2014-2018 Kirurobo
Released under CC0 <http://creativecommons.org/publicdomain/zero/1.0/deed.ja>


■ 概要
・Windows API 機能を Unity 上から呼び出すためのクラスです。


■ 確認済み動作環境
・Unity 5.6.5p4
・Windows 10 Pro x64 
・GeForce GTX980


■ 内容物
リポジトリにある一式をUnityで開くと、VRMビューアを含むプロジェクトとなっています。

そのうち重要なのは下記フォルダです。
・Assets/UniWinApi …このフォルダ以下が本体です。
・Assets/UniVRM    …これは DWANGO Co., Ltd. による UniVRM （MIT License）です。


■ 注意点
・ウィンドウハンドルを確実に取る方法が分ってないので、別のウィンドウが
　操作対象になったりするかも知れません。
　（むしろ別のウィンドウもタイトルやクラス名で指定して操作できます。）
・閉じるボタンがなくなったり、ウィンドウを見失った場合、タスクマネージャから
　終了する必要が出るかも知れません。


■ ライセンス
・UniVRM は MIT License となっています。
  https://github.com/dwango/UniVRM/ をご覧ください。

・Kiruroboが作成したファイルは、CC0（パブリックドメイン）としています。
　著作権表示なしで修正、複製、再配布も可能です。
　好きに使っていただけますがもちろん無保証です。

・Assets/StreamingAssets/default_vrm.vrm のVRMモデルは @cubic9com さん作の
  VRChat用超シンプル素体 を調整してVRMにしたものです。
  こちらも CC0 ですが、元モデルのサイトもご覧ください。
  https://cubic9.com/Devel/OculusRift/VRChat/


■ 更新履歴
2018/08/24 UniWinApiとして整理し直した
2015/03/03 最小化、最大化時はその直前の状態を保存するようにした
2014/04/26 公開用初版


■ 連絡先・配布元
@kirurobo
http://twitter.com/kirurobo
http://github.com/kirurobo
