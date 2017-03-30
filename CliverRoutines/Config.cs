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
        /// <summary>
        /// this object is ever to be loaded
        /// </summary>
        public class Obligatory : Attribute
        { } 
    }

    /// <summary>
    /// Manages Serializable settings.
    /// </summary>
    public class Config
    {
        static Config()
        {
            DefaultStorageDir = Log.GetAppCommonDataDir() + "\\" + CONFIG_FOLDER_NAME;
        }

        /// <summary>
        /// It allows to load only certain settings objects, while ignoring unneeded ones.
        /// However, objects attributed with [Settings.Obligatory] will be loaded in any way.
        /// </summary>
        /// <param name="required_object_names"></param>
        public static void Initialize(IEnumerable<string> required_object_names = null)
        {
            Config.required_object_names.Clear();
            if (required_object_names == null)
                return;
            foreach(string name in required_object_names)
                Config.required_object_names.Add(name);
        }
        static readonly HashSet<string> required_object_names = new HashSet<string>();

        public const string CONFIG_FOLDER_NAME = "config";
        public const string FILE_EXTENSION = "json";

        public static readonly string DefaultStorageDir;
        public static string StorageDir { get; private set; }

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
                            fis.AddRange(et.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(a => a.FieldType == st));
                        if (fis.Count < 1)
                            throw new Exception("No field of type '" + st.FullName + "' was found.");
                        if (fis.Count > 1)
                            throw new Exception("More then 1 field of type '" + st.FullName + "' was found.");
                        FieldInfo fi = fis[0];
                        string name = fi.Name;

                        if (null == fi.GetCustomAttributes(typeof(Settings.Obligatory), false).FirstOrDefault() && !required_object_names.Contains(name))
                            continue;

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
                List<string> not_found_names = new List<string>();
                foreach (string ron in required_object_names)
                    if (!object_names2serializable.ContainsKey(ron))
                        not_found_names.Add(ron);
                if (not_found_names.Count > 0)
                    throw new Exception("The following settings objects where not found: " + string.Join(", ", not_found_names));
            }
        }
        static Dictionary<string, Serializable> object_names2serializable = new Dictionary<string, Serializable>();

        static public void Reload(string storage_dir = null, bool read_only = false)
        {
            StorageDir = storage_dir != null ? storage_dir : DefaultStorageDir;
            ReadOnly = read_only;
            get(false);
        }

        static public bool ReadOnly { get; private set; }

        static public void Reset()
        {
            StorageDir = DefaultStorageDir;
            get(true);
        }

        static public void Save(string storage_dir = null)
        {
            storage_dir = storage_dir != null ? storage_dir : DefaultStorageDir;
            if (ReadOnly && PathRoutines.ArePathsEqual(storage_dir, StorageDir))
                throw new Exception("Config is read-only and cannot be saved to the same location.");
            StorageDir = storage_dir;
            lock (object_names2serializable)
            {
                foreach (Serializable s in object_names2serializable.Values)
                    s.Save(StorageDir + "\\" + PathRoutines.GetFileNameFromPath(s.__File));
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
                string d = PathRoutines.CreateDirectory(to_directory + "\\" + CONFIG_FOLDER_NAME);
                foreach (Serializable s in object_names2serializable.Values)
                    if (File.Exists(s.__File))//it can be absent if default settings used still
                        File.Copy(s.__File, d + "\\" + PathRoutines.GetFileNameFromPath(s.__File));
            }
        }
    }
}