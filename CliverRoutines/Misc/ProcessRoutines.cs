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
        public class AntiZombieJob : IDisposable
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

            public enum JOBOBJECTLIMIT : uint
            {
                // Basic Limits
                Workingset = 0x00000001,
                ProcessTime = 0x00000002,
                JobTime = 0x00000004,
                ActiveProcess = 0x00000008,
                Affinity = 0x00000010,
                PriorityClass = 0x00000020,
                PreserveJobTime = 0x00000040,
                SchedulingClass = 0x00000080,

                // Extended Limits
                ProcessMemory = 0x00000100,
                JobMemory = 0x00000200,
                DieOnUnhandledException = 0x00000400,
                BreakawayOk = 0x00000800,
                SilentBreakawayOk = 0x00001000,
                KillOnJobClose = 0x00002000,
                SubsetAffinity = 0x00004000,

                // Notification Limits
                JobReadBytes = 0x00010000,
                JobWriteBytes = 0x00020000,
                RateControl = 0x00040000,
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
            //[StructLayout(LayoutKind.Sequential)]
            //struct JOBOBJECT_BASIC_LIMIT_INFORMATION//win64
            //{
            //    public Int64 PerProcessUserTimeLimit;
            //    public Int64 PerJobUserTimeLimit;
            //    public JOBOBJECTLIMIT LimitFlags;
            //    public UIntPtr MinimumWorkingSetSize;
            //    public UIntPtr MaximumWorkingSetSize;
            //    public Int16 ActiveProcessLimit;
            //    public Int64 Affinity;
            //    public Int16 PriorityClass;
            //    public Int16 SchedulingClass;
            //}
            [StructLayout(LayoutKind.Sequential)]
            struct JOBOBJECT_BASIC_LIMIT_INFORMATION
            {
                public Int64 PerProcessUserTimeLimit;
                public Int64 PerJobUserTimeLimit;
                public JOBOBJECTLIMIT LimitFlags;
                public UIntPtr MinimumWorkingSetSize;
                public UIntPtr MaximumWorkingSetSize;
                public UInt32 ActiveProcessLimit;
                public Int64 Affinity;
                public UInt32 PriorityClass;
                public UInt32 SchedulingClass;
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
            static extern IntPtr CreateJobObject(IntPtr a, string lpName);

            [DllImport("kernel32.dll")]
            static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, UInt32 cbJobObjectInfoLength);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);
            
            public AntiZombieJob()
            {
                jobHandle = CreateJobObject(IntPtr.Zero, null);

                JOBOBJECT_EXTENDED_LIMIT_INFORMATION jeli = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
                jeli.BasicLimitInformation.LimitFlags = JOBOBJECTLIMIT.KillOnJobClose;

                int jeliSize = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
                extendedInfoPtr = Marshal.AllocHGlobal(jeliSize);
                Marshal.StructureToPtr(jeli, extendedInfoPtr, false);

                if (!SetInformationJobObject(jobHandle, JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)jeliSize))
                        //SetInformationJobObject(jobHandle, JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)length);
                        //if(Marshal.GetLastWin32Error() != 0)//for unclear reason SetInformationJobObject returns false with no error
                        throw new Exception("Unable to set information. Error: " + Win32Routines.GetLastErrorMessage());
            }
            IntPtr jobHandle = IntPtr.Zero;
            IntPtr extendedInfoPtr = IntPtr.Zero;

            public void Dispose()
            {
                if (jobHandle != IntPtr.Zero)
                {
                    Win32.CloseHandle(jobHandle);
                    jobHandle = IntPtr.Zero;
                }
                if (extendedInfoPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(extendedInfoPtr);
                    extendedInfoPtr = IntPtr.Zero;
                }

                //GC.SuppressFinalize(this);
            }
            
            public bool MakeProcessLiveNoLongerThanJob(Process p)
            {
                return AssignProcessToJobObject(jobHandle, p.Handle);
            }
        }
    }
}