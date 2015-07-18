namespace Cliver.CrawlerHost
{
    internal partial class CrawlersForm
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State_ = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Site = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Command_ = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AdminEmails = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RunTimeSpan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CrawlProductTimeout = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RestartDelayIfBroken = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._LastSessionState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._LastSessionState_ = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this._NextStartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._SessionStartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._LastStartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._LastEndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._LastProcessId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._LastLog = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._Archive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._ProductsTable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._LastProductTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.crawlersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cliverCrawlersDataSet1 = new Cliver.CrawlerHost.CliverCrawlersDataSet();
            this.crawlersTableAdapter1 = new Cliver.CrawlerHost.CliverCrawlersDataSetTableAdapters.CrawlersTableAdapter();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.bClose = new System.Windows.Forms.Button();
            this.bCheckNow = new System.Windows.Forms.Button();
            this.bStop = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crawlersBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cliverCrawlersDataSet1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.State,
            this.State_,
            this.Site,
            this.Command,
            this.Command_,
            this.AdminEmails,
            this.RunTimeSpan,
            this.CrawlProductTimeout,
            this.RestartDelayIfBroken,
            this.Comment,
            this._LastSessionState,
            this._LastSessionState_,
            this._NextStartTime,
            this._SessionStartTime,
            this._LastStartTime,
            this._LastEndTime,
            this._LastProcessId,
            this._LastLog,
            this._Archive,
            this._ProductsTable,
            this._LastProductTime});
            this.dataGridView1.DataSource = this.crawlersBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1234, 233);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            this.dataGridView1.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_DefaultValuesNeeded);
            this.dataGridView1.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView1_RowsRemoved);
            this.dataGridView1.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowValidated);
            this.dataGridView1.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_RowValidating);
            this.dataGridView1.Leave += new System.EventHandler(this.dataGridView1_Leave);
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            // 
            // State
            // 
            this.State.DataPropertyName = "State";
            this.State.HeaderText = "State";
            this.State.Name = "State";
            this.State.Visible = false;
            // 
            // State_
            // 
            this.State_.HeaderText = "State";
            this.State_.Name = "State_";
            this.State_.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Site
            // 
            this.Site.DataPropertyName = "Site";
            this.Site.HeaderText = "Site";
            this.Site.Name = "Site";
            // 
            // Command
            // 
            this.Command.DataPropertyName = "Command";
            this.Command.HeaderText = "Command";
            this.Command.Name = "Command";
            this.Command.Visible = false;
            // 
            // Command_
            // 
            this.Command_.HeaderText = "Command";
            this.Command_.Name = "Command_";
            // 
            // AdminEmails
            // 
            this.AdminEmails.DataPropertyName = "AdminEmails";
            this.AdminEmails.HeaderText = "AdminEmails";
            this.AdminEmails.Name = "AdminEmails";
            // 
            // RunTimeSpan
            // 
            this.RunTimeSpan.DataPropertyName = "RunTimeSpan";
            this.RunTimeSpan.HeaderText = "RunTimeSpan";
            this.RunTimeSpan.Name = "RunTimeSpan";
            // 
            // CrawlProductTimeout
            // 
            this.CrawlProductTimeout.DataPropertyName = "CrawlProductTimeout";
            this.CrawlProductTimeout.HeaderText = "CrawlProductTimeout";
            this.CrawlProductTimeout.Name = "CrawlProductTimeout";
            // 
            // RestartDelayIfBroken
            // 
            this.RestartDelayIfBroken.DataPropertyName = "RestartDelayIfBroken";
            this.RestartDelayIfBroken.HeaderText = "RestartDelayIfBroken";
            this.RestartDelayIfBroken.Name = "RestartDelayIfBroken";
            // 
            // Comment
            // 
            this.Comment.DataPropertyName = "Comment";
            this.Comment.HeaderText = "Comment";
            this.Comment.Name = "Comment";
            // 
            // _LastSessionState
            // 
            this._LastSessionState.DataPropertyName = "_LastSessionState";
            this._LastSessionState.HeaderText = "_LastSessionState";
            this._LastSessionState.Name = "_LastSessionState";
            this._LastSessionState.ReadOnly = true;
            this._LastSessionState.Visible = false;
            // 
            // _LastSessionState_
            // 
            this._LastSessionState_.HeaderText = "_LastSessionState";
            this._LastSessionState_.Name = "_LastSessionState_";
            this._LastSessionState_.ReadOnly = true;
            this._LastSessionState_.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // _NextStartTime
            // 
            this._NextStartTime.DataPropertyName = "_NextStartTime";
            this._NextStartTime.HeaderText = "_NextStartTime";
            this._NextStartTime.Name = "_NextStartTime";
            this._NextStartTime.ReadOnly = true;
            // 
            // _SessionStartTime
            // 
            this._SessionStartTime.DataPropertyName = "_SessionStartTime";
            this._SessionStartTime.HeaderText = "_SessionStartTime";
            this._SessionStartTime.Name = "_SessionStartTime";
            this._SessionStartTime.ReadOnly = true;
            // 
            // _LastStartTime
            // 
            this._LastStartTime.DataPropertyName = "_LastStartTime";
            this._LastStartTime.HeaderText = "_LastStartTime";
            this._LastStartTime.Name = "_LastStartTime";
            this._LastStartTime.ReadOnly = true;
            // 
            // _LastEndTime
            // 
            this._LastEndTime.DataPropertyName = "_LastEndTime";
            this._LastEndTime.HeaderText = "_LastEndTime";
            this._LastEndTime.Name = "_LastEndTime";
            this._LastEndTime.ReadOnly = true;
            // 
            // _LastProcessId
            // 
            this._LastProcessId.DataPropertyName = "_LastProcessId";
            this._LastProcessId.HeaderText = "_LastProcessId";
            this._LastProcessId.Name = "_LastProcessId";
            this._LastProcessId.ReadOnly = true;
            // 
            // _LastLog
            // 
            this._LastLog.DataPropertyName = "_LastLog";
            this._LastLog.HeaderText = "_LastLog";
            this._LastLog.Name = "_LastLog";
            this._LastLog.ReadOnly = true;
            // 
            // _Archive
            // 
            this._Archive.DataPropertyName = "_Archive";
            this._Archive.HeaderText = "_Archive";
            this._Archive.Name = "_Archive";
            this._Archive.ReadOnly = true;
            // 
            // _ProductsTable
            // 
            this._ProductsTable.DataPropertyName = "_ProductsTable";
            this._ProductsTable.HeaderText = "_ProductsTable";
            this._ProductsTable.Name = "_ProductsTable";
            this._ProductsTable.ReadOnly = true;
            // 
            // _LastProductTime
            // 
            this._LastProductTime.DataPropertyName = "_LastProductTime";
            this._LastProductTime.HeaderText = "_LastProductTime";
            this._LastProductTime.Name = "_LastProductTime";
            this._LastProductTime.ReadOnly = true;
            // 
            // crawlersBindingSource
            // 
            this.crawlersBindingSource.DataMember = "Crawlers";
            this.crawlersBindingSource.DataSource = this.cliverCrawlersDataSet1;
            // 
            // cliverCrawlersDataSet1
            // 
            this.cliverCrawlersDataSet1.DataSetName = "CliverCrawlersDataSet";
            this.cliverCrawlersDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // crawlersTableAdapter1
            // 
            this.crawlersTableAdapter1.ClearBeforeFill = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.bClose);
            this.flowLayoutPanel1.Controls.Add(this.bCheckNow);
            this.flowLayoutPanel1.Controls.Add(this.bStop);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 233);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1234, 29);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // bClose
            // 
            this.bClose.Location = new System.Drawing.Point(1156, 3);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(75, 23);
            this.bClose.TabIndex = 1;
            this.bClose.Text = "Close";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.Close_Click);
            // 
            // bCheckNow
            // 
            this.bCheckNow.Location = new System.Drawing.Point(1075, 3);
            this.bCheckNow.Name = "bCheckNow";
            this.bCheckNow.Size = new System.Drawing.Size(75, 23);
            this.bCheckNow.TabIndex = 0;
            this.bCheckNow.Text = "Check Now";
            this.bCheckNow.UseVisualStyleBackColor = true;
            this.bCheckNow.Click += new System.EventHandler(this.bCheckNow_Click);
            // 
            // bStop
            // 
            this.bStop.Location = new System.Drawing.Point(994, 3);
            this.bStop.Name = "bStop";
            this.bStop.Size = new System.Drawing.Size(75, 23);
            this.bStop.TabIndex = 2;
            this.bStop.Text = "Stop";
            this.bStop.UseVisualStyleBackColor = true;
            this.bStop.Click += new System.EventHandler(this.bStop_Click);
            // 
            // CrawlersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 262);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "CrawlersForm";
            this.Text = "Crawlers";
            this.Deactivate += new System.EventHandler(this.CrawlersForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CrawlersForm_FormClosing);
            this.Load += new System.EventHandler(this.DbForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crawlersBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cliverCrawlersDataSet1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private CliverCrawlersDataSet cliverCrawlersDataSet1;
        private CliverCrawlersDataSetTableAdapters.CrawlersTableAdapter crawlersTableAdapter1;
        private System.Windows.Forms.BindingSource crawlersBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn State;
        private System.Windows.Forms.DataGridViewComboBoxColumn State_;
        new private System.Windows.Forms.DataGridViewTextBoxColumn Site;
        private System.Windows.Forms.DataGridViewTextBoxColumn Command;
        private System.Windows.Forms.DataGridViewComboBoxColumn Command_;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdminEmails;
        private System.Windows.Forms.DataGridViewTextBoxColumn RunTimeSpan;
        private System.Windows.Forms.DataGridViewTextBoxColumn CrawlProductTimeout;
        private System.Windows.Forms.DataGridViewTextBoxColumn RestartDelayIfBroken;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn _LastSessionState;
        private System.Windows.Forms.DataGridViewComboBoxColumn _LastSessionState_;
        private System.Windows.Forms.DataGridViewTextBoxColumn _NextStartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn _SessionStartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn _LastStartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn _LastEndTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn _LastProcessId;
        private System.Windows.Forms.DataGridViewTextBoxColumn _LastLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn _Archive;
        private System.Windows.Forms.DataGridViewTextBoxColumn _ProductsTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn _LastProductTime;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        internal System.Windows.Forms.Button bCheckNow;
        private System.Windows.Forms.Button bClose;
        internal System.Windows.Forms.Button bStop;
    }
}