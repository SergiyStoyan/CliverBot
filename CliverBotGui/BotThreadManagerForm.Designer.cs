namespace Cliver.BotGui
{
    partial class BotThreadManagerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BotThreadManagerForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBotThreads = new System.Windows.Forms.ListBox();
            this.groupBoxBotThread = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBotThreads);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(86, 306);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Thread List";
            // 
            // listBotThreads
            // 
            this.listBotThreads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBotThreads.IntegralHeight = false;
            this.listBotThreads.Location = new System.Drawing.Point(3, 16);
            this.listBotThreads.Name = "listBotThreads";
            this.listBotThreads.Size = new System.Drawing.Size(80, 287);
            this.listBotThreads.TabIndex = 0;
            this.listBotThreads.SelectedIndexChanged += new System.EventHandler(this.listBotThreads_SelectedIndexChanged);
            // 
            // groupBoxBotThread
            // 
            this.groupBoxBotThread.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxBotThread.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxBotThread.Location = new System.Drawing.Point(0, 0);
            this.groupBoxBotThread.Name = "groupBoxBotThread";
            this.groupBoxBotThread.Size = new System.Drawing.Size(471, 306);
            this.groupBoxBotThread.TabIndex = 23;
            this.groupBoxBotThread.TabStop = false;
            this.groupBoxBotThread.Text = "Bot Thread Window";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBoxBotThread);
            this.splitContainer1.Size = new System.Drawing.Size(561, 306);
            this.splitContainer1.SplitterDistance = 86;
            this.splitContainer1.TabIndex = 24;
            // 
            // BotThreadManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(561, 306);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(569, 333);
            this.Name = "BotThreadManagerForm";
            this.Text = "Bot Thread Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BotThreadManagerForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBoxBotThread;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBotThreads;



    }
}