//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//        26 November 2014
//Copyright: (C) 2014, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Text.RegularExpressions;
using Cliver.Bot;

namespace Cliver.BotGui
{
    public partial class OutputConfigControl : Cliver.BotGui.ConfigControl
    {
        override public string Section { get { return "Output"; } }

        public OutputConfigControl()
        {
            InitializeComponent();
        }

        override protected void Set()
        {
            set_group_box_values_from_config();

            switch (Cliver.Bot.Settings.Output.FileFormat)
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
                Bot.Settings.Output.FileFormat = FileFormatEnum.CSV;
            else if (_1_TsvFormat.Checked)
                Bot.Settings.Output.FileFormat = FileFormatEnum.TSV;
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
            toolTip1.SetToolTip(this.FileChunkSizeInBytes, "Output data will be recorded in files with this size.");
            toolTip1.SetToolTip(this.FileName, "Name of output file.");
            //toolTip1.SetToolTip(this.OutputFile, ".");
            //toolTip1.SetToolTip(this.OutputFieldSeparator, "Char/string used to separate values in the output file.");
            //toolTip1.SetToolTip(this.OutputFieldSeparatorSubstitute, "Char/string that substitutes the output field separator found within values in the output file.");
            //toolTip1.SetToolTip(this.OutputEmptyFieldSubstitute, "String that substitutes an output field if it is empty.");
            toolTip1.SetToolTip(this.Write2CommonFolder, "Write output file to root of Work Dir irrelatively to sessions.");          
        }

        private void _1_CsvFormat_CheckedChanged(object sender, EventArgs e)
        {
            if (_1_CsvFormat.Checked)
                FileName.Text = Cliver.PathRoutines.ReplaceFileExtention(FileName.Text, "csv");
        }

        private void _1_TsvFormat_CheckedChanged(object sender, EventArgs e)
        {
            if (_1_TsvFormat.Checked)
                FileName.Text = Cliver.PathRoutines.ReplaceFileExtention(FileName.Text, "tsv");
        }
    }
}

