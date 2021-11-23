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
using Cliver.Win;

namespace Cliver.BotGui
{
    public partial class InputConfigControl : Cliver.BotGui.ConfigControl
    {
        override public string Section { get { return "Input"; } }

        public InputConfigControl()
        {
            InitializeComponent();
        }

        override protected void Set()
        {
            set_group_box_values_from_config();

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

        override protected bool Get()
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

            put_control_values_to_config(Name, group_box);
            return true;
        }

        override protected void SetToolTip()
        {
            //toolTip1.SetToolTip(this.InputFieldSeparator, "Char/string used to separate values in the input file.");
            toolTip1.SetToolTip(this.File, "Absolute path or only name of the input file.");
        }

        private void ChooseInputFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.InitialDirectory = PathRoutines.GetFileDir(File.Text);
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

        private void _1_CsvFormat_CheckedChanged(object sender, EventArgs e)
        {
            //if(_1_CsvFormat.Checked)
            //    File.Text = Cliver.PathRoutines.ReplaceFileExtention(File.Text, "csv");
        }

        private void _1_TsvFormat_CheckedChanged(object sender, EventArgs e)
        {
            //if (_1_TsvFormat.Checked)
            //    File.Text = Cliver.PathRoutines.ReplaceFileExtention(File.Text, "tsv");
        }
    }
}