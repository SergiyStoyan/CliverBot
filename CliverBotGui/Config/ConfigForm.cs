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

namespace Cliver.BotGui
{
    internal partial class ConfigForm : BaseForm
    {
        public ConfigForm()
        {
            InitializeComponent();
            
            listConfigTabs.DisplayMember = "Name";
            listConfigTabs.ValueMember = "CC";

            //this.Text = Program.Title + " Settings";
            
            splitContainer.Panel2.Controls.Clear();
            listConfigTabs.Items.Clear();
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
                    if(!cci.CC.PutValues2Properties())
                        return;
                Config.Save();
                Config.Reload();
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
                Type[] custom_ConfigControl_types = (from x in Assembly.GetEntryAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(ConfigControl)) select x).ToArray();
                Dictionary<string, ConfigControl> cccn2cc = new Dictionary<string, ConfigControl>();
                foreach (Type cct in custom_ConfigControl_types)
                {
                    ConfigControl cc = (ConfigControl)Activator.CreateInstance(cct);
                    if (cccn2cc.ContainsKey(cc.Name))
                        throw new Exception("There is one more ConfigControl named '" + cc.Name + "'");
                    cccn2cc.Add(cc.Name, cc);
                }

                Type[] default_ConfigControl_types = (from x in Assembly.GetExecutingAssembly().GetTypes() where x.BaseType == typeof(ConfigControl) select x).ToArray();
                Dictionary<string, ConfigControl> dccn2cc = new Dictionary<string, ConfigControl>();
                foreach (Type cct in default_ConfigControl_types)
                {
                    ConfigControl cc = (ConfigControl)Activator.CreateInstance(cct);
                    if (dccn2cc.ContainsKey(cc.Name))
                        throw new Exception("There is one more ConfigControl named '" + cc.Name + "'");
                    dccn2cc.Add(cc.Name, cc);
                }

                foreach (string name in CustomizationGuiApi.BotGui.GetConfigControlNames())
                {
                    ConfigControl cc = null;
                    if (!cccn2cc.TryGetValue(name, out cc))
                        if (!dccn2cc.TryGetValue(name, out cc))
                            LogMessage.Error("No ConfigControl found for name '" + name + "'");
                    add_ConfigControl(cc);
                }

                int last_open_config_tab_index = 0;
                int.TryParse(ConfigurationManager.AppSettings["LastOpenConfigTabIndex"], out last_open_config_tab_index);
                if (listConfigTabs.Items.Count > last_open_config_tab_index)
                    listConfigTabs.SelectedIndex = last_open_config_tab_index;
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
            ConfigurationManager.AppSettings["LastOpenConfigTabIndex"] = listConfigTabs.SelectedIndex.ToString();
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
    }
}
