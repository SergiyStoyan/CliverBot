//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Configuration;
using System.Diagnostics;
using System.Data.Odbc;
using System.Reflection;
//using System.Timers;
using Cliver.Bot;


namespace Cliver.BotGui
{
    public partial class MainForm : BaseForm//Form//
    {
        public MainForm()
        {
            InitializeComponent();
            this.Text = Cliver.Bot.Program.Title;

            if (ProgramRoutines.IsParameterSet(CommandLineParameters.CONFIGURE))
                this.buttonStart.Enabled = false;

            ProgressBarInputItemQueueName = Session.GetFirstDeclaredInputItemType().Name;
            Session.Closing += Session_Closing;
            Session.Closed += Session_Closed;
            InputItemQueue.Progress += new InputItemQueue.OnProgress(Session_InputItemQueueProgress);
        }

        private void Session_Closed()
        {
            if (Program.Mode == Program.ProgramMode.AUTOMATIC)
                System.Windows.Forms.Application.Exit();
        }

        void Session_Closing()
        {
            try
            {
                this.Invoke(() => {
                    on_session_closing();
                });
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }

        void Session_InputItemQueueProgress(InputItemQueue input_item_queue, int total_item_count, int processed_item_count)
        {
            This.DisplayStatus(input_item_queue.Name, "taken " + processed_item_count.ToString() + " /remain " + (total_item_count - processed_item_count).ToString());
            if (ProgressBarInputItemQueueName == input_item_queue.Name)
                This.display_progress(total_item_count, processed_item_count);
        }

        internal string ProgressBarInputItemQueueName;

        internal readonly static MainForm This = Bot.Activator.Create<MainForm>(false);
        internal readonly BotThreadManagerForm BotThreadManagerForm = Bot.Activator.Create<BotThreadManagerForm>(false);

        private void MainForm_Load(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            int button_count = 0;
            foreach (ButtonAction ba in GetButtonActions())
            {
                button_count++;
                   Button b = new Button();
                b.Size = buttonStart.Size;
                b.Padding = buttonStart.Padding;
                b.Margin = buttonStart.Margin;
                b.FlatStyle = buttonStart.FlatStyle;
                b.ForeColor = Color.Black;
                b.Text = ba.Name;
                b.Click += (object s, EventArgs ea) => { ba.Action(); };
                flowLayoutPanel1.Controls.Add(b);
                flowLayoutPanel1.Controls.SetChildIndex(b, 0);
            }
            button_count++;
            flowLayoutPanel1.Controls.Add(buttonStart);
            flowLayoutPanel1.Controls.SetChildIndex(buttonStart, 0);
            
            int bw2 = this.ClientSize.Width / button_count;
            if (bw2 > buttonStart.Width)
                foreach (Button b in flowLayoutPanel1.Controls)
                    b.Width = bw2;
            this.Width = this.Width - this.ClientSize.Width + buttonStart.Width * button_count;
            
            listBoxStatus.BackColor = Color.FromKnownColor(KnownColor.Control);
            // Progress.Enabled = false; 
            progressBar.Value = 0;
            buttonStart.Select();
            buttonStart.Focus();

            if (Program.Mode == Program.ProgramMode.AUTOMATIC)
            {
                this.WindowState = FormWindowState.Minimized;
                start_session();
            }
        }

        virtual public List<ButtonAction> GetButtonActions()
        {
            return new List<ButtonAction> {
                new ButtonAction { Name = "About", Action=()=> 
                    {
                        AboutForm f = Bot.Activator.Create<AboutForm>(false);
                        f.ShowDialog();
                    }
                },
                new ButtonAction { Name = "Help", Action=()=> 
                    {
                        try
                        {
                            Process.Start(Bot.Properties.App.Default.HelpUri);
                        }
                        catch (Exception ex)
                        {
                            LogMessage.Error(ex);
                        }
                    }
                },
                new ButtonAction { Name = "Input", Action=()=> 
                    {
                        try
                        {
                            Process.Start(Bot.Settings.Input.File);
                        }
                        catch (Exception ex)
                        {
                            LogMessage.Error(ex);
                        }
                    }
                },
                new ButtonAction { Name = "Work Dir", Action=()=> 
                    {
                        try
                        {
                            Process.Start(Log.WorkDir);
                        }
                        catch (Exception ex)
                        {
                            LogMessage.Error(ex);
                        }
                    }
                },
                new ButtonAction { Name = "Settings", Action=()=>
                    {
                        ConfigForm f = Bot.Activator.Create<ConfigForm>(false);
                        f.ShowDialog(MainForm.This);
                    }
                },
                new ButtonAction { Name = "Threads", Action=()=>
                    {
                        BotThreadManagerForm.Visible = true;
                        //BotThreadManagerForm.ShowInTaskbar = true;
                        //BotThreadManagerForm.WindowState = FormWindowState.Normal;
                        BotThreadManagerForm.Activate();
                    }
                },
            };
        }
        public class ButtonAction
        {
            public string Name;
            public Action Action;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (closing)//looped by Application.Exit();
                return;
            if (buttonStart.Text == "Stop" && !LogMessage.AskYesNo("Terminating Session. Are you sure to proceed?", true))
            {
                e.Cancel = true;
                return;
            }
            Session.Close();
            try
            {
                closing = true;
                Application.Exit();
            }
            catch { }
            Environment.Exit(0);
            //Thread.Sleep(1000);
            //System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        bool closing = false;

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (buttonStart.Text != "Stop")
            {
                if (Session.This != null)
                    return;
                start_session();
            }
            else
            {
                if (Session.This == null)
                    return;
                if (!LogMessage.AskYesNo("Terminating Session. Are you sure to proceed?", true))
                    return;
                Session.Close();
            }
        }

        IntPtr ip1 = IntPtr.Zero;

        void start_session()
        {
            try
            {
                set_start_button(true);
                Environment.CurrentDirectory = Log.AppDir;

                ip1 = this.listBoxStatus.Handle;//it is gotten here in order that this.InvokeRequired to work correctly

                listBoxStatus.BackColor = Color.FromKnownColor(KnownColor.Window);
                clear_status_rows();

                DisplayStatus2("Session", "starting...");
                DisplayStatus2("Thread Count", "0");

                start_session_t = ThreadRoutines.Start(start_session_);

                /*
                 * Why does InvokeRequired return false when you'd like it to return true?
                 * I’ve seen a number of developers surprised by InvokeRequired returning false in situations when they know it’s a cross thread call.
                 * The reason is that the underlying window handle associated with the control has not been created yet at the time of the call.
                 * Since InvokeRequired is meant to be used with either BeginInvoke or just Invoke, and neither of these methods can succeed until a windows message pump gets associated with the control, it elects to return false.                  
                 */
                IntPtr ip = BotThreadManagerForm.Handle;//that's why handle is gotten here                
            }
            //catch (ThreadAbortException)
            //{
            //    Session.Close();
            //}
            catch (Exception e)
            {
                Session.Close();
                if (Program.Mode == Program.ProgramMode.AUTOMATIC)
                    LogMessage.Exit(e);
                else
                    LogMessage.Error(e);
            }
        }

        Thread start_session_t;

        void start_session_()
        {
            try
            {
                Session.Start();

                Log.Main.Inform("Mode: " + Program.Mode);

                if (Session.This != null)
                    if (Session.This.Restored)
                        DisplayStatus2("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss") + ", restored: " + Session.This.RestoreTime.ToString("dd-MM-yy HH:mm:ss"));
                    else
                        DisplayStatus2("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss"));
            }
            catch (ThreadAbortException)
            {
                Session.Close();
            }
            catch (Exception e)
            {
                Session.Close();
                //if (Program.Mode == Program.ProgramMode.AUTOMATIC)
                //    System.Windows.Forms.Application.Exit();
                if (Program.Mode == Program.ProgramMode.AUTOMATIC)
                    LogMessage.Exit(e);
                else
                    LogMessage.Error(e);
            }
        }

