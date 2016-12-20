//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Cliver.Bot;
using Cliver;

namespace Cliver.BotGui
{
    public partial class BotThreadManagerForm : BaseForm
    {
        //BotCycleControl current_displayed_bot_thread_control = null;
        //public BotCycleControl CurrentDisplayedBotThreadControl
        //{
        //    get
        //    {
        //        return current_displayed_bot_thread_control;
        //    }
        //}

        class BotThreadItem
        {
            public string Name { get; set; }
            public readonly BotThreadControl BTC;

            public BotThreadItem(string name, BotThreadControl btc)
            {
                this.Name = name;
                BTC = btc;
            }
        }

        public BotThreadManagerForm()
        {
            InitializeComponent();
            listBotThreads.DisplayMember = "Name";
            listBotThreads.ValueMember = "BTC";

            BotCycle.Created += BotCycle_Created;
            BotCycle.Finishing += BotCycle_Finishing;
        }

        void BotCycle_Created(int id)
        {
            try
            {
                this.Invoke(() => { _BotCycle_Created(id); });
            }
            catch (ThreadAbortException)
            {
            }
        }

        void _BotCycle_Created(int id)
        {
            BotThreadControl btc = (BotThreadControl)System.Activator.CreateInstance(GetBotThreadControlType(), id);
            btc.AutoSize = true;
            btc.Dock = System.Windows.Forms.DockStyle.Fill;
            btc.Location = new System.Drawing.Point(3, 16);
            btc.MinimumSize = new System.Drawing.Size(100, 100);
            btc.Size = new System.Drawing.Size(402, 286);
            btc.TabIndex = 0;
            // btc.Visible = false;
            this.groupBoxBotThread.Controls.Add(btc);

            lock (this)
            {
                id2bot_thread_control[id] = btc;
                listBotThreads.Items.Add(new BotThreadItem("#" + id, btc));
                MainForm.This.DisplayStatus2("Thread Count", Convert.ToString(listBotThreads.Items.Count));
            }
        }

        virtual public Type GetBotThreadControlType()
        {
            return typeof(BotThreadControl);
        }

        void BotCycle_Finishing(int id)
        {
            try
            {
                this.BeginInvoke(() => { _BotCycle_Finishing(id); });
            }
            catch (ThreadAbortException)
            {
            }
        }

        void _BotCycle_Finishing(int id)
        {
            lock (this)
            {
                BotThreadItem bti = null;
                BotThreadControl btc = null;
                foreach (BotThreadItem i in listBotThreads.Items)
                {
                    btc = i.BTC;
                    if (btc.Id == id)
                    {
                        bti = i;
                        break;
                    }
                }
                if (bti == null)
                    return;
                id2bot_thread_control.Remove(btc.Id);
                listBotThreads.Items.Remove(bti);
                this.groupBoxBotThread.Controls.Remove(btc);
                btc.Dispose();

                MainForm.This.DisplayStatus2("Thread Count", Convert.ToString(listBotThreads.Items.Count));
            }
        }

        private void listBotThreads_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lock (this)
                {
                    if (current_btc != null)
                        current_btc.Visible = false;
                    if (listBotThreads.SelectedIndex < 0)
                    {
                        current_btc = null;
                        return;
                    }
                    current_btc = ((BotThreadItem)listBotThreads.Items[listBotThreads.SelectedIndex]).BTC;
                    current_btc.Visible = true;
                    groupBoxBotThread.SetText(current_btc.Text);
                }
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }
        BotThreadControl current_btc = null;

        public void ClearBotThreadPanel()
        {
            try
            {
                this.Text = "Bot Thread Manager";

                //foreach (Control c in this.groupBox2.Controls)
                //    c.Enabled = false;
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }

        private void BotThreadManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                this.Visible = false;
                //this.ShowInTaskbar = false;
                //this.WindowState = FormWindowState.Minimized; 
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        internal BotThreadControl GetBotThreadControlById(int bot_cycle_id)
        {
            lock (this)
            {
                return id2bot_thread_control[bot_cycle_id];
            }
        }
        Dictionary<int, BotThreadControl> id2bot_thread_control = new Dictionary<int, BotThreadControl>();
    }
}