namespace Cliver.CrawlerManager
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
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.state_ = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.site = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.command_ = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.admin_emails = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.run_time_span = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.crawl_product_timeout = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.restart_delay_if_broken = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._last_session_state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._last_session_state_ = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this._next_start_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._session_start_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._last_start_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._last_end_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._last_process_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._last_log = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._archive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._products_table = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._last_product_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.crawlersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cliverCrawlersDataSet1 = new Cliver.CrawlerManager.CliverCrawlersDataSet();
            this.crawlersTableAdapter1 = new Cliver.CrawlerManager.CliverCrawlersDataSetTableAdapters.crawlersTableAdapter();
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
            this.id,
            this.state,
            this.state_,
            this.site,
            this.command,
            this.command_,
            this.admin_emails,
            this.run_time_span,
            this.crawl_product_timeout,
            this.restart_delay_if_broken,
            this.comment,
            this._last_session_state,
            this._last_session_state_,
            this._next_start_time,
            this._session_start_time,
            this._last_start_time,
            this._last_end_time,
            this._last_process_id,
            this._last_log,
            this._archive,
            this._products_table,
            this._last_product_time});
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
            // id
            // 
            this.id.DataPropertyName = "id";
            this.id.HeaderText = "id";
            this.id.Name = "id";
            // 
            // state
            // 
            this.state.DataPropertyName = "state";
            this.state.HeaderText = "state";
            this.state.Name = "state";
            this.state.Visible = false;
            // 
            // state_
            // 
            this.state_.HeaderText = "state";
            this.state_.Name = "state_";
            this.state_.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // site
            // 
            this.site.DataPropertyName = "site";
            this.site.HeaderText = "site";
            this.site.Name = "site";
            // 
            // command
            // 
            this.command.DataPropertyName = "command";
            this.command.HeaderText = "command";
            this.command.Name = "command";
            this.command.Visible = false;
            // 
            // command_
            // 
            this.command_.HeaderText = "command";
            this.command_.Name = "command_";
            // 
            // admin_emails
            // 
            this.admin_emails.DataPropertyName = "admin_emails";
            this.admin_emails.HeaderText = "admin_emails";
            this.admin_emails.Name = "admin_emails";
            // 
            // run_time_span
            // 
            this.run_time_span.DataPropertyName = "run_time_span";
            this.run_time_span.HeaderText = "run_time_span";
            this.run_time_span.Name = "run_time_span";
            // 
            // crawl_product_timeout
            // 
            this.crawl_product_timeout.DataPropertyName = "crawl_product_timeout";
            this.crawl_product_timeout.HeaderText = "crawl_product_timeout";
            this.crawl_product_timeout.Name = "crawl_product_timeout";
            // 
            // restart_delay_if_broken
            // 
            this.restart_delay_if_broken.DataPropertyName = "restart_delay_if_broken";
            this.restart_delay_if_broken.HeaderText = "restart_delay_if_broken";
            this.restart_delay_if_broken.Name = "restart_delay_if_broken";
            // 
            // comment
            // 
            this.comment.DataPropertyName = "comment";
            this.comment.HeaderText = "comment";
            this.comment.Name = "comment";
            // 
            // _last_session_state
            // 
            this._last_session_state.DataPropertyName = "_last_session_state";
            this._last_session_state.HeaderText = "_last_session_state";
            this._last_session_state.Name = "_last_session_state";
            this._last_session_state.ReadOnly = true;
            this._last_session_state.Visible = false;
            // 
            // _last_session_state_
            // 
            this._last_session_state_.HeaderText = "_last_session_state";
            this._last_session_state_.Name = "_last_session_state_";
            this._last_session_state_.ReadOnly = true;
            this._last_session_state_.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // _next_start_time
            // 
            this._next_start_time.DataPropertyName = "_next_start_time";
            this._next_start_time.HeaderText = "_next_start_time";
            this._next_start_time.Name = "_next_start_time";
            this._next_start_time.ReadOnly = true;
            // 
            // _session_start_time
            // 
            this._session_start_time.DataPropertyName = "_session_start_time";
            this._session_start_time.HeaderText = "_session_start_time";
            this._session_start_time.Name = "_session_start_time";
            this._session_start_time.ReadOnly = true;
            // 
            // _last_start_time
            // 
            this._last_start_time.DataPropertyName = "_last_start_time";
            this._last_start_time.HeaderText = "_last_start_time";
            this._last_start_time.Name = "_last_start_time";
            this._last_start_time.ReadOnly = true;
            // 
            // _last_end_time
            // 
            this._last_end_time.DataPropertyName = "_last_end_time";
            this._last_end_time.HeaderText = "_last_end_time";
            this._last_end_time.Name = "_last_end_time";
            this._last_end_time.ReadOnly = true;
            // 
            // _last_process_id
            // 
            this._last_process_id.DataPropertyName = "_last_process_id";
            this._last_process_id.HeaderText = "_last_process_id";
            this._last_process_id.Name = "_last_process_id";
            this._last_process_id.ReadOnly = true;
            // 
            // _last_log
            // 
            this._last_log.DataPropertyName = "_last_log";
            this._last_log.HeaderText = "_last_log";
            this._last_log.Name = "_last_log";
            this._last_log.ReadOnly = true;
            // 
            // _archive
            // 
            this._archive.DataPropertyName = "_archive";
            this._archive.HeaderText = "_archive";
            this._archive.Name = "_archive";
            this._archive.ReadOnly = true;
            // 
            // _products_table
            // 
            this._products_table.DataPropertyName = "_products_table";
            this._products_table.HeaderText = "_products_table";
            this._products_table.Name = "_products_table";
            this._products_table.ReadOnly = true;
            // 
            // _last_product_time
            // 
            this._last_product_time.DataPropertyName = "_last_product_time";
            this._last_product_time.HeaderText = "_last_product_time";
            this._last_product_time.Name = "_last_product_time";
            this._last_product_time.ReadOnly = true;
            // 
            // crawlersBindingSource
            // 
            this.crawlersBindingSource.DataMember = "crawlers";
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
        private CliverCrawlersDataSetTableAdapters.crawlersTableAdapter crawlersTableAdapter1;
        private System.Windows.Forms.BindingSource crawlersBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn state;
        private System.Windows.Forms.DataGridViewComboBoxColumn state_;
        private System.Windows.Forms.DataGridViewTextBoxColumn site;
        private System.Windows.Forms.DataGridViewTextBoxColumn command;
        private System.Windows.Forms.DataGridViewComboBoxColumn command_;
        private System.Windows.Forms.DataGridViewTextBoxColumn admin_emails;
        private System.Windows.Forms.DataGridViewTextBoxColumn run_time_span;
        private System.Windows.Forms.DataGridViewTextBoxColumn crawl_product_timeout;
        private System.Windows.Forms.DataGridViewTextBoxColumn restart_delay_if_broken;
        private System.Windows.Forms.DataGridViewTextBoxColumn comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn _last_session_state;
        private System.Windows.Forms.DataGridViewComboBoxColumn _last_session_state_;
        private System.Windows.Forms.DataGridViewTextBoxColumn _next_start_time;
        private System.Windows.Forms.DataGridViewTextBoxColumn _session_start_time;
        private System.Windows.Forms.DataGridViewTextBoxColumn _last_start_time;
        private System.Windows.Forms.DataGridViewTextBoxColumn _last_end_time;
        private System.Windows.Forms.DataGridViewTextBoxColumn _last_process_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn _last_log;
        private System.Windows.Forms.DataGridViewTextBoxColumn _archive;
        private System.Windows.Forms.DataGridViewTextBoxColumn _products_table;
        private System.Windows.Forms.DataGridViewTextBoxColumn _last_product_time;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        internal System.Windows.Forms.Button bCheckNow;
        private System.Windows.Forms.Button bClose;
        internal System.Windows.Forms.Button bStop;
    }
}