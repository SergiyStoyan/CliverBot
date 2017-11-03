//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Web;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cliver
{
    public static class SystemRoutines
    {
        static public string GetWindowsUserName()
        {
            uint session_id = Cliver.Win32.WTSGetActiveConsoleSessionId();
            if (session_id == 0xFFFFFFFF)
                return null;

            IntPtr buffer;
            int strLen;
            if (!Cliver.Win32.WTSQuerySessionInformation(IntPtr.Zero, session_id, Cliver.Win32.WTS_INFO_CLASS.WTSUserName, out buffer, out strLen) || strLen < 1)
                return null;

            string userName = Marshal.PtrToStringAnsi(buffer);
            Cliver.Win32.WTSFreeMemory(buffer);
            return userName;
        }

        static public string GetWindowsUserName2()
        {
            return System.Windows.Forms.SystemInformation.UserName;
        }

        public static void StartShutDown(string param)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd";
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.Arguments = "/C shutdown " + param;
            Process.Start(psi);
        }
    }
}

