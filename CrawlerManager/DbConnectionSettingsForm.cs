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
    public partial class DbConnectionSettingsForm : BaseForm
    {
        public DbConnectionSettingsForm(string message, string connection_string)
        {
            InitializeComponent();

            this.message = message;

            DbConnectionString.Text = connection_string;
        }

        string message = null;

        public string ConnectionString
        {
            get
            {
                return string.IsNullOrWhiteSpace(DbConnectionString.Text) ? null : DbConnectionString.Text;
            }
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            DbConnectionString.Text = null;
            Close();
        }

        private void PickFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.DefaultExt = "mdf";
            d.ShowDialog();
            DbConnectionString.Text = @"Data Source=.\SQLEXPRESS;AttachDbFilename=" + d.FileName + ";Integrated Security=True;Connect Timeout=30";
        }

        private void DbConnectionSettingsForm_Shown(object sender, EventArgs e)
        {
            if (message == null)
                return;
            bool o2c = LogMessage.Output2Console;
            LogMessage.Output2Console = false;
            LogMessage.Error(message);
            LogMessage.Output2Console = o2c;
        }
    }
}
