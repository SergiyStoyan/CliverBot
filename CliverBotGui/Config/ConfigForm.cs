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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Configuration;
using Cliver.Bot;
using System.Linq;
using System.Diagnostics;

namespace Cliver.BotGui
{
    internal partial class ConfigForm : BaseForm//Form// 
    {
        public ConfigForm()
        {
            InitializeComponent();

            listConfigTabs.DisplayMember = "Name";
            listConfigTabs.ValueMember = "CC";

            //this.Text = Program.Title + " Settings";
        }

        void add_ConfigControl(ConfigControl config_control)
        {
            try
            {
                config_control.AutoSize = true;
                config_control.Dock = System.Windows.Forms.DockStyle.Fill;
                this.splitContainer.Panel2.Controls.Add(config_control);
                listConfigTabs.Items.Add(new ConfigControlItem(config_control));
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }

        private void Save_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (Session.This != null)
                {
                    LogMessage.Inform("You cannot change settings while the bot is running.");
                    return;
                }
                foreach (ConfigControlItem cci in listConfigTabs.Items)
                    if (!cci.CC.GetData())
                    {
                        listConfigTabs.SelectedItem = cci;
                        return;
                    }
                //string storage_dir = null;
                //if (Cliver.Config.ReadOnly)
                //{
                //FolderBrowserDialog d = new FolderBrowserDialog();
                //d.Description = "The current config was open as read-only and must be saved in another location. Please select a folder.";
                //d.SelectedPath = Log.GetAppCommonDataDir();
                //if (d.ShowDialog() != DialogResult.OK)
                //    return;
                //storage_dir = d.SelectedPath;
                //}
                //Config.Save(storage_dir);
                if (Cliver.Config.ReadOnly && !LogMessage.AskYesNo("The current config was loaded from a not default location and will replace the config stored in the default location. Are you sure to proceed?", false))
                    return;
                Config.Save();
                //Config.Reload();
                Close();
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        private void About_Click(object sender, System.EventArgs e)
        {
            AboutForm a = new AboutForm();
            a.ShowDialog();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Config.Reload(Cliver.Config.DefaultStorageDir);
            this.Close();
        }

        private void listConfigTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ((ConfigControlItem)listConfigTabs.SelectedItem).CC.BringToFront();
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            try
            {
                splitContainer.Panel2.Controls.Clear();
                listConfigTabs.Items.Clear();

                List<Type> default_ConfigControl_types = (from x in Assembly.GetExecutingAssembly().GetTypes() where x.BaseType == typeof(ConfigControl) select x).ToList();
                List<Type> custom_ConfigControl_types = (from x in Assembly.GetEntryAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(ConfigControl)) select x).ToList();
                List<Type> ConfigControl_types = new List<Type>(default_ConfigControl_types);
                ConfigControl_types.AddRange(custom_ConfigControl_types);
                Dictionary<string, ConfigControl> sections2cc = new Dictionary<string, ConfigControl>();
                foreach (Type cct in ConfigControl_types)
                {
                    ConfigControl cc = (ConfigControl)Activator.CreateInstance(cct);
                    sections2cc[cc.Section] = cc;
                }

                foreach (string section in BotGui.ConfigControlSections)
                {
                    ConfigControl cc = null;
                    if (!sections2cc.TryGetValue(section, out cc))
                        LogMessage.Error("No ConfigControl found for section '" + section + "'");
                    add_ConfigControl(cc);
                }

                if (Settings.Gui.ConfigFormSize != System.Drawing.Size.Empty)
                    this.Size = Settings.Gui.ConfigFormSize;
                if (Settings.Gui.LastOpenConfigTabIndex >= 0)
                    listConfigTabs.SelectedIndex = Settings.Gui.LastOpenConfigTabIndex;
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }

            if (Session.This != null)
            {
                foreach (ConfigControlItem cci in listConfigTabs.Items)
                    cci.CC.Enabled = false;
                this.Save.Enabled = false;
            }
            else
            {
                foreach (ConfigControlItem cci in listConfigTabs.Items)
                    cci.CC.Enabled = true;
                this.Save.Enabled = true;
            }
        }

        class ConfigControlItem
        {
            public string Name { get; set; }
            public readonly ConfigControl CC;

            public ConfigControlItem(ConfigControl cc)
            {
                Name = cc.Name;
                CC = cc;
            }
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Gui.ConfigFormSize = this.Size;
            Settings.Gui.LastOpenConfigTabIndex = listConfigTabs.SelectedIndex;
            Settings.Gui.Save();
        }

        private void bReset_Click(object sender, EventArgs e)
        {
            if (LogMessage.AskYesNo("Do you really want to reset properties to their default values?", false))
            {
                this.splitContainer.Panel2.Controls.Clear();
                listConfigTabs.Items.Clear();
                Config.Reset();
                //Config.Save();
                ConfigForm_Load(null, null);
            }
        }

        private void bStore_Click(object sender, EventArgs e)
        {
            Process.Start(Cliver.Config.CompleteStorageDir);
        }

        private void bLoad_Click(object sender, EventArgs e)
        {
            SessionsForm d = new SessionsForm();
            d.ShowDialog();
            if (d.SessionDir == null)
                return;
            Config.Reload(d.SessionDir, true);
            ConfigForm_Load(null, null);
        }
    }
}