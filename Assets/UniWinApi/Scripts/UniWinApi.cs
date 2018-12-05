/**
 * UniWinApi
 * 
 * License: CC0, https://creativecommons.org/publicdomain/zero/1.0/
 * 
 * Author: Kirurobo, http://twitter.com/kirurobo
 */

using UnityEngine;
using System;
using System.Text;
using AOT;
using System.Collections.Generic;

public class UniWinApi : IDisposable {

	/// <summary>
	/// ウィンドウハンドルのカプセル化
	/// </summary>
	public class WindowHandle
	{
		public IntPtr hWnd;
		public string Title = "";
		public string ClassName = "";
		public string ProcessName = "";
		public int ProcessId = 0;

		public WindowHandle()
		{
			hWnd = IntPtr.Zero;
			Title = "";
			ClassName = "";
			ProcessName = "";
			ProcessId = 0;
		}

		public WindowHandle(IntPtr hwnd)
		{
			hWnd = hwnd;
			if (hWnd == IntPtr.Zero) return;

			const int len = 1024;
			StringBuilder sb = new StringBuilder(len);

			// クラス名を取得
			if (WinApi.GetClassName(hWnd, sb, len) > 0)
			{
				ClassName = sb.ToString();
			}

			// ウィンドウタイトルを取得
			sb.Length = 0;	// StringBuilder内を消去
			if (WinApi.GetWindowText(hWnd, sb, len) > 0)
			{
				Title = sb.ToString();
			}

			// プロセス名を取得
			int pid;
			WinApi.GetWindowThreadProcessId(hWnd, out pid);

			// GetProcessNyId(PID) で指定PIDが存在しない例外になる場合があるため try {} を使用
			try
			{
				System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(pid);
				ProcessName = p.ProcessName;
				//ProcessId = p.Id;	// = pid; でおそらく同様
			} catch
			{
				ProcessName = "";
			}
			ProcessId = pid;
			//Debug.Log("PID: " + pid + ", Name: " + ProcessName);
		}

		public override string ToString()
		{
			return string.Format("HWND:{0} Proc:{1} Title:{2} Class:{3}", hWnd, ProcessName, Title, ClassName);
		}
	}

	/// <summary>
	/// このウィンドウのハンドル
	/// </summary>
	private IntPtr hWnd = IntPtr.Zero;

	/// <summary>
	/// ウィンドウ操作ができる状態ならtrueを返す
	/// </summary>
	/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
	public bool IsActive { get{return (hWnd != IntPtr.Zero && WinApi.IsWindow(hWnd));} }

	/// <summary>
	/// ウィンドウが最大化されていればtrue
	/// </summary>
	/// <value><c>true</c> if this instance is maximized; otherwise, <c>false</c>.</value>
	public bool IsMaximized { get {return (IsActive && WinApi.IsZoomed(hWnd));}}

	/// <summary>
	/// ウィンドウが最小化されていればtrue
	/// </summary>
	/// <value><c>true</c> if this instance is minimized; otherwise, <c>false</c>.</value>
	public bool IsMinimized { get {return (IsActive && WinApi.IsIconic(hWnd));}}

	/// <summary>
	/// ウィンドウ透過時に最前面に表示するかどうか
	/// </summary>
	public bool IsTopmost { get { return (IsActive && _isTopmost); } }
	private bool _isTopmost = false;

	/// <summary>
	/// ウィンドウ位置
	/// </summary>
	public Vector2 OriginalWindowPosition;

	/// <summary>
	/// 標準ウィンドウサイズの指定
	/// </summary>
	public Vector2 OriginalWindowSize; 

	/// <summary>
	/// 元のウィンドウスタイル
	/// </summary>
	private long OriginalWindowStyle;

	/// <summary>
	/// Original extended window style
	/// </summary>
	private long OriginalWindowExStyle;


