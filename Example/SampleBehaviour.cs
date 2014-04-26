/**
 * WindowController sample
 * 
 * Author: Kirurobo, http://twitter.com/kirurobo
 * Copyright: (c) 2014 Kirurobo
 * License: Unlicense
 * 
 * This is free and unencumbered software released into the public domain.
 * 
 * Anyone is free to copy, modify, publish, use, compile, sell, or
 * distribute this software, either in source code form or as a compiled
 * binary, for any purpose, commercial or non-commercial, and by any
 * means.
 * 
 * In jurisdictions that recognize copyright laws, the author or authors
 * of this software dedicate any and all copyright interest in the
 * software to the public domain. We make this dedication for the benefit
 * of the public at large and to the detriment of our heirs and
 * successors. We intend this dedication to be an overt act of
 * relinquishment in perpetuity of all present and future rights to this
 * software under copyright law.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 * 
 * For more information, please refer to <http://unlicense.org/>
 */
using UnityEngine;
using System.Collections;

public class SampleBehaviour : MonoBehaviour {
	/// <summary>
	/// Window controller
	/// </summary>
	private WindowController Window;
	
	/// <summary>
	/// 最初からウィンドウ透過するならtrueにしておく
	/// </summary>
	public bool IsTransparent = false;

	/// <summary>
	/// ウィンドウタイトル（ProductNameが入る）
	/// </summary>
	private string Title = "";

	// Use this for initialization
	void Start () {
		// ウィンドウ制御用のインスタンス作成
		Window = new WindowController();

		// 名前からウィンドウを取得
		Title = WindowController.GetProjectName();
		FindMyWindow();

		// 透過時に常に最前面にする
		Window.TopmostWhenTransparent = true;

		// 起動時からウィンドウ透過を反映
		Window.EnableTransparency(IsTransparent);
	}
	
	// Update is called once per frame
	void Update () {
		// もしウィンドウハンドル取得に失敗していたら再取得
		//	キー押下時点でアクティブなのは自分のウィンドウと仮定
		//	特にビルドしたものの実行だと起動時に見失ったりするので。
		if (Input.anyKeyDown) {
			if (!Window.IsActive) {
				FindMyWindow();
			}
		}

		// End を押すとウィンドウ透過切替
		if (Input.GetKeyDown(KeyCode.End)) {
			ToggleWindowTransparency();

		}

		// Home を押すと最大化切替
		if (Input.GetKeyDown(KeyCode.Home)) {
			ToggleMaximize();
		}

		// ジョイスティックまたはカーソルキーでウィンドウ移動
		//	画面Y座標は下が大なので上下反転
		Vector2 axes = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"));
		if (axes.sqrMagnitude != 0f) {
			Vector2 windowPosition = Window.GetPosition();	// 現在のウィンドウ位置を取得
			windowPosition += axes * 10.0f;	// ウィンドウ位置に上下左右移動分を加える。係数10.0fは適当。
			Window.SetPosition(windowPosition);	// ウィンドウ位置を設定
		}
		
		// ウィンドウ枠が復活している場合があるので監視するため、呼ぶ
		Window.Update();
	}

	// Show GUI objects.
	void OnGUI() {
		float buttonWidth = 100f;
		float buttonHeight = 40f;
		float margin = 10f;
		if (
			GUI.Button(
				new Rect(
					Screen.width - buttonWidth - margin,
					Screen.height - buttonHeight - margin,
					buttonWidth,
					buttonHeight),
				"Transparent"
				)
			) {
			// 透過の ON/OFF ボタン
			ToggleWindowTransparency();
		}
	}


	/// <summary>
	/// 自分のウィンドウハンドルを見つける
	/// </summary>
	private void FindMyWindow()
	{
		// まず自分のウィンドウタイトルで探す
		if (Title != "" || !Window.FindHandleByTitle(Title)) {
			// 名前がダメならアクティブなウィンドウを取得
			Window.FindHandle();
		}
	}

	/// <summary>
	/// ウィンドウ透過切替
	/// </summary>
	public void ToggleWindowTransparency() {
		IsTransparent = !IsTransparent;
		Window.EnableTransparency(IsTransparent);
	}
	
	/// <summary>
	/// 最大化を切替
	/// </summary>
	public void ToggleMaximize() {
		if (Window.IsMaximized) {
			Window.Restore();
		} else {
			Window.Maximize();
		}
	}
}
