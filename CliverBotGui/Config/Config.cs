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
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace Cliver.BotGui
{
    /// <summary>
    /// Syncronizes controls and settings
    /// </summary>
    public static class Config
    {
        static Config()
        {
            List<Assembly> sas = new List<Assembly>();
            sas.Add(Assembly.GetEntryAssembly());
            foreach (AssemblyName an in Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(an => Regex.IsMatch(an.Name, @"^Cliver")))
                sas.Add(Assembly.Load(an));
            setting_sections2fi = new Dictionary<string, FieldInfo>();
            foreach (Assembly sa in sas)
                foreach (Type st in sa.GetExportedTypes().Where(t => t.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase))))
                    setting_sections2fi[st.Name] = st.GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static);
            setting_sections2fi2 = new Dictionary<string, FieldInfo>();
            foreach (Assembly sa in sas)
                foreach (Type st in sa.GetExportedTypes().Where(t => t.IsSubclassOf(typeof(Cliver.Settings))))
                    setting_sections2fi2[st.Name] = st.GetField("This", BindingFlags.Public | BindingFlags.Static);

            Reload();
        }
        static readonly Dictionary<string, FieldInfo> setting_sections2fi;
        static readonly Dictionary<string, FieldInfo> setting_sections2fi2;

        public static void Initialize()
        {
            //dummy to trigger static constructor
        }

        public static void Set(string section, string parameter, object value)
        {
            try
            {
                FieldInfo fi;
                if (setting_sections2fi.TryGetValue(section, out fi))
                {
                    PropertyInfo pi = fi.FieldType.GetProperty(parameter);
                    if (pi.PropertyType == typeof(int) && value is string)
                        value = int.Parse((string)value);
                    else if (pi.PropertyType == typeof(double) && value is string)
                        value = double.Parse((string)value);
                    else if (pi.PropertyType == typeof(long) && value is string)
                        value = long.Parse((string)value);
                    ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null))[parameter] = value;
                    return;
                }
                fi = setting_sections2fi2[section];
                {
                    FieldInfo pi = fi.FieldType.GetField(parameter);
                    if (pi.FieldType == typeof(int) && value is string)
                        value = int.Parse((string)value);
                    else if (pi.FieldType == typeof(double) && value is string)
                        value = double.Parse((string)value);
                    else if (pi.FieldType == typeof(long) && value is string)
                        value = long.Parse((string)value);
                    pi.SetValue(fi.GetValue(null), value);
                }
            }
            catch (Exception e)
            {
                for (; e.InnerException != null; e = e.InnerException) ;
                LogMessage.Error("Could not set '" + section + "." + parameter + "' to " + value.ToString() + "\r\n\r\n" + e.Message);
            }
        }

        public static object Get(string section, string parameter)
        { 
            try
            {
                FieldInfo fi;
                if (setting_sections2fi.TryGetValue(section, out fi))
                    return ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null))[parameter];
                fi = setting_sections2fi2[section];
                FieldInfo pi = fi.FieldType.GetField(parameter);
                return pi.GetValue(fi.GetValue(null));
            }
            catch (Exception e)
            {
                for (; e.InnerException != null; e = e.InnerException) ;
                LogMessage.Error("Could not find setting '" + section + "." + parameter + "'\r\n\r\n" + e.Message);
            }
            return null;
        }

        static void invoke(System.Reflection.FieldInfo fi, string method)
        {
            object di = fi.GetValue(null);
            fi.FieldType.GetMethod(method).Invoke(di, null);
        }

        public static void Reload()
        {
            foreach (System.Reflection.FieldInfo fi in setting_sections2fi.Values)
                invoke(fi, "Reload");
            Cliver.Config.Reload();
        }

        public static void Save()
        {
            foreach (System.Reflection.FieldInfo fi in setting_sections2fi.Values)
                invoke(fi, "Save");
            Cliver.Config.Save();
        }

        public static void Reset()
        {
            foreach (System.Reflection.FieldInfo fi in setting_sections2fi.Values)
                invoke(fi, "Reset");
            Cliver.Config.Reset();
        }
    }
}