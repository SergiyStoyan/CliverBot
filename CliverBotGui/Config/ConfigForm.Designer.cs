//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System.Windows.Forms;
namespace Cliver.BotGui
{
	/// <summary>
	/// Used to edit the bot settings. Currently not used.
	/// </summary>
    partial class ConfigForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.Cancel = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listConfigTabs = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.bReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Cancel
            // 
            this.Cancel.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Cancel.Location = new System.Drawing.Point(509, 0);
            this.Cancel.Margin = new System.Windows.Forms.Padding(0);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(90, 24);
            this.Cancel.TabIndex = 16;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Save
            // 
            this.Save.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.Save.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Save.Location = new System.Drawing.Point(419, 0);
            this.Save.Margin = new System.Windows.Forms.Padding(0);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(90, 24);
            this.Save.TabIndex = 15;
            this.Save.Text = "Save";
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer.Size = new System.Drawing.Size(599, 337);
            this.splitContainer.SplitterDistance = 82;
            this.splitContainer.TabIndex = 19;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listConfigTabs);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(82, 337);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sections";
            // 
            // listConfigTabs
            // 
            this.listConfigTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listConfigTabs.FormattingEnabled = true;
            this.listConfigTabs.IntegralHeight = false;
            this.listConfigTabs.Items.AddRange(new object[] {
            "General",
            "Input",
            "Output",
            "Database",
            "Web",
            "Proxy",
            "Browser",
            "Log",
            "Spider"});
            this.listConfigTabs.Location = new System.Drawing.Point(3, 16);
            this.listConfigTabs.Name = "listConfigTabs";
            this.listConfigTabs.Size = new System.Drawing.Size(76, 318);
            this.listConfigTabs.TabIndex = 0;
            this.listConfigTabs.SelectedIndexChanged += new System.EventHandler(this.listConfigTabs_SelectedIndexChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.Cancel);
            this.flowLayoutPanel1.Controls.Add(this.Save);
            this.flowLayoutPanel1.Controls.Add(this.bReset);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 337);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(599, 25);
            this.flowLayoutPanel1.TabIndex = 50;
            // 
            // bReset
            // 
            this.bReset.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.bReset.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bReset.Location = new System.Drawing.Point(329, 0);
            this.bReset.Margin = new System.Windows.Forms.Padding(0);
            this.bReset.Name = "bReset";
            this.bReset.Size = new System.Drawing.Size(90, 24);
            this.bReset.TabIndex = 17;
            this.bReset.Text = "Reset";
            this.bReset.Click += new System.EventHandler(this.bReset_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(599, 362);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion


		private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Save;
        private SplitContainer splitContainer;
        private ListBox listConfigTabs;
        private GroupBox groupBox1;
        private ToolTip toolTip1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button bReset;		

		
	}
}
