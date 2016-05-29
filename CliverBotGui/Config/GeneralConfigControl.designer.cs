namespace Cliver.BotGui
{
    partial class GeneralConfigControl
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
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.MaxProcessorErrorNumber = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.MaxTime2WaitForSessionStopInSecs = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.UseFilesFromCache = new System.Windows.Forms.CheckBox();
            this.RestoreErrorItemsAsNew = new System.Windows.Forms.CheckBox();
            this.label24 = new System.Windows.Forms.Label();
            this.WriteSessionRestoringLog = new System.Windows.Forms.CheckBox();
            this.RestoreBrokenSession = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.MaxBotThreadNumber = new System.Windows.Forms.TextBox();
            this.group_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.Controls.Add(this.label39);
            this.group_box.Controls.Add(this.label38);
            this.group_box.Controls.Add(this.MaxProcessorErrorNumber);
            this.group_box.Controls.Add(this.label37);
            this.group_box.Controls.Add(this.MaxTime2WaitForSessionStopInSecs);
            this.group_box.Controls.Add(this.label32);
            this.group_box.Controls.Add(this.label11);
            this.group_box.Controls.Add(this.UseFilesFromCache);
            this.group_box.Controls.Add(this.RestoreErrorItemsAsNew);
            this.group_box.Controls.Add(this.label24);
            this.group_box.Controls.Add(this.WriteSessionRestoringLog);
            this.group_box.Controls.Add(this.RestoreBrokenSession);
            this.group_box.Controls.Add(this.label2);
            this.group_box.Controls.Add(this.MaxBotThreadNumber);
            this.group_box.Text = "TestCustom";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 100000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // label39
            // 
            this.label39.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label39.Location = new System.Drawing.Point(82, 181);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(413, 2);
            this.label39.TabIndex = 66;
            // 
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(95, 206);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(350, 16);
            this.label38.TabIndex = 65;
            this.label38.Text = "Max Consecutive Error Number To Terminate Session:";
            this.label38.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // MaxProcessorErrorNumber
            // 
            this.MaxProcessorErrorNumber.Location = new System.Drawing.Point(457, 201);
            this.MaxProcessorErrorNumber.Name = "MaxProcessorErrorNumber";
            this.MaxProcessorErrorNumber.Size = new System.Drawing.Size(40, 20);
            this.MaxProcessorErrorNumber.TabIndex = 64;
            this.MaxProcessorErrorNumber.Text = "5";
            // 
            // label37
            // 
            this.label37.Location = new System.Drawing.Point(103, 236);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(342, 16);
            this.label37.TabIndex = 63;
            this.label37.Text = "Max Time Waiting Until Session Is Stopped (secs):";
            this.label37.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // MaxTime2WaitForSessionStopInSecs
            // 
            this.MaxTime2WaitForSessionStopInSecs.Location = new System.Drawing.Point(457, 231);
            this.MaxTime2WaitForSessionStopInSecs.Name = "MaxTime2WaitForSessionStopInSecs";
            this.MaxTime2WaitForSessionStopInSecs.Size = new System.Drawing.Size(40, 20);
            this.MaxTime2WaitForSessionStopInSecs.TabIndex = 62;
            this.MaxTime2WaitForSessionStopInSecs.Text = "100";
            // 
            // label32
            // 
            this.label32.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label32.Location = new System.Drawing.Point(28, 63);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(213, 2);
            this.label32.TabIndex = 61;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label11.Location = new System.Drawing.Point(285, 63);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(213, 2);
            this.label11.TabIndex = 60;
            // 
            // UseFilesFromCache
            // 
            this.UseFilesFromCache.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.UseFilesFromCache.Location = new System.Drawing.Point(276, 27);
            this.UseFilesFromCache.Name = "UseFilesFromCache";
            this.UseFilesFromCache.Size = new System.Drawing.Size(221, 20);
            this.UseFilesFromCache.TabIndex = 59;
            this.UseFilesFromCache.Text = "Use Cached Files";
            this.UseFilesFromCache.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RestoreErrorItemsAsNew
            // 
            this.RestoreErrorItemsAsNew.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.RestoreErrorItemsAsNew.Location = new System.Drawing.Point(311, 111);
            this.RestoreErrorItemsAsNew.Name = "RestoreErrorItemsAsNew";
            this.RestoreErrorItemsAsNew.Size = new System.Drawing.Size(186, 20);
            this.RestoreErrorItemsAsNew.TabIndex = 58;
            this.RestoreErrorItemsAsNew.Text = "Restore Error Items As New";
            this.RestoreErrorItemsAsNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label24
            // 
            this.label24.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label24.Location = new System.Drawing.Point(262, 23);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(2, 142);
            this.label24.TabIndex = 57;
            // 
            // WriteSessionRestoringLog
            // 
            this.WriteSessionRestoringLog.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.WriteSessionRestoringLog.Location = new System.Drawing.Point(276, 137);
            this.WriteSessionRestoringLog.Name = "WriteSessionRestoringLog";
            this.WriteSessionRestoringLog.Size = new System.Drawing.Size(221, 20);
            this.WriteSessionRestoringLog.TabIndex = 56;
            this.WriteSessionRestoringLog.Text = "Write Current Session Restoring Log";
            this.WriteSessionRestoringLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RestoreBrokenSession
            // 
            this.RestoreBrokenSession.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.RestoreBrokenSession.Location = new System.Drawing.Point(311, 85);
            this.RestoreBrokenSession.Name = "RestoreBrokenSession";
            this.RestoreBrokenSession.Size = new System.Drawing.Size(186, 20);
            this.RestoreBrokenSession.TabIndex = 55;
            this.RestoreBrokenSession.Text = "Restore Broken Session";
            this.RestoreBrokenSession.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(74, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 16);
            this.label2.TabIndex = 53;
            this.label2.Text = "Thread Number:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // MaxBotThreadNumber
            // 
            this.MaxBotThreadNumber.Location = new System.Drawing.Point(199, 26);
            this.MaxBotThreadNumber.Name = "MaxBotThreadNumber";
            this.MaxBotThreadNumber.Size = new System.Drawing.Size(40, 20);
            this.MaxBotThreadNumber.TabIndex = 52;
            this.MaxBotThreadNumber.Text = "1";
            // 
            // GeneralConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "GeneralConfigControl";
            this.group_box.ResumeLayout(false);
            this.group_box.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox MaxProcessorErrorNumber;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox MaxTime2WaitForSessionStopInSecs;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox UseFilesFromCache;
        private System.Windows.Forms.CheckBox RestoreErrorItemsAsNew;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.CheckBox WriteSessionRestoringLog;
        private System.Windows.Forms.CheckBox RestoreBrokenSession;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox MaxBotThreadNumber;



    }
}
