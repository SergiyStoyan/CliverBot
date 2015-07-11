//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Linq;
using System.Reflection;

namespace Cliver.Bot
{
    /// <summary>
    /// By default queue's name is type name of items that are contained by it. But generally a queue can have its own name and even contain items of different types.
    /// </summary>
    public class InputItemQueue
    {
        internal InputItemQueue(string name)
        {
            Name = name;
            switch (Session.This.SourceType)
            {
                case ItemSourceType.DB:
                    this.table = "_queue_" + Name;
                    break;
                case ItemSourceType.FILE:
                    item_id2items = new OrderedDictionary();
                    break;
                default:
                    throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
            }
        }

        internal static void Close()
        {
            item_keys.Clear();
        }

        /// <summary>
        /// Queue name.
        /// </summary>        
        public readonly string Name;

        /// <summary>
        /// Position in the queue order.
        /// </summary>
        public int Position
        {
            get
            {
                return position;
            }
            internal set
            {
                position = value;
            }
        }
        int position = -1;

        readonly string table;
        OrderedDictionary item_id2items;
        static HashSet<string> item_keys = new HashSet<string>();

        public delegate void OnProgress(InputItemQueue input_item_queue, int total_item_count, int processed_item_count);
        static public event InputItemQueue.OnProgress Progress = null;

        internal bool Enqueue(InputItem item)
        {
            switch (Session.This.SourceType)
            {
                case ItemSourceType.DB:
                    return true;
                case ItemSourceType.FILE:
                    lock (this)
                    {
                        string item_key = item.GetKey();
                        lock (item_keys)
                        {
                            if (item_keys.Contains(item_key))
                                return false;
                            item_keys.Add(item_key);
                        }
                        Session.This.LogInputItem(item);
                        item_id2items.Add(item.__Id, item);
                        if (Progress != null)
                            Progress.Invoke(this, CountOfProcessed + CountOfNew, CountOfProcessed);
                        return true;
                    }
                default:
                    throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
            }
        }

        internal void OmitRestoredProcessedItems()
        {
            lock (this)
            {
                for (int i = item_id2items.Count - 1; i >= 0; i--)
                {
                    InputItem ii = (InputItem)item_id2items[i];
                    if (ii.__State != InputItemState.NEW)
                    {
                        item_id2items.RemoveAt(i);
                        count_of_processed_items++;
                    }
                }
                if (Progress != null)
                    Progress.Invoke(this, CountOfProcessed + CountOfNew, CountOfProcessed);
            }
        }

        internal InputItem GetNext()
        {
            switch (Session.This.SourceType)
            {
                case ItemSourceType.DB:
                    return null;
                case ItemSourceType.FILE:
                    lock (this)
                    {
                        //if (current_input_item != null && current_input_item.__State == InputItemState.NEW)
                        //    throw new Exception("The previously picked up InputItem was not marked as processed");
                        if (item_id2items.Count < 1)
                            return null;
                        //do
                        //{
                        current_input_item = (InputItem)item_id2items[0];
                        item_id2items.RemoveAt(0);
                        count_of_processed_items++;
                        //}
                        //while (current_input_item.__State != InputItemState.NEW);
                        if (Progress != null)
                            Progress.Invoke(this, CountOfProcessed + CountOfNew, CountOfProcessed);
                        return current_input_item;
                    }
                default:
                    throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
            }
        }
        InputItem current_input_item = null;

        internal int CountOfNew
        {
            get
            {
                lock (item_id2items)
                {
                    return item_id2items.Count;
                }
            }
        }

        internal int CountOfProcessed
        {
            get
            {
                return count_of_processed_items;
            }
        }
        int count_of_processed_items = 0;

        /// <summary>
        /// Add item as dynamic object to queue. It is possible to create a named queue.
        /// It is the same like BotCycle.Add() but not so efficient and safe.
        /// </summary>
        /// <param name="queue_name"></param>
        /// <param name="item"></param>
        public bool Add(InputItem item)
        {
            return InputItem.Add2Queue(this, BotCycle.GetCurrentInputItemForThisThread(), item);
        }

        /// <summary>
        /// Add item as dynamic object to queue. By default name of queue is name of item type.
        /// It is the same as BotCycle.Add() but not so efficient and safe.
        /// </summary>
        /// <typeparam name="ItemT"></typeparam>
        /// <param name="anonymous_object"></param>
        /// <returns></returns>
        public bool Add<ItemT>(object anonymous_object) where ItemT : InputItem
        {
            return InputItem.Add2Queue<ItemT>(this, BotCycle.GetCurrentInputItemForThisThread(), anonymous_object);
        }
    }
}

