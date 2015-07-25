//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace Cliver
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
        }
    }

    public static class ControlRoutines
    {
        /// <summary>
        /// Set text to the control thread-safely.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="m"></param>
        public static void SetText(this Control c, string m)
        {
            if (c.InvokeRequired)
                c.BeginInvoke(new MethodInvoker(() => { c.Text = m; }));
            else
                c.Text = m;
        }

        public static void Invoke(this Control c, MethodInvoker code)
        {
            if (c.InvokeRequired)
            {
                c.Invoke(code);
                return;
            }
            code.Invoke();
        }

        public static void BeginInvoke(this Control c, MethodInvoker code)
        {
            //c.BeginInvoke(code);
            if (c.InvokeRequired)
                c.BeginInvoke(code);
            else
                c.Invoke(code);
        }

        public static void Sleep(int mss)
        {
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, mss);
            while (dt > DateTime.Now)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
        }

        public static bool WaitForCondition(Func<bool> check_condition, int timeout_in_mss)
        {
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout_in_mss);
            do
            {
                if (check_condition())
                    return true;
                Thread.Sleep(10);
                Application.DoEvents();
            }
            while (dt > DateTime.Now);
            return false;
        }
    }
}

