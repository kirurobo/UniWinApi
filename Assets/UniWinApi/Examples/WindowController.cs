/**
 * UniWinApi sample
 * 
 * Author: Kirurobo http://twitter.com/kirurobo
 * License: CC0 https://creativecommons.org/publicdomain/zero/1.0/
 */

using System.Collections;
using UnityEngine;

/// <summary>
/// デスクトップマスコット風の利用法を想定した UniWinApi サンプル。
/// これを空のオブジェクトにアタッチすれば、[End][Home]等のキーでウィンドウを操作できます。
/// このスクリプトはアプリケーションに合わせて改造してください。
/// </summary>
public class WindowController : MonoBehaviour {
	/// <summary>
	/// Window controller
	/// </summary>
	public UniWinApi uniWin;
	
	/// <summary>
	/// 最初からウィンドウ透過するならtrueにしておく
	/// </summary>
	public bool isTransparent = false;

    /// <summary>
    /// 操作を受け付ける状態か
    /// </summary>
	public bool isFocusable
	{
		get { return _isFocusable; }
	}
    private bool _isFocusable = true;

	/// <summary>
	/// Is this window minimized?
	/// </summary>
	public bool isTopmost { get { return ((uniWin != null) && uniWin.IsTopmost); } }

	/// <summary>
	/// Is this window maximized?
	/// </summary>
	public bool isMaximized { get { return ((uniWin != null) && uniWin.IsMaximized); } }

	/// <summary>
	/// Is this window minimized?
	/// </summary>
	public bool isMinimized { get { return ((uniWin != null) && uniWin.IsMinimized); } }

	/// <summary>
	/// Is the mouse pointer on not transparent pixel?
	/// </summary>
	private bool onOpaquePixel = true;

	// カメラの背景をアルファゼロの黒に置き換えるため、本来の背景を保存
    private CameraClearFlags originalCameraClearFlags;
    private Color originalCameraBackground;
	public Color pickedColor;

    private bool isDragging = false;
    private Vector2 lastMousePosition;

	/// <summary>
	/// ファイルドロップ時のイベントハンドラー。 UniWinApiの OnFilesDropped にそのまま渡す。
	/// </summary>
	public event UniWinApi.FilesDropped OnFilesDropped
	{
		add { this.uniWin.OnFilesDropped += value; }
		remove { this.uniWin.OnFilesDropped -= value; }
	}

	/// <summary>
	/// ウィンドウ状態が変化したときに発生するイベント
	/// </summary>
	public event OnStateChangeDelegate OnStateChange;
	public delegate void OnStateChangeDelegate();

	/// <summary>
	/// 表示されたテクスチャ
	/// </summary>
	private Texture2D colorPickerTexture = null;


	// Use this for initialization
	void Awake () {
		// カメラの元の背景を記憶
		this.originalCameraClearFlags = Camera.main.clearFlags;
		this.originalCameraBackground = Camera.main.backgroundColor;

		// 描画色抽出用テクスチャ
		this.colorPickerTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);

		// ウィンドウ制御用のインスタンス作成
		uniWin = new UniWinApi();

