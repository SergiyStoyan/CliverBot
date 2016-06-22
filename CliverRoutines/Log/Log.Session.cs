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
        public partial class Session
        {
            static Dictionary<string, Session> names2session = new Dictionary<string, Session>();

            Session(string name, Session parent_session = null)
            {
                Name = name;

                if (Log.mode == Mode.ONLY_LOG)
                    throw new Exception("SessionDir cannot be used in Log.Mode.ONLY_LOG");

                if (parent_session == null)
                    session_dir = WorkDir + @"\Session" + "_" + Name + "_" + TimeMark;
                else
                    session_dir = parent_session.Path + "_" + Name;

                DirectoryInfo di = new DirectoryInfo(session_dir);
                for (int count = 0; di.Exists; )
                {
                    count++;
                    session_dir = WorkDir + @"\Session" + "_" + Log.TimeMark + "_" + count.ToString();
                    di = new DirectoryInfo(session_dir);
                }
                di.Create();

                Path = session_dir;
            }
            
            ~Session()
            {
                Close();
            }

            public readonly string Name;
            public readonly string Path;
            public readonly string TimeMark = DateTime.Now.ToString("yyMMddHHmmss");

            Dictionary<string, NamedLog> names2tl = new Dictionary<string, NamedLog>();

            public static Session Get(string name)
            {
                lock (names2session)
                {
                    Session session;
                    if (!names2session.TryGetValue(name, out session))
                    {
                        session = new Session(name);
                        names2session[name] = session;
                    }
                    return session;
                }
            }

            //public void Rename(string name)
            //{
            //    lock (names2session)
            //    {
            //        Session session;
            //        if (!names2session.TryGetValue(name, out session))
            //        {
            //            session = new Session(name);
            //            names2session[name] = session;
            //        }
            //        return session;
            //    }
            //}

            //public Session Get(string name)
            //{
            //    lock (names2session)
            //    {
            //        this.n

            //        Session session;
            //        if (!names2session.TryGetValue(name, out session))
            //        {
            //            session = new Session(name);
            //            names2session[name] = session;
            //        }
            //        return session;
            //    }
            //}

            //public static void Close(string name)
            //{
            //    lock (names2session)
            //    {
            //        names2session.Remove(name);
            //    }
            //}

            public static void CloseAll()
            {
                lock (names2session)
                {
                    foreach (Session s in names2session.Values)
                        s.Close();
                    names2session.Clear();
                }
            }

            public static Session Default
            {
                get
                {
                    return Get(SINGLE_SESSION_NAME);
                }
            }

            internal const string SINGLE_SESSION_NAME = "";

            public void Close()
            {
                lock (names2tl)
                {
                    foreach (Thread tl in names2tl.Values)
                        tl.Close();
                    names2tl.Clear();
                }
                lock (names2session)
                {
                    names2session.Remove(this.Name);
                }
            }

            public int TotalErrorCount
            {
                get
                {
                    lock (names2tl)
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
                        
            public NamedLog DefaultLog
            {
                get
                {
                    return NamedLog.Get(this, MAIN_NAMED_LOG);
                }
            }

            internal const string MAIN_NAMED_LOG = "";
                       
            public void Error(Exception e)
            {
                DefaultLog.Error(e);
            }

            public void Error(string message)
            {
                DefaultLog.Error(message);
            }

            public void Trace(object message = null)
            {
                DefaultLog.Trace(message);
            }

            public void Exit(string message)
            {
                DefaultLog.Error(message);
            }

            public void Exit(Exception e)
            {
                DefaultLog.Exit(e);
            }

            public void Warning(string message)
            {
                DefaultLog.Warning(message);
            }

            public void Warning(Exception e)
            {
                DefaultLog.Warning(e);
            }

            public void Inform(string message)
            {
                DefaultLog.Inform(message);
            }

            public void Write(MessageType type, string message, string details = null)
            {
                DefaultLog.Write(type, message, details);
            }

            public void Write(string message)
            {
                DefaultLog.Write(MessageType.LOG, message);
            }
        }
    }
}