	/// <summary>
	/// 現在のウィンドウスタイル
	/// </summary>
	private long CurrentWindowStyle;
	/// <summary>
	/// Current extended window style
	/// </summary>
	private long CurrentWindowExStyle;


	/// <summary>
	/// ウィンドウプロシージャを変更済みか否か
	/// </summary>
	private bool IsHookSet = false;

	/// <summary>
	/// ファイルドロップを受け付ける状態か
	/// </summary>
	public bool enableFileDrop { get { return IsHookSet; } }

	/// <summary>
	/// ファイルドロップのフックからイベントを呼び出すためインスタンス一覧
	///		hWndをキーとしたDictionaryでは複数に対応できないため、単純なListにして毎回検索
	/// </summary>
	private static List<UniWinApi> instances = new List<UniWinApi>();


	/// <summary>
	/// ウィンドウ制御のコンストラクタ
	/// </summary>
	public UniWinApi() {
	}

	/// <summary>
	/// 指定されたウィンドウに関連付けたコンストラクタ
	/// </summary>
	/// <param name="window"></param>
	public UniWinApi(WindowHandle window)
	{
		SetWindow(window);
	}

	/// <summary>
	/// デストラクタ
	/// </summary>
	~UniWinApi() {
		Dispose();
	}

	/// <summary>
	/// Stores the size and position of the window.
	/// </summary>
	private void StoreWindowSize() {
		// 今の状態を記憶
		if (IsActive && !WinApi.IsZoomed(hWnd) && !WinApi.IsIconic(hWnd)) {
			WinApi.RECT rect = new WinApi.RECT();
			
			// ウィンドウ位置とサイズ
			WinApi.GetWindowRect(hWnd, out rect);
			this.OriginalWindowPosition = new Vector2(rect.left, rect.top);
			this.OriginalWindowSize = new Vector2(rect.right - rect.left, rect.bottom - rect.top);
		}
	}

	/// <summary>
	/// Set the window z-order (Topmost or not).
	/// </summary>
	/// <param name="isTopmost">If set to <c>true</c> is top.</param>
	public void EnableTopmost(bool isTopmost)
	{
		if (!IsActive) return;
		WinApi.SetWindowPos (
			hWnd,
			(isTopmost ? WinApi.HWND_TOPMOST : WinApi.HWND_NOTOPMOST),
			0, 0, 0, 0,
			WinApi.SWP_NOSIZE | WinApi.SWP_NOMOVE
			| WinApi.SWP_FRAMECHANGED | WinApi.SWP_NOOWNERZORDER
			| WinApi.SWP_NOACTIVATE | WinApi.SWP_ASYNCWINDOWPOS
			);
		this._isTopmost = isTopmost;
	}

	/// <summary>
	/// Set the window size.
	/// </summary>
	/// <param name="size">Size.</param>
	public void SetSize(Vector2 size)
	{
		if (!IsActive) return;
		WinApi.SetWindowPos (
			hWnd,
			IntPtr.Zero,
			0, 0, (int)size.x, (int)size.y,
			WinApi.SWP_NOMOVE | WinApi.SWP_NOZORDER
			| WinApi.SWP_FRAMECHANGED | WinApi.SWP_NOOWNERZORDER
			| WinApi.SWP_NOACTIVATE | WinApi.SWP_ASYNCWINDOWPOS
			);
	}

	/// <summary>
	/// Set the window position.
	/// </summary>
	/// <param name="position">Position.</param>
	public void SetPosition(Vector2 position)
	{
		if (!IsActive) return;
		WinApi.SetWindowPos (
			hWnd,
			IntPtr.Zero,
			(int)position.x, (int)position.y, 0, 0,
			WinApi.SWP_NOSIZE | WinApi.SWP_NOZORDER
			| WinApi.SWP_FRAMECHANGED | WinApi.SWP_NOOWNERZORDER
			| WinApi.SWP_NOACTIVATE | WinApi.SWP_ASYNCWINDOWPOS
			);
	}

