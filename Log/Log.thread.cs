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

namespace Cliver.Bot
{
    public class ThreadLog
    {
        ThreadLog(int id, string log_file)
        {
            this.Id = id;
            this.Path = log_file;
        }

        const int MAIN_THREAD_LOG_ID = -1;

        static Dictionary<Thread, ThreadLog> thread2tls = new Dictionary<Thread, ThreadLog>();

        static ThreadLog get_thread_log(Thread thread)
        {
            lock (thread2tls)
            {
                ThreadLog tl;
                if (!thread2tls.TryGetValue(thread, out tl))
                {
                    try
                    {
                        //cleanup for dead thread logs
                        List<Thread> old_log_keys = (from t in thread2tls.Keys where !t.IsAlive select t).ToList();
                        foreach (Thread t in old_log_keys)
                        {
                            t.Abort();
                            thread2tls[t].Error("This thread was detected to be not alive. Aborting it...");
                            thread2tls[t].Close();
                            thread2tls.Remove(t);
                        }

                        int log_id;
                        if (thread == Log.MainThread)
                            log_id = MAIN_THREAD_LOG_ID;
                        else
                        {
                            log_id = 1;
                            var ids = from x in thread2tls.Keys orderby thread2tls[x].Id select thread2tls[x].Id;
                            foreach (int id in ids)
                                if (log_id == id) log_id++;
                        }

                        string log_file;
                        switch (Log.LOGGING_MODE)
                        {
                            case Log.LoggingMode.ONLY_LOG:
                                log_file = Log.WorkDir + @"\" + Log.ProcessName;
                                break;
                            case Log.LoggingMode.SESSIONS:
                                log_file = Log.SessionDir + @"\" + Log.ProcessName;
                                break;
                            default:
                                throw new Exception("Unknown LOGGING_MODE:" + Log.LOGGING_MODE);
                        }
                        if (log_id < 0)
                            log_file += "_" + Log.TimeMark + ".log";
                        else
                            log_file += "_" + log_id.ToString() + "_" + Log.TimeMark + ".log";

                        tl = new ThreadLog(log_id, log_file);
                        thread2tls.Add(thread, tl);
                    }
                    catch (Exception e)
                    {
                        Log.Main.Error(e);
                    }
                }
                return tl;
            }
        }

        /// <summary>
        /// Log belonging to the first (main) thread of the process.
        /// </summary>
        public static ThreadLog Main
        {
            get
            {
                return get_thread_log(Log.MainThread);
            }
        }

        /// <summary>
        /// Log beloning to the current thread.
        /// </summary>
        public static ThreadLog This
        {
            get
            {
                return get_thread_log(Thread.CurrentThread);
            }
        }

        public static void CloseAll()
        {
            lock (thread2tls)
            {
                foreach (ThreadLog tl in thread2tls.Values)
                    tl.Close();
                thread2tls.Clear();
            }
        }

        /// <summary>
        /// Log id that is used for logging and browsing in GUI
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// Log path
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// Used to close Log
        /// </summary>
        public void Close()
        {
            lock (this)
            {
                if (log_writer != null)
                    log_writer.Close();
                log_writer = null;
            }
        }

        /// <summary>
        /// Write the error to the current thread's log
        /// </summary>
        /// <param name="e"></param>
        public void Error(Exception e)
        {
            lock (this)
            {
                Write("ERROR: " + Log.GetExceptionMessage(e));
            }
        }

        /// <summary>
        /// Write the error to the current thread's log
        /// </summary>
        /// <param name="e"></param>
        public void Error(string message)
        {
            lock (this)
            {
                Write("ERROR: " + message + "\r\n" + Log.GetStackString());
            }
        }

        /// <summary>
        /// Write the stack informtion for the caller to the current thread's log
        /// </summary>
        /// <param name="e"></param>
        public void Trace(object message = null)
        {
            lock (this)
            {
                if (message != null)
                    Write("TRACE: " + message.ToString() + "\r\n" + Log.GetStackString());
                else
                    Write("TRACE: " + Log.GetStackString());
            }
        }

        /// <summary>
        /// Write the error to the current thread's log and terminate the process.
        /// </summary>
        /// <param name="e"></param>
        public void Exit(string message)
        {
            lock (this)
            {
                Write("EXIT: " + message + "\r\nStack: " + Log.GetStackString());
                if (Id >= 0)
                    Log.Main.Exit("Exited due to thread #" + Id.ToString() + ". See the respective Log");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Write the error to the current thread's log and terminate the process.
        /// </summary>
        /// <param name="e"></param>
        public void Exit(Exception e)
        {
            lock (this)
            {
                Write("EXIT: " + Log.GetExceptionMessage(e));
                if (Id >= 0)
                    Log.Main.Exit("Exited due to thread #" + Id.ToString() + ". See the respective Log");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Write the warning to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        public void Warning(string message)
        {
            lock (this)
            {
                Write("WARNING: " + message);
            }
        }

        /// <summary>
        /// Write the exception as warning to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        public void Warning(Exception e)
        {
            lock (this)
            {
                Write("WARNING: " + Log.GetExceptionMessage(e));
            }
        }

        /// <summary>
        /// Write the notification to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        public void Inform(string message)
        {
            lock (this)
            {
                Write("INFORM: " + message);
            }
        }

        /// <summary>
        /// Write the message to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        public void Write(string message)
        {
            if (!Properties.Log.Default.WriteLog)
                return;

            lock (this)
            {
                if (log_writer == null)
                    log_writer = new StreamWriter(Path, true);

                log_writer.WriteLine(DateTime.Now.ToString("[dd-MM-yy HH:mm:ss] ") + message);
                log_writer.Flush();
            }
        }
        TextWriter log_writer = null;
    }
}
