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
    internal partial class SettingsForm : BaseForm
    {
        internal SettingsForm()
        {
            InitializeComponent();

            CrawlerProcessMaxNumber.Text = Properties.Settings.Default.CrawlerProcessMaxNumber.ToString();
            SmtpHost.Text = Properties.Settings.Default.SmtpHost;
            SmtpLogin.Text = Properties.Settings.Default.SmtpLogin;
            SmtpPassword.Text = Properties.Settings.Default.SmtpPassword;
            SmtpPort.Text = Properties.Settings.Default.SmtpPort.ToString();
            AdminEmailSender.Text = Properties.Settings.Default.EmailSender;
            DefaultAdminEmails.Text = Properties.Settings.Default.DefaultAdminEmails;
            DbConnectionString.Text = DbApi.DbConnectionString;
            if (string.IsNullOrWhiteSpace(DbConnectionString.Text))
                DbConnectionString.Text = Properties.Settings.Default.DbConnectionString;
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.CrawlerProcessMaxNumber = int.Parse(CrawlerProcessMaxNumber.Text);
                Properties.Settings.Default.SmtpHost = SmtpHost.Text;
                Properties.Settings.Default.SmtpLogin = SmtpLogin.Text;
                Properties.Settings.Default.SmtpPassword = SmtpPassword.Text;
                Properties.Settings.Default.SmtpPort = int.Parse(SmtpPort.Text);
                Properties.Settings.Default.EmailSender = AdminEmailSender.Text;
                Properties.Settings.Default.DefaultAdminEmails = DefaultAdminEmails.Text;
                Properties.Settings.Default.Save();
                DbApi.DbConnectionString = DbConnectionString.Text;
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
    }
}
