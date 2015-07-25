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

        public static int ShowDialog(string title, Icon icon, string message, string[] buttons, int default_button, Form owner)
        {
            MessageForm mf = new MessageForm(title, icon, message, buttons, default_button, owner);
            mf.ShowInTaskbar = ShowInTaskbar;
            return mf.ShowDialog();
        }
    }
}

