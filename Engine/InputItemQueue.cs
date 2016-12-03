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
            item_id2items = new OrderedDictionary();
            PickNext = default_pick_next;
        }

        internal static void Close()
        {
            lock (item_keys)
            {
                item_keys.Clear();
            }
        }

        /// <summary>
        /// Queue name.
        /// </summary>        
        public readonly string Name;

        /// <summary>
        /// Position in the queue order.
        /// </summary>
        public int Position { get; internal set; }
        
        OrderedDictionary item_id2items;
        static HashSet<string> item_keys = new HashSet<string>();

        public delegate void OnProgress(InputItemQueue input_item_queue, int total_item_count, int processed_item_count);
        static public event InputItemQueue.OnProgress Progress = null;

        internal bool Enqueue(InputItem item)
        {
            lock (this)
            {
                string item_key = item.GetKey();
                lock (item_keys)
                {
                    if (item_keys.Contains(item_key))
                        return false;
                    item_keys.Add(item_key);
                }
                item.__State = InputItemState.NEW;
                item_id2items.Add(item.__Id, item);
                if (Progress != null)
                    Progress.Invoke(this, CountOfProcessed + CountOfNew, CountOfProcessed);
                return true;
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

        //thread safe method that can be called from a custom code to enumerate through items
        public object Enumerate(OnEnumerate on_enum)
        {
            lock (this)
            {
                return on_enum.Invoke(item_id2items.Values.GetEnumerator());
            }
        }
        public delegate object OnEnumerate(System.Collections.IEnumerator items_ennumerator);

        public OnPickNext PickNext
        {
            internal get
            {
                return _PickNext;
            }
            set
            {
                if (Session.State > Session.SessionState.STARTING)
                    throw new Session.FatalException("PickNext should be set before bot cycle started.");
                _PickNext = value;
            }
        }
        public delegate InputItem OnPickNext(System.Collections.IEnumerator items_ennumerator);
        OnPickNext _PickNext = null;

        InputItem default_pick_next(System.Collections.IEnumerator items_ennumerator)
        {
            lock (this)
            {
                items_ennumerator.Reset();
                if (items_ennumerator.MoveNext())
                    return (InputItem)items_ennumerator.Current;
                return null;
            }
        }

        internal InputItem GetNext()
        {
            lock (this)
            {
                //if (current_input_item != null && current_input_item.__State == InputItemState.NEW)
                //    throw new Exception("The previously picked up InputItem was not marked as processed");
                InputItem current_input_item = PickNext(item_id2items.Values.GetEnumerator());
                if (current_input_item == null)
                    return null;
                item_id2items.Remove(current_input_item.__Id);
                count_of_processed_items++;
                if (Progress != null)
                    Progress.Invoke(this, CountOfProcessed + CountOfNew, CountOfProcessed);
                return current_input_item;
            }
        }

        public int CountOfNew
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
        public bool Add<ItemT>(dynamic anonymous_object) where ItemT : InputItem
        {
            return InputItem.Add2Queue<ItemT>(this, BotCycle.GetCurrentInputItemForThisThread(), anonymous_object);
        }
    }
}