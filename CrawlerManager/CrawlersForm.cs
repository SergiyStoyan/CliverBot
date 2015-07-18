using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cliver.Bot;
using Cliver.CrawlerHost;

namespace Cliver.CrawlerHost
{
    internal partial class CrawlersForm : BaseForm
    {
        public CrawlersForm()
        {
            InitializeComponent();

            //System.Threading.Timer g = new System.Threading.Timer();

            foreach (DataGridViewColumn c in dataGridView1.Columns)
                if (c.ReadOnly)
                {
                    //c.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 255, 240);
                    c.DefaultCellStyle.BackColor = Color.FromArgb(255, 0, 0, 0);
                    c.DefaultCellStyle.ForeColor = Color.FromArgb(255, 255, 255, 0);
                }

            command_i_ = dataGridView1.Columns["Command_"].Index;
            command_i = dataGridView1.Columns["Command"].Index;
            state_i_ = dataGridView1.Columns["State_"].Index;
            state_i = dataGridView1.Columns["State"].Index;
            _last_session_state_i_ = dataGridView1.Columns["_LastSessionState_"].Index;
            _last_session_state_i = dataGridView1.Columns["_LastSessionState"].Index;
        }
        int command_i_;
        int command_i;
        int state_i_;
        int state_i;
        int _last_session_state_i_;
        int _last_session_state_i;

        private void DbForm_Load(object sender, EventArgs e)
        {
            load_table();
            //this.CrawlersTableAdapter1.Fill(this.cliverCrawlersDataSet1.Crawlers);
            Activate();
        }

