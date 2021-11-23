using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;

namespace Cliver.Bot
{
    public class Config
    {
        public static bool ReadOnly { get; private set; } = true;

        static internal void Reload(string configfile = null)
        {

            foreach (SettingsFieldInfo sfi in Cliver.Config.GetSettingsFieldInfos())
                sections2SettingsFieldInfo[sfi.Name] = sfi;

            if (configfile == null)
            {
                ReadOnly = false;
                Cliver.Config.Reload();
            }
            else
            {
                ReadOnly = true;
                Dictionary<string, Dictionary<string, object>> sections2parameters2value = Serialization.Json.Load<Dictionary<string, Dictionary<string, object>>>(configfile);
                foreach (string section in sections2parameters2value.Keys)
                {
                    Dictionary<string, object> parameters2value = sections2parameters2value[section];
                    SettingsFieldInfo sfi = sections2SettingsFieldInfo[section];
                    Cliver.Settings o = sfi.GetObject();
                    foreach (string p in parameters2value.Keys)
                    {
                        FieldInfo fi = sfi.Type.GetField(p, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        object v = parameters2value[p];
                        setValue(fi, o, v);
                    }
                }
            }
        }
        static Dictionary<string, SettingsFieldInfo> sections2SettingsFieldInfo = new Dictionary<string, SettingsFieldInfo>();

        static void setValue(FieldInfo fi, object o, object v)
        {
            if (v is string)
            {
                if (fi.FieldType == typeof(int))
                    v = int.Parse((string)v);
                else if (fi.FieldType == typeof(double))
                    v = double.Parse((string)v);
                else if (fi.FieldType == typeof(long))
                    v = long.Parse((string)v);
            }
            fi.SetValue(o, v);
        }

        static public void Save(string configfile = null)
        {
            if (configfile == null)
            {
                Cliver.Config.Save();
            }
            else
            {
                Dictionary<string, Dictionary<string, object>> sections2parameters2value = new Dictionary<string, Dictionary<string, object>>();
                foreach (string s in sections2SettingsFieldInfo.Keys)
                {
                    SettingsFieldInfo sfi = sections2SettingsFieldInfo[s];
                    Dictionary<string, object> parameters2value = new Dictionary<string, object>();
                    sections2parameters2value[s] = parameters2value;
                    Cliver.Settings o = sfi.GetObject();
                    foreach (FieldInfo fi in sfi.Type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                        parameters2value[fi.Name] = fi.GetValue(o);
                }

                Serialization.Json.Save(configfile, sections2parameters2value);
            }
        }

        static public void Set(string section, string parameter, object value)
        {
            SettingsFieldInfo sfi = sections2SettingsFieldInfo[section];
            FieldInfo fi = sfi.Type.GetField(parameter, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Cliver.Settings o = sfi.GetObject();
            setValue(fi, o, value);
        }

        static public object Get(string section, string parameter)
        {
            SettingsFieldInfo sfi = sections2SettingsFieldInfo[section];
            FieldInfo fi = sfi.Type.GetField(parameter, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Cliver.Settings o = sfi.GetObject();
            return fi.GetValue(o);
        }
    }
}