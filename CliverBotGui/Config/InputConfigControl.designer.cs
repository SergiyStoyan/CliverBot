namespace Cliver.BotGui
{
    partial class InputConfigControl
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
            this.label20 = new System.Windows.Forms.Label();
            this.ChooseInputFile = new System.Windows.Forms.Button();
            this.File = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this._0_bViewInputFile = new System.Windows.Forms.Button();
            this._1_TsvFormat = new System.Windows.Forms.RadioButton();
            this._1_CsvFormat = new System.Windows.Forms.RadioButton();
            this.label27 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.group_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.Controls.Add(this.label1);
            this.group_box.Controls.Add(this.label27);
            this.group_box.Controls.Add(this._1_CsvFormat);
            this.group_box.Controls.Add(this._1_TsvFormat);
            this.group_box.Controls.Add(this._0_bViewInputFile);
            this.group_box.Controls.Add(this.label20);
            this.group_box.Controls.Add(this.ChooseInputFile);
            this.group_box.Controls.Add(this.File);
            this.group_box.Controls.Add(this.label17);
            this.group_box.Text = "TestCustom";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 100000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(353, 127);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(113, 16);
            this.label20.TabIndex = 55;
            this.label20.Text = "File Format:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ChooseInputFile
            // 
            this.ChooseInputFile.Location = new System.Drawing.Point(472, 45);
            this.ChooseInputFile.Name = "ChooseInputFile";
            this.ChooseInputFile.Size = new System.Drawing.Size(24, 23);
            this.ChooseInputFile.TabIndex = 2;
            this.ChooseInputFile.Text = "...";
            this.ChooseInputFile.UseVisualStyleBackColor = true;
            this.ChooseInputFile.Click += new System.EventHandler(this.ChooseInputFile_Click);
            // 
            // File
            // 
            this.File.Location = new System.Drawing.Point(6, 45);
            this.File.Multiline = true;
            this.File.Name = "File";
            this.File.Size = new System.Drawing.Size(460, 47);
            this.File.TabIndex = 1;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 29);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(53, 13);
            this.label17.TabIndex = 51;
            this.label17.Text = "Input File:";
            // 
            // _0_bViewInputFile
            // 
            this._0_bViewInputFile.Location = new System.Drawing.Point(74, 19);
            this._0_bViewInputFile.Name = "_0_bViewInputFile";
            this._0_bViewInputFile.Size = new System.Drawing.Size(76, 23);
            this._0_bViewInputFile.TabIndex = 0;
            this._0_bViewInputFile.Text = "View";
            this._0_bViewInputFile.UseVisualStyleBackColor = true;
            this._0_bViewInputFile.Click += new System.EventHandler(this.bInputFile_Click);
            // 
            // _1_TsvFormat
            // 
            this._1_TsvFormat.AutoSize = true;
            this._1_TsvFormat.Location = new System.Drawing.Point(420, 177);
            this._1_TsvFormat.Name = "_1_TsvFormat";
            this._1_TsvFormat.Size = new System.Drawing.Size(46, 17);
            this._1_TsvFormat.TabIndex = 4;
            this._1_TsvFormat.TabStop = true;
            this._1_TsvFormat.Text = "TSV";
            this._1_TsvFormat.UseVisualStyleBackColor = true;
            this._1_TsvFormat.CheckedChanged += new System.EventHandler(this._1_TsvFormat_CheckedChanged);
            // 
            // _1_CsvFormat
            // 
            this._1_CsvFormat.AutoSize = true;
            this._1_CsvFormat.Location = new System.Drawing.Point(420, 154);
            this._1_CsvFormat.Name = "_1_CsvFormat";
            this._1_CsvFormat.Size = new System.Drawing.Size(46, 17);
            this._1_CsvFormat.TabIndex = 3;
            this._1_CsvFormat.TabStop = true;
            this._1_CsvFormat.Text = "CSV";
            this._1_CsvFormat.UseVisualStyleBackColor = true;
            this._1_CsvFormat.CheckedChanged += new System.EventHandler(this._1_CsvFormat_CheckedChanged);
            // 
            // label27
            // 
            this.label27.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label27.Location = new System.Drawing.Point(15, 111);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(483, 2);
            this.label27.TabIndex = 73;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(15, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(483, 2);
            this.label1.TabIndex = 74;
            // 
            // InputConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "InputConfigControl";
            this.group_box.ResumeLayout(false);
            this.group_box.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button ChooseInputFile;
        private System.Windows.Forms.TextBox File;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button _0_bViewInputFile;
        private System.Windows.Forms.RadioButton _1_CsvFormat;
        private System.Windows.Forms.RadioButton _1_TsvFormat;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label1;
    }
}
