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

namespace Cliver.Bot
{
    public partial class Session
    {
        public readonly ItemSourceType SourceType = ItemSourceType.FILE;

        internal InputItem GetNext()
        {
            lock (input_item_queue_name2input_item_queues)
            {
                foreach (InputItemQueue iiq in input_item_queue_name2input_item_queues.Values)
                {
                    InputItem ii = iiq.GetNext();
                    if (ii != null)
                        return ii;
                }
                return null;
            }
        }

        OrderedDictionary input_item_queue_name2input_item_queues = new OrderedDictionary();

        internal int GetNewItemId()
        {
            lock (this)
            {
                return ++item_count;
            }
        }
        int item_count = 0;

        static public InputItemQueue GetInputItemQueue(string queue_name)
        {
            lock (This.input_item_queue_name2input_item_queues)
            {
                InputItemQueue iiq = (InputItemQueue)This.input_item_queue_name2input_item_queues[queue_name];
                if (iiq == null)
                {
                    SetInputItemQueuePosition(queue_name, 0);
                    iiq = (InputItemQueue)This.input_item_queue_name2input_item_queues[queue_name];
                }
                return iiq;
            }
        }

        /// <summary>
        /// This is the ONLY method to add a queue!
        /// When a queue is constructed dynamicly, it is possible to set its order.
        /// </summary>
        /// <param name="queue_name"></param>
        /// <param name="position"></param>
        static public void SetInputItemQueuePosition(string queue_name, int position)
        {
            lock (This.input_item_queue_name2input_item_queues)
            {
                InputItemQueue iiq = (InputItemQueue)This.input_item_queue_name2input_item_queues[queue_name];
                if (iiq == null)
                    iiq = new InputItemQueue(queue_name);
                else
                {
                    iiq = (InputItemQueue)This.input_item_queue_name2input_item_queues[queue_name];
                    int p = iiq.Position;
                    if (p == position)
                        return;
                    This.input_item_queue_name2input_item_queues.RemoveAt(p);
                    position--;
                }
                This.input_item_queue_name2input_item_queues.Insert(position, queue_name, iiq);
         
                for (int i = 0; i < This.input_item_queue_name2input_item_queues.Count; i++)
                    ((InputItemQueue)This.input_item_queue_name2input_item_queues[i]).Position = i;
                This.LogInputItemQueuePosition(iiq);
            }
        }

        public bool IsUnprocessedInputItem
        {
            get
            {
                lock (input_item_queue_name2input_item_queues)
                {
                    foreach (InputItemQueue iiq in input_item_queue_name2input_item_queues.Values)
                    {
                        if (iiq.CountOfNew > 0)
                            return true;
                    }
                    return false;
                }
            }
        }

        public bool IsItemToRestore
        {
            get
            {
                return IsItem2Restore;
            }
        }
        internal bool IsItem2Restore = false;

        internal RestoredWorkItemDictionary GetRestoredWorkItemDictionary(Type item_type)
        {
            lock (work_item_type2work_item_dictionary)
            {
                RestoredWorkItemDictionary wid;
                if (!work_item_type2work_item_dictionary.TryGetValue(item_type, out wid))
                {
                    if (!work_item_type_name2work_item_types.ContainsKey(item_type.Name))
                        throw new Exception("Type " + item_type + " is not derivative of WorkItem");
                    wid = new RestoredWorkItemDictionary(item_type);
                    work_item_type2work_item_dictionary[item_type] = wid;
                }
                return wid;
            }
        }
        Dictionary<Type, RestoredWorkItemDictionary> work_item_type2work_item_dictionary = new Dictionary<Type, RestoredWorkItemDictionary>();

        internal WorkItemDictionary<WorkItemT> GetWorkItemDictionary_<WorkItemT>() where WorkItemT : WorkItem
        {
            RestoredWorkItemDictionary wid = GetRestoredWorkItemDictionary(typeof(WorkItemT));
            return wid.Get<WorkItemT>();
        }

        internal SingleValueWorkItemDictionary<WorkItemT, ValueT> GetSingleValueWorkItemDictionary_<WorkItemT, ValueT>() where WorkItemT : SingleValueWorkItem<ValueT>
        {
            RestoredWorkItemDictionary wid = GetRestoredWorkItemDictionary(typeof(WorkItemT));
            return wid.Get<WorkItemT, ValueT>();
        }
    }
}