	/// <summary>
	/// Get the window size.
	/// </summary>
	/// <returns>The size.</returns>
	public Vector2 GetSize()
	{
		if (!IsActive) return Vector2.zero;
		WinApi.RECT rect = new WinApi.RECT();
		WinApi.GetWindowRect(hWnd, out rect);
		return new Vector2(rect.right - rect.left, rect.bottom - rect.top);
	}

	/// <summary>
	/// Get the window position.
	/// </summary>
	/// <returns>The position.</returns>
	public Vector2 GetPosition()
	{
		if (!IsActive) return Vector2.zero;
		WinApi.RECT rect = new WinApi.RECT();
		WinApi.GetWindowRect(hWnd, out rect);
		return new Vector2(rect.left, rect.top);
	}

	/// <summary>
	/// 現在のウィンドウスタイルを記憶
	/// </summary>
	private void MemorizeWindowState()
	{
		StoreWindowSize();
		this.OriginalWindowStyle = WinApi.GetWindowLong(hWnd, WinApi.GWL_STYLE);
		this.OriginalWindowExStyle = WinApi.GetWindowLong(hWnd, WinApi.GWL_EXSTYLE);

		this.CurrentWindowStyle = this.OriginalWindowStyle;
		this.CurrentWindowExStyle = this.OriginalWindowExStyle;
	}

	/// <summary>
	/// ウィンドウスタイルを最初のものに戻す
	/// </summary>
	private void RestoreWindowState()
	{
		WinApi.SetWindowLong(hWnd, WinApi.GWL_STYLE, this.OriginalWindowStyle);
		WinApi.SetWindowLong(hWnd, WinApi.GWL_EXSTYLE, this.OriginalWindowExStyle);
		WinApi.ShowWindow(hWnd, WinApi.SW_SHOW);
	}

	/// <summary>
	/// ウィンドウハンドルを指定してウィンドウを選択
	/// </summary>
	/// <param name="hWnd"></param>
	/// <returns></returns>
	public void SetWindow(WindowHandle window)
	{
		Dispose();

		if (window == null) {
			hWnd = IntPtr.Zero;
			return;
		}
		hWnd = window.hWnd;

		MemorizeWindowState();
	}

	/// <summary>
	/// アクティブウィンドウのハンドルを取得
	/// </summary>
	/// <returns><c>true</c>, if window handle was set, <c>false</c> otherwise.</returns>
	static public WindowHandle FindWindow()
	{
		// 自分自身のPIDを取得し、スレッドを取得
		System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
		//return new WindowHandle(process.MainWindowHandle);	// ←これではダメだった。MainWindowHandle == 0 となった。

		int pid = process.Id;
		//Debug.Log("PID: " + pid);

		// 現存するウィンドウ一式を取得
		WindowHandle[] handles = FindWindows();
		foreach (WindowHandle window in handles)
		{
			// PIDが一致するものを検索
			if (window.ProcessId == pid)
			{
				return window;
			}
		}
		return null;
	}

	/// <summary>
	/// ウィンドウタイトルを元にハンドルを取得
	/// </summary>
	/// <param name="title">Title.</param>
	static public WindowHandle FindWindowByTitle(string title)
	{
		IntPtr hwnd = WinApi.FindWindow(null, title);
		if (hwnd == IntPtr.Zero) return null;

		WindowHandle window = new WindowHandle(hwnd);
		return window;
	}

	/// <summary>
	/// ウィンドウクラスを元にハンドルを取得
	/// </summary>
	/// <returns><c>true</c>, if handle by title was found, <c>false</c> otherwise.</returns>
	/// <param name="classname">Title.</param>
	static public WindowHandle FindWindowByClass(string classname)
	{
		IntPtr hwnd = WinApi.FindWindow(classname, null);
		if (hwnd == IntPtr.Zero) return null;

		WindowHandle window = new WindowHandle();
		return window;
	}