		// 名前からウィンドウを取得
		FindMyWindow();
	}

    void Start()
    {
		// 設定により起動時からウィンドウ透過を反映
		SetTransparent(isTransparent);

		// 最前面はデフォルトで有効
		SetTopmost(true);

		// マウスカーソル下の色を取得させるコルーチン
		StartCoroutine(PickColorCoroutine());
	}

	void OnDestroy()
    {
        uniWin.Dispose();
    }

    // Update is called once per frame
    void Update () {
        // キー、マウス操作の下ウィンドウへの透過状態を更新
        UpdateFocusable();

		// もしウィンドウハンドル取得に失敗していたら再取得
		//	キー押下時点でアクティブなのは自分のウィンドウと仮定
		//	特にビルドしたものの実行だと起動時に見失ったりするので。
		if (Input.anyKeyDown) {
			if (!uniWin.IsActive) {
				FindMyWindow();
			}
		}

		// End を押すとウィンドウ透過切替
		if (Input.GetKeyDown(KeyCode.End)) {
			ToggleTransparent();
			StateChangedEvent();
		}

		// Home を押すと最前面切替
		if (Input.GetKeyDown(KeyCode.Home))
		{
			ToggleTopmost();
			StateChangedEvent();
		}
		// F11 を押すと最大化切替
		if (Input.GetKeyDown(KeyCode.F11)) {
			ToggleMaximized();
			StateChangedEvent();
		}

		// Insert を押すと最小化切替
		if (Input.GetKeyDown(KeyCode.Insert)) {
			ToggleMinimized();
			StateChangedEvent();
		}

        // マウスドラッグでウィンドウ移動
        DragMove();

		// ウィンドウ枠が復活している場合があるので監視するため、呼ぶ
		uniWin.Update();
	}

	/// <summary>
	/// ウィンドウ状態が変わったときに呼ぶイベントを処理
	/// </summary>
	private void StateChangedEvent()
	{
		if (OnStateChange != null)
		{
			OnStateChange();
		}
	}

    /// <summary>
    /// 最大化時以外なら、マウスドラッグによってウィンドウを移動
    /// </summary>
    void DragMove()
    {
        // 最大化時はウィンドウドラッグは行わない
        if (uniWin.IsMaximized)
        {
            this.isDragging = false;
            return;
        }

        // マウスドラッグでウィンドウ移動
        if (Input.GetMouseButtonDown(0))
        {
            this.lastMousePosition = uniWin.GetCursorPosition();
            this.isDragging = true;
        }
        if (!Input.GetMouseButton(0))
        {
            this.isDragging = false;
        }
        if (isDragging)
        {
            Vector2 mousePos = uniWin.GetCursorPosition();
            Vector2 delta = mousePos - this.lastMousePosition;
            this.lastMousePosition = mousePos;

            Vector2 windowPosition = uniWin.GetPosition();  // 現在のウィンドウ位置を取得
            windowPosition += delta; // ウィンドウ位置に上下左右移動分を加える
            uniWin.SetPosition(windowPosition);   // ウィンドウ位置を設定
        }
    }

    /// <summary>
    /// 画素の色を基に操作受付を切り替える
    /// </summary>
    void UpdateFocusable()
    {
        if (!this._isFocusable)
        {
            if (this.onOpaquePixel)
            {
                uniWin.EnableUnfocusable(false);
                this._isFocusable = true;
            }
        }
        else
        {
            if (this.isTransparent && !this.onOpaquePixel && !this.isDragging)
            {
                uniWin.EnableUnfocusable(true);
                this._isFocusable = false;
			}
		}
    }

	/// <summary>
	/// OnPostRenderではGUI描画前になってしまうため、コルーチンを用意
	/// </summary>
	/// <returns></returns>
	private IEnumerator PickColorCoroutine()
	{
		while (Application.isPlaying)
		{
			yield return new WaitForEndOfFrame();
			MyPostRender(Camera.main);
		}
		yield return null;
	}

	/// <summary>
	/// マウス下の画素が透明かどうかを確認
	/// </summary>
	/// <param name="cam"></param>
	void MyPostRender(Camera cam)
    {
        Vector2 mousePos = Input.mousePosition;
        Rect camRect = cam.pixelRect;

		// コルーチン & WaitForEndOfFrame ではなく、OnPostRenderで呼ぶならば、MSAAによって上下反転しないといけない？
        //if (QualitySettings.antiAliasing > 1) mousePos.y = camRect.height - mousePos.y;

        if (camRect.Contains(mousePos))
        {
			try
			{
				// Reference http://tsubakit1.hateblo.jp/entry/20131203/1386000440
				colorPickerTexture.ReadPixels(new Rect(mousePos, Vector2.one), 0, 0);
				Color color = colorPickerTexture.GetPixel(0, 0);
				this.pickedColor = color;
				this.onOpaquePixel = (color.a > 0.1f);  // αが0.1より大きければ不透過とする
			} catch (System.Exception ex)
			{
				// 稀に範囲外になってしまうよう
				Debug.LogError(ex.Message);
				this.onOpaquePixel = false;
			}
        } else
        {
            this.onOpaquePixel = false;
        }
    }

	/// <summary>
	/// 自分のウィンドウハンドルを見つける
	/// </summary>
	private void FindMyWindow()
	{
        // Unityプロジェクト名を取得
        string title = UniWinApi.GetProjectName();

        // まず自分のウィンドウタイトルで探す
        if (title != "" || !uniWin.FindHandleByTitle(title)) {
			// 名前がダメなら、今アクティブなウィンドウを取得
			uniWin.FindHandle();
		}
	}

    /// <summary>
    /// ウィンドウ透過状態になった際、自動的に背景を透明単色に変更する
    /// </summary>
    /// <param name="isTransparent"></param>
    void SetCameraBackground(bool isTransparent)
    {
        if (isTransparent)
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = Color.clear;
        }
        else
        {
            Camera.main.clearFlags = this.originalCameraClearFlags;
            Camera.main.backgroundColor = this.originalCameraBackground;
        }
    }

	public void SetTransparent(bool transparent)
	{
		this.isTransparent = transparent;
		SetCameraBackground(transparent);
		uniWin.EnableTransparent(transparent);
		
		// 少しサイズを変更して戻してみる
		Vector2 size = uniWin.GetSize();
		uniWin.SetSize(size - Vector2.one);
		uniWin.SetSize(size);
		
		UpdateFocusable();
	}

	public void SetMaximized(bool maximized)
	{
		if (maximized)
		{
			uniWin.Maximize();
		} else if (uniWin.IsMaximized)
		{
			uniWin.Restore();
		}
	}

	public void SetMinimized(bool minimized)
	{
		if (minimized)
		{
			uniWin.Minimize();
		}
		else if (uniWin.IsMinimized)
		{
			uniWin.Restore();
		}
	}

	public void SetTopmost(bool topmost)
	{
		uniWin.EnableTopmost(topmost);
	}

	/// <summary>
	/// ウィンドウ透過切替
	/// </summary>
	private void ToggleTransparent()
	{
		isTransparent = !isTransparent;
		SetTransparent(isTransparent);
	}

	/// <summary>
	/// 最前面切替
	/// </summary>
	private void ToggleTopmost()
	{
		SetTopmost(!isTopmost);
	}
	
	/// <summary>
	/// 最大化を切替
	/// </summary>
	public void ToggleMaximized() {
		if (uniWin.IsMaximized) {
			uniWin.Restore();
		} else {
			uniWin.Maximize();
		}
	}
	
	/// <summary>
	/// 最小化を切替
	/// </summary>
	public void ToggleMinimized() {
		if (uniWin.IsMinimized) {
			uniWin.Restore();
		} else {
			uniWin.Minimize();
		}
	}

	/// <summary>
	/// Begin to accept file drop.
	/// </summary>
    public void BeginFileDrop()
    {
        uniWin.BeginFileDrop();
    }

	/// <summary>
	/// End to accept file drop.
	/// </summary>
    public void EndFileDrop()
    {
        uniWin.EndFileDrop();
    }

    /// <summary>
    /// 終了時にはウィンドウプロシージャを戻す処理が必要
    /// </summary>
    void OnApplicationQuit()
    {
        if (Application.isPlaying)
        {
            uniWin.Dispose();
        }
    }
}
