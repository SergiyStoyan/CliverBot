//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        15 December 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Cliver
{
    public partial class Win32
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        public class MemoryProtection
        {
            public const int PAGE_READWRITE = 0x04;
            public const int PAGE_NOACCESS = 0x01;
            public const int PAGE_GUARD = 0x100;
        }

        public class MemoryState
        {
            public const int MEM_COMMIT = 0x00001000;
        }

        public class ProcessRights
        {
            public const int PROCESS_QUERY_INFORMATION = 0x0400;
            public const int PROCESS_WM_READ = 0x0010;
        }

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            ushort reserved;
            public uint pageSize;
            public UIntPtr minimumApplicationAddress;
            public UIntPtr maximumApplicationAddress;
            public UIntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        public struct MEMORY_BASIC_INFORMATION
        {
            public Int32 BaseAddress;
            public Int32 AllocationBase;
            public Int32 AllocationProtect;
            public Int32 RegionSize;
            public Int32 State;
            public Int32 Protect;
            public Int32 lType;
        }

        //public delegate bool EnumProc(IntPtr hwnd, int lParam);

        public delegate bool EnumProc(IntPtr hwnd, IntPtr lParam);
        //public delegate IntPtr HookProc(IntPtr nCode, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetForegroundWindow(IntPtr h);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern byte VkKeyScan(char c);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetLastError();

        //[DllImport("kernel32.dll", SetLastError = true)]
        //public static extern int FormatMessage(int   dwFlags,  _In_opt_ LPCVOID lpSource,  _In_ DWORD   dwMessageId,  _In_ DWORD   dwLanguageId,  _Out_ LPTSTR  lpBuffer,  _In_ DWORD   nSize,  _In_opt_ va_list *Arguments);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType hook, HookProc callback, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern IntPtr UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        //[DllImport("user32.dll")]
        //public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, CustomWindowProc dwNewLong);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        //[DllImport("user32.dll")] 
        //public static extern IntPtr SetClassLong(IntPtr hWnd, int nIndex, CustomWindowProc dwNewLong); 

        //[DllImport("user32.dll")] 
        //public static extern IntPtr DefWindowProc(IntPtr hWnd, uint Msg, IntPtr wParam,IntPtr lParam); 

        [DllImport("user32.dll")]
        public static extern IntPtr DefDlgProc(IntPtr hDlg, uint Msg, IntPtr wParam, IntPtr lParam);

        //[DllImport("User32.dll")]
        //public static extern UIntPtr SetTimer(IntPtr hwnd, UIntPtr nIDEvent, uint uElapse, CallBack cbf);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int EndDialog(IntPtr hDlg, IntPtr nResult);

        [DllImport("user32.dll")]
        public static extern int GetDlgItem(IntPtr hWnd, int itemId);

        [DllImport("user32.dll")]
        public static extern int GetDlgItemText(IntPtr hWnd, int itemId, StringBuilder text, int maxLength);

        [DllImport("user32.dll")]
        public static extern int SetWindowText(IntPtr hwnd, string str);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);

        [DllImport("user32.dll")]
        public static extern int InternalGetWindowText(IntPtr hwnd, StringBuilder text, int nMaxCount);

        [DllImport("User32.Dll")]
        public static extern void GetClassName(IntPtr hwnd, StringBuilder s, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);
        public enum GetWindowCmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetAncestor(IntPtr hWnd, GetAncestorCmd uCmd);
        public enum GetAncestorCmd : uint
        {
            GA_PARENT = 1,
            GA_ROOT = 2,
            GA_ROOTOWNER = 3
        }

        //[DllImport("user32.dll")]
        //public static extern int GetWindowLong(IntPtr hWnd, GetAncestorCmd uCmd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern int FindWindowEx(IntPtr parent_h, IntPtr child_h, string lpClassName, string lpWindowName);

        [DllImport("User32.dll", SetLastError = true, EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", SetLastError = true, EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

        [DllImport("User32.dll", SetLastError = true, EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("User32.dll", SetLastError = true, EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "PostMessage")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(int hWnd, uint Msg, int wParam, Int64 lParam);

        [DllImport("User32.dll", EntryPoint = "SetActiveWindow")]
        public static extern int SetActiveWindow(IntPtr hwnd);

        [DllImport("user32")]
        public static extern bool EnumWindows(EnumProc cbf, int lParam);
        //public static extern int EnumWindows(EnumProc cbf, int lParam);

        [DllImport("user32")]
        public static extern int EnumChildWindows(IntPtr hwnd, EnumProc cbf, int lParam);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

        [DllImport("User32.dll", EntryPoint = "EnumThreadWindows")]
        public static extern bool EnumThreadWindows(uint dwThreadId, EnumProc cbf, IntPtr lParam);

        [DllImport("Wininet.dll", SetLastError = true)]
        //public static extern bool GetUrlCacheEntryInfo(string Url, StringBuilder CacheFile, ref int Size);
        public static extern bool GetUrlCacheEntryInfo(string Url, IntPtr lpCacheEntryInfo, ref int Size);

        [DllImport("User32.dll", EntryPoint = "EnumThreadWindows")]
        public static extern bool EnumThreadWindows(uint dwThreadId, EnumProc cbf, int lParam);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(string url, string cookieName, StringBuilder cookieData, ref int size, Int32 dwFlags, IntPtr lpReserved);
        public const Int32 InternetCookieHttponly = 0x2000;

        [DllImport("wininet.dll")]
        public static extern bool InternetGetCookie(string Url, string CookieName, StringBuilder CookieData, ref int Size);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        public enum HookType : uint
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public struct CWPRETSTRUCT
        {
            public IntPtr lResult;
            public IntPtr lParam;
            public IntPtr wParam;
            public uint message;
            public IntPtr hwnd;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPSTRUCT
        {
            public IntPtr lparam;
            public IntPtr wparam;
            public int message;
            public IntPtr hwnd;
        }
    }
}