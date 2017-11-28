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
using Microsoft.Win32.SafeHandles;

namespace Cliver
{
    public class Win32File
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(string lpFileName, dwDesiredAccess dwDesiredAccess, dwShareMode dwShareMode, ref SECURITY_ATTRIBUTES lpSecurityAttributes, dwCreationDisposition dwCreationDisposition, dwFlagsAndAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        public enum dwDesiredAccess : long
        {
            GENERIC_READ = 0x80000000L,
            GENERIC_WRITE = 0x40000000L,
            GENERIC_EXECUTE = 0x20000000L,
            GENERIC_ALL = 0x10000000L,
        }

        public enum dwShareMode : long
        {
            FILE_SHARE_READ = 0x00000001,
            FILE_SHARE_WRITE = 0x00000002,
            FILE_SHARE_DELETE = 0x00000004,
        }

        public enum dwFlagsAndAttributes : long
        {
            FILE_ATTRIBUTE_READONLY = 0x00000001,
            FILE_ATTRIBUTE_HIDDEN = 0x00000002,
            FILE_ATTRIBUTE_SYSTEM = 0x00000004,
            FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
            FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
            FILE_ATTRIBUTE_DEVICE = 0x00000040,
            FILE_ATTRIBUTE_NORMAL = 0x00000080,
            FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
            FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
            FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
        }

        public enum dwCreationDisposition : long
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXISTING = 5,
        }
    }
}