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
using Cliver.Bot;

namespace Cliver.CrawlerHost
{
    internal partial class SysTrayForm : BaseForm
    {
        SysTrayForm()
        {
            InitializeComponent();

            ServiceManager.StateChanged += ServiceManager_StateChanged;
            ServiceManager.Start();
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

        private void show_crawlers(object sender, System.EventArgs e)
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
        
        private void show_services(object sender, System.EventArgs e)
        {
            MessageBox.Show("TBD");
        }

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
            if (!ServiceManager.Work)
                ServiceManager.Work = true;
            else
            {
                StopService.Text = "Stopping...";
                if (crawlers_form != null)
                    crawlers_form.SetControlText(crawlers_form.bStop, "Stopping...");
                ServiceManager.Work = false;
            }
        }

        private void StopService_Click(object sender, System.EventArgs e)
        {
            ToggleService();
        }

        void ServiceManager_StateChanged(bool started)
        {
            if (started)
            {
                Invoke(() =>
                {
                    StopService.Text = "Stop Manager";
                    this.notifyIcon1.Icon = this.Icon;
                    if (crawlers_form != null)
                        crawlers_form.SetControlText(crawlers_form.bStop, "Stop");
                });
            }
            else
            {
                Invoke(() =>
                {
                    StopService.Text = "Start Manager";

                    Bitmap i = this.Icon.ToBitmap();
                    Graphics g = Graphics.FromImage(i);
                    ControlPaint.DrawImageDisabled(g, i, 0, 0, Color.Transparent);
                    this.notifyIcon1.Icon = Icon.FromHandle(i.GetHicon());
         
                    if (crawlers_form != null)
                        crawlers_form.SetControlText(crawlers_form.bStop, "Start");
                });
            }
        }

        private void menuHelp_Click(object sender, EventArgs e)
        {
            Process.Start(Properties.Settings.Default.HelpUri);
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            AboutForm f = new AboutForm();
            f.ShowDialog();
        }

        private void mManagerSettings_Click(object sender, EventArgs e)
        {
            SettingsForm f = new SettingsForm();
            f.ShowDialog();
        }

        private void mHostSettings_Click(object sender, EventArgs e)
        {
            HostSettingsForm f = new HostSettingsForm();
            f.ShowDialog();
        }
    }
}
