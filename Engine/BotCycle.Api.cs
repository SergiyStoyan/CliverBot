//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************

using System;
using System.Linq;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Text;
using System.Threading;
using System.Reflection;

namespace Cliver.Bot
{
    public partial class BotCycle
    {
        /// <summary>
        /// Add item to queue. It is possible to create a named queue.
        /// Preferred method for adding items.
        /// </summary>
        /// <param name="queue_name"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(string queue_name, InputItem item)
        {
            if (queue_name == null)
                queue_name = item.GetType().Name;
            InputItemQueue iiq = Session.GetInputItemQueue(queue_name);
            return InputItem.Add2Queue(iiq, current_item, item);
        }

        /// <summary>
        /// Add item to queue. By default name of queue is name of item type.
        /// Preferred method for adding items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(InputItem item)
        {
            return Add(null, item);
        }

        /// <summary>
        /// Add item as dynamic object to queue. It is possible to create a named queue.
        /// Preferred method for adding items.
        /// </summary>
        /// <typeparam name="ItemT"></typeparam>
        /// <param name="queue_name"></param>
        /// <param name="anonymous_object"></param>
        /// <returns></returns>
        public bool Add<ItemT>(string queue_name, dynamic anonymous_object) where ItemT : InputItem
        {
            InputItemQueue iiq = Session.GetInputItemQueue(queue_name);
            return InputItem.Add2Queue<ItemT>(iiq, current_item, anonymous_object);
        }

        /// <summary>
        /// Add item as dynamic object to queue. By default name of queue is name of item type.
        /// Preferred method for adding items.
        /// </summary>
        /// <typeparam name="ItemT"></typeparam>
        /// <param name="anonymous_object"></param>
        /// <returns></returns>
        public bool Add<ItemT>(dynamic anonymous_object) where ItemT : InputItem
        {
            return Add<ItemT>(typeof(ItemT).Name, anonymous_object);
        }

        public ItemT WorkItem<ItemT>(dynamic anonymous_object) where ItemT : WorkItem
        {
            return Cliver.Bot.WorkItem.Create<ItemT>(anonymous_object);
        }

        public ItemT WorkItem<ItemT, ValueT>(ValueT value) where ItemT : SingleValueWorkItem<ValueT>
        {
            return Cliver.Bot.SingleValueWorkItem<ValueT>.Create<ItemT>(value);
        }

        public ItemT TagItem<ItemT>(dynamic anonymous_object) where ItemT : TagItem
        {
            return Cliver.Bot.TagItem.Create<ItemT>(anonymous_object);
        }

        //public ItemT TagItem<ItemT, ValueT>(ValueT value) where ItemT : SingleValueTagItem<ValueT>
        //{
        //    return Cliver.SingleValueTagItem<ValueT>.Create<ItemT>(value);
        //}

        public WorkItemDictionary<WorkItemT> GetWorkItemDictionary<WorkItemT>() where WorkItemT : WorkItem
        {
            return Session.GetWorkItemDictionary<WorkItemT>();
        }

        public SingleValueWorkItemDictionary<WorkItemT, ValueT> GetSingleValueWorkItemDictionary<WorkItemT, ValueT>() where WorkItemT : SingleValueWorkItem<ValueT>
        {
            return Session.GetSingleValueWorkItemDictionary<WorkItemT, ValueT>();
        }

        public delegate void OnCreated(int id);
        static public event OnCreated Created = null;

        public delegate void OnFinishing(int id);
        static public event OnFinishing Finishing = null;

        public static int CountOfProcessed<InputItemT>() where InputItemT : InputItem
        {
            InputItemQueue iiq = Session.GetInputItemQueue(typeof(InputItemT).Name);
            return iiq.CountOfProcessed;
        }

        public static CustomBotCycle GetBotForThisThread<CustomBotCycle>() where CustomBotCycle : BotCycle
        {
            lock (threads2bot_cycle)
            {
                BotCycle bc;
                if (threads2bot_cycle.TryGetValue(Thread.CurrentThread, out bc))
                    return (CustomBotCycle)bc;
                return null;
            }
        }

        public static bool TreatExceptionAsFatal = false;

        public static string GetCurrentInputItemQueueNameThisThread()
        {
            lock (threads2bot_cycle)
            {
                BotCycle bc;
                if (threads2bot_cycle.TryGetValue(Thread.CurrentThread, out bc))
                    return bc.current_item.__Queue.Name;
                return null;
            }
        }

        virtual public void __Starting()
        {
        }
        
        virtual public void __Exiting()
        {
        }

        virtual public void __Processor(InputItem item)
        {
            MethodInfo mi;
            if (input_item_types2processor_mi.TryGetValue(item.GetType(), out mi))
                mi.Invoke(this, new object[] { item });
            else
                Session.This.__Processor(item);
        }
        static Dictionary<Type, MethodInfo> input_item_types2processor_mi = new Dictionary<Type, MethodInfo>();

        public Session Session { get { return Session.This; } }
    }
}
