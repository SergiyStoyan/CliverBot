using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

namespace Cliver.BotGui
{
    public partial class SessionsForm : BaseForm//Form//
    {
        public SessionsForm()
        {
            InitializeComponent();

            Ok.Enabled = false;

            Sessions.ValueMember = "Value";
            Sessions.DisplayMember = "Name";
            foreach (string d in Directory.EnumerateDirectories(Log.WorkDir))
                if (Directory.Exists(d + "\\" + Cliver.Config.CONFIG_FOLDER_NAME))
                    Sessions.Items.Add(new { Value = d, Name = PathRoutines.GetDirNameFromPath(d) });
        }

        public string SessionDir { get; private set; }

        private void Ok_Click(object sender, EventArgs e)
        {
            if (Sessions.SelectedItem == null)
            {
                Message.Error("No folder is selected.");
                return;
            }
            dynamic d = (dynamic)Sessions.SelectedItem;
            SessionDir = (string)d.Value;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            SessionDir = null;
            Close();
        }

        private void Sessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Sessions.SelectedItem != null)
                Ok.Enabled = true;
        }

        private void Sessions_DoubleClick(object sender, EventArgs e)
        {
            Ok_Click(null, null);
        }
    }
}
