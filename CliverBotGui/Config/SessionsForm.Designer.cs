namespace Cliver.BotGui
{
    partial class SessionsForm
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
            this.Sessions = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.Cancel = new System.Windows.Forms.Button();
            this.Ok = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Sessions
            // 
            this.Sessions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Sessions.FormattingEnabled = true;
            this.Sessions.Location = new System.Drawing.Point(0, 0);
            this.Sessions.Name = "Sessions";
            this.Sessions.Size = new System.Drawing.Size(231, 262);
            this.Sessions.TabIndex = 0;
            this.Sessions.SelectedIndexChanged += new System.EventHandler(this.Sessions_SelectedIndexChanged);
            this.Sessions.DoubleClick += new System.EventHandler(this.Sessions_DoubleClick);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.Cancel);
            this.flowLayoutPanel1.Controls.Add(this.Ok);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 237);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(231, 25);
            this.flowLayoutPanel1.TabIndex = 51;
            // 
            // Cancel
            // 
            this.Cancel.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Cancel.Location = new System.Drawing.Point(141, 0);
            this.Cancel.Margin = new System.Windows.Forms.Padding(0);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(90, 24);
            this.Cancel.TabIndex = 0;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Ok
            // 
            this.Ok.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.Ok.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Ok.Location = new System.Drawing.Point(51, 0);
            this.Ok.Margin = new System.Windows.Forms.Padding(0);
            this.Ok.Name = "Ok";
            this.Ok.Size = new System.Drawing.Size(90, 24);
            this.Ok.TabIndex = 3;
            this.Ok.Text = "OK";
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // SessionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(231, 262);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.Sessions);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SessionsForm";
            this.Text = "Load config from a session";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox Sessions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Ok;
    }
}