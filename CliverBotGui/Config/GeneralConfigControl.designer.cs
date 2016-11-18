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
            this.label38 = new System.Windows.Forms.Label();
            this.MaxProcessorErrorNumber = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.MaxTime2WaitForSessionStopInSecs = new System.Windows.Forms.TextBox();
            this.RestoreErrorItemsAsNew = new System.Windows.Forms.CheckBox();
            this.label24 = new System.Windows.Forms.Label();
            this.WriteSessionRestoringLog = new System.Windows.Forms.CheckBox();
            this.RestoreBrokenSession = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.MaxBotThreadNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.group_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.Controls.Add(this.label1);
            this.group_box.Controls.Add(this.label3);
            this.group_box.Controls.Add(this.label38);
            this.group_box.Controls.Add(this.MaxProcessorErrorNumber);
            this.group_box.Controls.Add(this.label37);
            this.group_box.Controls.Add(this.MaxTime2WaitForSessionStopInSecs);
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
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(93, 154);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(350, 16);
            this.label38.TabIndex = 65;
            this.label38.Text = "Max Consecutive Error Number To Terminate Session:";
            this.label38.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // MaxProcessorErrorNumber
            // 
            this.MaxProcessorErrorNumber.Location = new System.Drawing.Point(455, 149);
            this.MaxProcessorErrorNumber.Name = "MaxProcessorErrorNumber";
            this.MaxProcessorErrorNumber.Size = new System.Drawing.Size(40, 20);
            this.MaxProcessorErrorNumber.TabIndex = 64;
            this.MaxProcessorErrorNumber.Text = "5";
            // 
            // label37
            // 
            this.label37.Location = new System.Drawing.Point(101, 184);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(342, 16);
            this.label37.TabIndex = 63;
            this.label37.Text = "Max Time Waiting Until Session Is Stopped (secs):";
            this.label37.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // MaxTime2WaitForSessionStopInSecs
            // 
            this.MaxTime2WaitForSessionStopInSecs.Location = new System.Drawing.Point(455, 179);
            this.MaxTime2WaitForSessionStopInSecs.Name = "MaxTime2WaitForSessionStopInSecs";
            this.MaxTime2WaitForSessionStopInSecs.Size = new System.Drawing.Size(40, 20);
            this.MaxTime2WaitForSessionStopInSecs.TabIndex = 62;
            this.MaxTime2WaitForSessionStopInSecs.Text = "100";
            // 
            // RestoreErrorItemsAsNew
            // 
            this.RestoreErrorItemsAsNew.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.RestoreErrorItemsAsNew.Location = new System.Drawing.Point(309, 53);
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
            this.label24.Size = new System.Drawing.Size(2, 90);
            this.label24.TabIndex = 57;
            // 
            // WriteSessionRestoringLog
            // 
            this.WriteSessionRestoringLog.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.WriteSessionRestoringLog.Location = new System.Drawing.Point(274, 79);
            this.WriteSessionRestoringLog.Name = "WriteSessionRestoringLog";
            this.WriteSessionRestoringLog.Size = new System.Drawing.Size(221, 20);
            this.WriteSessionRestoringLog.TabIndex = 56;
            this.WriteSessionRestoringLog.Text = "Write Current Session Restoring Log";
            this.WriteSessionRestoringLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RestoreBrokenSession
            // 
            this.RestoreBrokenSession.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.RestoreBrokenSession.Location = new System.Drawing.Point(309, 27);
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
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(12, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(483, 2);
            this.label3.TabIndex = 70;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(12, 219);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(483, 2);
            this.label1.TabIndex = 71;
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
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox MaxProcessorErrorNumber;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox MaxTime2WaitForSessionStopInSecs;
        private System.Windows.Forms.CheckBox RestoreErrorItemsAsNew;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.CheckBox WriteSessionRestoringLog;
        private System.Windows.Forms.CheckBox RestoreBrokenSession;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox MaxBotThreadNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
    }
}
