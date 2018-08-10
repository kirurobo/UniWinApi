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
	private WindowController window;
	
	/// <summary>
	/// 最初からウィンドウ透過するならtrueにしておく
	/// </summary>
	public bool isTransparent = false;

	/// <summary>
	/// ウィンドウタイトル（ProductNameが入る）
	/// </summary>
	private string title = "";

    /// <summary>
    /// 操作を受け付ける状態か
    /// </summary>
    private bool isFocusable = true;

    /// <summary>
    /// Is the mouse pointer on not transparent pixel?
    /// </summary>
    private bool onOpaquePixel = true;

    private Rect camRect;
    private Color pickedColor;

    private bool isDragging = false;
    private Vector2 lastMousePosition;

    /// <summary>
    /// 表示されたテクスチャ
    /// </summary>
    private Texture2D colorPickerTexture = null;

	// Use this for initialization
	void Awake () {
		// ウィンドウ制御用のインスタンス作成
		window = new WindowController();

		// 名前からウィンドウを取得
		title = WindowController.GetProjectName();
		FindMyWindow();

		// 透過時に常に最前面にする
		window.TopmostWhenTransparent = true;

		// 起動時からウィンドウ透過を反映
		window.EnableTransparency(isTransparent);

        // ファイルがドロップされた時のハンドラ
        window.OnFilesDropped += FileDropped;
	}

    void Start()
    {
        colorPickerTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);

        Camera.onPostRender += MyPostRender;
    }

    void OnDestroy()
    {
        Camera.onPostRender -= MyPostRender;
    }

    // Update is called once per frame
    void Update () {
        // キー、マウス操作の下ウィンドウへの透過状態を更新
        UpdateFocusable();

		// もしウィンドウハンドル取得に失敗していたら再取得
		//	キー押下時点でアクティブなのは自分のウィンドウと仮定
		//	特にビルドしたものの実行だと起動時に見失ったりするので。
		if (Input.anyKeyDown) {
			if (!window.IsActive) {
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

		// Insert を押すと最小化切替
		if (Input.GetKeyDown(KeyCode.Insert)) {
			ToggleMinimize();
		}

        // Insert を押すと最小化切替
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            ToggleMinimize();
        }

        // マウスドラッグでウィンドウ移動
        if (Input.GetMouseButtonDown(0))
        {
            this.lastMousePosition = window.GetCursorPosition();
            this.isDragging = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            this.isDragging = false;
        }
        if (isDragging)
        {
            Vector2 mousePos = window.GetCursorPosition();
            Vector2 delta = mousePos - this.lastMousePosition;
            this.lastMousePosition = mousePos;

            Vector2 windowPosition = window.GetPosition();  // 現在のウィンドウ位置を取得
            windowPosition += delta; // ウィンドウ位置に上下左右移動分を加える
            window.SetPosition(windowPosition);	// ウィンドウ位置を設定
        }

        // ジョイスティックまたはカーソルキーでウィンドウ移動
        //	画面Y座標は下が大なので上下反転
        Vector2 axes = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"));
		if (axes.sqrMagnitude != 0f) {
			Vector2 windowPosition = window.GetPosition();	// 現在のウィンドウ位置を取得
			windowPosition += axes * 10.0f;	// ウィンドウ位置に上下左右移動分を加える。係数10.0fは適当。
			window.SetPosition(windowPosition);	// ウィンドウ位置を設定
		}

        //ToggleFocusableOnRay();

		// ウィンドウ枠が復活している場合があるので監視するため、呼ぶ
		window.Update();
	}

    /// <summary>
    /// 画素の色を基に操作受付を切り替える
    /// </summary>
    void UpdateFocusable()
    {
        if (!this.isFocusable)
        {
            if (this.onOpaquePixel)
            {
                window.EnableUnfocusable(false);
                this.isFocusable = true;
                //Debug.Log(this.isFocusable);
            }
        }
        else
        {
            if (this.isTransparent && !this.onOpaquePixel && !this.isDragging)
            {
                window.EnableUnfocusable(true);
                this.isFocusable = false;
                //Debug.Log(this.isFocusable);
            }
        }
    }

    /// <summary>
    /// マウス下の画素が透明かどうかを確認
    /// </summary>
    /// <param name="cam"></param>
    void MyPostRender(Camera cam)
    {
        Vector2 mousePos = Input.mousePosition;
        this.camRect = cam.pixelRect;
        if (QualitySettings.antiAliasing > 1) mousePos.y = camRect.height - mousePos.y;
        if (cam.pixelRect.Contains(mousePos))
        {
            // Reference http://tsubakit1.hateblo.jp/entry/20131203/1386000440
            colorPickerTexture.ReadPixels(new Rect(mousePos, Vector2.one), 0, 0);
            Color color = colorPickerTexture.GetPixel(0, 0);
            this.onOpaquePixel = (color.a > 0.1f);
            this.pickedColor = color;
        } else
        {
            this.onOpaquePixel = false;
        }

        //if (!this.onOpaquePixel) Debug.Log(this);
    }

    // Show GUI objects.
    void OnGUI() {
		float buttonWidth = 140f;
		float buttonHeight = 40f;
		float margin = 10f;
		if (
			GUI.Button(
				new Rect(
					Screen.width - buttonWidth - margin,
					Screen.height - buttonHeight - margin,
					buttonWidth,
					buttonHeight),
				"Toggle transparency"
				)
			) {
			// 透過の ON/OFF ボタン
			ToggleWindowTransparency();
		}

        //if (this.onOpaquePixel)
        {
            GUI.color = new Color(pickedColor.r, pickedColor.g, pickedColor.b, 1f);
            Vector2 pos = Input.mousePosition;
            GUI.Label(
                new Rect(
                    Screen.width - buttonWidth - margin,
                    Screen.height - buttonHeight * 2 - margin * 2,
                    buttonWidth,
                    buttonHeight),
                string.Format("({0,4},{1,4})/({2,4},{3,4})", pos.x, pos.y, camRect.width, camRect.height) + (onOpaquePixel ? " *" : "")
                );
        }
    }


	/// <summary>
	/// 自分のウィンドウハンドルを見つける
	/// </summary>
	private void FindMyWindow()
	{
		// まず自分のウィンドウタイトルで探す
		if (title != "" || !window.FindHandleByTitle(title)) {
			// 名前がダメならアクティブなウィンドウを取得
			window.FindHandle();
		}
	}

	/// <summary>
	/// ウィンドウ透過切替
	/// </summary>
	public void ToggleWindowTransparency() {
		isTransparent = !isTransparent;
		window.EnableTransparency(isTransparent);
	}
	
	/// <summary>
	/// 最大化を切替
	/// </summary>
	public void ToggleMaximize() {
		if (window.IsMaximized) {
			window.Restore();
		} else {
			window.Maximize();
		}
	}
	
	/// <summary>
	/// 最小化を切替
	/// </summary>
	public void ToggleMinimize() {
		if (window.IsMinimized) {
			window.Restore();
		} else {
			window.Minimize();
		}
	}

    /// <summary>
    /// ファイルがドロップされたときの処理
    /// </summary>
    /// <param name="files"></param>
    public void FileDropped(string[] files)
    {
        foreach (string path in files)
        {
            Debug.Log(path);
        }
    }
}
