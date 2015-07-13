namespace Cliver.BotGui
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.Progress = new System.Windows.Forms.Button();
            this.Settings = new System.Windows.Forms.Button();
            this.bAbout = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.Help = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonInput = new System.Windows.Forms.Button();
            this.buttonWorkDir = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.bTools = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Progress
            // 
            this.Progress.BackColor = System.Drawing.SystemColors.Control;
            this.Progress.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Progress.Location = new System.Drawing.Point(450, 0);
            this.Progress.Margin = new System.Windows.Forms.Padding(0);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(75, 28);
            this.Progress.TabIndex = 1;
            this.Progress.Text = "Threads";
            this.Progress.UseVisualStyleBackColor = false;
            this.Progress.Click += new System.EventHandler(this.Progress_Click);
            // 
            // Settings
            // 
            this.Settings.BackColor = System.Drawing.SystemColors.Control;
            this.Settings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Settings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Settings.Location = new System.Drawing.Point(375, 0);
            this.Settings.Margin = new System.Windows.Forms.Padding(0);
            this.Settings.Name = "Settings";
            this.Settings.Size = new System.Drawing.Size(75, 28);
            this.Settings.TabIndex = 2;
            this.Settings.Text = "Settings";
            this.Settings.UseVisualStyleBackColor = false;
            this.Settings.Click += new System.EventHandler(this.Settings_Click);
            // 
            // bAbout
            // 
            this.bAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bAbout.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bAbout.Location = new System.Drawing.Point(0, 0);
            this.bAbout.Margin = new System.Windows.Forms.Padding(0);
            this.bAbout.Name = "bAbout";
            this.bAbout.Size = new System.Drawing.Size(75, 28);
            this.bAbout.TabIndex = 6;
            this.bAbout.Text = "About";
            this.bAbout.Click += new System.EventHandler(this.About_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStart.ForeColor = System.Drawing.Color.MidnightBlue;
            this.buttonStart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonStart.Location = new System.Drawing.Point(525, 0);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(0);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 28);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBoxStatus);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(600, 144);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxStatus.FormattingEnabled = true;
            this.listBoxStatus.IntegralHeight = false;
            this.listBoxStatus.Location = new System.Drawing.Point(3, 16);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.Size = new System.Drawing.Size(594, 125);
            this.listBoxStatus.TabIndex = 17;
            // 
            // Help
            // 
            this.Help.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Help.Location = new System.Drawing.Point(75, 0);
            this.Help.Margin = new System.Windows.Forms.Padding(0);
            this.Help.Name = "Help";
            this.Help.Size = new System.Drawing.Size(75, 28);
            this.Help.TabIndex = 5;
            this.Help.Text = "Help";
            this.Help.Click += new System.EventHandler(this.Help_Click);
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.ForeColor = System.Drawing.Color.SeaGreen;
            this.progressBar.Location = new System.Drawing.Point(0, 144);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(600, 10);
            this.progressBar.TabIndex = 27;
            // 
            // buttonInput
            // 
            this.buttonInput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonInput.Location = new System.Drawing.Point(225, 0);
            this.buttonInput.Margin = new System.Windows.Forms.Padding(0);
            this.buttonInput.Name = "buttonInput";
            this.buttonInput.Size = new System.Drawing.Size(75, 28);
            this.buttonInput.TabIndex = 4;
            this.buttonInput.Text = "Input";
            this.buttonInput.Click += new System.EventHandler(this.buttonInput_Click);
            // 
            // buttonWorkDir
            // 
            this.buttonWorkDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonWorkDir.Location = new System.Drawing.Point(300, 0);
            this.buttonWorkDir.Margin = new System.Windows.Forms.Padding(0);
            this.buttonWorkDir.Name = "buttonWorkDir";
            this.buttonWorkDir.Size = new System.Drawing.Size(75, 28);
            this.buttonWorkDir.TabIndex = 3;
            this.buttonWorkDir.Text = "Work Dir";
            this.buttonWorkDir.Click += new System.EventHandler(this.buttonWorkDir_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonStart);
            this.flowLayoutPanel1.Controls.Add(this.Progress);
            this.flowLayoutPanel1.Controls.Add(this.Settings);
            this.flowLayoutPanel1.Controls.Add(this.buttonWorkDir);
            this.flowLayoutPanel1.Controls.Add(this.buttonInput);
            this.flowLayoutPanel1.Controls.Add(this.bTools);
            this.flowLayoutPanel1.Controls.Add(this.Help);
            this.flowLayoutPanel1.Controls.Add(this.bAbout);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 154);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(600, 29);
            this.flowLayoutPanel1.TabIndex = 18;
            // 
            // bTools
            // 
            this.bTools.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bTools.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bTools.Location = new System.Drawing.Point(150, 0);
            this.bTools.Margin = new System.Windows.Forms.Padding(0);
            this.bTools.Name = "bTools";
            this.bTools.Size = new System.Drawing.Size(75, 28);
            this.bTools.TabIndex = 7;
            this.bTools.Text = "Tools";
            this.bTools.Click += new System.EventHandler(this.bTools_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 183);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(608, 210);
            this.Name = "MainForm";
            this.Text = "CliverBot";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Progress;
        private System.Windows.Forms.Button Settings;
        private System.Windows.Forms.Button bAbout;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Help;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button buttonWorkDir;
        private System.Windows.Forms.Button buttonInput;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button bTools;
    }
}