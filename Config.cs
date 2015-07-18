//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace Cliver.Bot
{
    public static class Config
    {
        static Config()
        {
            Reload();
        }

        public static void Initialize()
        {
            //dummy to trigger static constructor
        }
                
        public static void Set(string section, string parameter, object value)
        {
            FieldInfo fi = get_setting_for_namespace(Assembly.GetExecutingAssembly(), "Cliver.Bot.Properties", section);
            try
            {
                PropertyInfo pi = fi.FieldType.GetProperty(parameter);
                if (pi.PropertyType == typeof(int) && value is string)
                    value = int.Parse((string)value);
                ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null))[parameter] = value;
                return;
            }
            catch { }
            fi = get_setting_for_namespace(Assembly.GetEntryAssembly(), CustomizationApi.CUSTOM_NAMESPACE + ".Properties", section);
            try
            {
                PropertyInfo pi = fi.FieldType.GetProperty(parameter);
                if (pi.PropertyType == typeof(int) && value is string)
                    value = int.Parse((string)value);
                ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null))[parameter] = value;
                return;
            }
            catch { }
            LogMessage.Error("Could not set '" + section + "." + parameter + "' to " + value.ToString());

            //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ////config.AppSettings.Settings
            //ClientSettingsSection cs = (ClientSettingsSection)config.GetSectionGroup("userSettings").Sections["Cliver.Properties.General.Default"];
            //string t = cs.Settings.Get("RestoreErrorItemsAsNew").Value.ValueXml.InnerText;
        }

        public static object Get(string section, string parameter)
        {
            FieldInfo fi = get_setting_for_namespace(Assembly.GetExecutingAssembly(), "Cliver.Bot.Properties", section);
            try
            {
                return ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null))[parameter];
            }
            catch { }
            fi = get_setting_for_namespace(Assembly.GetEntryAssembly(), CustomizationApi.CUSTOM_NAMESPACE + ".Properties", section);
            try
            {
                return ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null))[parameter];
            }
            catch { }
            LogMessage.Error("Could not find setting '" + section + "." + parameter + "'");
            return null;
        }

        static FieldInfo get_setting_for_namespace(Assembly assembly, string ns, string section)
        {
            Type settings_type = assembly.GetType(ns + "." + section);
            if (settings_type == null)
                return null;
            return settings_type.GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static);
        }

        //public static T Get<T>(string section, string parameter)
        //{
        //    return (T)((global::System.Configuration.ApplicationSettingsBase)typeof(Config).GetField(section).GetValue(null))[parameter];
        //}

        static void invoke(Type settings_type, string method)
        {
            object di = settings_type.GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            settings_type.GetMethod(method).Invoke(di, null);
        }

        public static void Reload()
        {
            Type[] settings_types = (from x in Assembly.GetExecutingAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x).ToArray();
            foreach (Type t in settings_types)
                invoke(t, "Reload");

            Type[] custom_settings_types = (from x in Assembly.GetEntryAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x).ToArray();
            foreach (Type t in settings_types)
                invoke(t, "Reload");
        }

        public static void Save()
        {
            Type[] settings_types = (from x in Assembly.GetExecutingAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x).ToArray();
            foreach (Type t in settings_types)
                invoke(t, "Save");

            Type[] custom_settings_types = (from x in Assembly.GetEntryAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x).ToArray();
            foreach (Type t in settings_types)
                invoke(t, "Save");
        }

        public static void Reset()
        {
            Type[] settings_types = (from x in Assembly.GetExecutingAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x).ToArray();
            foreach (Type t in settings_types)
                invoke(t, "Reset");

            Type[] custom_settings_types = (from x in Assembly.GetEntryAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x).ToArray();
            foreach (Type t in settings_types)
                invoke(t, "Reset");
        }

        //public static void Save()
        //{
        //    FieldInfo[] default_settings_section_fis = (from x in Assembly.GetExecutingAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x.GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static)).ToArray();
        //    foreach (FieldInfo fi in default_settings_section_fis)
        //        ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null)).Save();
        //    FieldInfo[] custom_settings_section_fis = (from x in Assembly.GetEntryAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x.GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static)).ToArray();
        //    foreach (FieldInfo fi in custom_settings_section_fis)
        //        ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null)).Save();
        //}

        //public static void Reload()
        //{
        //    FieldInfo[] default_settings_section_fis = (from x in Assembly.GetExecutingAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x.GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static)).ToArray();
        //    foreach (FieldInfo fi in default_settings_section_fis)
        //        ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null)).Reload();
        //    //Properties.Log.Default.Reload();
        //    FieldInfo[] custom_settings_section_fis = (from x in Assembly.GetEntryAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x.GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static)).ToArray();
        //    foreach (FieldInfo fi in custom_settings_section_fis)
        //        ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null)).Reload();
        //}

        //public static void Reset()
        //{
        //    FieldInfo[] default_settings_section_fis = (from x in Assembly.GetExecutingAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x.GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static)).ToArray();
        //    foreach (FieldInfo fi in default_settings_section_fis)
        //        ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null)).Reset();
        //    FieldInfo[] custom_settings_section_fis = (from x in Assembly.GetEntryAssembly().GetExportedTypes() where x.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase)) select x.GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static)).ToArray();
        //    foreach (FieldInfo fi in custom_settings_section_fis)
        //        ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null)).Reset();
        //}
    }
}
