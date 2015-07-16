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
        public DbConnectionSettingsForm(string message)
        {
            InitializeComponent();
            
            this.message = message;

            DbConnectionString.Text = DbApi.ConnectionString;
            if (string.IsNullOrWhiteSpace(DbConnectionString.Text))
                DbConnectionString.Text = Properties.Settings.Default.DbConnectionString;
        }

        string message = null;

        private void bOK_Click(object sender, EventArgs e)
        {
            try
            {
                DbApi.ConnectionString = DbConnectionString.Text;
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
            DbConnectionString.Text = @"Data Source=.\SQLEXPRESS;AttachDbFilename=" + d.FileName + ";Integrated Security=True;Connect Timeout=30";
        }

        private void DbConnectionSettingsForm_Shown(object sender, EventArgs e)
        {
            if (message != null)
                LogMessage.Inform(message);
        }
    }
}
