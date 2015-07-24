namespace Cliver.CrawlerHost
{
    partial class SettingsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.SmtpHost = new System.Windows.Forms.TextBox();
            this.SmtpPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SmtpLogin = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SmtpPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.CrawlerProcessMaxNumber = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.AdminEmailSender = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DefaultAdminEmails = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.DbConnectionString = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bCancel
            // 
            this.bCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(817, 366);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 2;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOK
            // 
            this.bOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOK.Location = new System.Drawing.Point(721, 366);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 1;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(135, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "SMTP Host:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // SmtpHost
            // 
            this.SmtpHost.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.SmtpHost.Location = new System.Drawing.Point(227, 56);
            this.SmtpHost.Name = "SmtpHost";
            this.SmtpHost.Size = new System.Drawing.Size(665, 20);
            this.SmtpHost.TabIndex = 3;
            // 
            // SmtpPort
            // 
            this.SmtpPort.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.SmtpPort.Location = new System.Drawing.Point(227, 89);
            this.SmtpPort.Name = "SmtpPort";
            this.SmtpPort.Size = new System.Drawing.Size(665, 20);
            this.SmtpPort.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(111, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "SMTP Port:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // SmtpLogin
            // 
            this.SmtpLogin.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.SmtpLogin.Location = new System.Drawing.Point(227, 122);
            this.SmtpLogin.Name = "SmtpLogin";
            this.SmtpLogin.Size = new System.Drawing.Size(665, 20);
            this.SmtpLogin.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(111, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "SMTP Login:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // SmtpPassword
            // 
            this.SmtpPassword.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.SmtpPassword.Location = new System.Drawing.Point(227, 155);
            this.SmtpPassword.Name = "SmtpPassword";
            this.SmtpPassword.PasswordChar = '#';
            this.SmtpPassword.Size = new System.Drawing.Size(665, 20);
            this.SmtpPassword.TabIndex = 10;
            this.SmtpPassword.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(68, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(153, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "SMTP Password:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // CrawlerProcessMaxNumber
            // 
            this.CrawlerProcessMaxNumber.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CrawlerProcessMaxNumber.Location = new System.Drawing.Point(227, 23);
            this.CrawlerProcessMaxNumber.Name = "CrawlerProcessMaxNumber";
            this.CrawlerProcessMaxNumber.Size = new System.Drawing.Size(665, 20);
            this.CrawlerProcessMaxNumber.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(21, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(200, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Max Number of Crawler Processes:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // AdminEmailSender
            // 
            this.AdminEmailSender.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.AdminEmailSender.Location = new System.Drawing.Point(227, 188);
            this.AdminEmailSender.Name = "AdminEmailSender";
            this.AdminEmailSender.Size = new System.Drawing.Size(665, 20);
            this.AdminEmailSender.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(53, 189);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Email Sender Address:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // DefaultAdminEmails
            // 
            this.DefaultAdminEmails.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DefaultAdminEmails.Location = new System.Drawing.Point(227, 221);
            this.DefaultAdminEmails.Name = "DefaultAdminEmails";
            this.DefaultAdminEmails.Size = new System.Drawing.Size(665, 20);
            this.DefaultAdminEmails.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(24, 222);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(197, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Default Admin Email Addresses:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // DbConnectionString
            // 
            this.DbConnectionString.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DbConnectionString.Location = new System.Drawing.Point(227, 259);
            this.DbConnectionString.Multiline = true;
            this.DbConnectionString.Name = "DbConnectionString";
            this.DbConnectionString.Size = new System.Drawing.Size(665, 91);
            this.DbConnectionString.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(24, 260);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(197, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Database Connection String:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(925, 424);
            this.Controls.Add(this.DbConnectionString);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.DefaultAdminEmails);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.AdminEmailSender);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CrawlerProcessMaxNumber);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.SmtpPassword);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.SmtpLogin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SmtpPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SmtpHost);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SmtpHost;
        private System.Windows.Forms.TextBox SmtpPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SmtpLogin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox SmtpPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox CrawlerProcessMaxNumber;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox AdminEmailSender;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DefaultAdminEmails;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox DbConnectionString;
        private System.Windows.Forms.Label label8;
    }
}