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
            this.bPickDir = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.DeleteLogsOlderDays = new System.Windows.Forms.TextBox();
            this.WriteLog = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.PreWorkDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.group_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.Controls.Add(this.label1);
            this.group_box.Controls.Add(this.label2);
            this.group_box.Controls.Add(this.bPickDir);
            this.group_box.Controls.Add(this.label14);
            this.group_box.Controls.Add(this.DeleteLogsOlderDays);
            this.group_box.Controls.Add(this.WriteLog);
            this.group_box.Controls.Add(this.label9);
            this.group_box.Controls.Add(this.PreWorkDir);
            this.group_box.Text = "TestCustom";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 100000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
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
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(290, 107);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(159, 16);
            this.label14.TabIndex = 47;
            this.label14.Text = "Delete Log Older Than Days:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DeleteLogsOlderDays
            // 
            this.DeleteLogsOlderDays.Location = new System.Drawing.Point(455, 104);
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
            this.WriteLog.Location = new System.Drawing.Point(391, 135);
            this.WriteLog.Name = "WriteLog";
            this.WriteLog.Size = new System.Drawing.Size(108, 26);
            this.WriteLog.TabIndex = 44;
            this.WriteLog.Text = "Write Log";
            this.WriteLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(15, 172);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(483, 2);
            this.label2.TabIndex = 69;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(17, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(483, 2);
            this.label1.TabIndex = 70;
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
        private System.Windows.Forms.Button bPickDir;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox DeleteLogsOlderDays;
        private System.Windows.Forms.CheckBox WriteLog;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox PreWorkDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