	/// <summary>
	/// ウィンドウタイトルを元にハンドルを取得
	/// </summary>
	/// <returns><c>true</c>, if handle by title was found, <c>false</c> otherwise.</returns>
	/// <param name="title">Title.</param>
	static public WindowHandle[] FindWindows()
	{
		List<WindowHandle> windowList = new List<WindowHandle>();

		WinApi.EnumWindows(new WinApi.EnumWindowsDelegate(delegate (IntPtr hWnd, long lParam)
		{
			StringBuilder sb = new StringBuilder(1024);
			if (WinApi.IsWindow(hWnd) && WinApi.GetWindowText(hWnd, sb, sb.Capacity) != 0)
			{
				WindowHandle window = new WindowHandle(hWnd);
				window.Title = sb.ToString();
				windowList.Add(window);
			}
			return 1;	// 列挙を継続するため0以外を返す
		}), 0);

		return windowList.ToArray();
	}

	/// <summary>
	/// 現在のアクティブウインドウが自分自身ならばtrueを返す
	/// </summary>
	/// <returns></returns>
	public bool CheckActiveWindow()
	{
		IntPtr hwnd = WinApi.GetActiveWindow();
		return (hwnd == hWnd);
	}

	/// <summary>
	/// ウィンドウスタイルを監視して、替わっていれば戻す
	/// </summary>
	public void Update() {
		if (!IsActive) return;
		long style = WinApi.GetWindowLong(hWnd, WinApi.GWL_STYLE);
		long exstyle = WinApi.GetWindowLong(hWnd, WinApi.GWL_EXSTYLE);
		if (!WinApi.IsIconic(hWnd) && !WinApi.IsZoomed(hWnd)) {
			if (style != this.CurrentWindowStyle) {
				WinApi.SetWindowLong (hWnd, WinApi.GWL_STYLE, this.CurrentWindowStyle);
				WinApi.ShowWindow(hWnd, WinApi.SW_SHOW);
			}
			if (exstyle != this.CurrentWindowExStyle)
			{
				WinApi.SetWindowLong(hWnd, WinApi.GWL_EXSTYLE, this.CurrentWindowExStyle);
				WinApi.ShowWindow(hWnd, WinApi.SW_SHOW);
			}
		}
	}

	/// <summary>
	/// ウィンドウ透過をON/OFF
	/// </summary>
	public void EnableTransparent(bool enable) {
		if (!IsActive) return;

		if (enable) {
			// 現在のウィンドウ情報を記憶
			StoreWindowSize();

			// 全面をGlassにする
			DwmApi.DwmExtendIntoClientAll (hWnd);

			// 枠無しウィンドウにする
			EnableBorderless(true);

			// ウィンドウ再描画
			WinApi.ShowWindow(hWnd, WinApi.SW_SHOW);
			SetSize(GetSize());
		}
		else {
			// ウィンドウスタイルを戻す
			EnableBorderless(false);

			// 操作の透過をやめる
			EnableClickThrough(false);

			// 枠のみGlassにする
			//	※ 本来のウィンドウが枠のみで無かった場合は残念ながら表示が戻りません
			DwmApi.MARGINS margins = new DwmApi.MARGINS (0, 0, 0, 0);
			DwmApi.DwmExtendFrameIntoClientArea (hWnd, margins);

			// サイズ変更イベントを発生させる
			SetSize(GetSize());
		}

		// ウィンドウ再描画
		WinApi.ShowWindow(hWnd, WinApi.SW_SHOW);
	}

