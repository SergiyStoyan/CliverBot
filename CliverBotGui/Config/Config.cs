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

namespace Cliver.Bot
{
    /// <summary>
    /// Syncronizes controls and settings
    /// </summary>
    public static class Config
    {
        static Config()
        {
            setting_sections2fi = new Dictionary<string, FieldInfo>();
            List<Assembly> sas = new List<Assembly>();
            sas.Add(Assembly.GetEntryAssembly());
            foreach (AssemblyName an in Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(an => Regex.IsMatch(an.Name, @"^Cliver")))
                sas.Add(Assembly.Load(an));
            foreach (Assembly sa in sas)
                foreach (Type st in sa.GetExportedTypes().Where(t => t.IsSubclassOf(typeof(global::System.Configuration.ApplicationSettingsBase))))
                    setting_sections2fi[st.Name] = st.GetField("defaultInstance", BindingFlags.NonPublic | BindingFlags.Static); 

            Reload();
        }
        static readonly Dictionary<string, FieldInfo> setting_sections2fi;

        public static void Initialize()
        {
            //dummy to trigger static constructor
        }

        public static void Set(string section, string parameter, object value)
        {
            string error = null;
            if (set_(setting_sections2fi[section], parameter, value, out error))
                return;
            LogMessage.Error("Could not set '" + section + "." + parameter + "' to " + value.ToString() + "\r\n\r\n" + error);
        }

        static bool set_(FieldInfo fi, string parameter, object value, out string error)
        {
            try
            {
                PropertyInfo pi = fi.FieldType.GetProperty(parameter);
                if (pi.PropertyType == typeof(int) && value is string)
                    value = int.Parse((string)value);
                else if (pi.PropertyType == typeof(double) && value is string)
                    value = double.Parse((string)value);
                else if (pi.PropertyType == typeof(long) && value is string)
                    value = long.Parse((string)value);
                ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null))[parameter] = value;
                error = null;
                return true;
            }
            catch (Exception e)
            {
                for (; e.InnerException != null; e = e.InnerException) ;
                error = e.Message;
            }
            return false;
        }

        public static object Get(string section, string parameter)
        {
            object value;
            string error; 
            if (get_(setting_sections2fi[section], parameter, out value, out error))
                return value;
            LogMessage.Error("Could not find setting '" + section + "." + parameter + "'\r\n\r\n" + error);
            return null;
        }

        static bool get_(FieldInfo fi, string parameter, out object value, out string error)
        {
            try
            {
                error = null;
                value = ((global::System.Configuration.ApplicationSettingsBase)fi.GetValue(null))[parameter];
                return true;
            }
            catch (Exception e)
            {
                for (; e.InnerException != null; e = e.InnerException) ;
                error = e.Message;
                value = true;
            }
            return false;
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
        }

        public static void Save()
        {
            foreach (System.Reflection.FieldInfo fi in setting_sections2fi.Values)
                invoke(fi, "Save");
        }

        public static void Reset()
        {
            foreach (System.Reflection.FieldInfo fi in setting_sections2fi.Values)
                invoke(fi, "Reset");
        }
    }
}