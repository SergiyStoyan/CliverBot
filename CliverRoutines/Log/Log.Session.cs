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
    public static partial class Log
    {
        public partial class Session
        {
            const string MAIN_SESSION_NAME = "";

            public Session(string name = MAIN_SESSION_NAME)
            {
                Name = name;

                if (Log.mode == Mode.ONLY_LOG)
                    throw new Exception("SessionDir cannot be used in Log.Mode.ONLY_LOG");

                path = get_path(name);
                Directory.CreateDirectory(path);
            }

            string get_path(string name)
            {
                string path = WorkDir + @"\Session" + "_" + (string.IsNullOrWhiteSpace(name) ? "" : name + "_") + TimeMark;
                for (int count = 1; Directory.Exists(path); count++)
                    path = WorkDir + @"\Session" + "_" + (string.IsNullOrWhiteSpace(name) ? "" : name + "_") + TimeMark + "_" + count.ToString();
                return path;
            }

            public readonly string Name;

            public string Path
            {
                get
                {
                    return path;
                }
            }
            string path;

            public readonly string TimeMark = DateTime.Now.ToString("yyMMddHHmmss");

            Dictionary<string, NamedLog> names2tl = new Dictionary<string, NamedLog>();

            public void Close(string new_name = null)
            {
                lock (this)
                {
                    if (names2tl.Count < 1)
                        return;

                    Default.Write("Closing the session");

                    string path2 = null;
                    if (new_name != null && new_name != Name)
                    {
                        path2 = get_path(new_name);
                        Default.Write("The session '" + Path + "' will be renamed to '" + path2 + "' just after closing.");
                    }

                    foreach (Thread tl in names2tl.Values)
                        tl.Close();
                    names2tl.Clear();

                    if (new_name != null && new_name != Name)
                        try
                        {
                            Directory.Move(Path, path2);
                            path = path2;
                        }
                        catch (Exception e)
                        {
                            Log.Main.Error(e);
                        }
                }
            }

            public int TotalErrorCount
            {
                get
                {
                    lock (this)
                    {
                        int ec = 0;
                        foreach (Thread tl in names2tl.Values)
                            ec += tl.ErrorCount;
                        return ec;
                    }
                }
            }
            
            public NamedLog this[string name]
            {
                get
                {
                    return NamedLog.Get(this, name);
                }
            }
            
            ///// <summary>
            ///// Output directory for current session
            ///// </summary>
            //public static string OutputDir
            //{
            //    get
            //    {
            //        if (output_dir == null)
            //        {
            //            lock (lock_object)
            //            {
            //                output_dir = SessionDir + @"\" + OutputDirName;

            //                DirectoryInfo di = new DirectoryInfo(output_dir);
            //                if (!di.Exists)
            //                    di.Create();
            //            }
            //        }
            //        return output_dir;
            //    }
            //}
            //static string output_dir = null;

            ///// <summary>
            ///// Output folder name
            ///// </summary>
            //public static string OutputDirName = @"output";

            ///// <summary>
            ///// Download directory for session. 
            ///// This dir can be used to calculate value of downloaded bytes.
            ///// </summary>
            //public static string DownloadDir
            //{
            //    get
            //    {
            //        if (download_dir == null)
            //        {
            //            lock (lock_object)
            //            {
            //                download_dir = SessionDir + "\\" + DownloadDirName;

            //                DirectoryInfo di = new DirectoryInfo(download_dir);
            //                if (!di.Exists)
            //                    di.Create();
            //            }
            //        }
            //        return download_dir;
            //    }
            //}
            //static string download_dir = null;
            //public const string DownloadDirName = "cache";
                        
            public NamedLog Default
            {
                get
                {
                    return NamedLog.Get(this, MAIN_NAMED_LOG);
                }
            }

            internal const string MAIN_NAMED_LOG = "";
                       
            public void Error(Exception e)
            {
                Default.Error(e);
            }

            public void Error(string message)
            {
                Default.Error(message);
            }

            public void Trace(object message = null)
            {
                Default.Trace(message);
            }

            public void Exit(string message)
            {
                Default.Error(message);
            }

            public void Exit(Exception e)
            {
                Default.Exit(e);
            }

            public void Warning(string message)
            {
                Default.Warning(message);
            }

            public void Warning(Exception e)
            {
                Default.Warning(e);
            }

            public void Inform(string message)
            {
                Default.Inform(message);
            }

            public void Write(MessageType type, string message, string details = null)
            {
                Default.Write(type, message, details);
            }

            public void Write(string message)
            {
                Default.Write(MessageType.LOG, message);
            }
        }
    }
}