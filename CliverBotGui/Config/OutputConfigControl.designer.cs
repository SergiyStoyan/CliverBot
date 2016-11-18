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
            this.Write2CommonFolder = new System.Windows.Forms.CheckBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.FileChunkSizeInBytes = new System.Windows.Forms.TextBox();
            this.FileName = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.Append = new System.Windows.Forms.CheckBox();
            this._1_CsvFormat = new System.Windows.Forms.RadioButton();
            this._1_TsvFormat = new System.Windows.Forms.RadioButton();
            this.label20 = new System.Windows.Forms.Label();
            this.group_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.Controls.Add(this._1_CsvFormat);
            this.group_box.Controls.Add(this._1_TsvFormat);
            this.group_box.Controls.Add(this.label20);
            this.group_box.Controls.Add(this.Append);
            this.group_box.Controls.Add(this.Write2CommonFolder);
            this.group_box.Controls.Add(this.label33);
            this.group_box.Controls.Add(this.label29);
            this.group_box.Controls.Add(this.FileChunkSizeInBytes);
            this.group_box.Controls.Add(this.FileName);
            this.group_box.Controls.Add(this.label27);
            this.group_box.Controls.Add(this.label26);
            this.group_box.Text = "TestCustom";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 100000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // Write2CommonFolder
            // 
            this.Write2CommonFolder.AutoSize = true;
            this.Write2CommonFolder.Location = new System.Drawing.Point(35, 56);
            this.Write2CommonFolder.Name = "Write2CommonFolder";
            this.Write2CommonFolder.Size = new System.Drawing.Size(259, 17);
            this.Write2CommonFolder.TabIndex = 68;
            this.Write2CommonFolder.Text = "Write Output File to WorkDir (irrelative of Session)";
            this.Write2CommonFolder.UseVisualStyleBackColor = true;
            // 
            // label33
            // 
            this.label33.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label33.Location = new System.Drawing.Point(14, 286);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(483, 2);
            this.label33.TabIndex = 62;
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(16, 133);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(168, 17);
            this.label29.TabIndex = 61;
            this.label29.Text = "Output File Chunk Size (bytes):";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FileChunkSizeInBytes
            // 
            this.FileChunkSizeInBytes.Location = new System.Drawing.Point(190, 133);
            this.FileChunkSizeInBytes.Name = "FileChunkSizeInBytes";
            this.FileChunkSizeInBytes.Size = new System.Drawing.Size(141, 20);
            this.FileChunkSizeInBytes.TabIndex = 60;
            this.FileChunkSizeInBytes.Text = "0";
            // 
            // FileName
            // 
            this.FileName.Location = new System.Drawing.Point(130, 28);
            this.FileName.Name = "FileName";
            this.FileName.Size = new System.Drawing.Size(342, 20);
            this.FileName.TabIndex = 59;
            // 
            // label27
            // 
            this.label27.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label27.Location = new System.Drawing.Point(15, 173);
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
            // Append
            // 
            this.Append.AutoSize = true;
            this.Append.Location = new System.Drawing.Point(35, 79);
            this.Append.Name = "Append";
            this.Append.Size = new System.Drawing.Size(117, 17);
            this.Append.TabIndex = 69;
            this.Append.Text = "Append Output File";
            this.Append.UseVisualStyleBackColor = true;
            // 
            // _1_CsvFormat
            // 
            this._1_CsvFormat.AutoSize = true;
            this._1_CsvFormat.Location = new System.Drawing.Point(426, 221);
            this._1_CsvFormat.Name = "_1_CsvFormat";
            this._1_CsvFormat.Size = new System.Drawing.Size(46, 17);
            this._1_CsvFormat.TabIndex = 75;
            this._1_CsvFormat.TabStop = true;
            this._1_CsvFormat.Text = "CSV";
            this._1_CsvFormat.UseVisualStyleBackColor = true;
            this._1_CsvFormat.CheckedChanged += new System.EventHandler(this._1_CsvFormat_CheckedChanged);
            // 
            // _1_TsvFormat
            // 
            this._1_TsvFormat.AutoSize = true;
            this._1_TsvFormat.Location = new System.Drawing.Point(426, 244);
            this._1_TsvFormat.Name = "_1_TsvFormat";
            this._1_TsvFormat.Size = new System.Drawing.Size(46, 17);
            this._1_TsvFormat.TabIndex = 74;
            this._1_TsvFormat.TabStop = true;
            this._1_TsvFormat.Text = "TSV";
            this._1_TsvFormat.UseVisualStyleBackColor = true;
            this._1_TsvFormat.CheckedChanged += new System.EventHandler(this._1_TsvFormat_CheckedChanged);
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(359, 194);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(113, 16);
            this.label20.TabIndex = 73;
            this.label20.Text = "File Format:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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

        private System.Windows.Forms.CheckBox Write2CommonFolder;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox FileChunkSizeInBytes;
        private System.Windows.Forms.TextBox FileName;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.CheckBox Append;
        private System.Windows.Forms.RadioButton _1_CsvFormat;
        private System.Windows.Forms.RadioButton _1_TsvFormat;
        private System.Windows.Forms.Label label20;
    }
}
