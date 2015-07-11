namespace Cliver.BotGui
{
    partial class LogConfigControl
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
            this.LogPostRequestParameters = new System.Windows.Forms.CheckBox();
            this.bPickDir = new System.Windows.Forms.Button();
            this.label23 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.DeleteLogsOlderDays = new System.Windows.Forms.TextBox();
            this.WriteLog = new System.Windows.Forms.CheckBox();
            this.LogDownloadedFiles = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.PreWorkDir = new System.Windows.Forms.TextBox();
            this.group_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.Controls.Add(this.LogPostRequestParameters);
            this.group_box.Controls.Add(this.bPickDir);
            this.group_box.Controls.Add(this.label23);
            this.group_box.Controls.Add(this.label14);
            this.group_box.Controls.Add(this.DeleteLogsOlderDays);
            this.group_box.Controls.Add(this.WriteLog);
            this.group_box.Controls.Add(this.LogDownloadedFiles);
            this.group_box.Controls.Add(this.label9);
            this.group_box.Controls.Add(this.PreWorkDir);
            this.group_box.Text = "TestCustom";
            // 
            // LogPostRequestParameters
            // 
            this.LogPostRequestParameters.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LogPostRequestParameters.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LogPostRequestParameters.Location = new System.Drawing.Point(251, 126);
            this.LogPostRequestParameters.Name = "LogPostRequestParameters";
            this.LogPostRequestParameters.Size = new System.Drawing.Size(219, 26);
            this.LogPostRequestParameters.TabIndex = 50;
            this.LogPostRequestParameters.Text = "Log Post Request Parameters";
            this.LogPostRequestParameters.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bPickDir
            // 
            this.bPickDir.Location = new System.Drawing.Point(476, 47);
            this.bPickDir.Name = "bPickDir";
            this.bPickDir.Size = new System.Drawing.Size(24, 23);
            this.bPickDir.TabIndex = 49;
            this.bPickDir.Text = "...";
            this.bPickDir.UseVisualStyleBackColor = true;
            this.bPickDir.Click += new System.EventHandler(this.bPickDir_Click);
            // 
            // label23
            // 
            this.label23.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label23.Location = new System.Drawing.Point(248, 89);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(2, 153);
            this.label23.TabIndex = 48;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(27, 99);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(159, 16);
            this.label14.TabIndex = 47;
            this.label14.Text = "Delete Log Older Than Days:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DeleteLogsOlderDays
            // 
            this.DeleteLogsOlderDays.Location = new System.Drawing.Point(192, 96);
            this.DeleteLogsOlderDays.Name = "DeleteLogsOlderDays";
            this.DeleteLogsOlderDays.Size = new System.Drawing.Size(44, 20);
            this.DeleteLogsOlderDays.TabIndex = 46;
            // 
            // WriteLog
            // 
            this.WriteLog.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.WriteLog.Checked = true;
            this.WriteLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WriteLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.WriteLog.Location = new System.Drawing.Point(128, 127);
            this.WriteLog.Name = "WriteLog";
            this.WriteLog.Size = new System.Drawing.Size(108, 26);
            this.WriteLog.TabIndex = 44;
            this.WriteLog.Text = "Write Log";
            this.WriteLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LogDownloadedFiles
            // 
            this.LogDownloadedFiles.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LogDownloadedFiles.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LogDownloadedFiles.Location = new System.Drawing.Point(326, 98);
            this.LogDownloadedFiles.Name = "LogDownloadedFiles";
            this.LogDownloadedFiles.Size = new System.Drawing.Size(144, 26);
            this.LogDownloadedFiles.TabIndex = 45;
            this.LogDownloadedFiles.Text = "Log Downloaded Files";
            this.LogDownloadedFiles.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(4, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(172, 16);
            this.label9.TabIndex = 42;
            this.label9.Text = "Folder Containing Work Folder:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PreWorkDir
            // 
            this.PreWorkDir.Location = new System.Drawing.Point(6, 48);
            this.PreWorkDir.Multiline = true;
            this.PreWorkDir.Name = "PreWorkDir";
            this.PreWorkDir.Size = new System.Drawing.Size(467, 20);
            this.PreWorkDir.TabIndex = 43;
            this.PreWorkDir.Text = "c:\\temp";
            // 
            // LogConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "LogConfigControl";
            this.group_box.ResumeLayout(false);
            this.group_box.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox LogPostRequestParameters;
        private System.Windows.Forms.Button bPickDir;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox DeleteLogsOlderDays;
        private System.Windows.Forms.CheckBox WriteLog;
        private System.Windows.Forms.CheckBox LogDownloadedFiles;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox PreWorkDir;



    }
}
