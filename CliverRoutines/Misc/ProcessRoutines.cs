//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.Media;
using System.Web;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Management;

namespace Cliver
{
    public static class ProcessRoutines
    {
        public static bool IsElevated()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool IsProgramRunningAlready()
        {
            Process p = Process.GetCurrentProcess();
            return (from x in Process.GetProcessesByName(p.ProcessName) where x.MainModule.FileName == p.MainModule.FileName select x).Count() > 1;
        }

        public static void RunSingleProcessOnly(bool silent = false)
        {
            string app_name = ProgramRoutines.GetAppName();
            GLOBAL_SINGLE_PROCESS_MUTEX = new Mutex(false, @"Global\CliverSoft_" + app_name + "_SINGLE_PROCESS");
            // Wait for a few seconds when contended, if the other instance of the program is still in progress of shutting down.
            if (!GLOBAL_SINGLE_PROCESS_MUTEX.WaitOne(1000, false))
                if (!silent)
                    LogMessage.Exit2(app_name + " is already running, so this instance will exit.");
                else
                    Environment.Exit(0);
        }
        static Mutex GLOBAL_SINGLE_PROCESS_MUTEX = null;

        /// <summary>
        /// Must be invoked if this process needs some time to exit.
        /// </summary>
        public static void Exit()
        {
            if (GLOBAL_SINGLE_PROCESS_MUTEX != null)
                GLOBAL_SINGLE_PROCESS_MUTEX.ReleaseMutex();
            Environment.Exit(0);
        }

        public static void Restart(bool as_administarator = false)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.WorkingDirectory = Environment.CurrentDirectory;
            psi.FileName = Application.ExecutablePath;
            if (as_administarator)
                psi.Verb = "runas";
            if (GLOBAL_SINGLE_PROCESS_MUTEX != null)
                GLOBAL_SINGLE_PROCESS_MUTEX.ReleaseMutex();
            try
            {
                Process.Start(psi);
            }
            catch
            { //if the user cancelled
            }
            Environment.Exit(0);
        }

        public static bool IsProcessAlive(int process_id)
        {
            return GetProcess(process_id) != null;
        }

        public static bool IsProcessAlive(Process process)
        {
            return GetProcess(process.Id) != null;
        }

        public static Process GetProcess(int process_id)
        {
            try
            {
                return Process.GetProcessById(process_id);
            }
            catch
            {
                return null;
            }
        }

        public static bool KillProcessTree(int process_id)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + process_id);
            foreach (ManagementObject mo in searcher.Get())
                KillProcessTree(Convert.ToInt32(mo["ProcessID"]));
            
            Process p;
            try
            {
                p = Process.GetProcessById(process_id);
            }
            catch
            {
                return true;
            }
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    p.Kill();
                }
                catch
                {
                    // Process already exited.
                    return true;
                }
                p.WaitForExit(1000);
                if (!IsProcessRunning(p))
                    return true;
            }
            return false;
        }

        public static bool IsProcessRunning(Process p)
        {
            try
            {
                return !p.HasExited;
            }
            catch
            {//it was not started
                return false;
            }
        }

        /// <summary>
        /// Making a process live no longer than this object
        /// </summary>
        public class Job : IDisposable
        {
            enum JobObjectInfoType
            {
                AssociateCompletionPortInformation = 7,
                BasicLimitInformation = 2,
                BasicUIRestrictions = 4,
                EndOfJobTimeInformation = 6,
                ExtendedLimitInformation = 9,
                SecurityLimitInformation = 5,
                GroupInformation = 11
            }

            [StructLayout(LayoutKind.Sequential)]
            struct SECURITY_ATTRIBUTES
            {
                public int nLength;
                public IntPtr lpSecurityDescriptor;
                public int bInheritHandle;
            }

            //[StructLayout(LayoutKind.Sequential)]
            //struct JOBOBJECT_BASIC_LIMIT_INFORMATION//win32
            //{
            //    public Int64 PerProcessUserTimeLimit;
            //    public Int64 PerJobUserTimeLimit;
            //    public Int16 LimitFlags;
            //    public UInt32 MinimumWorkingSetSize;
            //    public UInt32 MaximumWorkingSetSize;
            //    public Int16 ActiveProcessLimit;
            //    public Int64 Affinity;
            //    public Int16 PriorityClass;
            //    public Int16 SchedulingClass;
            //}
            [StructLayout(LayoutKind.Sequential)]
            struct JOBOBJECT_BASIC_LIMIT_INFORMATION//win64
            {
                public Int64 PerProcessUserTimeLimit;
                public Int64 PerJobUserTimeLimit;
                public Int16 LimitFlags;
                public UIntPtr MinimumWorkingSetSize;
                public UIntPtr MaximumWorkingSetSize;
                public Int16 ActiveProcessLimit;
                public Int64 Affinity;
                public Int16 PriorityClass;
                public Int16 SchedulingClass;
            }

            [StructLayout(LayoutKind.Sequential)]
            struct IO_COUNTERS
            {
                public UInt64 ReadOperationCount;
                public UInt64 WriteOperationCount;
                public UInt64 OtherOperationCount;
                public UInt64 ReadTransferCount;
                public UInt64 WriteTransferCount;
                public UInt64 OtherTransferCount;
            }

            [StructLayout(LayoutKind.Sequential)]
            struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
            {
                public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
                public IO_COUNTERS IoInfo;
                public UInt32 ProcessMemoryLimit;
                public UInt32 JobMemoryLimit;
                public UInt32 PeakProcessMemoryUsed;
                public UInt32 PeakJobMemoryUsed;
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            static extern IntPtr CreateJobObject(object a, string lpName);

            [DllImport("kernel32.dll")]
            static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

            public Job()
            {
                m_handle = CreateJobObject(null, null);

                JOBOBJECT_BASIC_LIMIT_INFORMATION info = new JOBOBJECT_BASIC_LIMIT_INFORMATION();
                info.LimitFlags = 0x2000;

                JOBOBJECT_EXTENDED_LIMIT_INFORMATION extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
                extendedInfo.BasicLimitInformation = info;

                int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
                IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
                Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

                if (!SetInformationJobObject(m_handle, JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)length))
                    throw new Exception(string.Format("Unable to set information.  Error: {0}", Marshal.GetLastWin32Error()));
            }
            private IntPtr m_handle;

            public void Dispose()
            {
                if (m_handle != IntPtr.Zero)
                {
                    Win32.CloseHandle(m_handle);
                    m_handle = IntPtr.Zero;
                }

                //GC.SuppressFinalize(this);
            }
            
            public bool MakeProcessLiveNoLongerThanJob(Process p)
            {
                return AssignProcessToJobObject(m_handle, p.Handle);
            }
        }
    }
}