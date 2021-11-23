using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Cliver.Win;

namespace Cliver.Bot
{
    public class CommandLineParameters
    {
        public const string PRODUCTION = "-production";
        public const string NOT_RESTORE_SESSION = "-not_restore_session";
    }

    public static class Program
    {
        public static void Initialize()
        {
            //to force static constructor
        }

        //public const string LogSessionPrefix = "Log";

        static Program()
        {
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs args)
            {
                Exception e = (Exception)args.ExceptionObject;
                LogMessage.Exit(e);
            };

            LogMessage.DisableStumblingDialogs = true;

            if (CommandLine.IsParameterSet(CommandLineParameters.PRODUCTION))
            {
                Settings.Engine.RestoreBrokenSession = true;
                Settings.Engine.RestoreErrorItemsAsNew = false;
                Settings.Engine.WriteSessionRestoringLog = true;
            }

            AssemblyRoutines.AssemblyInfo ai = new AssemblyRoutines.AssemblyInfo();
            Name = ai.Product;
            Version = ai.Version;
            AssemblyRoutines.AssemblyInfo cai = new AssemblyRoutines.AssemblyInfo(Assembly.GetEntryAssembly());
            FullName = cai.Product + "-" + cai.Version.ToString(3) + " [" + Name + "-" + Version.ToString(3) + "]";

            Config.Reload();

            if (!Settings.Log.WriteLog)
                Log.DefaultLevel = Log.Level.NONE;
            Log.Initialize(Log.Mode.FOLDER_PER_SESSION, new List<string> { Settings.Log.PreWorkDir }, Settings.Log.DeleteLogsOlderDays);
        }
        public static readonly string Name;
        public static readonly Version Version;
        public static readonly string FullName;

        public static Assembly GetAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        public static DateTime GetCustomizationCompiledTime()
        {
            string filePath = Assembly.GetEntryAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                    s.Close();
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.ToLocalTime();
            return dt;
        }

        /// <summary>
        /// Invoked by CliverBotCustomization to launch CliverBot
        /// </summary>
        static public void Run()
        {
            try
            {
                Bot.Config.Reload();

                if (Settings.App.SingleProcessOnly)
                    ProcessRoutines.RunMeInSingleProcessOnly((string m) => { LogMessage.Inform(m); });
                Session.Start();
                //         MainForm.This.Text = Program.Title;
                //       MainForm.This.ShowDialog();
            }
            catch (Exception e)
            {
                LogMessage.Exit(e);
            }
        }

        static internal void Help()
        {
            try
            {
                Process.Start(Settings.App.HelpUri);
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }
    }
}