        private void load_table()
        {
            try
            {
                this.crawlersTableAdapter1.Connection.ConnectionString = DbApi.ConnectionString;
                this.crawlersTableAdapter1.Fill(this.cliverCrawlersDataSet1.Crawlers);
                //dataGridView1.AutoResizeColumns();
                
                //foreach (DataColumn c in CrawlersTableAdapter1.GetData().Columns)
                //    if (c.ColumnName.StartsWith("_"))
                //        c.AllowDBNull = true;

                DataGridViewComboBoxColumn c_ = (DataGridViewComboBoxColumn)dataGridView1.Columns["Command_"];
                c_.DataSource = Enum.GetValues(typeof(DbApi.CrawlerCommand));
                c_.ValueType = typeof(DbApi.CrawlerCommand);
                //c_.ValueMember = "Value";
                //c_.DisplayMember = "Display";
                foreach (DataGridViewRow r in dataGridView1.Rows)
                {
                    try
                    {
                        r.Cells[command_i_].Value = (DbApi.CrawlerCommand)r.Cells[command_i].Value;
                    }
                    catch
                    {
                        r.Cells[command_i_].Value = -1;
                    }
                }

                c_ = (DataGridViewComboBoxColumn)dataGridView1.Columns["State_"];
                c_.DataSource = Enum.GetValues(typeof(DbApi.CrawlerState));
                c_.ValueType = typeof(DbApi.CrawlerState);
                foreach (DataGridViewRow r in dataGridView1.Rows)
                {
                    try
                    {
                        r.Cells[state_i_].Value = (DbApi.CrawlerState)r.Cells[state_i].Value;
                    }
                    catch
                    {
                        r.Cells[state_i_].Value = -1;
                    }
                }

                c_ = (DataGridViewComboBoxColumn)dataGridView1.Columns["_LastSessionState_"];
                c_.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                c_.DataSource = Enum.GetValues(typeof(DbApi.SessionState));
                c_.ValueType = typeof(DbApi.SessionState);
                foreach (DataGridViewRow r in dataGridView1.Rows)
                {
                    try
                    {
                        r.Cells[_last_session_state_i_].Value = (DbApi.SessionState)r.Cells[_last_session_state_i].Value;
                    }
                    catch
                    {
                        r.Cells[_last_session_state_i_].Value = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        private void prepare_row(int row_index)
        {
            try
            {
                //dataGridView1.EndEdit();
                DataGridViewRow r = dataGridView1.Rows[row_index];
                //try
                //{
                //    if (((DataRowView)r.DataBoundItem).Row.RowState == DataRowState.Unchanged)
                //        return;
                //}
                //catch
                //{
                //    return;
                //}

                try
                {
                    r.Cells[command_i].Value = (byte)(DbApi.CrawlerCommand)r.Cells[command_i_].Value;
                }
                catch { }
                try
                {
                    r.Cells[state_i].Value = (byte)(DbApi.CrawlerState)r.Cells[state_i_].Value;
                }
                catch { }
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        private void save_row(int row_index)
        {
            try
            {
                DataGridViewRow r = dataGridView1.Rows[row_index];
                try
                {
                    if (((DataRowView)r.DataBoundItem).Row.RowState == DataRowState.Unchanged)
                        return;
                }
                catch
                {
                    return;
                }
                crawlersTableAdapter1.Update(((DataRowView)r.DataBoundItem).Row);
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                if(dataGridView1.Rows[e.RowIndex].IsNewRow)
                    return;
                LogMessage.Error(e.Exception.Message);                
                e.Cancel = true;
            }
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (dataGridView1.Rows.Count < 1)
                return;
            crawlersTableAdapter1.Update(cliverCrawlersDataSet1);
        }

        private void dataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            prepare_row(e.RowIndex);
        }

        private void dataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            save_row(e.RowIndex);
        }

        private void dataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                //e.Row.Cells["Id"].Value = 
                e.Row.Cells["State_"].Value = DbApi.CrawlerState.DISABLED;
                //e.Row.Cells["Site"].Value = 
                e.Row.Cells["Command_"].Value = DbApi.CrawlerCommand.EMPTY;
                //e.Row.Cells["AdminEmails"].Value = 
                e.Row.Cells["RunTimeSpan"].Value = 86400;
                e.Row.Cells["CrawlProductTimeout"].Value = 600;
                e.Row.Cells["RestartDelayIfBroken"].Value = 600;
                //e.Row.Cells["Comment"].Value = 
                e.Row.Cells["_NextStartTime"].Value = "2000-01-01 00:00:00";
                //e.Row.Cells["_LastStartTime"].Value = (new DateTime(1, 1, 1)).ToString(); 
                //e.Row.Cells["_LastEndTime"].Value = (new DateTime(1, 1, 1)).ToString(); 
                //e.Row.Cells["_LastProcessId"].Value = 0;
                //e.Row.Cells["_LastSessionState"].Value = 0;
                e.Row.Cells["_ProductsTable"].Value = DateTime.Now.Ticks.ToString();
                //_LastLog, , _Archive
                if (e.Row.Index > 0)
                {
                    DataGridViewRow previous_row = dataGridView1.Rows[e.Row.Index - 1];
                    //e.Row.Cells["Id"].Value = previous_row.Cells["Id"].Value;
                    //e.Row.Cells["State_"].Value = previous_row.Cells["State_"].Value;
                    //e.Row.Cells["Site"].Value = previous_row.Cells["Site"].Value;
                    //e.Row.Cells["Command_"].Value = previous_row.Cells["Command_"].Value;
                    e.Row.Cells["AdminEmails"].Value = previous_row.Cells["AdminEmails"].Value;
                    e.Row.Cells["RunTimeSpan"].Value = previous_row.Cells["RunTimeSpan"].Value;
                    e.Row.Cells["CrawlProductTimeout"].Value = previous_row.Cells["CrawlProductTimeout"].Value;
                    e.Row.Cells["RestartDelayIfBroken"].Value = previous_row.Cells["RestartDelayIfBroken"].Value;
                    //e.Row.Cells["Comment"].Value = previous_row.Cells["Comment"].Value;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        private void CrawlersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CrawlersForm_Deactivate(null, null);
        }

        private void CrawlersForm_Deactivate(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            prepare_row(dataGridView1.CurrentRow.Index);
            save_row(dataGridView1.CurrentRow.Index);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        internal void RefreshView()
        {
            if (dataGridView1.IsCurrentCellInEditMode)
                return;
            load_table();
        }

        private void bCheckNow_Click(object sender, EventArgs e)
        {
            SysTrayForm.This.CheckNow();
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            SysTrayForm.This.ToggleService();
        }

        private void dataGridView1_Leave(object sender, EventArgs e)
        {
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //if (((DataGridView)sender).CurrentRow == null)
            //    return;
            //load_table();
        }
    }
}

