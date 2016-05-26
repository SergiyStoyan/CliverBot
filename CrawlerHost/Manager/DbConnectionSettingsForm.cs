using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cliver.Bot;

namespace Cliver.CrawlerHostManager
{
    public partial class DbConnectionSettingsForm : BaseForm
    {
        public DbConnectionSettingsForm(string db_name, string message, string connection_string)
        {
            InitializeComponent();

            this.Text = Log.EntryAssemblyName;

            lConectionString.Text = "Connection String to " + db_name;

            this.message = message;

            DbConnectionString.Text = connection_string;
        }

        string message = null;

        public string ConnectionString
        {
            get
            {
                return connection_string;
            }
        }
        string connection_string = null;

        private void bOK_Click(object sender, EventArgs e)
        {
            try
            {
                connection_string = DbConnectionString.Text;
                Close();
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
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
