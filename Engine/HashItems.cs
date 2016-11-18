//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Cliver
{
    /// <summary>   
    /// Hashtable used to remeber any values by keys during bot session
    /// and (optionally) store them to a file to not repeat them in the future.
    /// </summary>
    public class HashItems
    {
        Dictionary<string, string> hash_items = new Dictionary<string, string>();

        static Dictionary<string, HashItems> HashItemsPool = new Dictionary<string, HashItems>();

        /// <summary>
        /// Create HashItems if these was not created still, else return it from the hash.
        /// </summary>
        /// <param name="file_path">file where items are listed</param>
        /// <returns></returns>
        public static HashItems GetHashItems(string file_path)
        {
            lock (HashItemsPool)
            {
                try
                {
                    HashItems his;
                    if (!HashItemsPool.TryGetValue(file_path, out his))
                    {
                        his = new HashItems(file_path);
                        HashItemsPool[file_path] = his;
                    }

                    return his;
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
                return null;
            }
        }

        /// <summary>
        /// Used to create a list of processed hash items
        /// </summary>
        /// <param name="file_path">file where hash items are listed</param>
        internal HashItems(string file_path)
        {
            lock (this)
            {
                try
                {
                    file_abs_path = file_path;
                    if (!file_abs_path.Contains(":"))
                        file_abs_path = Log.WorkDir + "\\" + file_abs_path;

                    string data_string = "";
                    if (!File.Exists(file_abs_path))
                    {
                        if (!LogMessage.AskYesNo(file_abs_path + " does not exists.\nDo you want it to be created?\nIf you run the app in first time, press OK.", false))
                            Log.Exit(file_abs_path + " does not exists");
                    }
                    else
                        data_string = File.ReadAllText(file_abs_path);

                    fw = new FileWriter(file_abs_path, true, false, -1);

                    if (string.IsNullOrEmpty(data_string))
                        return;
                    data_string = data_string.Trim();

                    Match m = Regex.Match(data_string, @"^(.*?)" + Config.Output.OutputFieldSeparator + "(.*?)$", RegexOptions.Compiled | RegexOptions.Multiline);
                    while (m.Success)
                    {
                        hash_items[m.Groups[1].Value.Trim()] = m.Groups[2].Value.Trim();
                        m = m.NextMatch();
                    }
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
            }
        }

        FileWriter fw = null;
        string file_abs_path = null;

        /// <summary>
        /// HashItems without writting to file.
        /// </summary>
        public HashItems()
        {
        }

        public string this[string key]
        {
            set
            {
                lock (this)
                {
                    if (key == null)
                        return;

                    key = key.Trim();

                    string item;
                    if (hash_items.TryGetValue(key, out item))
                    {
                        if (item == value)
                            return;
                    }
                    else if (item == COMPLETED)
                        throw (new Exception(key + " is COMPLETED so cannot change its value"));

                    hash_items[key] = value;

                    if (fw != null)
                        fw.WriteLine(key, value);
                }
            }
            get
            {
                lock (this)
                {
                    if (key == null)
                        return null;

                    string item;
                    if (!hash_items.TryGetValue(key.Trim(), out item))
                        return null;
                    return item;
                }
            }
        }

        /// <summary>
        /// Alias for this[hash_item] = COMPLETED;
        /// </summary>
        public void Complete(string key)
        {
            lock (this)
            {
                this[key] = COMPLETED;
            }
        }

        public readonly string COMPLETED = "_COMPLETED";

        public bool IsCompleted(string key)
        {
            lock (this)
            {
                string item;
                if (!hash_items.TryGetValue(key.Trim(), out item))
                    return false;
                return item == COMPLETED;
            }
        }

        public Dictionary<string, string>.Enumerator GetEnumerator()
        {
            lock (this)
            {
                return hash_items.GetEnumerator();
            }
        }

        public Dictionary<string, string>.KeyCollection Keys
        {
            get
            {
                lock (this)
                {
                    return hash_items.Keys;
                }
            }
        }

        static internal void ClearSession()
        {
            lock (HashItemsPool)
            {
                foreach (string file_path in HashItemsPool.Keys)
                {
                    HashItems his = (HashItems)HashItemsPool[file_path];
                    his.fw.Close();

                    //rewrite the hash item file to remove old values
                    string file_abs_path = file_path;
                    if (!file_abs_path.Contains(":"))
                        file_abs_path = Log.WorkDir + "\\" + file_abs_path;
                    string file_abs_path_old = file_abs_path + ".back";
                    File.Delete(file_abs_path_old);
                    File.Move(file_abs_path, file_abs_path_old);
                    FileWriter fw = new FileWriter(file_abs_path, false, false, -1);
                    foreach (string key in his.Keys)
                        fw.WriteLine(key, (string)his[key]);
                }
                HashItemsPool.Clear();
            }
        }
    }
}
