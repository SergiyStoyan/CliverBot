//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************

using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Text;
using System.Threading;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Dynamic;

namespace Cliver.Bot
{
    public partial class Session
    {
        internal abstract class StorageBase
        {
            TextWriter tw = null;
            TextReader tr = null;
            //FileStream fs = null;
            string file;

            internal StorageBase(string file)
            {
                PathRoutines.CreateDirectory(PathRoutines.GetDirFromPath(file));//done because cleanup can remove the dir
                this.file = file;
                //fs = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }

            ~StorageBase()
            {
                //Close();
            }

            internal void Close()
            {
                lock (this)
                {
                    //if (fs != null)
                    //{
                    //    fs.Close();
                    //    fs = null;
                    //    tr = null;
                    //    tw = null;
                    //}
                    if (tw != null)
                    {
                        tw.Close();
                        tw = null;
                    }
                    if (tr != null)
                    {
                        tr.Close();
                        tr = null;
                    }
                }
            }

            protected void writeElement(string tag, dynamic anonymous_object)
            {
                lock (this)
                {
                    if (tw == null)
                    {
                        if (tr != null)
                        {
                            tr.Close();
                            tr = null;
                        }
                        tw = new StreamWriter(file, true, Encoding.UTF8);
                    }
                    tw.WriteLine(tag + " " + serializer.Serialize(anonymous_object));
                    tw.Flush();
                }
            }

            protected bool startReading()
            {
                lock (this)
                {
                    if (!File.Exists(file))
                        return false;

                    if (tr == null)
                    {
                        if (tw != null)
                        {
                            tw.Close();
                            tw = null;
                        }
                    }
                    else
                        tr.Close();

                    tr = new StreamReader(file, Encoding.UTF8);
                    return true;
                }
            }

            protected bool readNextElement(out string tag, out Dictionary<string, object> names2value)
            {
                lock (this)
                {
                    //try
                    //{
                    string l = tr.ReadLine();
                    if (l == null)
                    {
                        tr.Close();
                        tr = null;
                        tag = null;
                        names2value = null;
                        return false;
                    }
                    int p = l.IndexOf(' ');
                    tag = l.Substring(0, p);
                    names2value = serializer.Deserialize<Dictionary<string, object>>(l.Substring(p + 1));
                    return true;
                    //}
                    //catch (Exception e)
                    //{
                    //    Log.Main.Warning(e);
                    //    Log.Main.Warning("Could not parse an element. Closing storage reader.");
                    //    tr.Close();
                    //    tr = null;
                    //}
                    //tag = null;
                    //names2value = null;
                    //return false;
                }
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            protected Dictionary<string, object> readNextElementByTag(string tag)
            {
                lock (this)
                {
                    //try
                    //{
                    while (true)
                    {
                        string l = tr.ReadLine();
                        if (l == null)
                        {
                            tr.Close();
                            tr = null;
                            return null;
                        }
                        int p = l.IndexOf(' ');
                        string t = l.Substring(0, p);
                        if (tag == t)
                            return serializer.Deserialize<Dictionary<string, object>>(l.Substring(p + 1));
                    }
                    //}
                    //catch (Exception e)
                    //{
                    //    Log.Main.Warning(e);
                    //    Log.Main.Warning("Could not parse an element. Closing storage reader.");
                    //    tr.Close();
                    //    tr = null;
                    //}
                    //return null;
                }
            }
        }

        internal class SessionStorage : StorageBase
        {
            internal SessionStorage() : base(This.Dir + "\\session_log.txt")
            {
            }

            internal SessionState ReadLastState(out DateTime start_time, out string session_time_mark)
            {
                start_time = DateTime.Now;
                session_time_mark = null;
                try
                {
                    if (!startReading())
                        return SessionState.NULL;
                    SessionState state = SessionState.NULL;
                    while (true)
                    {
                        Dictionary<string, object> names2value = readNextElementByTag(SessionTag);
                        if (names2value == null)
                            return state;

                        state = (SessionState)names2value["state"];
                        if (state == SessionState.STARTING)
                        {
                            start_time = (DateTime)names2value["session_start_time"];
                            session_time_mark = (string)names2value["session_time_mark"];
                        }
                    }
                }
                catch (Exception e)
                {
                    __FatalErrorClose(e);
                    return SessionState.NULL;
                }
            }

            internal void WriteState(SessionState state, dynamic names2value)
            {
                Dictionary<string, object> d = new Dictionary<string, object>();
                d.Add("state", state);
                d.Add("time", DateTime.Now);
                StringBuilder sb = new StringBuilder();
                foreach (PropertyInfo pi in names2value.GetType().GetProperties())
                {
                    d.Add(pi.Name, pi.GetValue(names2value));
                    sb.Append("\r\n" + pi.Name + "=" + pi.GetValue(names2value));
                }
                Log.Main.Inform("STATE: " + sb.ToString());
                writeElement(SessionTag, d);
            }
            const string SessionTag = "__SESSION";

            internal void WriteInputItem(InputItem item)
            {
                if (!Settings.Engine.WriteSessionRestoringLog)
                    return;

                if (!recorded_InputItem_ids.Contains(item.__Id))
                {
                    if (item.__State != InputItemState.NEW)
                        throw new Exception("InputItem has state not NEW but was not recorded.");

                    recorded_InputItem_ids.Add(item.__Id);

                    Dictionary<string, TagItem> tag_item_names2tag_item = item.GetTagItemNames2TagItem();
                    foreach (KeyValuePair<string, TagItem> n2ti in tag_item_names2tag_item)
                    {
                        if (recorded_TagItem_ids.Contains(n2ti.Value.__Id))
                            continue;

                        recorded_TagItem_ids.Add(n2ti.Value.__Id);
                        if (!restoring)
                            writeElement(n2ti.Value.GetType().Name, new { id = n2ti.Value.__Id, seed = n2ti.Value.GetSeed() });
                    }

                    Dictionary<string, object> d = new Dictionary<string, object>();
                    d["id"] = item.__Id;
                    d["state"] = item.__State;
                    d["seed"] = item.GetSeed();
                    if (item.__Queue.Name != item.GetType().Name)
                        d["queue"] = item.__Queue.Name;
                    if (item.__ParentItem != null)
                        d["parent_id"] = item.__ParentItem.__Id;
                    foreach (KeyValuePair<string, TagItem> kv in tag_item_names2tag_item)
                        d["id_of_" + kv.Key] = kv.Value.__Id;
                    if (!restoring)
                        writeElement(item.GetType().Name, d);
                }
                else if (!restoring)
                    writeElement(item.GetType().Name, new { id = item.__Id, state = item.__State });
            }
            HashSet<int> recorded_InputItem_ids = new HashSet<int>();
            HashSet<int> recorded_TagItem_ids = new HashSet<int>();

            internal void WriteWorkItem(WorkItem item, string key)
            {
                if (!Settings.Engine.WriteSessionRestoringLog)
                    return;

                if (!restoring)
                    writeElement(item.GetType().Name, new { id = item.__Id, seed = item.GetSeed(), key = key });
            }

            internal void WriteInputItemQueuePosition(InputItemQueue queue)
            {
                if (!Settings.Engine.WriteSessionRestoringLog)
                    return;

                if (!restoring)
                    writeElement(QueueTag, new { name = queue.Name, position = queue.Position });
            }
            const string QueueTag = "__QUEUE";

            internal void RestoreSession()
            {
                Log.Main.Inform("Restoring session: " + This.TimeMark);
                restoring = true;
                Dictionary<int, InputItem> input_item_id2input_items = new Dictionary<int, InputItem>();
                Dictionary<int, TagItem> tag_item_id2tag_items = new Dictionary<int, TagItem>();
                try
                {
                    startReading();
                    string tag;
                    Dictionary<string, object> names2value;
                    while (readNextElement(out tag, out names2value))
                    {
                        if (tag == QueueTag)
                        {
                            string name = (string)names2value["name"];
                            int position = (int)names2value["position"];
                            Session.SetInputItemQueuePosition(name, position);
                            continue;
                        }

                        if (tag.StartsWith("__"))
                            continue;

                        string type_name = tag;

                        int id = (int)names2value["id"];
                        if (id > This.item_count)
                            This.item_count = id;

                        if (input_item_type_names2input_item_type.ContainsKey(type_name))
                        {
                            InputItem parent_item = null;
                            object o;
                            if (names2value.TryGetValue("parent_id", out o))
                                parent_item = input_item_id2input_items[(int)o];

                            string queue;
                            if (names2value.TryGetValue("queue", out o))
                                queue = (string)o;
                            else
                                queue = type_name;

                            InputItemState state = (InputItemState)names2value["state"];
                            if (state == InputItemState.ERROR_RESTORE_AS_NEW && !Settings.Engine.RestoreErrorItemsAsNew)
                                state = InputItemState.ERROR;

                            switch (state)
                            {
                                case InputItemState.NEW:
                                    Type type = input_item_type_names2input_item_type[type_name];
                                    Dictionary<string, TagItem> fields2tag_item = new Dictionary<string, TagItem>();
                                    Dictionary<string, string> attr_names2tag_item_id = new Dictionary<string, string>();
                                    foreach (string name in names2value.Keys)
                                    {
                                        Match m = Regex.Match(name, "^id_of_(?'ItemTypeName'.+)");
                                        if (!m.Success)
                                            continue;
                                        fields2tag_item[m.Groups["ItemTypeName"].Value] = tag_item_id2tag_items[int.Parse(attr_names2tag_item_id[name])];
                                    }
                                    ArrayList seed = (ArrayList)names2value["seed"];
                                    InputItem item = InputItem.Restore(GetInputItemQueue(queue), type, seed, id, parent_item, fields2tag_item);
                                    input_item_id2input_items[id] = item;
                                    break;
                                case InputItemState.COMPLETED:
                                case InputItemState.ERROR:
                                    input_item_id2input_items[id].__State = state;
                                    break;
                                default:
                                    throw new Exception("Unknown item state: " + state);
                            }
                        }
                        else if (work_item_type_names2work_item_type.ContainsKey(type_name))
                        {
                            string key = (string)names2value["key"];
                            ArrayList seed = (ArrayList)names2value["seed"];
                            This.GetRestoredWorkItemDictionary(work_item_type_names2work_item_type[type_name]).Restore(key, seed, id);
                        }
                        else if (tag_item_type_names2tag_item_type.ContainsKey(type_name))
                        {
                            ArrayList seed = (ArrayList)names2value["seed"];
                            TagItem item = Cliver.Bot.TagItem.Restore(tag_item_type_names2tag_item_type[type_name], seed, id);
                            tag_item_id2tag_items[item.__Id] = item;
                        }
                        else
                            throw new Exception("Unknown item type in the sesson log: " + type_name);
                    }
                }
                catch (Exception e)
                {
                    __FatalErrorClose(e);
                }
                foreach (InputItemQueue iiq in This.input_item_queue_name2input_item_queues.Values)
                    iiq.OmitRestoredProcessedItems();
                restoring = false;
            }
            bool restoring = false;
        }
        internal SessionStorage Storage { get; private set; }
    }
}