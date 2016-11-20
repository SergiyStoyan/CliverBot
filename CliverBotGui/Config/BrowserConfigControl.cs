//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//        26 November 2014
//Copyright: (C) 2014, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Cliver.BotGui
{
    public partial class BrowserConfigControl : Cliver.BotGui.ConfigControl
    {
       override public string Section { get { return "Browser"; } }

        public BrowserConfigControl()
        {
            InitializeComponent();
        }
        
        override protected void SetToolTip()
        {
            toolTip1.SetToolTip(this.PageCompletedTimeoutInSeconds, "Timeout for waiting IE completed downloading page event.");
            toolTip1.SetToolTip(this.CloseWebBrowserDialogsAutomatically, "When checked the bot will close IE dialog boxes automatically.");
            toolTip1.SetToolTip(this.SuppressScriptErrors, "When checked, IE will not throw error messages as of script errors.");
        }
    }
}

