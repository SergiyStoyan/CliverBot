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

        override protected void Loading()
        {
            switch (Cliver.Bot.Settings.Input.FileFormat)
            {
                case FileFormatEnum.NULL:
                    _1_CsvFormat.Checked = false;
                    _1_TsvFormat.Checked = false;
                    break;
                case FileFormatEnum.CSV:
                    _1_CsvFormat.Checked = true;
                    break;
                case FileFormatEnum.TSV:
                    _1_TsvFormat.Checked = true;
                    break;
            }
        }

        override protected bool Saving()
        {
            if (_1_CsvFormat.Checked)
                Bot.Settings.Input.FileFormat = FileFormatEnum.CSV;
            else if (_1_TsvFormat.Checked)
                Bot.Settings.Input.FileFormat = FileFormatEnum.TSV;
            else
            {
                Message.Error("File format is not defined.");
                return false;
            }
            return true;
        }

        override protected void set_tool_tip()
        {
            //toolTip1.SetToolTip(this.InputFieldSeparator, "Char/string used to separate values in the input file.");
            toolTip1.SetToolTip(this.File, "Absolute path or only name of the input file.");
        }

        private void ChooseInputFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.InitialDirectory = PathRoutines.GetDirFromPath(File.Text);
            d.ShowDialog();
            if (!string.IsNullOrWhiteSpace(d.FileName))
                File.Text = d.FileName;
        }

        private void bInputFile_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(File.Text);
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }
    }
}