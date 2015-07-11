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
using System.Configuration;
using System.Xml;
using System.Text;
using System.Threading;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Cliver.Bot
{
    public partial class Session
    {
        const string ITEMS_FILE_NAME = "session_items.xml";
        const string STATES_FILE_NAME = "session_workflow.xml";

        /// <summary>
        /// xml file where processed input Items are stored (to restore this session)
        /// </summary>
        XmlTextWriter items_xtw = null;

        /// <summary>
        /// xml file where current session status is stored (to restore this session)
        /// </summary>
        XmlTextWriter workflow_xtw = null;

        internal void LogInputItem(InputItem item)
        {
            if (items_xtw == null)
                return;

            lock (this)
            {
                try
                {
                    Dictionary<string, TagItem> tag_item_name2tag_items = item.GetTagItemName2TagItems();
                    foreach (KeyValuePair<string, TagItem> n2ti in tag_item_name2tag_items)
                    {
                        if (logged_input_and_tag_item_ids.Contains(n2ti.Value.__Id))
                            continue;
                        logged_input_and_tag_item_ids.Add(n2ti.Value.__Id);
                        items_xtw.WriteStartElement(n2ti.Value.GetType().Name);
                        items_xtw.WriteAttributeString("id", n2ti.Value.__Id.ToString());
                        items_xtw.WriteAttributeString("seed", n2ti.Value.GetSeed());
                        items_xtw.WriteEndElement();
                    }

                    items_xtw.WriteStartElement(item.GetType().Name);
                    items_xtw.WriteAttributeString("id", item.__Id.ToString());
                    items_xtw.WriteAttributeString("state", ((uint)item.__State).ToString());
                    if (!logged_input_and_tag_item_ids.Contains(item.__Id))
                    {
                        logged_input_and_tag_item_ids.Add(item.__Id);
                        if (item.__Queue.Name != item.GetType().Name)
                            items_xtw.WriteAttributeString("queue", item.__Queue.Name);
                        items_xtw.WriteAttributeString("seed", item.GetSeed());
                        if (item.__ParentItem != null)
                            items_xtw.WriteAttributeString("parent_id", item.__ParentItem.__Id.ToString());
                        foreach (KeyValuePair<string, TagItem> kv in tag_item_name2tag_items)
                            items_xtw.WriteAttributeString("id_of_" + kv.Key, kv.Value.__Id.ToString());
                    }
                    //else if (item.__State == InputItemState.NEW)
                    //    throw new Exception("Logged item has state NEW");
                    items_xtw.WriteEndElement();
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
                items_xtw.Flush();
            }
        }
        HashSet<int> logged_input_and_tag_item_ids = new HashSet<int>();

        internal void LogWorkItem(WorkItem item, string key)
        {
            if (items_xtw == null)
                return;

            lock (this)
            {
                try
                {
                    items_xtw.WriteStartElement(item.GetType().Name);
                    items_xtw.WriteAttributeString("id", item.__Id.ToString());
                    items_xtw.WriteAttributeString("seed", item.GetSeed());
                    items_xtw.WriteAttributeString("key", key);
                    items_xtw.WriteEndElement();
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
            }
            items_xtw.Flush();
        }

        //internal void LogInputItemQueueOrder()
        //{
        //    if (items_xtw == null)
        //        return;

        //    lock (this)
        //    {
        //        try
        //        {
        //            items_xtw.WriteStartElement("__QueueOrder");
        //            List<string> iiqns = new List<string>();
        //            foreach (string iiqn in input_item_queue_name2input_item_queues.Keys)
        //                iiqns.Add(iiqn);
        //            items_xtw.WriteString(string.Join("\n", iiqns));
        //            items_xtw.WriteEndElement();
        //        }
        //        catch (Exception e)
        //        {
        //            LogMessage.Exit(e);
        //        }
        //        items_xtw.Flush();
        //    }
        //}

        internal void LogInputItemQueuePosition(InputItemQueue queue)
        {
            if (items_xtw == null)
                return;

            lock (this)
            {
                try
                {
                    items_xtw.WriteStartElement("__Queue");
                    items_xtw.WriteAttributeString("name", queue.Name);
                    items_xtw.WriteAttributeString("position", queue.Position.ToString());
                    items_xtw.WriteEndElement();
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
                items_xtw.Flush();
            }
        }

        /// <summary>
        /// Find previous session and if it was broken restore it.
        /// </summary>
        /// <returns>false if session was not restored due to any reason</returns>
        bool restore(ref DateTime start_time)
        {
            lock (this)
            {
                try
                {
                    switch (Session.This.SourceType)
                    {
                        case ItemSourceType.DB:
                            return false;
                        case ItemSourceType.FILE:

                            DirectoryInfo work_di = new DirectoryInfo(Log.WorkDir);
                            DirectoryInfo[] session_dis = work_di.GetDirectories("Session*", SearchOption.TopDirectoryOnly);
                            Array.Sort(session_dis, new CompareDirectoryInfo());
                            for (int i = session_dis.Length - 2; i >= 0; i--)//pass own session
                            {
                                //if (dis[i].FullName == Log.SessionDir)//pass own session
                                //    continue;

                                FileInfo ssfi = new FileInfo(session_dis[i].FullName + "\\" + STATES_FILE_NAME);
                                if (!ssfi.Exists)
                                    continue;

                                XmlTextReader xtr = new XmlTextReader(ssfi.FullName);
                                SessionState last_state = SessionState.EMPTY;
                                start_time = DateTime.Now;
                                while (xtr.Move2Element("State", true) != null)
                                {
                                    string s = xtr.Move2Attribute("value", true);
                                    last_state = (SessionState)Enum.Parse(typeof(SessionState), s, true);
                                    if (last_state == SessionState.STARTED)
                                        start_time = DateTime.Parse(xtr.Move2Attribute("session_start_time", true));
                                }
                                switch (last_state)
                                {
                                    case SessionState.EMPTY:
                                    case SessionState.RESTORING:
                                        continue;
                                    case SessionState.COMPLETED:
                                        Log.Main.Write("Previous session was completed successfully.");
                                        return false;
                                }

                                //Log.Main.Write("Previous session was broken.");

                                string previous_broken_session_dir = session_dis[i].FullName;

                                if (!LogMessage.AskYesNo("Previous session " + previous_broken_session_dir + " was not completed. Restore it?", true))
                                    return false;

                                FileInfo broken_session_items_fi = new FileInfo(previous_broken_session_dir + "\\" + ITEMS_FILE_NAME);
                                if (!broken_session_items_fi.Exists)
                                {
                                    string str = "Could not find " + broken_session_items_fi.Name + " in " + previous_broken_session_dir + "!";
                                    if (LogMessage.AskYesNo(str + " Proceed without session restoring?", true))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        Close();
                                        return false;
                                    }
                                }

                                Log.Main.Write("Restoring session " + previous_broken_session_dir);

                                set_session_state(SessionState.RESTORING, "source_session", previous_broken_session_dir);
                                Log.Main.Write("Restoring session from " + previous_broken_session_dir);
                                restore_session_from_xml_file(broken_session_items_fi.FullName);

                                //string previous_broken_session_output_file = previous_broken_session_dir + "\\" + Log.OutputDirName + "\\" + Properties.Output.Default.OutputFile;

                                //if (File.Exists(previous_broken_session_output_file))
                                //{
                                //    File.Copy(previous_broken_session_output_file, Log.OutputDir + "\\" + Properties.Output.Default.OutputFile);
                                //    Log.Main.Write("Output file of previous broken session was copied to the current session and will be appended if has the name name: " + previous_broken_session_output_file);
                                //}
                                //else
                                //    Log.Main.Write("Output file of previous broken session does not exists: " + previous_broken_session_output_file);

                                DirectoryInfo output_di = new DirectoryInfo(previous_broken_session_dir + "\\" + Log.OutputDirName);
                                if (output_di.Exists)
                                {
                                    foreach (FileInfo fi in output_di.GetFiles())
                                    {
                                        string old_output_file2;
                                        if (!Properties.Output.Default.WriteOutputFile2CommonFolder)
                                            old_output_file2 = Log.OutputDir + "\\" + fi.Name;
                                        else
                                            old_output_file2 = Log.WorkDir + "\\" + fi.Name;
                                        if (!File.Exists(old_output_file2))
                                        {
                                            File.Copy(fi.FullName, old_output_file2);
                                            Log.Main.Write("Output file of previous broken session was copied to the current output folder: " + old_output_file2);
                                        }
                                        else
                                            Log.Main.Warning("Output file of previous broken session was not copied because the destination one exists already: " + old_output_file2);
                                    }
                                }
                                return true;
                            }
                            return false;
                        default:
                            throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
                    }
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
                return false;
            }
        }
        internal class CompareDirectoryInfo : IComparer<DirectoryInfo>
        {
            public int Compare(DirectoryInfo d1, DirectoryInfo d2)
            {
                return d1.Name.CompareTo(d2.Name);
            }
        }

        /// <summary>
        /// Write session state to file.
        /// </summary>
        /// <param name="state">current session state</param>
        void set_session_state(SessionState state, params string[] attribute_value_pairs)
        {
            lock (this)
            {
                List<string> avps = new List<string>();
                avps.Add("value");
                avps.Add(state.ToString().ToUpper());
                avps.Add("time");
                avps.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                avps.AddRange(attribute_value_pairs);
                XmlRoutines.WriteElement(workflow_xtw, "State", avps.ToArray());
            }
        }
        internal enum SessionState
        {
            EMPTY,
            RESTORING,//restoring phase
            STARTED,//session started from scratch
            COMPLETED,
            UNCOMPLETED,//error items to be restored exist
            ABORTED
        }

        /// <summary>
        /// Restore session from the specified session file.
        /// </summary>
        /// <param name="items_xml_file">previous session file where data is to be restored from</param>
        void restore_session_from_xml_file(string items_xml_file)
        {
            Dictionary<int, InputItem> input_item_id2input_items = new Dictionary<int, InputItem>();
            Dictionary<int, TagItem> tag_item_id2tag_items = new Dictionary<int, TagItem>();
            Regex tag_item_field_filter = new Regex("^id_of_(?'ItemTypeName'.+)");
            try
            {
                XmlTextReader xtr = new XmlTextReader(items_xml_file);
                while (xtr.Read())
                {
                    if (xtr.Name == "Items")
                        break;
                }
                while (xtr.Read())
                {
                    if (xtr.NodeType != XmlNodeType.Element)
                        continue;

                    //if (xtr.Name == "__QueueOrder")
                    //{
                    //    string[] iiqns = xtr.ReadInnerXml().Split('\n');
                    //    Session.SetInputItemQueuesOrder(iiqns);
                    //    continue;
                    //}

                    if (xtr.Name == "__Queue")
                    {
                        string name = xtr.GetAttribute("name");
                        int position = int.Parse(xtr.GetAttribute("position"));
                        Session.SetInputItemQueuePosition(name, position);
                        continue;
                    }

                    string type_name = xtr.Name;
                    int id = int.Parse(xtr.GetAttribute("id"));
                    string seed = xtr.GetAttribute("seed");

                    if (id > item_count)
                        item_count = id;

                    if (input_item_type_name2input_item_types.ContainsKey(type_name))
                    {
                        InputItem parent_item = null;
                        string parent_id = xtr.GetAttribute("parent_id");
                        if(parent_id !=null)
                           parent_item = input_item_id2input_items[int.Parse(parent_id)];                        
                    string queue = xtr.GetAttribute("queue");
                        if(queue == null)
                            queue = type_name;
                        InputItemState state = (InputItemState)Enum.Parse(typeof(InputItemState), xtr.GetAttribute("state"), true);
                        if (state == InputItemState.ERROR_RESTORE_AS_NEW && !Properties.General.Default.RestoreErrorItemsAsNew)
                            state = InputItemState.ERROR;

                        switch (state)
                        {
                            case InputItemState.NEW:
                                Type type = input_item_type_name2input_item_types[type_name];
                                Dictionary<string, string> attr_name2tag_item_id_s = xtr.GetAttributeName2AttributeValues(tag_item_field_filter);
                                Dictionary<string, TagItem> field2tag_items = new Dictionary<string, TagItem>();
                                foreach (string name in attr_name2tag_item_id_s.Keys)
                                    field2tag_items[tag_item_field_filter.Match(name).Groups["ItemTypeName"].Value] = tag_item_id2tag_items[int.Parse(attr_name2tag_item_id_s[name])];
                                InputItem item = InputItem.Restore(GetInputItemQueue(queue), type, seed, id, parent_item, field2tag_items);
                                input_item_id2input_items[id] = item;
                                break;
                            case InputItemState.COMPLETED:
                            case InputItemState.ERROR:
                            //case InputItemState.ERROR_RESTORE_AS_NEW:
                                input_item_id2input_items[id].__State = state;
                                break;
                        }
                    }
                    else if (work_item_type_name2work_item_types.ContainsKey(type_name))
                    {
                        string key = xtr.GetAttribute("key");
                        GetRestoredWorkItemDictionary(work_item_type_name2work_item_types[type_name]).Restore(key, seed, id);
                    }
                    else if (tag_item_type_name2tag_item_types.ContainsKey(type_name))
                    {
                        TagItem item = Cliver.Bot.TagItem.Restore(tag_item_type_name2tag_item_types[type_name], seed, id);
                        tag_item_id2tag_items[item.__Id] = item;
                    }
                }
            }
            catch (XmlException e)
            {
                Log.Main.Warning("Session restoring: " + Log.GetExceptionMessage(e));
            }
            catch (Exception e)
            {
                LogMessage.Error("Session restoring: " + Log.GetExceptionMessage(e));
            }
            foreach (InputItemQueue iiq in input_item_queue_name2input_item_queues.Values)
                iiq.OmitRestoredProcessedItems();
            Log.Main.Write("Items were restored from " + items_xml_file.ToString());
        }
    }
}