using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cliver.Bot;

namespace Cliver.CrawlerHost
{
    public partial class HostSettingsForm : BaseForm
    {
        public HostSettingsForm()
        {
            InitializeComponent();

            CrawlerHostFolder.Text = Properties.Host.Default.CrawlerHostPath;
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!System.IO.Directory.Exists(CrawlerHostFolder.Text))
                {
                    LogMessage.Error("Folder does not exist: " + CrawlerHostFolder.Text);
                    return;
                }
                Properties.Host.Default.CrawlerHostPath = CrawlerHostFolder.Text;
                Properties.Host.Default.Save();
                Close();
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PickFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.DefaultExt = "mdf";
            d.ShowDialog();
            CrawlerHostFolder.Text = @"Data Source=.\SQLEXPRESS;AttachDbFilename=" + d.FileName + ";Integrated Security=True;Connect Timeout=30";
        }

        private void PickFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.ShowDialog();
            CrawlerHostFolder.Text = d.SelectedPath;
        }
    }
}
