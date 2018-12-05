/**
 * Windows API wrapper
 * 
 * License: CC0, https://creativecommons.org/publicdomain/zero/1.0/
 * 
 * Author: Kirurobo, http://twitter.com/kirurobo
 * Author: Ru--en, http://twitter.com/ru__en
 */
using System;
using System.Runtime.InteropServices;
using System.Text;

public class WinApi {
	
	#region Windows API
	public static readonly int GWL_STYLE = -16;

	public static readonly int SW_HIDE		= 0;
	public static readonly int SW_MAXIMIZE	= 3;
	public static readonly int SW_MINIMIZE	= 6;
	public static readonly int SW_RESTORE	= 9;
	public static readonly int SW_SHOW		= 5;

	public static readonly uint SWP_REFRESH         = 0x237;
	public static readonly uint SWP_NOSIZE	        = 0x1;
	public static readonly uint SWP_NOMOVE	        = 0x2;
	public static readonly uint SWP_NOZORDER	    = 0x4;
	public static readonly uint SWP_NOACTIVATE	    = 0x10;
	public static readonly uint SWP_FRAMECHANGED	= 0x20;
	public static readonly uint SWP_SHOWWINDOW	    = 0x40;
	public static readonly uint SWP_NOCOPYBITS	    = 0x100;
	public static readonly uint SWP_NOOWNERZORDER	= 0x200;
	public static readonly uint SWP_NOREPOSITION	= 0x200;
	public static readonly uint SWP_NOSENDCHANGING	= 0x400;
	public static readonly uint SWP_ASYNCWINDOWPOS	= 0x4000;

	public static readonly long WS_BORDER		= 0x00800000L;
	public static readonly long WS_VISIBLE		= 0x10000000L;
	public static readonly long WS_OVERLAPPED	= 0x00000000L;
	public static readonly long WS_CAPTION		= 0x00C00000L;
	public static readonly long WS_SYSMENU		= 0x00080000L;
	public static readonly long WS_THICKFRAME	= 0x00040000L;
	public static readonly long WS_ICONIC		= 0x20000000L;
	public static readonly long WS_MINIMIZE		= 0x20000000L;
	public static readonly long WS_MAXIMIZE		= 0x01000000L;
	public static readonly long WS_MINIMIZEBOX	= 0x00020000L;
	public static readonly long WS_MAXIMIZEBOX	= 0x00010000L;
	public static readonly long WS_POPUP			= 0x80000000L;
	public static readonly long WS_OVERLAPPEDWINDOW = 0x00CF0000L;

	public static readonly long WS_EX_TRANSPARENT        = 0x00000020L;
	public static readonly long WS_EX_LAYERED            = 0x00080000L;
	public static readonly long WS_EX_TOPMOST            = 0x00000008L;
	public static readonly long WS_EX_OVERLAPPEDWINDOW   = 0x00000300L;
	public static readonly long WS_EX_ACCEPTFILES        = 0x00000010L;

	public static readonly IntPtr HWND_TOP		= new IntPtr(0);
	public static readonly IntPtr HWND_BOTTOM	= new IntPtr(1);
	public static readonly IntPtr HWND_TOPMOST 	= new IntPtr(-1);
	public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

	public static readonly uint GA_PARENT	= 1;
	public static readonly uint GA_ROOT		= 2;
	public static readonly uint GA_ROOTOWNER = 3;
	public static readonly uint GW_HWNDFIRST	= 0;
	public static readonly uint GW_HWNDLAST	= 1;
	public static readonly uint GW_HWNDNEXT	= 2;
	public static readonly uint GW_HWNDPREV	= 3;
	public static readonly uint GW_OWNER		= 4;
	public static readonly uint GW_CHILD		= 5;

	public static readonly uint WM_IME_CHAR = 0x0286;
	public static readonly uint WM_SETTEXT = 0x000C;
    public static readonly uint WM_NCDESTROY = 0x082;
    public static readonly uint WM_WINDOWPOSCHANGING = 0x046;
    public static readonly uint WM_DROPFILES = 0x233;

    public delegate int EnumWindowsDelegate (IntPtr hWnd, long lParam);


	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct RECT
	{
		public int left, top, right, bottom;
		
		public RECT (int left, int top, int right, int bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}
	}

	[DllImport("user32.dll")]
	public static extern int EnumWindows (EnumWindowsDelegate lpEnumFunc, long lParam);

    [DllImport("user32.dll")]
	public static extern bool IsWindow (IntPtr hWnd);
	
