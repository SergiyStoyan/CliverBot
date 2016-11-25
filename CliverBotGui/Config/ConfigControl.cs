using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Cliver.Bot;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Cliver.BotGui
{
    public partial class ConfigControl : UserControl
    {
        virtual public string Section
        {
            get;
        }

        public ConfigControl()
        {
            InitializeComponent();
        }

        virtual protected void SetToolTip()
        {
        }

        new internal bool Enabled
        {
            set
            {
                foreach (Control c in this.group_box.Controls)
                    if (Regex.IsMatch(c.Name, "^_0_"))
                        c.Enabled = true;
                    else
                        c.Enabled = value;
            }
        }

        private void ConfigControl_Load(object sender, EventArgs e)
        {
            group_box.Text = Section;
            Name = Section;
            toolTip1.AutoPopDelay = 100000;
            try
            {
                SetToolTip();
                Set();
            }
            catch(Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        virtual protected void Set()
        {
            set_group_box_values_from_config();
        }

        protected void set_group_box_values_from_config()
        {
            foreach (Control c in group_box.Controls)
            {
                if (Regex.IsMatch(c.Name, "^_1_"))
                    continue;
                Type t = c.GetType();
                if (t == typeof(System.Windows.Forms.TextBox))
                {
                    object o = Config.Get(group_box.Text, c.Name);
                    if (o != null)
                        c.Text = o.ToString();
                    else
                        c.Text = "";
                }
                else if (t == typeof(System.Windows.Forms.CheckBox))
                {
                    object o = Config.Get(group_box.Text, c.Name);
                    if (o != null)
                        ((System.Windows.Forms.CheckBox)c).Checked = (bool)o;
                    else
                        ((System.Windows.Forms.CheckBox)c).Checked = false;
                }
                else if (t == typeof(System.Windows.Forms.RadioButton))
                {
                    object o = Config.Get(group_box.Text, c.Name);
                    if (o != null)
                        ((System.Windows.Forms.RadioButton)c).Checked = (bool)o;
                    else
                        ((System.Windows.Forms.RadioButton)c).Checked = false;
                }
                else if (t == typeof(System.Windows.Forms.DateTimePicker))
                {
                    object o = Config.Get(group_box.Text, c.Name);
                    if (o != null)
                        ((System.Windows.Forms.DateTimePicker)c).Value = (DateTime)o;
                }
            }
        }
        
        virtual protected bool Get()
        {
            put_control_values_to_config(Name, group_box);
            return true;
        }

        internal bool GetData()
        {
            try
            {
                return Get();
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
                return false;
            }
        }

        protected void put_control_values_to_config(string section, Control control)
        {
            try
            {
                foreach (Control c in control.Controls)
                {
                    if (c.Controls.Count > 0)
                    {
                        put_control_values_to_config(section, c);
                        continue;
                    }
                    object o = get_value_for_config(c);
                    if (o != null)
                        Config.Set(section, c.Name, o);
                }
            }
            catch (Exception ex)
            {
                LogMessage.Error(ex);
            }
        }

        object get_value_for_config(Control control)
        {
            if (control.Name.StartsWith("_1_"))
                return null;
            Type t = control.GetType();
            if (t == typeof(System.Windows.Forms.TextBox))
                return ((System.Windows.Forms.TextBox)control).Text;
            else if (t == typeof(System.Windows.Forms.CheckBox))
                return (((System.Windows.Forms.CheckBox)control).Checked);
            else if (t == typeof(System.Windows.Forms.RadioButton))
                return (((System.Windows.Forms.RadioButton)control).Checked);
            else if (t == typeof(System.Windows.Forms.DateTimePicker))
                return ((System.Windows.Forms.DateTimePicker)control).Value;
            else
                //throw new Exception("No case for type: " + control.GetType());
                return null;
        }
    }
}
