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
        new public const string NAME = "Database";

        public DatabaseConfigControl()
        {
            InitializeComponent();
            Init(NAME);
        }

        override protected void set_tool_tip()
        {
            toolTip1.SetToolTip(this.DbConnectionString, "Database connection string");
        }
    }
}