	[DllImport("user32.dll")]
	public static extern int IsWindowVisible (IntPtr hWnd);
	
	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern int GetWindowText (IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
	
	[DllImport("user32.dll")]
	public static extern uint GetWindowThreadProcessId (IntPtr hWnd, out int lpdwProcessId);
	
	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow (string lpszClass, string lpszTitle);

	[DllImport("user32.dll")]
	public static extern long GetWindowRect (IntPtr hWnd, out RECT rect);

	[DllImport("user32.dll")]
	public static extern long GetClientRect (IntPtr hWnd, out RECT rect);

	[DllImport("user32.dll")]
	public static extern bool ShowWindow (IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll")]
	public static extern bool EnableWindow (IntPtr hWnd, bool bEnable);

	[DllImport("user32.dll")]
	public static extern bool IsIconic (IntPtr hWnd);
	
	[DllImport("user32.dll")]
	public static extern bool IsZoomed (IntPtr hWnd);
	
	[DllImport("user32.dll")]
	public static extern long SetWindowLong (IntPtr hWnd, int nIndex, long value);

	[DllImport("user32.dll")]
	public static extern long GetWindowLong(IntPtr hWnd, int nIndex);
	
	[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
	public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

	[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
	public static extern int SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewPtr);

	[DllImport("user32.dll")]
	public static extern bool SetWindowPos (IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
	
	[DllImport("user32.dll")]
	public static extern IntPtr GetActiveWindow ();
	
	[DllImport("user32.dll")]
	public static extern IntPtr GetAncestor (IntPtr hWnd, uint gaFlags);

	[DllImport("user32.dll")]
	public static extern bool PostMessage(IntPtr hWnd, uint msg, long wParam, IntPtr lParam);

#endregion

#region for mouse events
	public static readonly ulong MOUSEEVENTF_ABSOLUTE	= 0x8000;
	public static readonly ulong MOUSEEVENTF_LEFTDOWN	= 0x0002;
	public static readonly ulong MOUSEEVENTF_LEFTUP		= 0x0004;
	public static readonly ulong MOUSEEVENTF_MIDDLEDOWN	= 0x0020;
	public static readonly ulong MOUSEEVENTF_MIDDLEUP	= 0x0040;
	public static readonly ulong MOUSEEVENTF_MOVE		= 0x0001;
	public static readonly ulong MOUSEEVENTF_RIGHTDOWN	= 0x0008;
	public static readonly ulong MOUSEEVENTF_RIGHTUP	= 0x0010;
	public static readonly ulong MOUSEEVENTF_XDOWN		= 0x0080;
	public static readonly ulong MOUSEEVENTF_XUP		= 0x0100;
	public static readonly ulong MOUSEEVENTF_WHEEL		= 0x0800;
	public static readonly ulong MOUSEEVENTF_HWHEEL		= 0x1000;
	public static readonly ulong XBUTTON1	= 0x0001;
	public static readonly ulong XBUTTON2	= 0x0002;

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public struct POINT
	{
		public int x, y;
	}

	[DllImport("user32.dll")]
	public static extern bool GetCursorPos (out POINT point);

	[DllImport("user32.dll")]
	public static extern bool SetCursorPos (int x, int y);

	[DllImport("user32.dll")]
	public static extern uint mouse_event (ulong dwFlags, int dx, int dy, ulong dwData, IntPtr dwExtraInfo);

	public static readonly int GWL_EXSTYLE = -20;
	public static readonly int GWLP_HINSTANCE = -6;
	public static readonly int GWLP_ID = -12;
	public static readonly int GWLP_STYLE = -16;
	public static readonly int GWLP_USERDATA = -21;
	public static readonly int GWLP_WNDPROC = -4;
	
	[DllImport("user32.dll")]
	public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll")]
	public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

#endregion


#region for shell functions

	[DllImport("shell32.dll")]
	public static extern void DragAcceptFiles(IntPtr hWnd, bool bAccept);

	[DllImport("shell32.dll", CharSet=CharSet.Unicode)]
	public static extern uint DragQueryFile(IntPtr hDrop, uint iFIle, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpszFile, uint cch);

	[DllImport("shell32.dll")]
	public static extern void DragFinish(IntPtr hDrop);

	/// <summary>
	/// Set window procedure.
	/// This method is not implemented in user32.dll
	/// </summary>
	/// <returns>Previous window procedure</returns>
	public static IntPtr SetWindowProcedure(IntPtr hWnd, IntPtr wndProcPtr)
	{
		// Reference https://qiita.com/DandyMania/items/d1404c313f67576d395f

		if (IntPtr.Size == 8)
		{
			// 64bit
			return SetWindowLongPtr(hWnd, GWLP_WNDPROC, wndProcPtr);
		}
		else
		{
			// 32bit
			return new IntPtr(SetWindowLongPtr32(hWnd, GWLP_WNDPROC, wndProcPtr.ToInt32()));
		}
	}
    #endregion

    #region Hooks
    public static readonly int WH_CALLWNDPROC = 4;
    public static readonly int WH_CALLWNDPROCRET = 12;
    public static readonly int WH_CBT = 5;
    public static readonly int WH_DEBUG = 9;
    public static readonly int WH_FOREGROUNDIDLE = 11;
    public static readonly int WH_GETMESSAGE = 3;
    public static readonly int WH_JOURNALPLAYBACK = 1;
    public static readonly int WH_JOURNALRECORD = 0;
    public static readonly int WH_KEYBOARD = 2;
    public static readonly int WH_KEYBOARD_LL = 13;
    public static readonly int WH_MOUSE = 7;
    public static readonly int WH_MOUSE_LL = 14;
    public static readonly int WH_MSGFILTER = -1;
    public static readonly int WH_SHELL = 10;
    public static readonly int WH_SYSMSGFILTER = 6;

    [StructLayout(LayoutKind.Sequential)]
    public struct CWPSTRUCT
    {
        public long lParam;
        public long wParam;
        public uint message;
        public IntPtr hwnd;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public ushort time;
        public POINT pt;
    }

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hmod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref MSG lParam);

    [DllImport("kernel32.dll")]
    public static extern long GetLastError();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr HookProc(int code, IntPtr wParam, ref MSG lParam);

    #endregion
}
