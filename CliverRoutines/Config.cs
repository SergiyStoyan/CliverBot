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

namespace Cliver
{
    public class Settings : Serializable
    {
    }

    public class Config
    {
        static Config()
        {
            Reload();
        }

        public static void Initialize()
        {
            //dummy to trigger static constructor
        }
        
        static void get(bool reset)
        {
            type_names2object = new Dictionary<string, Serializable>();
            List<Assembly> sas = new List<Assembly>();
            sas.Add(Assembly.GetEntryAssembly());
            foreach (AssemblyName an in Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(an => Regex.IsMatch(an.Name, @"^Cliver")))
                sas.Add(Assembly.Load(an));
            foreach (Assembly sa in sas)
                foreach (Type st in sa.GetExportedTypes().Where(t => t.IsSubclassOf(typeof(Settings))))
                {
                    Serializable t;
                    string f = Log.GetAppCommonDataDir() + "\\" + st.FullName + ".setting";
                    if (reset)
                        t = Serializable.Create(st, f);
                    else
                        t = Serializable.Load(st, f);
                    FieldInfo fi = st.GetField("This", BindingFlags.Public | BindingFlags.Static);
                    if (fi == null)
                        throw new Exception("Class " + st.FullName + " does not have 'public static readonly <Type> This' property.");
                    fi.SetValue(null, t);

                    type_names2object[st.FullName] = t;
                }
        }
        static Dictionary<string, Serializable> type_names2object = null;

        static public void Reload()
        {
            get(false);
        }

        static public void Reset()
        {
            get(true);
        }

        static public void Save()
        {
            foreach (Serializable s in type_names2object.Values)
                s.Save();
        }
    }
}