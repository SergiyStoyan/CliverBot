namespace Cliver.BotGui
{
    partial class BrowserConfigControl
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
            this.SuppressScriptErrors = new System.Windows.Forms.CheckBox();
            this.CloseWebBrowserDialogsAutomatically = new System.Windows.Forms.CheckBox();
            this.label28 = new System.Windows.Forms.Label();
            this.PageCompletedTimeoutInSeconds = new System.Windows.Forms.TextBox();
            this.group_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.Controls.Add(this.SuppressScriptErrors);
            this.group_box.Controls.Add(this.CloseWebBrowserDialogsAutomatically);
            this.group_box.Controls.Add(this.label28);
            this.group_box.Controls.Add(this.PageCompletedTimeoutInSeconds);
            this.group_box.Text = "TestCustom";
            // 
            // SuppressScriptErrors
            // 
            this.SuppressScriptErrors.AutoSize = true;
            this.SuppressScriptErrors.Checked = true;
            this.SuppressScriptErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SuppressScriptErrors.Location = new System.Drawing.Point(28, 120);
            this.SuppressScriptErrors.Name = "SuppressScriptErrors";
            this.SuppressScriptErrors.Size = new System.Drawing.Size(209, 17);
            this.SuppressScriptErrors.TabIndex = 35;
            this.SuppressScriptErrors.Text = "Web Browser Script Errors Suppressed";
            this.SuppressScriptErrors.UseVisualStyleBackColor = true;
            // 
            // CloseWebBrowserDialogsAutomatically
            // 
            this.CloseWebBrowserDialogsAutomatically.AutoSize = true;
            this.CloseWebBrowserDialogsAutomatically.Checked = true;
            this.CloseWebBrowserDialogsAutomatically.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CloseWebBrowserDialogsAutomatically.Location = new System.Drawing.Point(28, 84);
            this.CloseWebBrowserDialogsAutomatically.Name = "CloseWebBrowserDialogsAutomatically";
            this.CloseWebBrowserDialogsAutomatically.Size = new System.Drawing.Size(222, 17);
            this.CloseWebBrowserDialogsAutomatically.TabIndex = 34;
            this.CloseWebBrowserDialogsAutomatically.Text = "Close Web Browser Dialogs Automatically";
            this.CloseWebBrowserDialogsAutomatically.UseVisualStyleBackColor = true;
            // 
            // label28
            // 
            this.label28.Location = new System.Drawing.Point(14, 36);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(292, 16);
            this.label28.TabIndex = 32;
            this.label28.Text = "Web Browser Document Completed Timeout (secs):";
            this.label28.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // PageCompletedTimeoutInSeconds
            // 
            this.PageCompletedTimeoutInSeconds.Location = new System.Drawing.Point(312, 33);
            this.PageCompletedTimeoutInSeconds.Name = "PageCompletedTimeoutInSeconds";
            this.PageCompletedTimeoutInSeconds.Size = new System.Drawing.Size(54, 20);
            this.PageCompletedTimeoutInSeconds.TabIndex = 33;
            this.PageCompletedTimeoutInSeconds.Text = "30";
            // 
            // BrowserConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "BrowserConfigControl";
            this.group_box.ResumeLayout(false);
            this.group_box.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox SuppressScriptErrors;
        private System.Windows.Forms.CheckBox CloseWebBrowserDialogsAutomatically;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox PageCompletedTimeoutInSeconds;






    }
}
