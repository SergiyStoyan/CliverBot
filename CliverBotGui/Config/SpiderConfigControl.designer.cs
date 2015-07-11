namespace Cliver.BotGui
{
    partial class SpiderConfigControl
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
            this.MaxPageCountPerSite = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.ComplyRobotProtocol = new System.Windows.Forms.CheckBox();
            this.UnchangableDomainPartNumber = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.MaxDownloadLinkDepth = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.group_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.Controls.Add(this.MaxPageCountPerSite);
            this.group_box.Controls.Add(this.label34);
            this.group_box.Controls.Add(this.ComplyRobotProtocol);
            this.group_box.Controls.Add(this.UnchangableDomainPartNumber);
            this.group_box.Controls.Add(this.label12);
            this.group_box.Controls.Add(this.MaxDownloadLinkDepth);
            this.group_box.Controls.Add(this.label16);
            this.group_box.Text = "TestCustom";
            // 
            // MaxPageCountPerSite
            // 
            this.MaxPageCountPerSite.Location = new System.Drawing.Point(448, 89);
            this.MaxPageCountPerSite.Name = "MaxPageCountPerSite";
            this.MaxPageCountPerSite.Size = new System.Drawing.Size(40, 20);
            this.MaxPageCountPerSite.TabIndex = 66;
            this.MaxPageCountPerSite.Text = "100";
            // 
            // label34
            // 
            this.label34.Location = new System.Drawing.Point(243, 93);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(200, 16);
            this.label34.TabIndex = 67;
            this.label34.Text = "Max Crawled Page Count Per Site:";
            this.label34.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ComplyRobotProtocol
            // 
            this.ComplyRobotProtocol.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ComplyRobotProtocol.Enabled = false;
            this.ComplyRobotProtocol.Location = new System.Drawing.Point(291, 223);
            this.ComplyRobotProtocol.Name = "ComplyRobotProtocol";
            this.ComplyRobotProtocol.Size = new System.Drawing.Size(197, 17);
            this.ComplyRobotProtocol.TabIndex = 65;
            this.ComplyRobotProtocol.Text = "Comply With Robot Protocol";
            this.ComplyRobotProtocol.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ComplyRobotProtocol.UseVisualStyleBackColor = true;
            // 
            // UnchangableDomainPartNumber
            // 
            this.UnchangableDomainPartNumber.Location = new System.Drawing.Point(448, 58);
            this.UnchangableDomainPartNumber.Name = "UnchangableDomainPartNumber";
            this.UnchangableDomainPartNumber.Size = new System.Drawing.Size(40, 20);
            this.UnchangableDomainPartNumber.TabIndex = 63;
            this.UnchangableDomainPartNumber.Text = "2";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(243, 62);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(200, 16);
            this.label12.TabIndex = 64;
            this.label12.Text = "Unchangable Domain Part Number:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // MaxDownloadLinkDepth
            // 
            this.MaxDownloadLinkDepth.Location = new System.Drawing.Point(448, 26);
            this.MaxDownloadLinkDepth.Name = "MaxDownloadLinkDepth";
            this.MaxDownloadLinkDepth.Size = new System.Drawing.Size(40, 20);
            this.MaxDownloadLinkDepth.TabIndex = 61;
            this.MaxDownloadLinkDepth.Text = "2";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(243, 30);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(200, 16);
            this.label16.TabIndex = 62;
            this.label16.Text = "Maximal Depth Of Open Link:";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // CustomConfigControlExample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "CustomConfigControlExample";
            this.group_box.ResumeLayout(false);
            this.group_box.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MaxPageCountPerSite;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.CheckBox ComplyRobotProtocol;
        private System.Windows.Forms.TextBox UnchangableDomainPartNumber;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox MaxDownloadLinkDepth;
        private System.Windows.Forms.Label label16;


    }
}
