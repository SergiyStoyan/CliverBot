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

namespace Cliver.BotGui
{
    public partial class OutputConfigControl : Cliver.BotGui.ConfigControl
    {
        new public readonly string NAME = "Output";

        public OutputConfigControl()
        {
            InitializeComponent();
            Init(NAME);
        }

        override protected void set_tool_tip()
        {
            toolTip1.SetToolTip(this.OutputFileChunkSizeInBytes, "Output data will be recorded in files with this size.");
            toolTip1.SetToolTip(this.OutputFileName, "Name of output file.");
            //toolTip1.SetToolTip(this.OutputFile, ".");
            toolTip1.SetToolTip(this.OutputFieldSeparator, "Char/string used to separate values in the output file.");
            toolTip1.SetToolTip(this.OutputFieldSeparatorSubstitute, "Char/string that substitutes the output field separator found within values in the output file.");
            toolTip1.SetToolTip(this.OutputEmptyFieldSubstitute, "String that substitutes an output field if it is empty.");
            toolTip1.SetToolTip(this.WriteOutputFile2CommonFolder, "Write output file to root of Work Dir irrelatively to sessions.");          
        }

        private void OutputFieldSeparator_TextChanged(object sender, EventArgs e)
        {
            _1_SetTAB2OutputFieldDelimiter.Checked = this.OutputFieldSeparator.Text == "\t";
            _1_SetTAB2OutputFieldDelimiter.Enabled = !_1_SetTAB2OutputFieldDelimiter.Checked;
            if (this.OutputFieldSeparator.Text == "\t")
            {
                this.OutputFieldSeparatorSubstitute.Text = " ";
                if (string.IsNullOrEmpty(this.OutputFileName.Text))
                    this.OutputFileName.Text = "output.tsv";
                else
                    this.OutputFileName.Text = Regex.Replace(this.OutputFileName.Text, @"([^\.]*)?$", "", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline) + "tsv";
            }
            else
                if (this.OutputFieldSeparator.Text == ",")
                {
                    this.OutputFieldSeparatorSubstitute.Text = ";";
                    if (string.IsNullOrEmpty(this.OutputFileName.Text))
                        this.OutputFileName.Text = "output.csv";
                    else
                        this.OutputFileName.Text = Regex.Replace(this.OutputFileName.Text, @"([^\.]*)?$", "", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline) + "csv";
                }
        }

        private void __SetTAB2OutputFieldDelimiter_CheckedChanged(object sender, EventArgs e)
        {
            if (_1_SetTAB2OutputFieldDelimiter.Checked)
                this.OutputFieldSeparator.Text = "\t";
            else
                this.OutputFieldSeparator.Text = "";
        }

        //private void flagWriteOutputFile2WorkFolder_CheckedChanged(object sender, EventArgs e)
        //{
        //    this.OutputFileName.Text = Regex.Replace(this.OutputFileName.Text, @"..\", "", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        //    if(WriteOutputFile2CommonFolder)
        //        this.OutputFileName.Text = @"\..\.." + this.OutputFileName.Text;
        //}

        //private void chooseOutputFolder_Click(object sender, EventArgs e)
        //{
        //    FolderBrowserDialog d = new FolderBrowserDialog();
        //    d.ShowDialog();
        //    this.OutputFile.Text = d.SelectedPath + "\\output";
        //    OutputFieldSeparator_TextChanged(null, null);
        //}
    }
}

