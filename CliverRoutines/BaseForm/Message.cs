//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        03 January 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************

//#define BRIEF_ERROR

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Cliver
{
    /// <summary>
    /// Show MessageForm with predefined features
    /// </summary>
    public static class Message
    {
        /// <summary>
        /// Whether the message box must be displayed in the Windows taskbar.
        /// </summary>
        public static bool ShowInTaskbar = true;

        /// <summary>
        /// Owner that is used by default
        /// </summary>
        public static Form Owner = null;

        /// <summary>
        /// Autosize buttons by text
        /// </summary>
        public static bool ButtonAutosize = false;

        /// <summary>
        /// Display only one message box for all same messages throuwn. When the first one is being diplayed, the rest are ignored.
        /// </summary>
        public static bool NoDuplicate = true;

        public static void Inform(string message, Form owner = null)
        {
            ShowDialog(Application.ProductName, SystemIcons.Information, message, new string[1] { "OK" }, 0, owner);
        }

        public static void Exclaim(string message, Form owner = null)
        {
            ShowDialog(Application.ProductName, SystemIcons.Exclamation, message, new string[1] { "OK" }, 0, owner);
        }

        public static void Warning(string message, Form owner = null)
        {
            ShowDialog(Application.ProductName, SystemIcons.Warning, message, new string[1] { "OK" }, 0, owner);
        }
        
        public static void Error(Exception e, Form owner = null)
        {
#if BRIEF_ERROR
            Error(e.Message, owner);
#else
            List<string> ms = new List<string>();
            bool stack_trace_added = false;
            for (; e != null; e = e.InnerException)
            {
                string s = e.Message;
                if (!stack_trace_added && e.StackTrace != null)
                {
                    stack_trace_added = true;
                    s += "\r\n" + e.StackTrace;
                }
                ms.Add(s);
            }

            ShowDialog(Application.ProductName, SystemIcons.Error, string.Join("\r\n=>\r\n", ms), new string[1] { "OK" }, 0, owner);
#endif
        }

        public static void Error(string message, Form owner = null)
        {
            ShowDialog(Application.ProductName, SystemIcons.Error, message, new string[1] { "OK" }, 0, owner);
        }

        public static bool YesNo(string question, Form owner = null)
        {
            return ShowDialog(Application.ProductName, SystemIcons.Question, question, new string[2] { "Yes", "No" }, 0, owner) == 0;
        }

        public static int ShowDialog(string title, Icon icon, string message, string[] buttons, int default_button, Form owner, bool? button_autosize = null, bool? no_duplicate = null)
        {
            owner = owner ?? Owner;
            if (owner == null || !owner.InvokeRequired)
                return show_dialog(title, icon, message, buttons, default_button, owner, button_autosize, no_duplicate);

            return (int)owner.Invoke(() =>
           {
               return show_dialog(title, icon, message, buttons, default_button, owner, button_autosize, no_duplicate);
           });
        }

        static int show_dialog(string title, Icon icon, string message, string[] buttons, int default_button, Form owner, bool? button_autosize = null, bool? no_duplicate = null)
        {
            string caller = null;
            if (no_duplicate ?? NoDuplicate)
            {
                StackTrace st = new StackTrace(true);
                StackFrame sf = null;
                for (int i = 1; i < st.FrameCount; i++)
                {
                    sf = st.GetFrame(i);
                    string file_name = sf.GetFileName();
                    if (file_name == null || !Regex.IsMatch(file_name, @"\\Message\.cs$"))
                        break;
                }
                caller = sf.GetMethod().Name + "," + sf.GetNativeOffset().ToString();
                string m = null;
                lock (callers2message)
                {
                    if (callers2message.TryGetValue(caller, out m) && m == message)
                        return -1;
                    callers2message[caller] = message;
                }
            }

            MessageForm mf = new MessageForm(title, icon, message, buttons, default_button, owner, button_autosize ?? ButtonAutosize);
            mf.ShowInTaskbar = ShowInTaskbar;
            int result = mf.ShowDialog();

            if (no_duplicate ?? NoDuplicate)
                lock (callers2message)
                {
                    callers2message.Remove(caller);
                }

            return result;
        }
        static Dictionary<string, string> callers2message = new Dictionary<string, string>();
    }
}

