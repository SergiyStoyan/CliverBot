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

namespace Cliver.Bot
{
    /// <summary>
    /// Multithreaded logging routines
    /// </summary>
    public static partial class Log
    {
        public enum Mode
        {
            /// <summary>
            /// Each session creates its own folder.
            /// </summary>
            SESSIONS,
            /// <summary>
            /// Writes only log file without creating session folder.
            /// </summary>
            ONLY_LOG
        }

        /// <summary>
        /// Must be set as first referencing Log in the code.
        /// </summary>
        public static Mode MODE
        {
            set
            {
                if (session_dir != null || work_dir != null)
                    throw new Exception("LOG.MODE can be set only in first referencing Log in code.");
                MODE_ = value;
            }
            get
            {
                return MODE_;
            }
        }
        static Mode MODE_;

        public static Thread MainThread = Thread.CurrentThread;

        /// <summary>
        /// Log belonging to the first (main) thread of the process.
        /// </summary>
        public static ThreadLog Main
        {
            get
            {
                return ThreadLog.Main;
            }
        }

        public static void CloseAll()
        {
            ThreadLog.CloseAll();
        }

        /// <summary>
        /// Log beloning to the current thread.
        /// </summary>
        public static ThreadLog This
        {
            get
            {
                return ThreadLog.This;
            }
        }

        /// <summary>
        /// Log path
        /// </summary>
        public static string Path
        {
            get
            {
                return ThreadLog.This.Path;
            }
        }

        /// <summary>
        /// Log id that is used for logging and browsing in GUI
        /// </summary>
        public static int Id
        {
            get
            {
                return ThreadLog.This.Id;
            }
        }

        /// <summary>
        /// Write the error to the current thread's log
        /// </summary>
        /// <param name="e"></param>
        public static void Error(Exception e)
        {
            ThreadLog.This.Error(e);
        }

        /// <summary>
        /// Write the error to the current thread's log
        /// </summary>
        /// <param name="e"></param>
        static public void Error(string message)
        {
            ThreadLog.This.Error(message);
        }

        /// <summary>
        /// Write the stack informtion for the caller to the current thread's log
        /// </summary>
        /// <param name="e"></param>
        static public void Trace(object message = null)
        {
            ThreadLog.This.Trace(message);
        }

        /// <summary>
        /// Write the error to the current thread's log and terminate the process.
        /// </summary>
        /// <param name="e"></param>
        static public void Exit(string message)
        {
            ThreadLog.This.Error(message);
        }

        /// <summary>
        /// Write the error to the current thread's log and terminate the process.
        /// </summary>
        /// <param name="e"></param>
        static public void Exit(Exception e)
        {
            ThreadLog.This.Exit(e);
        }

        /// <summary>
        /// Write the warning to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        static public void Warning(string message)
        {
            ThreadLog.This.Warning(message);
        }

        /// <summary>
        /// Write the exception as warning to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        static public void Warning(Exception e)
        {
            ThreadLog.This.Warning(e);
        }

        /// <summary>
        /// Write the notification to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        static public void Inform(string message)
        {
            ThreadLog.This.Inform(message);
        }

        /// <summary>
        /// Write the message to the current thread's log.
        /// </summary>
        /// <param name="e"></param>
        static public void Write(MessageType type, string message, string details = null)
        {
            ThreadLog.This.Write(type, message, details);
        }

        static public void Write(string message)
        {
            ThreadLog.This.Write(MessageType.LOG, message);
        }

        public enum MessageType
        {
            LOG = 0,
            INFORM = 1,
            WARNING = 2,
            ERROR = 3,
            EXIT = 4,
            TRACE = 5,
            //INFORM2 = 11,
            //WARNING2 = 21,
            //ERROR2 = 31,
            //EXIT2 = 41,
        }

        /// <summary>
        /// Return stack information for the caller.
        /// </summary>
        /// <returns></returns>
        public static string GetStackString()
        {
            System.Diagnostics.StackTrace st = new StackTrace(true);
            StackFrame sf;
            MethodBase mb = null;
            Type dt = null;
            for (int i = 2; ; i++)
            {
                sf = st.GetFrame(i);
                if (sf == null)
                    break;
                mb = sf.GetMethod();
                dt = mb.DeclaringType;
                if (dt != typeof(Log) && dt != typeof(ThreadLog))
                    break;
            }
            return "Stack: " + dt.ToString() + "::" + mb.Name + " \r\nfile: " + sf.GetFileName() + " \r\nline: " + sf.GetFileLineNumber();
        }

        public static string GetExceptionMessage(Exception e)
        {
            string m;
            string d;
            GetExceptionMessage(e, out m, out d);
            return m + " \r\n\r\n" + d; ;
        }

        static public void GetExceptionMessage(Exception e, out string message, out string details)
        {
            for (; e.InnerException != null; e = e.InnerException) ;
            message = "Exception: \r\n" + e.Message;
#if DEBUG            
            details = "Module:" + e.TargetSite.Module + " \r\n\r\nStack:" + e.StackTrace;
#else                
#endif
        }
    }

    public class TerminatingException : Exception
    {
        public TerminatingException(string message)
            : base(message)
        {
            LogMessage.Exit(message);
        }
    }
}
