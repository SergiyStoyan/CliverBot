using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cliver.CrawlerManager
{
    internal partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();
        }

        delegate void _set_control_text(Control c, string m);
        public void SetControlText(Control c, string m)
        {
            if (c.InvokeRequired)
            {
                _set_control_text d = new _set_control_text(SetControlText);
                base.BeginInvoke(d, new object[] { c, m });
            }
            else
                c.Text = m;
        }
        
        public void Invoke(MethodInvoker code)
        {
            if (base.InvokeRequired)
            {
                base.Invoke(code);
                return;
            }
            code.Invoke();
        }

        public void BeginInvoke(MethodInvoker code)
        {
            if (base.InvokeRequired)
            {
                base.BeginInvoke(code);
                return;
            }
            code.Invoke();
        }
    }
}
