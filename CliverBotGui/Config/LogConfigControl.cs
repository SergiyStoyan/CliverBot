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
    public partial class LogConfigControl : Cliver.BotGui.ConfigControl
    {
        new public const string NAME = "Log";

        public LogConfigControl()
        {
            InitializeComponent();
            Init(NAME);
        }

        override protected void SetToolTip()
        {
            toolTip1.SetToolTip(this.DeleteLogsOlderDays, "Old session folders contaning Log files and downloaded pages, that are older than this day number will be deleted.");
            toolTip1.SetToolTip(this.LogDownloadedFiles, "When checked, the bot will store all downloaded files to disk.");
            toolTip1.SetToolTip(this.WriteLog, "Define whether the bot will write Log files.");
            toolTip1.SetToolTip(this.PreWorkDir, "Absolute or relative path where the work folder of the bot will be created. Can be empty.");
            toolTip1.SetToolTip(this.LogPostRequestParameters, "Whether http post parameters are to be saved in cache.");                   
        }

        private void bPickDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            //d.SelectedPath = Log.WorkDir;
            d.ShowDialog();
            PreWorkDir.Text = d.SelectedPath;
        }
    }
}

