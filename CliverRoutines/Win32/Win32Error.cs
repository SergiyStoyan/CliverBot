using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Cliver
{
    public class Win32Error
    {
        public static Win32Exception GetLastError()
        {
            return new Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
        }

        public static string GetLastErrorMessage()
        {
            Win32Exception e = GetLastError();
            return e?.Message;
        }

        public static string GetLastErrorAndMessage()
        {
            Win32Exception e = GetLastError();
            return "Error: (" + GetLastError() + ") " + GetLastErrorMessage();
        }
    }
}