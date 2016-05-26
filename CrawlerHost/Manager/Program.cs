﻿//********************************************************************************************
//Author: Sergey Stoyan
//        stoyan@cliversoft.com        
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using Cliver.Bot;

namespace Cliver.CrawlerHostManager
{
    public static class Program
    {
        static Program()
        {
            LogMessage.DisableStumblingDialogs = Properties.Settings.Default.NoDialogMode;
            Log.MODE = Log.Mode.ONLY_LOG;
            Config.Initialize();
            SetTitle();
        }

        static public readonly string Title;
        static public readonly string Version;

        public static void SetTitle(Assembly assembly = null)
        {
            if(assembly == null)
                assembly = Assembly.GetEntryAssembly();
            AssemblyName ean = assembly.GetName();
            typeof(Program).GetField("Version").SetValue(null, ean.Version.Major + "." + ean.Version.Minor);
            typeof(Program).GetField("Title").SetValue(null, ean.Name + Version);
        }

        [STAThreadAttribute]
        public static void Main()
        {
            try
            {
                ProcessRoutines.RunSingleProcessOnly();
                SysTrayForm.This.Hide();
                Cliver.Bot.ThreadLog.MaxFileSize = Properties.Settings.Default.LogFileChunkSizeInBytes;
                Application.Run();
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }

        static public string AppName
        {
            get
            {
                return Application.ProductName;
            }
        }

        static public void ResetSettings()
        {
           Properties.Settings.Default.Reset();
           Properties.Settings.Default.Save();
           Properties.Settings.Default.Reload();
        }
    }
}
