//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        03 January 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;

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

        public static bool ButtonAutosize = false;

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
            string cs = e.StackTrace;
            List<string> ms = new List<string>();
            for (; e != null; e = e.InnerException)
                ms.Add(e.Message);
            ms.Add("\r\n" + cs);

            ShowDialog(Application.ProductName, SystemIcons.Error, string.Join("\r\n", ms), new string[1] { "OK" }, 0, owner);
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
            if (owner != null)
            {
                return (int)owner.Invoke(() =>
               {
                   return show_dialog(title, icon, message, buttons, default_button, owner, button_autosize, no_duplicate);
               });
            }
            return show_dialog(title, icon, message, buttons, default_button, owner, button_autosize, no_duplicate);
        }

        static int show_dialog(string title, Icon icon, string message, string[] buttons, int default_button, Form owner, bool? button_autosize = null, bool? no_duplicate = null)
        {
            string caller = null;
            if (no_duplicate != null ? (bool)no_duplicate : NoDuplicate)
            {
                System.Diagnostics.StackTrace st = new StackTrace(true);
                StackFrame sf = st.GetFrame(2);
                caller = sf.GetMethod().Name + "," + sf.GetNativeOffset().ToString();
                string m = null;
                lock (callers2message)
                {
                    if (callers2message.TryGetValue(caller, out m) && m == message)
                        return -1;
                    callers2message[caller] = m;
                }
            }

            MessageForm mf = new MessageForm(title, icon, message, buttons, default_button, owner, button_autosize != null ? (bool)button_autosize : ButtonAutosize);
            mf.ShowInTaskbar = ShowInTaskbar;
            int result = mf.ShowDialog();

            if (no_duplicate != null ? (bool)no_duplicate : NoDuplicate)
                lock (callers2message)
                {
                    callers2message.Remove(caller);
                }

            return result;
        }
        static Dictionary<string, string> callers2message = new Dictionary<string, string>();
    }
}

