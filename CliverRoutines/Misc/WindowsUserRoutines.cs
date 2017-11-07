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
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;

namespace Cliver
{
    public static class WindowsUserRoutines
    {
        static public string GetUserName()
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

        static public string GetUserName2()
        {
            return System.Windows.Forms.SystemInformation.UserName;
        }

        public static bool CurrentUserHasElevatedPrivileges()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        //public static bool IsAdministrator()
        //{
        //    return Win32.IsUserAnAdmin();
        //}

        //private static bool IsAdministrator(string username)//very slow
        //{//https://ayende.com/blog/158401/are-you-an-administrator
        //    PrincipalContext ctx;
        //    try
        //    {
        //        Domain.GetComputerDomain();
        //        try
        //        {
        //            ctx = new PrincipalContext(ContextType.Domain);
        //        }
        //        catch (PrincipalServerDownException)
        //        {
        //            // can't access domain, check local machine instead 
        //            ctx = new PrincipalContext(ContextType.Machine);
        //        }
        //    }
        //    catch (ActiveDirectoryObjectNotFoundException)
        //    {
        //        // not in a domain
        //        ctx = new PrincipalContext(ContextType.Machine);
        //    }
        //    var up = UserPrincipal.FindByIdentity(ctx, username);
        //    if (up != null)
        //    {
        //        PrincipalSearchResult<Principal> authGroups = up.GetAuthorizationGroups();
        //        return authGroups.Any(principal =>
        //                              principal.Sid.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid) ||
        //                              principal.Sid.IsWellKnown(WellKnownSidType.AccountDomainAdminsSid) ||
        //                              principal.Sid.IsWellKnown(WellKnownSidType.AccountAdministratorSid) ||
        //                              principal.Sid.IsWellKnown(WellKnownSidType.AccountEnterpriseAdminsSid));
        //    }
        //    return false;
        //}

        static public bool CurrentUserIsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity == null)
                throw new InvalidOperationException("Couldn't get the current user identity");
            var principal = new WindowsPrincipal(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                return true;

            // If we're not running in Vista onwards, we don't have to worry about checking for UAC.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT || Environment.OSVersion.Version.Major < 6)
                // Operating system does not support UAC; skipping elevation check.
                return false;

            int tokenInfLength = Marshal.SizeOf(typeof(int));
            IntPtr tokenInformation = Marshal.AllocHGlobal(tokenInfLength);
            try
            {
                uint returnedSize = 0;
                if (!Win32.GetTokenInformation(identity.Token, Win32.TOKEN_INFORMATION_CLASS.TokenElevationType, tokenInformation, (uint)tokenInfLength, out returnedSize))
                    throw new InvalidOperationException("Couldn't get token information", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

                switch ((Win32.TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(tokenInformation))
                {
                    case Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault:
                        // TokenElevationTypeDefault - User is not using a split token, so they cannot elevate.
                        return false;
                    case Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeFull:
                        // TokenElevationTypeFull - User has a split token, and the process is running elevated. Assuming they're an administrator.
                        return true;
                    case Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited:
                        // TokenElevationTypeLimited - User has a split token, but the process is not running elevated. Assuming they're an administrator.
                        return true;
                    default:
                        // Unknown token elevation type.
                        return false;
                }
            }
            finally
            {
                if (tokenInformation != IntPtr.Zero)
                    Marshal.FreeHGlobal(tokenInformation);
            }
        }
    }
}