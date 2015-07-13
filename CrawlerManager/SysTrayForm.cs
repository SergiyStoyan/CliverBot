//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        03 January 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Settings = Cliver.CrawlerHost.Properties.Settings;
using Cliver.Bot;

namespace Cliver.CrawlerHost
{
    internal partial class SysTrayForm : BaseForm
    {
        SysTrayForm()
        {
            InitializeComponent();
        }

        static internal SysTrayForm This
        {
            get
            {
                if (This_ == null)
                {
                    This_ = new SysTrayForm();
                }
                return This_;
            }
        }
        static SysTrayForm This_;

        private void show(object sender, System.EventArgs e)
        {
            if (crawlers_form != null)
            {
                crawlers_form.Activate();
                return;
            }
            crawlers_form = new CrawlersForm();
            crawlers_form.ShowDialog();
            crawlers_form = null;
        }
        CrawlersForm crawlers_form = null;

        internal void RefreshView()
        {
            if (crawlers_form == null)
                return;
            crawlers_form.RefreshView();
        }

        private void exit(object sender, System.EventArgs e)
        {
            Log.Main.Inform("QUIT");
            Environment.Exit(0);
        }

        public void ToggleService()
        {
            if (!Manager.Started)
                Manager.Start();
            else
            {
                StopService.Text = "Stopping...";
                if (crawlers_form != null)
                    crawlers_form.SetControlText(crawlers_form.bStop, "Stopping...");
                Manager.Stop();
            }
        }

        private void StopService_Click(object sender, System.EventArgs e)
        {
            ToggleService();
        }

        private void CheckNow_Click(object sender, EventArgs e)
        {
            CheckNow();
        }

        internal void CheckNow()
        {
            //CrawlerManager.RunManager.Set();
            Manager.CheckNow();
        }

        internal bool Started
        {
            set
            {
                if (value)
                {
                    Invoke(() =>
                    {
                        StopService.Text = "Stop Service";
                    });
                    if (crawlers_form != null)
                        crawlers_form.SetControlText(crawlers_form.bStop, "Stop");
                }
                else
                {
                    Invoke(() =>
                    {
                        StopService.Text = "Start Service";
                    });
                    if (crawlers_form != null)
                        crawlers_form.SetControlText(crawlers_form.bStop, "Start");
                }
            }
        }

        internal bool CheckingNow
        {
            set
            {
                if (value)
                {
                    Invoke(() =>
                    {
                        mCheckNow.Text = "Checking...";
                    });
                    if (crawlers_form != null)
                        crawlers_form.SetControlText(crawlers_form.bCheckNow, "Checking...");
                }
                else
                {
                    Invoke(() =>
                    {
                        mCheckNow.Text = "Check Now";
                    });
                    if (crawlers_form != null)
                        crawlers_form.SetControlText(crawlers_form.bCheckNow, "Check Now");
                }
            }
        }

        private void SysTray_Load(object sender, System.EventArgs e)
        {
        }

        private void menuHelp_Click(object sender, EventArgs e)
        {
            Process.Start(Settings.Default.HelpUri);
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            AboutForm f = new AboutForm();
            f.ShowDialog();
        }

        private void menuSettings_Click(object sender, EventArgs e)
        {
            SettingsForm f = new SettingsForm();
            f.ShowDialog();
        }
    }
}
