namespace Cliver.BotCustomization
{
    partial class IeRoutineBotThreadControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelState = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.Browser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // labelState
            // 
            this.labelState.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelState.Location = new System.Drawing.Point(0, 14);
            this.labelState.Multiline = true;
            this.labelState.Name = "labelState";
            this.labelState.ReadOnly = true;
            this.labelState.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.labelState.Size = new System.Drawing.Size(526, 62);
            this.labelState.TabIndex = 25;
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(526, 14);
            this.progressBar.TabIndex = 21;
            // 
            // Browser
            // 
            this.Browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Browser.Location = new System.Drawing.Point(0, 76);
            this.Browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.Browser.Name = "Browser";
            this.Browser.Size = new System.Drawing.Size(526, 293);
            this.Browser.TabIndex = 26;
            // 
            // CustomBotThreadControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.Browser);
            this.Controls.Add(this.labelState);
            this.Controls.Add(this.progressBar);
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "CustomBotThreadControl";
            this.Size = new System.Drawing.Size(526, 369);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox labelState;
        public System.Windows.Forms.ProgressBar progressBar;
        public System.Windows.Forms.WebBrowser Browser;


    }
}
