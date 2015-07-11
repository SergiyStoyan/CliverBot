namespace Cliver.BotGui
{
    partial class OutputConfigControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.WriteOutputFile2CommonFolder = new System.Windows.Forms.CheckBox();
            this.label36 = new System.Windows.Forms.Label();
            this.OutputEmptyFieldSubstitute = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.OutputFieldSeparatorSubstitute = new System.Windows.Forms.TextBox();
            this._1_SetTAB2OutputFieldDelimiter = new System.Windows.Forms.CheckBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.OutputFileChunkSizeInBytes = new System.Windows.Forms.TextBox();
            this.OutputFileName = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.OutputFieldSeparator = new System.Windows.Forms.TextBox();
            this.group_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.Controls.Add(this.WriteOutputFile2CommonFolder);
            this.group_box.Controls.Add(this.label36);
            this.group_box.Controls.Add(this.OutputEmptyFieldSubstitute);
            this.group_box.Controls.Add(this.label35);
            this.group_box.Controls.Add(this.OutputFieldSeparatorSubstitute);
            this.group_box.Controls.Add(this._1_SetTAB2OutputFieldDelimiter);
            this.group_box.Controls.Add(this.label33);
            this.group_box.Controls.Add(this.label29);
            this.group_box.Controls.Add(this.OutputFileChunkSizeInBytes);
            this.group_box.Controls.Add(this.OutputFileName);
            this.group_box.Controls.Add(this.label27);
            this.group_box.Controls.Add(this.label26);
            this.group_box.Controls.Add(this.label13);
            this.group_box.Controls.Add(this.OutputFieldSeparator);
            this.group_box.Text = "TestCustom";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 100000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // WriteOutputFile2CommonFolder
            // 
            this.WriteOutputFile2CommonFolder.AutoSize = true;
            this.WriteOutputFile2CommonFolder.Location = new System.Drawing.Point(35, 56);
            this.WriteOutputFile2CommonFolder.Name = "WriteOutputFile2CommonFolder";
            this.WriteOutputFile2CommonFolder.Size = new System.Drawing.Size(259, 17);
            this.WriteOutputFile2CommonFolder.TabIndex = 68;
            this.WriteOutputFile2CommonFolder.Text = "Write Output File to WorkDir (irrelative of Session)";
            this.WriteOutputFile2CommonFolder.UseVisualStyleBackColor = true;
            // 
            // label36
            // 
            this.label36.Location = new System.Drawing.Point(43, 217);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(138, 17);
            this.label36.TabIndex = 67;
            this.label36.Text = "Empty Field Substitute:";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // OutputEmptyFieldSubstitute
            // 
            this.OutputEmptyFieldSubstitute.Location = new System.Drawing.Point(190, 215);
            this.OutputEmptyFieldSubstitute.Name = "OutputEmptyFieldSubstitute";
            this.OutputEmptyFieldSubstitute.Size = new System.Drawing.Size(144, 20);
            this.OutputEmptyFieldSubstitute.TabIndex = 66;
            this.OutputEmptyFieldSubstitute.Text = " ";
            // 
            // label35
            // 
            this.label35.Location = new System.Drawing.Point(43, 185);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(138, 17);
            this.label35.TabIndex = 65;
            this.label35.Text = "Field Delimiter Substitute:";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // OutputFieldSeparatorSubstitute
            // 
            this.OutputFieldSeparatorSubstitute.Location = new System.Drawing.Point(190, 183);
            this.OutputFieldSeparatorSubstitute.Name = "OutputFieldSeparatorSubstitute";
            this.OutputFieldSeparatorSubstitute.Size = new System.Drawing.Size(17, 20);
            this.OutputFieldSeparatorSubstitute.TabIndex = 64;
            this.OutputFieldSeparatorSubstitute.Text = " ";
            // 
            // _1_SetTAB2OutputFieldDelimiter
            // 
            this._1_SetTAB2OutputFieldDelimiter.Appearance = System.Windows.Forms.Appearance.Button;
            this._1_SetTAB2OutputFieldDelimiter.Location = new System.Drawing.Point(228, 151);
            this._1_SetTAB2OutputFieldDelimiter.Name = "_1_SetTAB2OutputFieldDelimiter";
            this._1_SetTAB2OutputFieldDelimiter.Size = new System.Drawing.Size(75, 23);
            this._1_SetTAB2OutputFieldDelimiter.TabIndex = 63;
            this._1_SetTAB2OutputFieldDelimiter.Text = "Set TAB";
            this._1_SetTAB2OutputFieldDelimiter.UseVisualStyleBackColor = true;
            this._1_SetTAB2OutputFieldDelimiter.CheckedChanged += new System.EventHandler(this.@__SetTAB2OutputFieldDelimiter_CheckedChanged);
            // 
            // label33
            // 
            this.label33.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label33.Location = new System.Drawing.Point(14, 249);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(483, 2);
            this.label33.TabIndex = 62;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(16, 96);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(168, 17);
            this.label29.TabIndex = 61;
            this.label29.Text = "Output File Chunk Size (bytes):";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // OutputFileChunkSizeInBytes
            // 
            this.OutputFileChunkSizeInBytes.Location = new System.Drawing.Point(190, 96);
            this.OutputFileChunkSizeInBytes.Name = "OutputFileChunkSizeInBytes";
            this.OutputFileChunkSizeInBytes.Size = new System.Drawing.Size(141, 20);
            this.OutputFileChunkSizeInBytes.TabIndex = 60;
            this.OutputFileChunkSizeInBytes.Text = "0";
            // 
            // OutputFileName
            // 
            this.OutputFileName.Location = new System.Drawing.Point(130, 28);
            this.OutputFileName.Name = "OutputFileName";
            this.OutputFileName.Size = new System.Drawing.Size(342, 20);
            this.OutputFileName.TabIndex = 59;
            // 
            // label27
            // 
            this.label27.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label27.Location = new System.Drawing.Point(15, 136);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(483, 2);
            this.label27.TabIndex = 58;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(32, 29);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(92, 13);
            this.label26.TabIndex = 57;
            this.label26.Text = "Output File Name:";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(95, 156);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(83, 17);
            this.label13.TabIndex = 56;
            this.label13.Text = "Field Delimiter:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // OutputFieldSeparator
            // 
            this.OutputFieldSeparator.Location = new System.Drawing.Point(190, 154);
            this.OutputFieldSeparator.Name = "OutputFieldSeparator";
            this.OutputFieldSeparator.Size = new System.Drawing.Size(17, 20);
            this.OutputFieldSeparator.TabIndex = 55;
            this.OutputFieldSeparator.Text = ",";
            this.OutputFieldSeparator.TextChanged += new System.EventHandler(this.OutputFieldSeparator_TextChanged);
            // 
            // OutputConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "OutputConfigControl";
            this.group_box.ResumeLayout(false);
            this.group_box.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox WriteOutputFile2CommonFolder;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox OutputEmptyFieldSubstitute;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox OutputFieldSeparatorSubstitute;
        private System.Windows.Forms.CheckBox _1_SetTAB2OutputFieldDelimiter;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox OutputFileChunkSizeInBytes;
        private System.Windows.Forms.TextBox OutputFileName;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox OutputFieldSeparator;



    }
}
