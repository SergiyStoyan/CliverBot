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
            this.Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
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

        public static object Invoke(this Control c, Func<object> function)
        {
            return c.Invoke(function);
        }

        public static object Invoke(this Control c, MethodInvoker code)
        {
            return c.Invoke(code);
        }

        public static void BeginInvoke(this Control c, MethodInvoker code)
        {
            c.BeginInvoke(code);
        }
        
        public static void SlideVertically(this Control c, uint mss, int p2, MethodInvoker finished = null)
        {
            //if (st != null && st.IsAlive)
            //    return;

            int delta = c.Top > p2 ? -1 : 1;
            int sleep = (int)((double)mss / ((p2 - c.Top) / delta));
            ThreadRoutines.Start(() =>
            {
                while (
                    !(bool)ControlRoutines.Invoke(c, () =>
                    {
                        c.Top = c.Top + delta;
                        return delta < 0 ? c.Top <= p2 : c.Top >= p2;
                    })
                )
                    System.Threading.Thread.Sleep(sleep);
                ControlRoutines.Invoke(c, () =>
                    {
                        finished?.Invoke();
                    });
            });
        }
        //System.Threading.Thread st = null;
        
        public static void Condense(this Form f, uint mss, double o2, MethodInvoker finished = null)
        {
            //    if (ct != null && ct.IsAlive)
            //        return;

            double delta = f.Opacity < o2 ? 0.01 : -0.01;
            int sleep = (int)((double)mss / ((o2 - f.Opacity) / delta));
            ThreadRoutines.Start(() =>
            {
                while (
                    !(bool)ControlRoutines.Invoke(f, () =>
                    {
                        f.Opacity = f.Opacity + delta;
                        return delta > 0 ? f.Opacity >= o2 : f.Opacity <= o2;
                    })
                )
                    System.Threading.Thread.Sleep(sleep);
                ControlRoutines.Invoke(f, () =>
                {
                    finished?.Invoke();
                });
            });
        }
    }
}

