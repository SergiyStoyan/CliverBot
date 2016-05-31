using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using Cliver.Bot;

namespace Cliver.BotGui
{
    public partial class InputConfigControl : Cliver.BotGui.ConfigControl
    {
        new public const string NAME = "Input";

        public InputConfigControl()
        {
            InitializeComponent();
            Init(NAME);
        }

        override protected void set_tool_tip()
        {
            toolTip1.SetToolTip(this.InputFieldSeparator, "Char/string used to separate values in the input file.");
            toolTip1.SetToolTip(this.InputFile, "Absolute path or only name of the input file.");
        }

        private void ChooseInputFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.InitialDirectory = PathRoutines.GetDirFromPath(InputFile.Text);
            d.ShowDialog();
            if (!string.IsNullOrWhiteSpace(d.FileName))
                InputFile.Text = d.FileName;
        }

        private void __SetTAB2InputFieldDelimiter_CheckedChanged(object sender, EventArgs e)
        {
            if (_1_SetTAB2InputFieldDelimiter.Checked)
                this.InputFieldSeparator.Text = "\t";
            else
                this.InputFieldSeparator.Text = "";
        }

        private void InputFieldSeparator_TextChanged(object sender, EventArgs e)
        {
            _1_SetTAB2InputFieldDelimiter.Checked = InputFieldSeparator.Text == "\t";
            InputFieldSeparator.Enabled = !_1_SetTAB2InputFieldDelimiter.Checked;
        }

        private void bInputFile_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(InputFile.Text);
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }
    }
}