	/// <summary>
	/// ウィンドウの枠を消去/戻す
	/// </summary>
	/// <description></description>
	/// <param name="enable">trueだと枠無し。falseだと標準</param>
	public void EnableBorderless(bool enable) {
		if (!IsActive) return;

#if UNITY_EDITOR
// エディタの場合は枠無しにしない
#else
		// エディタでなければ枠無しに設定
		if (enable) {
			// 枠無しウィンドウにする
			//long val = WinApi.GetWindowLong (hWnd, WinApi.GWL_STYLE) & ~WinApi.WS_OVERLAPPEDWINDOW;
			long val = WinApi.WS_VISIBLE | WinApi.WS_POPUP;
			//this.CurrentWindowStyle = this.OriginalWindowStyle & ~WinApi.WS_OVERLAPPEDWINDOW;
			this.CurrentWindowStyle = val;
			WinApi.SetWindowLong (hWnd, WinApi.GWL_STYLE, this.CurrentWindowStyle);
		} else {
			// ウィンドウスタイルを戻す
			this.CurrentWindowStyle = this.OriginalWindowStyle;
			WinApi.SetWindowLong (hWnd, WinApi.GWL_STYLE, this.CurrentWindowStyle);
		}
#endif
	}

	/// <summary>
	/// Extended window style で操作の透過/戻す
	/// </summary>
	/// <param name="isClickThrough">If set to <c>true</c> is top.</param>
	public void EnableClickThrough(bool isClickThrough)
	{
		if (!IsActive) return;

#if UNITY_EDITOR
		// エディタの場合は操作の透過はやめておく
#else
		// エディタでなければ操作を透過
		if (isClickThrough)
		{
			long exstyle = this.CurrentWindowExStyle;
			exstyle |= WinApi.WS_EX_TRANSPARENT;
			exstyle |= WinApi.WS_EX_LAYERED;
			this.CurrentWindowExStyle = exstyle;
			WinApi.SetWindowLong(hWnd, WinApi.GWL_EXSTYLE, this.CurrentWindowExStyle);
		}
		else
		{
			this.CurrentWindowExStyle = this.OriginalWindowExStyle;
			if (enableFileDrop) this.CurrentWindowExStyle |= WinApi.WS_EX_ACCEPTFILES;
			WinApi.SetWindowLong(hWnd, WinApi.GWL_EXSTYLE, this.CurrentWindowExStyle);
		}
#endif
	}

	/// <summary>
	/// ウィンドウ最大化
	/// </summary>
	public void Maximize() {
		if (!IsActive) return;
		this.CurrentWindowStyle = WinApi.GetWindowLong(hWnd, WinApi.GWL_STYLE);
		WinApi.ShowWindow(hWnd, WinApi.SW_MAXIMIZE);
	}

	/// <summary>
	/// ウィンドウ最小化
	/// </summary>
	public void Minimize() {
		if (!IsActive) return;
		this.CurrentWindowStyle = WinApi.GetWindowLong(hWnd, WinApi.GWL_STYLE);
		WinApi.ShowWindow(hWnd, WinApi.SW_MINIMIZE);
	}
	
	/// <summary>
	/// 最大化または最小化したウィンドウを元に戻す
	/// </summary>
	public void Restore() {
		if (!IsActive) return;
		WinApi.ShowWindow(hWnd, WinApi.SW_RESTORE);
		this.CurrentWindowStyle = WinApi.GetWindowLong(hWnd, WinApi.GWL_STYLE);
	}

	/// <summary>
	/// Get the Unity executable file name
	/// </summary>
	/// <description>http://gamedev.stackexchange.com/questions/68784/how-do-i-access-the-product-name-in-unity-4</description>
	/// <returns>The project name.</returns>
	[Obsolete]
	public static string GetUnityProcessName() {
		string[] fileOrFolders = Application.dataPath.Split('/');
		string file =  fileOrFolders[fileOrFolders.Length - 1];
		if (file.Substring(file.Length - 4).ToLower() == ".exe")
		{
			file = file.Substring(0, file.Length - 4);
		}
		return file;
	}

#region マウス操作関連
	/// <summary>
	/// マウスカーソルを指定座標へ移動させる
	/// </summary>
	/// <param name="screenPosition">スクリーン上の絶対座標。（Unityのウィンドウが基準では無い）</param>
	static public void SetCursorPosition(Vector2 screenPosition) {
		WinApi.SetCursorPos((int)screenPosition.x, (int)screenPosition.y);
	}

