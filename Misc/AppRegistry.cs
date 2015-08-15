//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Cliver.Bot
{
    public static class AppRegistry
    {
        static AppRegistry()
        {
            string p;
            if (ProgramRoutines.IsWebContext)
                p = System.Web.Compilation.BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName(false).CodeBase;
            else
                p = System.Reflection.Assembly.GetEntryAssembly().GetName(false).CodeBase;

            if (string.IsNullOrWhiteSpace(Properties.App.Default.RegistryAppSubkeyNameRegexForBaseDirectory))
                AppRegistryPath = Properties.App.Default.RegistryGeneralSubkey.Trim('\\', '/');
            else
            {
                Match m = Regex.Match(p, Properties.App.Default.RegistryAppSubkeyNameRegexForBaseDirectory, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (!m.Success)
                    throw new Exception("Cannot parse binary path.");
                string app_rsk_name = m.Groups[1].Value;
                AppRegistryPath = Properties.App.Default.RegistryGeneralSubkey.Trim('\\', '/') + @"\" + app_rsk_name;
            }
            if (!ProgramRoutines.IsWebContext)
                Log.Main.Write("App registry key: " + AppRegistryPath);
        }
        static readonly string AppRegistryPath;

        static RegistryKey get_base_key()
        {
            return RegistryKey.OpenBaseKey(base_registry_hive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
        }
        readonly static RegistryHive base_registry_hive = Properties.App.Default.RegistryHiveIsUserDependent ? RegistryHive.CurrentUser : RegistryHive.LocalMachine;
        
        static object lock_object = new object();
        readonly static public string DefaultRegistryPath = get_base_key().ToString() + @"\" + Properties.App.Default.RegistryGeneralSubkey;

        public static string GetString(string name, bool look_in_app_key_if_not_found_in_union_key = false, string default_value = null)
        {
            lock (lock_object)
            {
                RegistryKey rk = get_base_key();
                rk = rk.open_subkey(AppRegistryPath);
                if (rk == null)
                {
                    if (look_in_app_key_if_not_found_in_union_key)
                    {
                        rk = get_base_key();
                        rk = rk.open_subkey(Properties.App.Default.RegistryGeneralSubkey);
                    }
                    if (rk == null)
                        return default_value;
                }
                return (string)rk.GetValue(name, default_value);
            }
        }

        public static void SetValue<T>(string name, T value)
        {
            get_base_key().create_subkey(AppRegistryPath).set_value(name, value);
        }

        static RegistryKey open_subkey(this RegistryKey rk, string subkey_path, bool writable = false)
        {
            lock (lock_object)
            {
                try
                {
                    return rk.OpenSubKey(subkey_path, writable);
                }
                catch (Exception e)
                {
                    process_exception(e);
                }
                return null;
            }
        }

        static RegistryKey create_subkey(this RegistryKey rk, string subkey_path)
        {
            lock (lock_object)
            {
                try
                {
                    if (!ProgramRoutines.IsWebContext)
                        Log.Main.Write("Creating registry key: " + rk.ToString() + @"\" + subkey_path);
                    return rk.CreateSubKey(subkey_path);
                }
                catch (Exception e)
                {
                    process_exception(e);
                }
                return null;
            }
        }

        static void set_value<T>(this RegistryKey rk, string name, T value)
        {
            lock (lock_object)
            {
                try
                {
                    if (!ProgramRoutines.IsWebContext)
                        Log.Main.Write("Setting registry key: " + rk.ToString() + @"\" + name + " = '" + value.ToString() + "'");
                    //RegistryAccessRule rule = new RegistryAccessRule(WindowsIdentity.GetCurrent().User, RegistryRights.FullControl, AccessControlType.Allow);
                    //RegistrySecurity Security = new RegistrySecurity();
                    //Security.SetOwner(WindowsIdentity.GetCurrent().User);
                    //Security.AddAccessRule(rule);
                    //rk.SetAccessControl(Security);
                    rk.SetValue(name, value, RegistryValueKind.String);
                }
                catch (Exception e)
                {
                    process_exception(e);
                }
            }
        }

        static void process_exception(Exception e)
        {
            if (ProgramRoutines.IsWebContext)
                throw e;
            
            LogMessage.Error(e);

            if (e is System.Security.SecurityException
                || e is System.UnauthorizedAccessException
                )
            {
                if (ProcessRoutines.IsElevated())
                    LogMessage.Exit("Despite the app is running with elevated privileges, it still cannot write to the resgistry. Please fix the problem before using the app.");
                LogMessage.Inform(Program.AppName + " needs administatrator privileges to create an initial configuration in the registry. So it will restart now and ask for elevated privileges.");
                try
                {
                    ProcessRoutines.Restart(true);
                }
                catch (Exception ex)
                {
                    LogMessage.Exit(ex);
                }
            }
        }
    }
}
