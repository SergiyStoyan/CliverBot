using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Win32;
using System.Windows.Forms;
using System.Collections;
using Cliver;

namespace Cliver.Bot
{    
    /// <summary>
    /// Intercept dialog box creation
    /// </summary>
    static internal class WindowInterceptor
    {
        static object static_lock_variable = new object();

        static IntPtr hook_id = IntPtr.Zero;
        static Win32.Functions.HookProc cbf = new Win32.Functions.HookProc(wnd_hook_proc);

        static IntPtr[] owner_windows = new IntPtr[0];
        static Dictionary<IntPtr, Cliver.Log.Thread> owner_window_logs = new Dictionary<IntPtr, Cliver.Log.Thread>();

        /// <summary>
        /// Add new owner window to be traced for dialog box creating
        /// </summary>
        /// <param name="Log">Log of owner window</param>
        /// <param name="owner_window">owner window</param>
        static public void AddOwnerWindow(IntPtr owner_window)
        {
            lock (static_lock_variable)
            {
                try
                {
                    if (owner_window_logs.ContainsKey(owner_window))
                        return;
                    ICollection a = (ICollection)owner_windows.Clone();
                    ArrayList al = new ArrayList((ICollection)a);
                    al.Add(owner_window);
                    owner_windows = (IntPtr[])al.ToArray(typeof(IntPtr));

                    owner_window_logs[owner_window] = Log.This;

                    if (hook_id == IntPtr.Zero)
                        hook_id = Win32.Functions.SetWindowsHookEx(Win32.HookType.WH_CALLWNDPROCRET, cbf, IntPtr.Zero, Win32.Functions.GetCurrentThreadId());
                }
                catch (Exception e)
                {
                    Log.Main.Exit(e);
                }
            }
        }

        /// <summary>
        /// Remove owner window from hook tracing
        /// </summary>
        /// <param name="owner_window">owner window</param>
        static public void RemoveOwnerWindow(IntPtr owner_window)
        {
            lock (static_lock_variable)
            {
                try
                {
                    ICollection a = (ICollection)owner_windows.Clone();
                    ArrayList al = new ArrayList((ICollection)a);
                    al.Remove(owner_window);
                    owner_windows = (IntPtr[])al.ToArray(typeof(IntPtr));

                    owner_window_logs.Remove(owner_window);

                    if (owner_windows.Length < 1 && hook_id != IntPtr.Zero)
                    {
                        Win32.Functions.UnhookWindowsHookEx(hook_id);
                        hook_id = IntPtr.Zero;
                    }
                }
                catch (Exception e)
                {
                    Log.Main.Exit(e);
                }
            }
        }

        // <summary>
        // Start dialog box interception for the specified owner window
        // </summary>
        // <param name="Log">Log of owner window</param>
        // <param name="owner_window">owner window</param>
        //static public void Start(Log Log, IntPtr owner_window)
        //{
        //    lock (static_lock_variable)
        //    {
        //        try
        //        {
        //            Stop();
        //            AddOwnerWindow(Log, owner_window);

        //            if (hook_id == IntPtr.Zero)
        //                hook_id = Win32.Functions.SetWindowsHookEx(Win32.HookType.WH_CALLWNDPROCRET, cbf, IntPtr.Zero, Win32.Functions.GetCurrentThreadId());
        //        }
        //        catch (Exception e)
        //        {
        //            Log.Main.Exit(e);
        //        }
        //    }
        //}

        /// <summary>
        /// Stop dialog box interception
        /// </summary>
        static public void Stop()
        {
            lock (static_lock_variable)
            {
                try
                {
                    owner_windows = new IntPtr[0];
                    owner_window_logs.Clear();

                    if (hook_id != IntPtr.Zero)
                    {
                        Win32.Functions.UnhookWindowsHookEx(hook_id);
                        hook_id = IntPtr.Zero;
                    }
                }
                catch (Exception e)
                {
                    Log.Main.Exit(e);
                }
            }
        }

        static private IntPtr wnd_hook_proc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            lock (static_lock_variable)
            {
                try
                {
                    if (nCode < 0)
                        return Win32.Functions.CallNextHookEx(hook_id, nCode, wParam, lParam);

                    Win32.CWPRETSTRUCT msg = (Win32.CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32.CWPRETSTRUCT));

                    if (msg.message == (uint)Win32.Messages.WM_SHOWWINDOW)
                    {
                        //check if owner is that was specified
                        IntPtr h = new IntPtr(Win32.Functions.GetWindow(msg.hwnd, Win32.Functions.GW_OWNER));
                        foreach (IntPtr owner_window in owner_windows)
                        {
                            if (owner_window != h)
                            {
                                StringBuilder text2 = new StringBuilder(255);
                                Win32.Functions.GetWindowText(h, text2, 255);
                                if (!text2.ToString().Contains("WindowsFormsParkingWindow"))
                                    continue;
                            }

                            StringBuilder text = new StringBuilder(255);
                            Win32.Functions.GetWindowText(msg.hwnd, text, 255);
                            owner_window_logs[owner_window].Write("Intercepted dialog box: " + text.ToString());

                            //short dw = (short)Win32.Functions.SendMessage(msg.hwnd, (uint)Win32.Messages.DM_GETDEFID, 0, 0);
                            //Win32.Functions.EndDialog(msg.hwnd, (IntPtr)dw);
                            Win32.Functions.SendMessage(msg.hwnd, (uint)Win32.Messages.WM_CLOSE, 0, 0);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Main.Exit(e);
                }

                return Win32.Functions.CallNextHookEx(hook_id, nCode, wParam, lParam);
            }
        }
    }
}
