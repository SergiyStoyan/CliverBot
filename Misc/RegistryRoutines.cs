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
using Microsoft.Win32;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;


namespace Cliver.Bot
{
    public static class RegistryRoutines
    {
        /// <summary>
        /// Get string value from windows registry.
        /// </summary>
        public static string GetValue(string name)
        {
            lock (lock_object)
            {
                try
                {
                    RegistryKey rk = base_registry_key.OpenSubKey(Properties.App.Default.RegistrySubkey);
                    if (rk == null)
                        return null;
                    return (string)rk.GetValue(name);
                }
                catch (Exception e)
                {
                    LogMessage.Error(e);
                }
                return null;
            }
        }
        readonly static RegistryKey base_registry_key = Registry.CurrentUser;
        static object lock_object = new object();

        readonly static public string DefaultRegistryKey = base_registry_key.ToString() + "/" + Properties.App.Default.RegistrySubkey;

        /// <summary>
        /// Set string value to windows registry.
        /// </summary>
        public static void SetValue(string name, string value)
        {
            lock (lock_object)
            {
                try
                {
                    RegistryKey rk = base_registry_key.CreateSubKey(Properties.App.Default.RegistrySubkey);
                    //RegistryAccessRule rule = new RegistryAccessRule(WindowsIdentity.GetCurrent().User, RegistryRights.FullControl, AccessControlType.Allow);
                    //RegistrySecurity Security = new RegistrySecurity();
                    //Security.SetOwner(WindowsIdentity.GetCurrent().User);
                    //Security.AddAccessRule(rule);
                    //rk.SetAccessControl(Security);
                    rk.SetValue(name, value, RegistryValueKind.String);
                }
                catch (System.Security.SecurityException e)
                {
                    LogMessage.Error(e);
                    LogMessage.Inform(Program.AppName + " needs administatrator privileges to create an initial congiguration in the registry. So it will restart now and ask for elevated privileges.");
                    try
                    {
                        ProcessRoutines.Restart(true);
                    }
                    catch (Exception ex)
                    {
                        LogMessage.Exit(ex);
                    }
                }
                catch (Exception e)
                {
                    LogMessage.Error(e);
                }
            }
        }
    }
}
