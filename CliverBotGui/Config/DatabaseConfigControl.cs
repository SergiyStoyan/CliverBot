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
    public partial class DatabaseConfigControl : Cliver.BotGui.ConfigControl
    {
        override public string Section { get { return "Database"; } }

        public DatabaseConfigControl()
        {
            InitializeComponent();
        }

        override protected void SetToolTip()
        {
            toolTip1.SetToolTip(this.DbConnectionString, "Database connection string");
        }
    }
}

