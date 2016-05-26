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
            this._1_SetTAB2InputFieldDelimiter = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.InputFieldSeparator = new System.Windows.Forms.TextBox();
            this.ChooseInputFile = new System.Windows.Forms.Button();
            this.InputFile = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this._0_bViewInputFile = new System.Windows.Forms.Button();
            this.group_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.Controls.Add(this._0_bViewInputFile);
            this.group_box.Controls.Add(this._1_SetTAB2InputFieldDelimiter);
            this.group_box.Controls.Add(this.label20);
            this.group_box.Controls.Add(this.InputFieldSeparator);
            this.group_box.Controls.Add(this.ChooseInputFile);
            this.group_box.Controls.Add(this.InputFile);
            this.group_box.Controls.Add(this.label17);
            this.group_box.Text = "TestCustom";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 100000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // _1_SetTAB2InputFieldDelimiter
            // 
            this._1_SetTAB2InputFieldDelimiter.Appearance = System.Windows.Forms.Appearance.Button;
            this._1_SetTAB2InputFieldDelimiter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._1_SetTAB2InputFieldDelimiter.Location = new System.Drawing.Point(342, 118);
            this._1_SetTAB2InputFieldDelimiter.Name = "_1_SetTAB2InputFieldDelimiter";
            this._1_SetTAB2InputFieldDelimiter.Size = new System.Drawing.Size(124, 23);
            this._1_SetTAB2InputFieldDelimiter.TabIndex = 56;
            this._1_SetTAB2InputFieldDelimiter.Text = "Use TAB as delimiter";
            this._1_SetTAB2InputFieldDelimiter.UseVisualStyleBackColor = true;
            this._1_SetTAB2InputFieldDelimiter.CheckedChanged += new System.EventHandler(this.@__SetTAB2InputFieldDelimiter_CheckedChanged);
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(200, 124);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(113, 16);
            this.label20.TabIndex = 55;
            this.label20.Text = "Field Delimiter:";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // InputFieldSeparator
            // 
            this.InputFieldSeparator.AcceptsTab = true;
            this.InputFieldSeparator.Location = new System.Drawing.Point(319, 120);
            this.InputFieldSeparator.Name = "InputFieldSeparator";
            this.InputFieldSeparator.Size = new System.Drawing.Size(17, 20);
            this.InputFieldSeparator.TabIndex = 54;
            this.InputFieldSeparator.Text = ",";
            this.InputFieldSeparator.TextChanged += new System.EventHandler(this.InputFieldSeparator_TextChanged);
            // 
            // ChooseInputFile
            // 
            this.ChooseInputFile.Location = new System.Drawing.Point(472, 45);
            this.ChooseInputFile.Name = "ChooseInputFile";
            this.ChooseInputFile.Size = new System.Drawing.Size(24, 23);
            this.ChooseInputFile.TabIndex = 53;
            this.ChooseInputFile.Text = "...";
            this.ChooseInputFile.UseVisualStyleBackColor = true;
            this.ChooseInputFile.Click += new System.EventHandler(this.ChooseInputFile_Click);
            // 
            // InputFile
            // 
            this.InputFile.Location = new System.Drawing.Point(6, 45);
            this.InputFile.Multiline = true;
            this.InputFile.Name = "InputFile";
            this.InputFile.Size = new System.Drawing.Size(460, 47);
            this.InputFile.TabIndex = 52;
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
            this._0_bViewInputFile.TabIndex = 59;
            this._0_bViewInputFile.Text = "View";
            this._0_bViewInputFile.UseVisualStyleBackColor = true;
            this._0_bViewInputFile.Click += new System.EventHandler(this.bInputFile_Click);
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

        private System.Windows.Forms.CheckBox _1_SetTAB2InputFieldDelimiter;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox InputFieldSeparator;
        private System.Windows.Forms.Button ChooseInputFile;
        private System.Windows.Forms.TextBox InputFile;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button _0_bViewInputFile;



    }
}
