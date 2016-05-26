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
    internal partial class MainForm : BaseForm
    {
        internal MainForm()
        {
            InitializeComponent();
            this.Text = Cliver.Bot.Program.Title;

            ProgressBarInputItemQueueName = Session.GetFirstDeclaredInputItemType().Name;
            Session.Closing += Session_Closing;
            InputItemQueue.Progress += new InputItemQueue.OnProgress(Session_InputItemQueueProgress);
        }

        void Session_Closing()
        {
            try
            {
                this.Invoke(() => { on_session_closing(); });
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

        internal readonly static MainForm This = new MainForm();
        internal readonly BotThreadManagerForm BotThreadManagerForm = new BotThreadManagerForm();
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            tools_form = CustomizationGuiApi.BotGui.GetToolsForm();
            if (tools_form == null)
            {//hide Tools button if no Tools exists
                bTools.Enabled = false;

                flowLayoutPanel1.Controls.Remove(bTools);
                bTools.Dispose();
                int button_count = 0;
                foreach (Control c in flowLayoutPanel1.Controls)
                    button_count++;
                int button_width = (int)((float)flowLayoutPanel1.Width / (float)button_count);
                foreach (Control c in flowLayoutPanel1.Controls)
                    c.Width = button_width;
                int width_diff = bAbout.Left - flowLayoutPanel1.Left;
                this.MinimumSize = new Size(this.MinimumSize.Width - width_diff, this.MinimumSize.Height);
                this.Width -= width_diff;
                this.AutoSizeMode = AutoSizeMode.GrowOnly;
            }
            else
                bTools.Text = tools_form.Text;

            listBoxStatus.BackColor = Color.FromKnownColor(KnownColor.Control);

            // Progress.Enabled = false; 
            progressBar.Value = 0;

            if (Cliver.Bot.Program.Mode == Bot.Program.ProgramMode.AUTOMATIC)
            {
                this.WindowState = FormWindowState.Minimized;
                start_session();
            }

            buttonStart.Select();
            buttonStart.Focus();
        }

        Form tools_form = null;

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
                start_session();
            }
            else
            {
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

                start_session_t = new Thread(new ThreadStart(start_session_));
                //start_session_.Priority = ThreadPriority.Lowest;
                //Thread.CurrentThread.Priority = ThreadPriority.Highest;
                start_session_t.Start();

                /*
                 * Why does InvokeRequired return false when you'd like it to return true?
                 * I’ve seen a number of developers surprised by InvokeRequired returning false in situations when they know it’s a cross thread call.
                 * The reason is that the underlying window handle associated with the control has not been created yet at the time of the call.
                 * Since InvokeRequired is meant to be used with either BeginInvoke or just Invoke, and neither of these methods can succeed until a windows message pump gets associated with the control, it elects to return false.                  
                 */
                IntPtr ip = BotThreadManagerForm.Handle;//that's why handle is gotten here                
            }
            catch (ThreadAbortException)
            {
                Session.Close();
            }
            catch (Exception e)
            {
                Session.Close();
                if (Cliver.Bot.Program.Mode == Bot.Program.ProgramMode.AUTOMATIC)
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

                if (Session.This != null)
                    if (Session.This.Restored)
                        DisplayStatus2("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss") + ", restored: " + Session.This.RestoreTime.ToString("dd-MM-yy HH:mm:ss"));
                    else
                        DisplayStatus2("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss"));
            }
            catch (Exception e)
            {
                if (!(e is ThreadAbortException))
                {
                    if (Cliver.Bot.Program.Mode == Bot.Program.ProgramMode.AUTOMATIC)
                        LogMessage.Exit(e);
                    else
                        LogMessage.Error(e);
                }
                Session.Close();
            }
        }

        System.Drawing.Font SDF;
        System.Drawing.Color SDC;
        //System.Drawing.Color SDC2;  

        void on_session_closing()
        {
            TimeSpan duration = DateTime.Now - Session.This.RestoreTime;
            string session_duration = Regex.Replace(duration.ToString(), @"(.*)\..*", "$1", RegexOptions.Compiled);
            if (Session.This.IsUnprocessedInputItem)
            {
                if (Session.This.Restored)
                    DisplayStatus("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss") + ", last duration: " + session_duration + ", broken: " + DateTime.Now.ToString("dd-MM-yy HH:mm:ss"));
                else
                    DisplayStatus("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss") + ", last duration: " + session_duration + ", broken: " + DateTime.Now.ToString("dd-MM-yy HH:mm:ss"));
                Log.Main.Write("BROKEN");
            }
            else if(Session.This.IsItemToRestore)
            {
                if (Session.This.Restored)
                    DisplayStatus("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss") + ", last duration: " + session_duration + ", finished with errors: " + DateTime.Now.ToString("dd-MM-yy HH:mm:ss"));
                else
                    DisplayStatus("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss") + ", last duration: " + session_duration + ", finished with errors: " + DateTime.Now.ToString("dd-MM-yy HH:mm:ss"));
                Log.Main.Write("UNCOMPLETED");
            }
            else
            {
                if (Session.This.Restored)
                    DisplayStatus("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss") + ", last duration: " + session_duration + ", completed: " + DateTime.Now.ToString("dd-MM-yy HH:mm:ss"));
                else
                    DisplayStatus("Session", "started: " + Session.This.StartTime.ToString("dd-MM-yy HH:mm:ss") + ", duration: " + session_duration + ", completed: " + DateTime.Now.ToString("dd-MM-yy HH:mm:ss"));
                Log.Main.Write("COMPLETED");
            }

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

        private void Settings_Click(object sender, EventArgs e)
        {
            CF = new ConfigForm();
            CF.Show(MainForm.This);
        }
        ConfigForm CF = null;

        private void Progress_Click(object sender, EventArgs e)
        {
            BotThreadManagerForm.Visible = true;
            //BotThreadManagerForm.ShowInTaskbar = true;
            //BotThreadManagerForm.WindowState = FormWindowState.Normal;
            BotThreadManagerForm.Activate();
        }

        private void Help_Click(object sender, EventArgs e)
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

        private void buttonWorkDir_Click(object sender, EventArgs e)
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

        private void buttonInput_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Bot.Properties.Input.Default.InputFile);
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        private void bTools_Click(object sender, EventArgs e)
        {
            try
            {
                if (tools_form == null)
                    return;
                tools_form.ShowDialog();
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }
    }
}