        System.Drawing.Font SDF;
        System.Drawing.Color SDC;
        //System.Drawing.Color SDC2;  

        void on_session_closing()
        {
            TimeSpan duration = DateTime.Now - Session.This.RestoreTime;
            string session_duration = Regex.Replace(duration.ToString(), @"(.*)\..*", "$1", RegexOptions.Compiled);
            if (Session.This.Restored)
                DisplayStatus("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss") + ", last duration: " + session_duration + ", " + Session.State.ToString() + ": " + DateTime.Now.ToString("dd-MM-yy HH:mm:ss"));
            else
                DisplayStatus("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss") + ", duration: " + session_duration + ", " + Session.State.ToString() + ": " + DateTime.Now.ToString("dd-MM-yy HH:mm:ss"));
            set_start_button(false);
            GC.Collect();
        }

        void set_start_button(bool started)
        {
            if (started)
            {
                buttonStart.Text = "Stop";
                SDF = buttonStart.Font;
                SDC = buttonStart.ForeColor;
                buttonStart.ForeColor = System.Drawing.Color.Crimson;
            }
            else
            {
                buttonStart.Text = "Start";
                listBoxStatus.BackColor = Color.FromKnownColor(KnownColor.Control);
                buttonStart.Font = SDF;
                buttonStart.ForeColor = SDC;
                progressBar.Value = 0;
            }
        }

        internal void DisplayStatus(string name, string state)
        {
            try
            {
                this.BeginInvoke(() => { _DisplayStatus(name, state); });
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }

        internal void DisplayStatus2(string name, string state)
        {
            try
            {
                this.BeginInvoke(() => { _DisplayStatus(name, state); });
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }

        internal void _DisplayStatus(string name, string state)
        {
            lock (this)
            {
                if (!status_row_name2status_row_is.ContainsKey(name))
                {
                    int i = this.listBoxStatus.Items.Add("");
                    if (i < 0)
                        return;
                    status_row_name2status_row_is[name] = i;
                }
                this.listBoxStatus.Items[status_row_name2status_row_is[name]] = name + ": " + state;
            }
        }
        Dictionary<string, int> status_row_name2status_row_is = new Dictionary<string, int>();

        void clear_status_rows()
        {
            status_row_name2status_row_is.Clear();
            listBoxStatus.Items.Clear();
        }

        public void display_progress(int total_item_count, int processed_item_count)
        {
            try
            {
                this.BeginInvoke(() => { _display_progress(total_item_count, processed_item_count); });
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }

        public void _display_progress(int total_item_count, int processed_item_count)
        {
            lock (this)
            {
                progressBar.Maximum = total_item_count + 1;
                progressBar.Value = processed_item_count;
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            AF = new AboutForm();
            AF.Show(this);
        }
        AboutForm AF = null;

        private void Progress_Click(object sender, EventArgs e)
        {
            BotThreadManagerForm.Visible = true;
            //BotThreadManagerForm.ShowInTaskbar = true;
            //BotThreadManagerForm.WindowState = FormWindowState.Normal;
            BotThreadManagerForm.Activate();
        }
    }
}