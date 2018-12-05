/**
 * UniWinApi sample
 * 
 * Author: Kirurobo http://twitter.com/kirurobo
 * License: CC0 https://creativecommons.org/publicdomain/zero/1.0/
 */

using System.Collections;
using UnityEngine;

/// <summary>
/// Set editable the bool property
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class BoolPropertyAttribute : PropertyAttribute {}

/// <summary>
/// Set the attribute as readonly
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class ReadOnlyAttribute : PropertyAttribute { }


/// <summary>
/// デスクトップマスコット風の利用法を想定した UniWinApi サンプル。
/// </summary>
public class WindowController : MonoBehaviour {

	/// <summary>
	/// Window controller
	/// </summary>
	public UniWinApi uniWin;

	/// <summary>
	/// 操作を透過する状態か
	/// </summary>
	public bool isClickThrough
	{
		get { return _isClickThrough; }
	}
	private bool _isClickThrough = true;

	/// <summary>
	/// Is this window transparent
	/// </summary>
	public bool isTransparent
	{
		get { return _isTransparent; }
		set { SetTransparent(value); }
	}
	[SerializeField, BoolProperty, Tooltip("Check to set transparent on startup")]
	private bool _isTransparent = false;

	/// <summary>
	/// Is this window minimized
	/// </summary>
	public bool isTopmost {
		get { return ((uniWin != null) ? _isTopmost : _isTopmost = uniWin.IsTopmost); }
		set { SetTopmost(value); }
	}
	[SerializeField, BoolProperty, Tooltip("Check to set topmost on startup")]
	private bool _isTopmost = false;

	/// <summary>
	/// Is this window maximized
	/// </summary>
	public bool isMaximized {
		get { return ((uniWin != null) ? _isMaximized : _isMaximized = uniWin.IsMaximized); }
		set { SetMaximized(value); }
	}
	[SerializeField, BoolProperty, Tooltip("Check to set maximized on startup")]
	private bool _isMaximized = false;

	/// <summary>
	/// Is this window minimized
	/// </summary>
	public bool isMinimized {
		get { return ((uniWin != null) ? _isMinimized : _isMinimized = uniWin.IsMinimized); }
		set { SetMinimized(value); }
	}
	[SerializeField, BoolProperty, Tooltip("Check to set minimized on startup")]
	private bool _isMinimized = false;

	/// <summary>
	/// ファイルドロップを有効にするならば最初からtrueにしておく
	/// </summary>
	public bool enableFileDrop {
		get { return _enableFileDrop; }
		set {
			if (value) { BeginFileDrop(); }
			else { EndFileDrop(); }
		}
	}
	[SerializeField, BoolProperty, Tooltip("Check to set enable file-drop on startup")]
	private bool _enableFileDrop = false;

	/// <summary>
	/// マウスドラッグでウィンドウを移動させるか
	/// </summary>
	public bool enableDragMove = true;


	// カメラの背景をアルファゼロの黒に置き換えるため、本来の背景を保存しておく変数
	private CameraClearFlags originalCameraClearFlags;
	private Color originalCameraBackground;

	/// <summary>
	/// Is the mouse pointer on an opaque pixel
	/// </summary>
	//[SerializeField, Tooltip("Is the mouse pointer on an opaque pixel? (Read only)")]
	private bool onOpaquePixel = true;

	/// <summary>
	/// The cut off threshold of alpha value.
	/// </summary>
	private float opaqueThreshold = 0.1f;

	/// <summary>
	/// Pixel color under the mouse pointer. (Read only)
	/// </summary>
	[ReadOnly, Tooltip("Pixel color under the mouse pointer. (Read only)")]
	public Color pickedColor;

	private bool isDragging = false;
	private Vector2 lastMousePosition;

	/// <summary>
	/// 現在対象としているウィンドウが自分自身らしいと確認できたらtrueとする
	/// </summary>
	private bool isWindowChecked = false;

	/// <summary>
	/// カメラのインスタンス
	/// </summary>
	private Camera currentCamera;


	/// <summary>
	/// ファイルドロップ時のイベントハンドラー。 UniWinApiの OnFilesDropped にそのまま渡す。
	/// </summary>
	public event UniWinApi.FilesDropped OnFilesDropped
	{
		add { uniWin.OnFilesDropped += value; }
		remove { uniWin.OnFilesDropped -= value; }
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
		if (!currentCamera)
		{
			// メインカメラを探す
			currentCamera = Camera.main;

			// もしメインカメラが見つからなければ、Findで探す
			if (!currentCamera)
			{
				currentCamera = FindObjectOfType<Camera>();
			}
		}

		// カメラの元の背景を記憶
		if (currentCamera)
		{
			originalCameraClearFlags = currentCamera.clearFlags;
			originalCameraBackground = currentCamera.backgroundColor;

		}
		// 描画色抽出用テクスチャ
		colorPickerTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);

