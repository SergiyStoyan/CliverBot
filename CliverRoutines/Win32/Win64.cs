using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Cliver
{
    public class Win64
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, Int64 lpBaseAddress, byte[] lpBuffer, Int64 dwSize, ref int lpNumberOfBytesRead);

        public struct MEMORY_BASIC_INFORMATION
        {
            public Int64 BaseAddress;
            public Int64 AllocationBase;
            public Int32 AllocationProtect;
            public Int32 __alignment1;
            public Int64 RegionSize;
            public Int32 State;
            public Int32 Protect;
            public Int32 Type;
            public Int32 __alignment2;
        }
    }
}