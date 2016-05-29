using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Cliver.Bot
{
    public class CommandLineParameters : ProgramRoutines.CommandLineParameters
    {
        public static readonly CommandLineParameters AUTOMATIC = new CommandLineParameters("-automatic");
        public static readonly CommandLineParameters PRODUCTION = new CommandLineParameters("-production");
        public static readonly CommandLineParameters NOT_RESTORE_SESSION = new CommandLineParameters("-not_restore_session");

        public CommandLineParameters(string value) : base(value) { }
    }
    
    public static class Program
    {
        public enum ProgramMode
        {
            AUTOMATIC,
            DIALOG
        }

        static readonly public ProgramMode Mode;

        public static void Initialize()
        {
            //to force static constructor
        }

        static Program()
        {
            Config.Initialize();

            //Mode = (!ProgramRoutines.IsParameterSet(CommandLineParameters.NOT_START) && (ProgramRoutines.IsParameterSet(CommandLineParameters.AUTOMATIC) || Properties.General.Default.RunSilently)) ? ProgramMode.AUTOMATIC : ProgramMode.DIALOG;
            Mode = ProgramRoutines.IsParameterSet(CommandLineParameters.AUTOMATIC) ? ProgramMode.AUTOMATIC : ProgramMode.DIALOG;
            LogMessage.DisableStumblingDialogs = Mode == ProgramMode.AUTOMATIC;

            if (ProgramRoutines.IsParameterSet(CommandLineParameters.PRODUCTION))
            {
                Properties.General.Default.RestoreBrokenSession = true;
                Properties.General.Default.RestoreErrorItemsAsNew = false;
                Properties.General.Default.WriteSessionRestoringLog = true;
                Properties.Log.Default.LogDownloadedFiles = false;
                Properties.General.Default.UseFilesFromCache = false;
            }
            
            AssemblyName ean = Assembly.GetEntryAssembly().GetName();
            string customization_title = ean.Name;
            if (ean.Version.Major > 0 || ean.Version.Minor > 0)
                customization_title += ean.Version.Major + "." + ean.Version.Minor;
            //CustomizationModificationTime = File.GetLastWriteTime(Log.AppDir + "\\" + ean);
            AssemblyName can = Assembly.GetExecutingAssembly().GetName();
            string CliverBot_title = can.Name;
            if (can.Version.Major > 0 || can.Version.Minor > 0)
                CliverBot_title += can.Version.Major + "." + can.Version.Minor;
            Title = customization_title + @" / " + CliverBot_title;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogMessage.Exit("Unhandled exception: " + e.ExceptionObject.ToString());
        }

        static public readonly string Title;
        //static public readonly DateTime CustomizationModificationTime = File.GetLastWriteTime(Application.ExecutablePath);
        static readonly public string AppName = Application.ProductName;

        public static DateTime GetCustomizationCompiledTime()
        {
            string filePath = CustomizationApi.CustomAssembly.Location;
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
                if (Properties.App.Default.SingleProcessOnly)
                    ProcessRoutines.RunSingleProcessOnly();
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
                Process.Start(Properties.App.Default.HelpUri);
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }
    }
}