		// ウィンドウ制御用のインスタンス作成
		uniWin = new UniWinApi();

		// 自分のウィンドウを取得
		FindMyWindow();
	}

	void Start()
	{
		// マウスカーソル下の色を取得させるコルーチンを開始
		StartCoroutine(PickColorCoroutine());
	}

	void OnDestroy()
	{
		uniWin.Dispose();
	}

	// Update is called once per frame
	void Update () {
		// キー、マウス操作の下ウィンドウへの透過状態を更新
		UpdateClickThrough();

		// マウスドラッグでウィンドウ移動
		DragMove();

		// ウィンドウ枠が復活している場合があるので監視するため、呼ぶ
		uniWin.Update();
	}

	/// <summary>
	/// ウィンドウへのフォーカスが変化したときに呼ばれる
	/// </summary>
	/// <param name="focus"></param>
	private void OnApplicationFocus(bool focus)
	{
		//if (focus)
		//{
		//	// もしウィンドウハンドル取得に失敗していたら再取得
		//	if (!uniWin.IsActive)
		//	{
		//		FindMyWindow();
		//	}

		//	// アクティブウィンドウを監視して
		//	if (!isWindowChecked)
		//	{
		//		if (uniWin.CheckActiveWindow())
		//		{
		//			isWindowChecked = true; // どうやら正しくウィンドウをつかめているよう
		//		}
		//		else
		//		{
		//			// ウィンドウが違っているようなので、もう一度アクティブウィンドウを取得
		//			uniWin.Reset();
		//			uniWin.Dispose();
		//			uniWin = new UniWinApi();
		//			FindMyWindow();
		//		}
		//	}
		//}
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
		// ドラッグでの移動が無効化されていた場合
		if (!enableDragMove)
		{
			isDragging = false;
			return;
		}

		// 最大化時またはフルスクリーンならウィンドウ移動は行わない
		if (uniWin.IsMaximized || Screen.fullScreen)
		{
			isDragging = false;
			return;
		}

		// マウスドラッグでウィンドウ移動
		if (Input.GetMouseButtonDown(0))
		{
			lastMousePosition = UniWinApi.GetCursorPosition();
			isDragging = true;
		}
		if (!Input.GetMouseButton(0))
		{
			isDragging = false;
		}
		if (isDragging)
		{
			Vector2 mousePos = UniWinApi.GetCursorPosition();
			Vector2 delta = mousePos - lastMousePosition;
			lastMousePosition = mousePos;

			Vector2 windowPosition = uniWin.GetPosition();  // 現在のウィンドウ位置を取得
			windowPosition += delta; // ウィンドウ位置に上下左右移動分を加える
			uniWin.SetPosition(windowPosition);   // ウィンドウ位置を設定
		}
	}

	/// <summary>
	/// 画素の色を基に操作受付を切り替える
	/// </summary>
	void UpdateClickThrough()
	{
		if (_isClickThrough)
		{
			if (onOpaquePixel)
			{
				if (uniWin != null) uniWin.EnableClickThrough(false);
				_isClickThrough = false;
			}
		}
		else
		{
			if (isTransparent && !onOpaquePixel && !isDragging)
			{
				if (uniWin != null) uniWin.EnableClickThrough(true);
				_isClickThrough = true;
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
			MyPostRender(currentCamera);
		}
		yield return null;
	}

	/// <summary>
	/// マウス下の画素が透明かどうかを確認
	/// </summary>
	/// <param name="cam"></param>
	void MyPostRender(Camera cam)
	{
		// カメラが不明ならば何もしない
		if (!cam) return;

		Vector2 mousePos = Input.mousePosition;
		Rect camRect = cam.pixelRect;

		//// コルーチン & WaitForEndOfFrame ではなく、OnPostRenderで呼ぶならば、MSAAによって上下反転しないといけない？
		//if (QualitySettings.antiAliasing > 1) mousePos.y = camRect.height - mousePos.y;

		if (camRect.Contains(mousePos))
		{
			try
			{
				// Reference http://tsubakit1.hateblo.jp/entry/20131203/1386000440
				colorPickerTexture.ReadPixels(new Rect(mousePos, Vector2.one), 0, 0);
				Color color = colorPickerTexture.GetPixel(0, 0);
				pickedColor = color;
				onOpaquePixel = (color.a >= opaqueThreshold);  // αがしきい値以上ならば不透過とする
			} catch (System.Exception ex)
			{
				// 稀に範囲外になってしまうよう
				Debug.LogError(ex.Message);
				onOpaquePixel = false;
			}
		} else
		{
			onOpaquePixel = false;
		}
	}

	/// <summary>
	/// 自分のウィンドウハンドルを見つける
	/// </summary>
	private void FindMyWindow()
	{
		// ウィンドウが確かではないとしておく
		isWindowChecked = false;

		//// 現在このウィンドウがアクティブでなければ、取得はやめておく
		//if (!Application.isFocused) return;

		// 今アクティブなウィンドウを取得
		var window = UniWinApi.FindWindow();
		if (window == null) return;

		//if (Application.isEditor) {
		//	// Unityエディタと一致するかチェック
		//	//  （別アプリのウィンドウは対象とさせない）
		//	if (window.ProcessName != "Unity") return;
		//} else {
		//	// このUnityプロジェクトの名前と一致するかどうかをチェック
		//	//  （別アプリのウィンドウは対象とさせない）
		//	if (window.Title != Application.productName) return;
		//}

		// 見つかったウィンドウを利用開始
		uniWin.SetWindow(window);

		// 初期状態を反映
		SetTopmost(_isTopmost);
		SetMaximized(_isMaximized);
		SetMinimized(_isMinimized);
		SetTransparent(_isTransparent);
		if (_enableFileDrop) BeginFileDrop();
	}

	/// <summary>
	/// ウィンドウ透過状態になった際、自動的に背景を透明単色に変更する
	/// </summary>
	/// <param name="isTransparent"></param>
	void SetCameraBackground(bool isTransparent)
	{
		// カメラが特定できていなければ何もしない
		if (!currentCamera) return;

		if (isTransparent)
		{
			currentCamera.clearFlags = CameraClearFlags.SolidColor;
			currentCamera.backgroundColor = Color.clear;
		}
		else
		{
			currentCamera.clearFlags = originalCameraClearFlags;
			currentCamera.backgroundColor = originalCameraBackground;
		}
	}

	/// <summary>
	/// 透明化状態を切替
	/// </summary>
	/// <param name="transparent"></param>
	public void SetTransparent(bool transparent)
	{
		//if (_isTransparent == transparent) return;

		_isTransparent = transparent;
		SetCameraBackground(transparent);

		if (uniWin != null)
		{
			uniWin.EnableTransparent(transparent);
		}
		UpdateClickThrough();
		StateChangedEvent();
	}

	/// <summary>
	/// 最大化を切替
	/// </summary>
	public void SetMaximized(bool maximized)
	{
		//if (_isMaximized == maximized) return;
		if (uniWin == null)
		{
			_isMaximized = maximized;
		} else
		{

			if (maximized)
			{
				uniWin.Maximize();
			}
			else if (uniWin.IsMaximized)
			{
				uniWin.Restore();
			}
			_isMaximized = uniWin.IsMaximized;
		}
		StateChangedEvent();
	}

	/// <summary>
	/// 最小化を切替
	/// </summary>
	public void SetMinimized(bool minimized)
	{
		//if (_isMinimized == minimized) return;
		if (uniWin == null)
		{
			_isMinimized = minimized;
		} else
		{
			if (minimized)
			{
				uniWin.Minimize();
			}
			else if (uniWin.IsMinimized)
			{
				uniWin.Restore();
			}
			_isMinimized = uniWin.IsMinimized;
		}
		StateChangedEvent();
	}

	/// <summary>
	/// 最前面を切替
	/// </summary>
	/// <param name="topmost"></param>
	public void SetTopmost(bool topmost)
	{
		//if (_isTopmost == topmost) return;
		if (uniWin == null) return;

		uniWin.EnableTopmost(topmost);
		_isTopmost = uniWin.IsTopmost;
		StateChangedEvent();
	}

	/// <summary>
	/// Begin to accept file drop.
	/// </summary>
	public void BeginFileDrop()
	{
		if (uniWin != null)
		{
			uniWin.BeginFileDrop();
		}
		_enableFileDrop = true;
	}

	/// <summary>
	/// End to accept file drop.
	/// </summary>
	public void EndFileDrop()
	{
		if (uniWin != null)
		{
			uniWin.EndFileDrop();
		}
		_enableFileDrop = false;
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
