namespace Cliver.CrawlerHostManager
{
    partial class DbConnectionSettingsForm
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
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.DbConnectionString = new System.Windows.Forms.TextBox();
            this.lConectionString = new System.Windows.Forms.Label();
            this.PickFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(573, 148);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 2;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // bOK
            // 
            this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOK.Location = new System.Drawing.Point(492, 148);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 1;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // DbConnectionString
            // 
            this.DbConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DbConnectionString.Location = new System.Drawing.Point(12, 26);
            this.DbConnectionString.Multiline = true;
            this.DbConnectionString.Name = "DbConnectionString";
            this.DbConnectionString.Size = new System.Drawing.Size(636, 116);
            this.DbConnectionString.TabIndex = 18;
            // 
            // lConectionString
            // 
            this.lConectionString.AutoSize = true;
            this.lConectionString.Location = new System.Drawing.Point(9, 10);
            this.lConectionString.Name = "lConectionString";
            this.lConectionString.Size = new System.Drawing.Size(143, 13);
            this.lConectionString.TabIndex = 17;
            this.lConectionString.Text = "Database Connection String:";
            this.lConectionString.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // PickFile
            // 
            this.PickFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PickFile.Location = new System.Drawing.Point(12, 148);
            this.PickFile.Name = "PickFile";
            this.PickFile.Size = new System.Drawing.Size(156, 23);
            this.PickFile.TabIndex = 19;
            this.PickFile.Text = "Pick Database File";
            this.PickFile.UseVisualStyleBackColor = true;
            this.PickFile.Click += new System.EventHandler(this.PickFile_Click);
            // 
            // DbConnectionSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 183);
            this.Controls.Add(this.PickFile);
            this.Controls.Add(this.DbConnectionString);
            this.Controls.Add(this.lConectionString);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DbConnectionSettingsForm";
            this.Text = "Settings";
            this.Shown += new System.EventHandler(this.DbConnectionSettingsForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.TextBox DbConnectionString;
        private System.Windows.Forms.Label lConectionString;
        private System.Windows.Forms.Button PickFile;
    }
}