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
    internal class RestoredWorkItemDictionary
    {
        internal RestoredWorkItemDictionary(Type item_type)
        {
            this.item_type = item_type;
            Name = item_type.ToString();

            switch (Session.This.SourceType)
            {
                case ItemSourceType.DB:
                    this.table = "_dictionary_" + Name;
                    throw new Exception("Not implemented");
                case ItemSourceType.FILE:
                    key2items = new Dictionary<string, WorkItem>();
                    break;
                default:
                    throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
            }
        }

        internal void Restore(string key, string item_seed, int item_id)
        {
            switch (Session.This.SourceType)
            {
                case ItemSourceType.DB:
                    throw new Exception("Not implemented");
                case ItemSourceType.FILE:
                    lock (this)
                    {
                        key2items[key] = (WorkItem)Item.Restore(item_type, item_seed, item_id);
                        return;
                    }
                default:
                    throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
            }
        }

        /// <summary>
        /// Dictionary name.
        /// </summary>        
        internal readonly string Name;

        protected readonly Type item_type;
        protected readonly string table;
        protected Dictionary<string, WorkItem> key2items;

        internal WorkItemDictionary<WorkItemT> Get<WorkItemT>() where WorkItemT : WorkItem
        {
            switch (Session.This.SourceType)
            {
                case ItemSourceType.DB:
                    throw new Exception("Not implemented");
                case ItemSourceType.FILE:
                    return new WorkItemDictionary<WorkItemT>(key2items);
                default:
                    throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
            }
        }

        internal SingleValueWorkItemDictionary<WorkItemT, ValueT> Get<WorkItemT, ValueT>() where WorkItemT : SingleValueWorkItem<ValueT>
        {
            switch (Session.This.SourceType)
            {
                case ItemSourceType.DB:
                    throw new Exception("Not implemented");
                case ItemSourceType.FILE:
                    return new SingleValueWorkItemDictionary<WorkItemT, ValueT>(key2items);
                default:
                    throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
            }
        }
    }

    /// <summary>
    /// Used to keep [string]=>[WorkItem derivative] dictionary which is logged and restored.
    /// </summary>
    /// <typeparam name="WorkItemT"></typeparam>
    public class WorkItemDictionary<WorkItemT> where WorkItemT : WorkItem
    {
        //for ItemSourceType.FILE
        internal WorkItemDictionary(Dictionary<string, WorkItem> key2items)
        {
            this.item_type = typeof(WorkItemT);
            Name = item_type.ToString();
            this.key2items = key2items;
        }

        /// <summary>
        /// Dictionary name.
        /// </summary>        
        internal readonly string Name;

        protected readonly Type item_type;
        protected readonly string table;
        protected Dictionary<string, WorkItem> key2items;
        
        public WorkItemT this[string key]
        {
            set
            {
                switch (Session.This.SourceType)
                {
                    case ItemSourceType.DB:
                        throw new Exception("Not implemented");
                    case ItemSourceType.FILE:
                        lock (this)
                        {
                            key2items[key] = value;
                            Session.This.LogWorkItem(value, key);
                            return;
                        }
                    default:
                        throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
                }
            }
            get
            {
                switch (Session.This.SourceType)
                {
                    case ItemSourceType.DB:
                        throw new Exception("Not implemented");
                    case ItemSourceType.FILE:
                        lock (this)
                        {
                            WorkItem wi;
                            if (!key2items.TryGetValue(key, out wi))
                                return null;
                            return (WorkItemT)wi;
                        }
                    default:
                        throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
                }
            }
        }

        public Dictionary<string, WorkItem>.KeyCollection Keys
        {
            get
            {
                return key2items.Keys;
            }
        }
    }

    /// <summary>
    /// Used to keep [string]=>[SingleValueWorkItem derivative] dictionary which is logged and restored.
    /// </summary>
    /// <typeparam name="WorkItemT"></typeparam>
    /// <typeparam name="ValueT"></typeparam>
    public class SingleValueWorkItemDictionary<WorkItemT, ValueT> : WorkItemDictionary<WorkItemT> where WorkItemT : SingleValueWorkItem<ValueT>
    {
        //for ItemSourceType.FILE
        internal SingleValueWorkItemDictionary(Dictionary<string, WorkItem> key2items)
            : base(key2items)
        {
        }
        
        new public ValueT this[string key]
        {
            set
            {
                switch (Session.This.SourceType)
                {
                    case ItemSourceType.DB:
                        throw new Exception("Not implemented");
                    case ItemSourceType.FILE:
                        lock (this)
                        {
                            base[key] = SingleValueWorkItem<ValueT>.Create<WorkItemT>(value);
                            return;
                        }
                    default:
                        throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
                }
            }
            get
            {
                switch (Session.This.SourceType)
                {
                    case ItemSourceType.DB:
                        throw new Exception("Not implemented");
                    case ItemSourceType.FILE:
                        lock (this)
                        {
                            WorkItem wi;
                            if (!key2items.TryGetValue(key, out wi))
                                return default(ValueT);
                            return ((SingleValueWorkItem<ValueT>)wi).__Value;
                        }
                    default:
                        throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
                }
            }
        }
    }
}

