//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Cliver
{
    /// <summary>
    /// Multithreaded logging routines
    /// </summary>
    public static partial class Log
    {
        public static void Initialize(int delete_logs_older_days, string pre_work_dir, bool write_log)
        {
            Log.delete_logs_older_days = delete_logs_older_days;
            Log.pre_work_dir = pre_work_dir;
            Log.write_log = write_log;
        }
        static int delete_logs_older_days = 10;
        static string pre_work_dir = null;
        static bool write_log;

        static object lock_object = new object();

        static Log()
        {
            if (ProgramRoutines.IsWebContext)
            {
                throw new Exception("Log is disabled in web context.");

                string p = System.Web.Compilation.BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName(false).CodeBase;
                EntryAssemblyName = System.IO.Path.GetFileNameWithoutExtension(p);
            }
            else
            {
                string p = System.Reflection.Assembly.GetEntryAssembly().GetName(false).CodeBase;
                EntryAssemblyName = System.IO.Path.GetFileNameWithoutExtension(p);
            }
            AppDir = AppDomain.CurrentDomain.BaseDirectory;

            AppCommonDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\CliverSoft\\" + Cliver.Log.EntryAssemblyName;

            //Log.DeleteOldLogs();
        }

        /// <summary>
        /// Normalized name of this process
        /// </summary>
        public static readonly string EntryAssemblyName;

        /// <summary>
        /// Directory where the application's data files independent on user are located.
        /// </summary>
        public static readonly string AppCommonDataDir;
        
        /// <summary>
        /// Directory where the application binary is located.
        /// </summary>
        public readonly static string AppDir;

        /// <summary>
        /// Constructs and returns time mark string for current session
        /// </summary>
        public static string TimeMark
        {
            get
            {
                if (time_mark == null)
                {
                    lock (lock_object)
                    {
                        time_mark = DateTime.Now.ToString("yyMMddHHmmss");
                    }
                }
                return time_mark;
            }
        }
        static string time_mark = null;
        
        /// <summary>
        ///Parent Log directory where logs are recorded
        /// </summary>
        public static string WorkDir
        {
            get
            {
                if (work_dir == null)
                {
                    lock (lock_object)
                    {
                        if (string.IsNullOrEmpty(pre_work_dir))
                            pre_work_dir = Log.EntryAssemblyName + WorkDirPrefix;
                        if (!pre_work_dir.Contains(":"))
                            pre_work_dir = Log.AppDir + @"\" + pre_work_dir;
                        work_dir = pre_work_dir + @"\" + Log.EntryAssemblyName + WorkDirPrefix;

                        DirectoryInfo di = new DirectoryInfo(work_dir);
                        if (!di.Exists)
                            di.Create();
                    }
                }
                return work_dir;
            }
        }
        static string work_dir = null;
        public const string WorkDirPrefix = @"_Sessions";

        /// <summary>
        /// Session directory for current session
        /// </summary>
        public static string SessionDir
        {
            get
            {
                if (session_dir == null)
                {
                    lock (lock_object)
                    {
                        if (Log.MODE != Mode.SESSIONS)
                            throw new Exception("SessionDir cannot be used while LOGGING_MODE != Log.Mode.SESSIONS");

                        session_dir = WorkDir + @"\Session" + "_" + Log.TimeMark;

                        DirectoryInfo di = new DirectoryInfo(session_dir);
                        int count = 0;
                        while (di.Exists)
                        {
                            count++;
                            session_dir = WorkDir + @"\Session" + "_" + Log.TimeMark + "_" + count.ToString();
                            di = new DirectoryInfo(session_dir);
                        }
                        di.Create();
                    }
                }
                return session_dir;
            }
        }
        static string session_dir = null;

        /// <summary>
        /// Output directory for current session
        /// </summary>
        public static string OutputDir
        {
            get
            {
                if (output_dir == null)
                {
                    lock (lock_object)
                    {
                        output_dir = SessionDir + @"\" + OutputDirName;

                        DirectoryInfo di = new DirectoryInfo(output_dir);
                        if (!di.Exists)
                            di.Create();
                    }
                }
                return output_dir;
            }
        }
        static string output_dir = null;

        /// <summary>
        /// Output folder name
        /// </summary>
        public static string OutputDirName = @"output";

        /// <summary>
        /// Download directory for session. 
        /// This dir can be used to calculate value of downloaded bytes.
        /// </summary>
        public static string DownloadDir
        {
            get
            {
                if (download_dir == null)
                {
                    lock (lock_object)
                    {
                        download_dir = SessionDir + "\\" + DownloadDirName;

                        DirectoryInfo di = new DirectoryInfo(download_dir);
                        if (!di.Exists)
                            di.Create();
                    }
                }
                return download_dir;
            }
        }
        static string download_dir = null;
        public const string DownloadDirName = "cache";
        
        /// <summary>
        /// Used to clear all session parameters in order to start a new session
        /// </summary>
        public static void ClearSession()
        {
            lock (lock_object)
            {
                Log.CloseAll();

                time_mark = null;
                work_dir = null;
                session_dir = null;
                output_dir = null;
                download_dir = null;

                GC.Collect();
            }
        }

        /// <summary>
        /// Deletes Log data from disk that is older the specified threshold
        /// </summary>
        public static void DeleteOldLogs()
        {
            if (ignore_delete_old_logs)
                return;
            lock (lock_object)
            {
                ignore_delete_old_logs = true;
                try
                {
                    if (delete_logs_older_days > 0)
                    {
                        DateTime FirstLogDate = DateTime.Now.AddDays(-delete_logs_older_days);

                        DirectoryInfo di = new DirectoryInfo(Log.WorkDir);
                        if (!di.Exists)
                            return;

                        string alert;
                        switch (MODE)
                        {
                            case Mode.SESSIONS:
                                alert = "Session data including caches and logs older than " + FirstLogDate.ToString() + " should be deleted along the specified threshold.\n Delete?";
                                foreach (DirectoryInfo d in di.GetDirectories())
                                {
                                    if (session_dir != null && d.FullName.StartsWith(session_dir, StringComparison.InvariantCultureIgnoreCase))
                                        continue;
                                    if (d.LastWriteTime >= FirstLogDate)
                                        continue;
                                    if (alert != null)
                                    {
                                        if (!LogMessage.AskYesNo(alert, true))
                                            return;
                                        else
                                            alert = null;
                                    }
                                    Log.Main.Inform("Deleting old directory: " + d.FullName);
                                    try
                                    {
                                        d.Delete(true);
                                    }
                                    catch (Exception e)
                                    {
                                        LogMessage.Error(e);
                                    }
                                }
                                break;
                            case Mode.ONLY_LOG:
                                alert = "Logs older than " + FirstLogDate.ToString() + " should be deleted along the specified threshold.\n Delete?";
                                foreach (FileInfo f in di.GetFiles())
                                {
                                    if (f.LastWriteTime >= FirstLogDate)
                                        continue;
                                    if (alert != null)
                                    {
                                        if (!LogMessage.AskYesNo(alert, true))
                                            return;
                                        else
                                            alert = null;
                                    }
                                    Log.Main.Inform("Deleting old file: " + f.FullName);
                                    try
                                    {
                                        f.Delete();
                                    }
                                    catch (Exception e)
                                    {
                                        LogMessage.Error(e);
                                    }
                                }
                                break;
                            default:
                                throw new Exception("Unknown LOGGING_MODE:" + MODE);
                        }
                    }
                }
                finally
                {
                    ignore_delete_old_logs = false;
                }
            }
        }
        static bool ignore_delete_old_logs = false;

        /// <summary>
        /// Create absolute path from app directory and relative path
        /// </summary>
        /// <param name="file">file path or name</param>
        /// <returns>absolute path</returns>
        public static string GetAbsolutePath(string path)
        {
            try
            {
                if (path.Contains(":"))
                    return path;
                return System.IO.Path.GetFullPath(Log.AppDir + "\\" + path);
            }
            catch (Exception e)
            {
                LogMessage.Exit(e);
            }

            return null;
        }
    }
}