	/// <summary>
	/// マウスカーソル座標を取得
	/// </summary>
	/// <returns>スクリーン上の絶対座標。（Unityのウィンドウが基準では無い）</returns>
	static public Vector2 GetCursorPosition() {
		WinApi.POINT point;
		WinApi.GetCursorPos(out point);
		return new Vector2(point.x, point.y);
	}

	/// <summary>
	/// マウスの左ボタンが離されたイベントを発生させます
	/// </summary>
	static public void SendMouseUp() {
		SendMouseUp(0);
	}

	/// <summary>
	/// マウスのボタンが離されたイベントを発生させます
	/// </summary>
	/// <param name="button">0:左, 1:右, 2:中</param>
	static public void SendMouseUp(int button) {
		WinApi.mouse_event(
			button == 1 ? WinApi.MOUSEEVENTF_RIGHTUP
			: button == 2 ? WinApi.MOUSEEVENTF_MIDDLEUP
			: WinApi.MOUSEEVENTF_LEFTUP,
			0, 0, 0, IntPtr.Zero
			);
	}

	/// <summary>
	/// マウスの左ボタンが押されたイベントを発生させます
	/// </summary>
	static public void SendMouseDown() {
		SendMouseDown(0);
	}
	
	/// <summary>
	/// マウスのボタンが押されたイベントを発生させます
	/// </summary>
	/// <param name="button">0:左, 1:右, 2:中</param>
	static public void SendMouseDown(int button) {
		WinApi.mouse_event(
			button == 1 ? WinApi.MOUSEEVENTF_RIGHTDOWN
			: button == 2 ? WinApi.MOUSEEVENTF_MIDDLEDOWN
			: WinApi.MOUSEEVENTF_LEFTDOWN,
			0, 0, 0, IntPtr.Zero
			);
	}
#endregion

#region キー操作関連
	/// <summary>
	/// キーコードを送ります
	/// </summary>
	public void SendKey(KeyCode code)
	{
		WinApi.PostMessage(this.hWnd, WinApi.WM_IME_CHAR, (long)code, IntPtr.Zero);
	}
#endregion

#region ファイルドロップ関連
	/// <summary>
	/// ファイルドロップ時に発生するイベント
	/// </summary>
	public event FilesDropped OnFilesDropped;
	public delegate void FilesDropped(string[] files);

	private WinApi.HookProc myHookCallback;
	private IntPtr myHook = IntPtr.Zero;

	/// <summary>
	/// メッセージのコールバック
	/// </summary>
	/// <param name="nCode"></param>
	/// <param name="wParam"></param>
	/// <param name="lParam"></param>
	/// <returns></returns>
	[MonoPInvokeCallback(typeof(WinApi.HookProc))]
	private static IntPtr MessageCallback(int nCode, IntPtr wParam, ref WinApi.MSG lParam)
	{
		if (nCode == 0 && lParam.message == WinApi.WM_DROPFILES)
		{
			IntPtr hDrop = lParam.wParam;
			uint num = WinApi.DragQueryFile(hDrop, 0xFFFFFFFF, null, 0);
			string[] files = new string[num];

			uint bufferSize = 1024;
			StringBuilder path = new StringBuilder((int)bufferSize);
			for (uint i = 0; i < num; i++)
			{
				//uint size = WinApi.DragQueryFile(hDrop, i, path, bufferSize);
				WinApi.DragQueryFile(hDrop, i, path, bufferSize);
				files[i] = path.ToString();
				path.Length = 0;
			}

			WinApi.DragFinish(hDrop);

			IntPtr hwnd = lParam.hwnd;
			if (hwnd != IntPtr.Zero)
			{
				int instanceCount = 0;

				// 存在するインスタンス内を検索
				foreach (UniWinApi uniwin in instances)
				{
					// 該当するウィンドウであった場合
					if (uniwin.hWnd == hwnd)
					{
						// ファイルドロップ時のイベントを実行
						if (uniwin.OnFilesDropped != null)
						{
							uniwin.OnFilesDropped(files);
						}
						instanceCount++;
					}
				}

				// もしインスタンスが見つからなければおかしいのでログ出力
				if (instanceCount < 1)
				{
					Debug.Log("File dropped, but no UniWinApi instances were found.");
				}
			}
		}
		return WinApi.CallNextHookEx(IntPtr.Zero, nCode, wParam, ref lParam);
	}

