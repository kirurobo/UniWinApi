/**
 * Windows API wrapper
 * 
 * Author: Ru--en, http://twitter.com/ru__en
 */
using System;
using System.Runtime.InteropServices;

public class WinApi {
	
	#region Windows API
	public static int GWL_STYLE = -16;

	public static int SW_HIDE		= 0;
	public static int SW_MAXIMIZE	= 3;
	public static int SW_MINIMIZE	= 6;
	public static int SW_RESTORE	= 9;
	public static int SW_SHOW		= 5;

	public static uint SWP_Refresh_ = 0x237;
	public static uint SWP_NOSIZE	= 0x1;
	public static uint SWP_NOMOVE	= 0x2;
	public static uint SWP_NOZORDER	= 0x4;
	public static uint SWP_NOACTIVATE	= 0x10;
	public static uint SWP_FRAMECHANGED	= 0x20;
	public static uint SWP_SHOWWINDOW	= 0x40;
	public static uint SWP_NOOWNERZORDER	= 0x200;
	public static uint SWP_NOREPOSITION		= 0x200;
	public static uint SWP_NOSENDCHANGING	= 0x400;
	public static uint SWP_ASYNCWINDOWPOS	= 0x4000;

	public static long WS_BORDER		= 0x00800000L;
	public static long WS_VISIBLE		= 0x10000000L;
	public static long WS_OVERLAPPED	= 0x00000000L;
	public static long WS_CAPTION		= 0x00C00000L;
	public static long WS_SYSMENU		= 0x00080000L;
	public static long WS_THICKFRAME	= 0x00040000L;
	public static long WS_ICONIC		= 0x20000000L;
	public static long WS_MINIMIZE		= 0x20000000L;
	public static long WS_MAXIMIZE		= 0x01000000L;
	public static long WS_MINIMIZEBOX	= 0x00020000L;
	public static long WS_MAXIMIZEBOX	= 0x00010000L;
	public static long WS_POPUP			= 0x80000000L;
	public static long WS_OVERLAPPEDWINDOW = 0x00CF0000L;

	public static IntPtr HWND_TOP		= new IntPtr((int)0);
	public static IntPtr HWND_BOTTOM	= new IntPtr((int)1);
	public static IntPtr HWND_TOPMOST 	= new IntPtr((int)-1);
	public static IntPtr HWND_NOTOPMOST = new IntPtr((int)-2);

	public static uint GA_PARENT	= 1;
	public static uint GA_ROOT		= 2;
	public static uint GA_ROOTOWNER = 3;
	public static uint GW_HWNDFIRST	= 0;
	public static uint GW_HWNDLAST	= 1;
	public static uint GW_HWNDNEXT	= 2;
	public static uint GW_HWNDPREV	= 3;
	public static uint GW_OWNER		= 4;
	public static uint GW_CHILD		= 5;
	
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
	public static extern int IsWindowVisible (IntPtr hWnd);
	
	//	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	//	public static extern int GetWindowText (IntPtr hWnd, StringBuilder lpString, int nMaxCount);
	
	[DllImport("user32.dll")]
	public static extern uint GetWindowThreadProcessId (IntPtr hWnd, out long lpdwProcessId);
	
	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow (string lpszClass, string lpszTitle);
	
	[DllImport("user32.dll")]
	public static extern long GetWindowLong (IntPtr hWnd, int nIndex);
	
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
	public static extern bool SetWindowPos (IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
	
	[DllImport("user32.dll")]
	public static extern IntPtr GetActiveWindow ();
	
	[DllImport("user32.dll")]
	public static extern IntPtr GetAncestor (IntPtr hWnd, uint gaFlags);
	#endregion

	#region for mouse events
	public static ulong MOUSEEVENTF_ABSOLUTE	= 0x8000;
	public static ulong MOUSEEVENTF_LEFTDOWN	= 0x0002;
	public static ulong MOUSEEVENTF_LEFTUP		= 0x0004;
	public static ulong MOUSEEVENTF_MIDDLEDOWN	= 0x0020;
	public static ulong MOUSEEVENTF_MIDDLEUP	= 0x0040;
	public static ulong MOUSEEVENTF_MOVE		= 0x0001;
	public static ulong MOUSEEVENTF_RIGHTDOWN	= 0x0008;
	public static ulong MOUSEEVENTF_RIGHTUP		= 0x0010;
	public static ulong MOUSEEVENTF_XDOWN		= 0x0080;
	public static ulong MOUSEEVENTF_XUP			= 0x0100;
	public static ulong MOUSEEVENTF_WHEEL		= 0x0800;
	public static ulong MOUSEEVENTF_HWHEEL		= 0x1000;
	public static ulong XBUTTON1	= 0x0001;
	public static ulong XBUTTON2	= 0x0002;

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
	#endregion

}
