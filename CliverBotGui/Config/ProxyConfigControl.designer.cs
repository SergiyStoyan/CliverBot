namespace Cliver.BotGui
{
    partial class ProxyConfigControl
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
            this.label31 = new System.Windows.Forms.Label();
            this.MaxAttemptCountWithNewProxy = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.ReloadProxyFileInSeconds = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this._1_ProxyTypeSocks5 = new System.Windows.Forms.RadioButton();
            this._1_ProxyTypeHttp = new System.Windows.Forms.RadioButton();
            this.ProxyLogin = new System.Windows.Forms.TextBox();
            this.ProxiesFileUri = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ProxyPassword = new System.Windows.Forms.TextBox();
            this.group_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_box
            // 
            this.group_box.Controls.Add(this.label31);
            this.group_box.Controls.Add(this.MaxAttemptCountWithNewProxy);
            this.group_box.Controls.Add(this.label21);
            this.group_box.Controls.Add(this.ReloadProxyFileInSeconds);
            this.group_box.Controls.Add(this.label30);
            this.group_box.Controls.Add(this._1_ProxyTypeSocks5);
            this.group_box.Controls.Add(this._1_ProxyTypeHttp);
            this.group_box.Controls.Add(this.ProxyLogin);
            this.group_box.Controls.Add(this.ProxiesFileUri);
            this.group_box.Controls.Add(this.label4);
            this.group_box.Controls.Add(this.label6);
            this.group_box.Controls.Add(this.label7);
            this.group_box.Controls.Add(this.ProxyPassword);
            this.group_box.Text = "TestCustom";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 100000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(182, 233);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(248, 13);
            this.label31.TabIndex = 81;
            this.label31.Text = "Maximal Download Attempt Count With New Proxy:";
            this.label31.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // MaxAttemptCountWithNewProxy
            // 
            this.MaxAttemptCountWithNewProxy.Location = new System.Drawing.Point(445, 230);
            this.MaxAttemptCountWithNewProxy.Name = "MaxAttemptCountWithNewProxy";
            this.MaxAttemptCountWithNewProxy.Size = new System.Drawing.Size(52, 20);
            this.MaxAttemptCountWithNewProxy.TabIndex = 82;
            this.MaxAttemptCountWithNewProxy.Text = "10";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(295, 209);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(135, 13);
            this.label21.TabIndex = 79;
            this.label21.Text = "Reload Proxy File In (secs):";
            this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ReloadProxyFileInSeconds
            // 
            this.ReloadProxyFileInSeconds.Location = new System.Drawing.Point(445, 204);
            this.ReloadProxyFileInSeconds.Name = "ReloadProxyFileInSeconds";
            this.ReloadProxyFileInSeconds.Size = new System.Drawing.Size(52, 20);
            this.ReloadProxyFileInSeconds.TabIndex = 80;
            this.ReloadProxyFileInSeconds.Text = "-1";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(350, 44);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(63, 13);
            this.label30.TabIndex = 78;
            this.label30.Text = "Proxy Type:";
            // 
            // _1_ProxyTypeSocks5
            // 
            this._1_ProxyTypeSocks5.AutoSize = true;
            this._1_ProxyTypeSocks5.Location = new System.Drawing.Point(419, 55);
            this._1_ProxyTypeSocks5.Name = "_1_ProxyTypeSocks5";
            this._1_ProxyTypeSocks5.Size = new System.Drawing.Size(67, 17);
            this._1_ProxyTypeSocks5.TabIndex = 77;
            this._1_ProxyTypeSocks5.TabStop = true;
            this._1_ProxyTypeSocks5.Text = "SOCKS5";
            this._1_ProxyTypeSocks5.UseVisualStyleBackColor = true;
            // 
            // _1_ProxyTypeHttp
            // 
            this._1_ProxyTypeHttp.AutoSize = true;
            this._1_ProxyTypeHttp.Location = new System.Drawing.Point(419, 32);
            this._1_ProxyTypeHttp.Name = "_1_ProxyTypeHttp";
            this._1_ProxyTypeHttp.Size = new System.Drawing.Size(54, 17);
            this._1_ProxyTypeHttp.TabIndex = 76;
            this._1_ProxyTypeHttp.TabStop = true;
            this._1_ProxyTypeHttp.Text = "HTTP";
            this._1_ProxyTypeHttp.UseVisualStyleBackColor = true;
            // 
            // ProxyLogin
            // 
            this.ProxyLogin.Location = new System.Drawing.Point(357, 140);
            this.ProxyLogin.Name = "ProxyLogin";
            this.ProxyLogin.Size = new System.Drawing.Size(140, 20);
            this.ProxyLogin.TabIndex = 75;
            // 
            // ProxiesFileUri
            // 
            this.ProxiesFileUri.Location = new System.Drawing.Point(95, 101);
            this.ProxiesFileUri.Multiline = true;
            this.ProxiesFileUri.Name = "ProxiesFileUri";
            this.ProxiesFileUri.Size = new System.Drawing.Size(402, 20);
            this.ProxiesFileUri.TabIndex = 71;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(20, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 16);
            this.label4.TabIndex = 70;
            this.label4.Text = "Proxies File:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(252, 170);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 16);
            this.label6.TabIndex = 72;
            this.label6.Text = "Proxy Password:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(273, 144);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 16);
            this.label7.TabIndex = 74;
            this.label7.Text = "Proxy Login:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ProxyPassword
            // 
            this.ProxyPassword.Location = new System.Drawing.Point(357, 166);
            this.ProxyPassword.Name = "ProxyPassword";
            this.ProxyPassword.Size = new System.Drawing.Size(140, 20);
            this.ProxyPassword.TabIndex = 73;
            // 
            // ProxyConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ProxyConfigControl";
            this.group_box.ResumeLayout(false);
            this.group_box.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox MaxAttemptCountWithNewProxy;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox ReloadProxyFileInSeconds;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.RadioButton _1_ProxyTypeSocks5;
        private System.Windows.Forms.RadioButton _1_ProxyTypeHttp;
        private System.Windows.Forms.TextBox ProxyLogin;
        private System.Windows.Forms.TextBox ProxiesFileUri;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ProxyPassword;




    }
}
