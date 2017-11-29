//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        15 December 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Cliver
{
    public class Win32Process3
    {
        public enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            MaxTokenInfoClass // MaxTokenInfoClass should always be the last enum
        }

        public const int STARTF_USESHOWWINDOW = 0x00000001;
        public const int STARTF_USESIZE = 0x00000002;
        public const int STARTF_USEPOSITION = 0x00000004;
        public const int STARTF_USECOUNTCHARS = 0x00000008;
        public const int STARTF_USEFILLATTRIBUTE = 0x00000010;
        public const int STARTF_RUNFULLSCREEN = 0x00000020;  // ignored for non-x86 platforms
        public const int STARTF_FORCEONFEEDBACK = 0x00000040;
        public const int STARTF_FORCEOFFFEEDBACK = 0x00000080;
        public const int STARTF_USESTDHANDLES = 0x00000100;

        public class dwCreationFlagValues
        {
            public const uint DEBUG_PROCESS = 0x00000001;
            public const uint DEBUG_ONLY_THIS_PROCESS = 0x00000002;
            public const uint CREATE_SUSPENDED = 0x00000004;
            public const uint DETACHED_PROCESS = 0x00000008;
            public const uint CREATE_NEW_CONSOLE = 0x00000010;
            public const uint NORMAL_PRIORITY_CLASS = 0x00000020;
            public const uint IDLE_PRIORITY_CLASS = 0x00000040;
            public const uint HIGH_PRIORITY_CLASS = 0x00000080;
            public const uint REALTIME_PRIORITY_CLASS = 0x00000100;
            public const uint CREATE_NEW_PROCESS_GROUP = 0x00000200;
            public const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;
            public const uint CREATE_SEPARATE_WOW_VDM = 0x00000800;
            public const uint CREATE_SHARED_WOW_VDM = 0x00001000;
            public const uint CREATE_FORCEDOS = 0x00002000;
            public const uint BELOW_NORMAL_PRIORITY_CLASS = 0x00004000;
            public const uint ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000;
            public const uint INHERIT_PARENT_AFFINITY = 0x00010000;
            public const uint INHERIT_CALLER_PRIORITY = 0x00020000;    // Deprecated
            public const uint CREATE_PROTECTED_PROCESS = 0x00040000;
            public const uint EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
            public const uint PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000;
            public const uint PROCESS_MODE_BACKGROUND_END = 0x00200000;
            public const uint CREATE_BREAKAWAY_FROM_JOB = 0x01000000;
            public const uint CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000;
            public const uint CREATE_DEFAULT_ERROR_MODE = 0x04000000;
            public const uint CREATE_NO_WINDOW = 0x08000000;
            public const uint PROFILE_USER = 0x10000000;
            public const uint PROFILE_KERNEL = 0x20000000;
            public const uint PROFILE_SERVER = 0x40000000;
            public const uint CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000;
        }

        public const int READ_CONTROL = 0x00020000;

        public const int STANDARD_RIGHTS_REQUIRED = 0x000F0000;

        public const int STANDARD_RIGHTS_READ = READ_CONTROL;
        public const int STANDARD_RIGHTS_WRITE = READ_CONTROL;
        public const int STANDARD_RIGHTS_EXECUTE = READ_CONTROL;

        public const int STANDARD_RIGHTS_ALL = 0x001F0000;

        public const int SPECIFIC_RIGHTS_ALL = 0x0000FFFF;

        public const int TOKEN_ASSIGN_PRIMARY = 0x0001;
        public const int TOKEN_DUPLICATE = 0x0002;
        public const int TOKEN_IMPERSONATE = 0x0004;
        public const int TOKEN_QUERY = 0x0008;
        public const int TOKEN_QUERY_SOURCE = 0x0010;
        public const int TOKEN_ADJUST_PRIVILEGES = 0x0020;
        public const int TOKEN_ADJUST_GROUPS = 0x0040;
        public const int TOKEN_ADJUST_DEFAULT = 0x0080;
        public const int TOKEN_ADJUST_SESSIONID = 0x0100;

        public const int TOKEN_ALL_ACCESS_P = (STANDARD_RIGHTS_REQUIRED |
                                               TOKEN_ASSIGN_PRIMARY |
                                               TOKEN_DUPLICATE |
                                               TOKEN_IMPERSONATE |
                                               TOKEN_QUERY |
                                               TOKEN_QUERY_SOURCE |
                                               TOKEN_ADJUST_PRIVILEGES |
                                               TOKEN_ADJUST_GROUPS |
                                               TOKEN_ADJUST_DEFAULT);

        public const int TOKEN_ALL_ACCESS = TOKEN_ALL_ACCESS_P | TOKEN_ADJUST_SESSIONID;

        public const int TOKEN_READ = STANDARD_RIGHTS_READ | TOKEN_QUERY;

        public const int TOKEN_WRITE = STANDARD_RIGHTS_WRITE |
                                       TOKEN_ADJUST_PRIVILEGES |
                                       TOKEN_ADJUST_GROUPS |
                                       TOKEN_ADJUST_DEFAULT;

        public const int TOKEN_EXECUTE = STANDARD_RIGHTS_EXECUTE;

        public const int MAXIMUM_ALLOWED = 0x2000000;

        public const string SE_DEBUG_NAME = "SeDebugPrivilege";
        public const string SE_RESTORE_NAME = "SeRestorePrivilege";
        public const string SE_BACKUP_NAME = "SeBackupPrivilege";

        public const int SE_PRIVILEGE_ENABLED = 0x0002;

        public const int ERROR_NOT_ALL_ASSIGNED = 1300;

        private const uint TH32CS_SNAPPROCESS = 0x00000002;

        public static int INVALID_HANDLE_VALUE = -1;

        [DllImport("userenv.dll", SetLastError = true)]
        public static extern bool CreateEnvironmentBlock(ref IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        public static uint CreateProcessInSession(uint dwSessionId, String commandLine, uint dwCreationFlags = 0, WinApi.Advapi32.STARTUPINFO? startupInfo = null, bool bElevate = false)
        {
            IntPtr hUserToken = IntPtr.Zero;
            IntPtr hUserTokenDup = IntPtr.Zero;
            IntPtr hPToken = IntPtr.Zero;
            IntPtr hProcess = IntPtr.Zero;
            try
            {
                // Log the client on to the local computer.
                //uint dwSessionId = WTSGetActiveConsoleSessionId();

                //// Find the winlogon process
                //var procEntry = new PROCESSENTRY32();

                //uint hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
                //if (hSnap == INVALID_HANDLE_VALUE)
                //    throw new Exception("CreateToolhelp32Snapshot == INVALID_HANDLE_VALUE. " + ErrorRoutines.GetLastError());

                //procEntry.dwSize = (uint)Marshal.SizeOf(procEntry); //sizeof(PROCESSENTRY32);
                //if (Process32First(hSnap, ref procEntry) == 0)
                //    throw new Exception("Process32First == 0. " + ErrorRoutines.GetLastError());

                //uint winlogonPid = 0;
                //String strCmp = "explorer.exe";
                //do
                //{
                //    if (strCmp.IndexOf(procEntry.szExeFile) == 0)
                //    {
                //        // We found a winlogon process...make sure it's running in the console session
                //        uint winlogonSessId = 0;
                //        if (ProcessIdToSessionId(procEntry.th32ProcessID, ref winlogonSessId) && winlogonSessId == dwSessionId)
                //        {
                //            winlogonPid = procEntry.th32ProcessID;
                //            break;
                //        }
                //    }
                //}
                //while (Process32Next(hSnap, ref procEntry) != 0);
                //if (winlogonPid == 0)
                //    throw new Exception("winlogonPid == 0");

                //Get the user token used by DuplicateTokenEx
                //WTSQueryUserToken(dwSessionId, ref hUserToken);
                //if (hUserToken == IntPtr.Zero)
                //    throw new Exception("WTSQueryUserToken == 0. " + ErrorRoutines.GetLastError());

                WinApi.Advapi32.STARTUPINFO si;
                if (startupInfo != null)
                    si = (WinApi.Advapi32.STARTUPINFO)startupInfo;
                else
                    si = new WinApi.Advapi32.STARTUPINFO();
                si.cb = Marshal.SizeOf(si);
                si.lpDesktop = "winsta0\\default";
                //hProcess = OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);
                //if (hProcess == IntPtr.Zero)
                //    throw new Exception("OpenProcess == IntPtr.Zero. " + ErrorRoutines.GetLastError());

                if (!WinApi.Advapi32.OpenProcessToken(Process.GetCurrentProcess().Handle, MAXIMUM_ALLOWED, ref hPToken))
                    //if (!OpenProcessToken(hProcess, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY | TOKEN_DUPLICATE | TOKEN_ASSIGN_PRIMARY | TOKEN_ADJUST_SESSIONID | TOKEN_READ | TOKEN_WRITE, ref hPToken))
                    throw new Exception("!OpenProcessToken. " + ErrorRoutines.GetLastError());

                //var luid = new LUID();
                //if (!LookupPrivilegeValue(IntPtr.Zero, SE_DEBUG_NAME, ref luid))
                //    throw new Exception("!LookupPrivilegeValue. " + ErrorRoutines.GetLastError());

                var sa = new WinApi.Advapi32.SECURITY_ATTRIBUTES();
                sa.Length = Marshal.SizeOf(sa);
                if (!WinApi.Advapi32.DuplicateTokenEx(hPToken, MAXIMUM_ALLOWED, ref sa, WinApi.Advapi32.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, WinApi.Advapi32.TOKEN_TYPE.TokenPrimary, ref hUserTokenDup))
                    throw new Exception("!DuplicateTokenEx. " + ErrorRoutines.GetLastError());

                //if (bElevate)
                //{
                //    var tp = new TOKEN_PRIVILEGES();
                //    //tp.Privileges[0].Luid = luid;
                //    //tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
                //    tp.PrivilegeCount = 1;
                //    tp.Privileges = new int[3];
                //    tp.Privileges[2] = SE_PRIVILEGE_ENABLED;
                //    tp.Privileges[1] = luid.HighPart;
                //    tp.Privileges[0] = luid.LowPart;

                //    //Adjust Token privilege
                //    if (!SetTokenInformation(hUserTokenDup, TOKEN_INFORMATION_CLASS.TokenSessionId, ref dwSessionId, (uint)IntPtr.Size))
                //        throw new Exception("!SetTokenInformation. " + ErrorRoutines.GetLastError());
                //    if (!AdjustTokenPrivileges(hUserTokenDup, false, ref tp, Marshal.SizeOf(tp), /*(PTOKEN_PRIVILEGES)*/IntPtr.Zero, IntPtr.Zero))
                //        throw new Exception("!AdjustTokenPrivileges. " + ErrorRoutines.GetLastError());
                //}

                //dwCreationFlags |= dwCreationFlagValues.NORMAL_PRIORITY_CLASS| dwCreationFlagValues.CREATE_NEW_CONSOLE;
                //IntPtr pEnv = IntPtr.Zero;
                //if (CreateEnvironmentBlock(ref pEnv, hUserTokenDup, true))
                //    dwCreationFlags |= dwCreationFlagValues.CREATE_UNICODE_ENVIRONMENT;
                //else
                //    pEnv = IntPtr.Zero;

                // Launch the process in the client's logon session.
                WinApi.Advapi32.PROCESS_INFORMATION pi;
                if (!WinApi.Advapi32.CreateProcessAsUser(hUserTokenDup, // client's access token
                    null, // file to execute
                    commandLine, // command line
                    ref sa, // pointer to process SECURITY_ATTRIBUTES
                    ref sa, // pointer to thread SECURITY_ATTRIBUTES
                    false, // handles are not inheritable
                    (int)dwCreationFlags, // creation flags
                    IntPtr.Zero,//pEnv, // pointer to new environment block 
                    null, // name of current directory 
                    ref si, // pointer to STARTUPINFO structure
                    out pi // receives information about new process
                    ))
                    throw new Exception("!CreateProcessAsUser. " + ErrorRoutines.GetLastError());
                return pi.dwProcessId;
            }
            //catch(Exception e)
            //{

            //}
            finally
            {
                if (hProcess != IntPtr.Zero)
                    WinApi.Kernel32.CloseHandle(hProcess);
                if (hUserToken != IntPtr.Zero)
                    WinApi.Kernel32.CloseHandle(hUserToken);
                if (hUserTokenDup != IntPtr.Zero)
                    WinApi.Kernel32.CloseHandle(hUserTokenDup);
                if (hPToken != IntPtr.Zero)
                    WinApi.Kernel32.CloseHandle(hPToken);
                //if (pEnv != IntPtr.Zero)
                //    DestroyEnvironmentBlock(pEnv);
            }
        }
    }
}