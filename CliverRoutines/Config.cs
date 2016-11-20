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
    /// <summary>
    /// Alternative to .NET settings. Inheritors of this class are automatically managed by Config.
    /// </summary>
    public class Settings : Serializable
    {
    }

    /// <summary>
    /// Manages Serializable settings.
    /// </summary>
    public class Config
    {
        static Config()
        {
            Reload();
        }

        const string FILE_EXTENSION = "json";

        public readonly static string DefaultStorageDir = Log.GetAppCommonDataDir();
        public static string StorageDir { get { return _StorageDir; } private set { _StorageDir = value; } }
        static string _StorageDir = DefaultStorageDir;

        public static void Initialize()
        {
            //dummy to trigger static constructor
        }

        static void get(bool reset)
        {
            lock (object_names2serializable)
            {
                object_names2serializable.Clear();
                List<Assembly> sas = new List<Assembly>();
                sas.Add(Assembly.GetEntryAssembly());
                foreach (AssemblyName an in Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(an => Regex.IsMatch(an.Name, @"^Cliver")))
                    sas.Add(Assembly.Load(an));
                foreach (Assembly sa in sas)
                {
                    Type[] ets = sa.GetTypes();
                    foreach (Type st in ets.Where(t => t.IsSubclassOf(typeof(Settings))))
                    {
                        List<FieldInfo> fis = new List<FieldInfo>();
                        foreach (Type et in ets)
                            fis.AddRange(et.GetFields(BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public).Where(a => a.FieldType == st));
                        if (fis.Count < 1)
                            throw new Exception("No field of type '" + st.FullName + "' was found.");
                        if (fis.Count > 1)
                            throw new Exception("More then 1 field of type '" + st.FullName + "' was found.");
                        FieldInfo fi = fis[0];
                        string name = fi.Name;

                        Serializable t;
                        string file = StorageDir + "\\" + name + "." + st.FullName + "." + FILE_EXTENSION;
                        if (reset)
                            t = Serializable.Create(st, file);
                        else
                            try
                            {
                                t = Serializable.Load(st, file);
                            }
                            catch (Exception e)
                            {
                                LogMessage.Error2(e);
                                t = Serializable.Create(st, file);
                            }

                        fi.SetValue(null, t);

                        if (object_names2serializable.ContainsKey(name))
                            throw new Exception("More then 1 field named '" + name + "' was found.");
                        object_names2serializable[name] = t;
                    }
                }
            }
        }
        static Dictionary<string, Serializable> object_names2serializable = new Dictionary<string, Serializable>();

        static public void Reload(string storage_directory = null)
        {
            if(storage_directory != null)
                StorageDir = storage_directory;
            get(false);
        }

        static public void Reset()
        {
            get(true);
        }

        static public void Save()
        {
            lock (object_names2serializable)
            {
                foreach (Serializable s in object_names2serializable.Values)
                    s.Save();
            }
        }

        static public Serializable GetInstance(string object_name)
        {
            lock (object_names2serializable)
            {
                Serializable s = null;
                object_names2serializable.TryGetValue(object_name, out s);
                return s;
            }
        }

        static public void CopyFiles(string to_directory)
        {
            lock (object_names2serializable)
            {
                foreach (Serializable s in object_names2serializable.Values)
                    File.Copy(s.GetFile(), to_directory + "\\" + PathRoutines.GetFileNameFromPath(s.GetFile()));
            }
        }
    }
}