	/// <summary>
	/// ファイルドロップの扱いを開始
	/// </summary>
	public void BeginFileDrop()
	{
		if (!IsActive)
		{
			EndFileDrop();
			return;
		}

		if (!instances.Contains(this)) {
			instances.Add(this);
		}
		BeginHook();

		// ドロップを受け付ける状態にする
		long exstyle = this.CurrentWindowExStyle;
		exstyle |= WinApi.WS_EX_ACCEPTFILES;
		this.CurrentWindowExStyle = exstyle;
		WinApi.SetWindowLong(hWnd, WinApi.GWL_EXSTYLE, this.CurrentWindowExStyle);
	}

	/// <summary>
	/// ファイルドロップの扱いを終了
	/// </summary>
	public void EndFileDrop()
	{
		EndHook();
		instances.Remove(this);

		if (!IsActive) return;

		// ドロップを受け付ける状態を元に戻す
		long exstyle = this.CurrentWindowExStyle;
		exstyle &= ~WinApi.WS_EX_ACCEPTFILES;
		exstyle |= (WinApi.WS_EX_ACCEPTFILES & this.OriginalWindowExStyle);	// 元からドロップ許可ならやはり受付
		this.CurrentWindowExStyle = exstyle;
		WinApi.SetWindowLong(hWnd, WinApi.GWL_EXSTYLE, this.CurrentWindowExStyle);
	}

	/// <summary>
	/// メッセージフックを開始
	/// </summary>
	private void BeginHook()
	{
		// ウィンドウが操作不可なら設定はできない
		if (!IsActive)
		{
			EndHook();	// フック設定なしということにしておく
			return;
		}

		// フック設定済みなら二重には設定しない
		if (IsHookSet) return;

		// フックを設定
		uint threadId = WinApi.GetCurrentThreadId();
		IntPtr module = WinApi.GetModuleHandle(null);
		myHookCallback = new WinApi.HookProc(MessageCallback);
		myHook = WinApi.SetWindowsHookEx(WinApi.WH_GETMESSAGE, myHookCallback, module, threadId);

		// フック設定失敗時
		if (myHook == IntPtr.Zero)
		{
			Debug.Log("SetWindowsHookEx:" + WinApi.GetLastError());
			return;
		}

		IsHookSet = true;
		//Debug.Log("BeginHook");
	}

	/// <summary>
	/// メッセージフックを終了
	/// </summary>
	private void EndHook()
	{
		if (myHook != IntPtr.Zero)
		{
			WinApi.UnhookWindowsHookEx(myHook);
			myHook = IntPtr.Zero;
			myHookCallback = null;

			//Debug.Log("EndHook");
		}
		IsHookSet = false;
	}

	/// <summary>
	/// 破棄時の処理
	/// </summary>
	public void Dispose()
	{
		if (IsActive)
		{
			EndFileDrop();
#if UNITY_EDITOR
			Reset();
#endif
		}

		hWnd = IntPtr.Zero;
	}

	/// <summary>
	/// ウィンドウ状態を最初に戻す
	/// </summary>
	public void Reset()
	{
		EnableTransparent(false);
		EnableTopmost(false);
		Restore();
		RestoreWindowState();
	}

	#endregion
}
