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
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Reflection;

namespace Cliver.Bot
{
    public partial class Session
    {
        static internal void SetInputItemQueuesOrder(params string[] ordered_input_item_queue_names)
        {
            lock (This.input_item_queue_name2input_item_queues)
            {
                if (This.input_item_queue_name2input_item_queues.Count > 0)
                    throw new Exception("SetInputItemQueuesOrder cannot be called after adding an InputItem");

                List<Type> ordered_iits = new List<Type>();

                List<Type> item_types = (from t in Assembly.GetEntryAssembly().GetTypes() where t.IsSubclassOf(typeof(InputItem)) && !t.IsGenericType select t).ToList();
                foreach (string queue_name in ordered_input_item_queue_names)
                {
                    bool found = false;
                    foreach (Type it in item_types)
                    {
                        if (it.Name == queue_name)
                        {
                            ordered_iits.Add(it);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        throw new Exception("Queue " + queue_name + " was not found among derivatives of InputItem");
                }
                ordered_iits.Reverse();
                foreach (Type item_type in ordered_iits)
                    SetInputItemQueuePosition(item_type.Name, 0);
            }
        }

        /// <summary>
        /// By default each item type has its own queue. It is always so at the beginning of session. During session more names queue can be added.
        /// </summary>
        /// <param name="ordered_input_item_types"></param>
        static public void SetInputItemQueuesOrder(params Type[] ordered_input_item_types)
        {
            SetInputItemQueuesOrder((from t in ordered_input_item_types select t.Name).ToArray());
        }

        static public void SetInputItemQueuePosition<ItemT>(int position) where ItemT : InputItem
        {
            SetInputItemQueuePosition(typeof(ItemT).Name, position);
        }

        static public void SetInputItemQueuePositionAfterQueue(string queue_name, string after_queue_name)
        {
            lock (This.input_item_queue_name2input_item_queues)
            {
                SetInputItemQueuePosition(queue_name, ((InputItemQueue)This.input_item_queue_name2input_item_queues[after_queue_name]).Position + 1);
            }
        }

        static public InputItemQueue GetInputItemQueue<ItemT>() where ItemT : InputItem
        {
            return GetInputItemQueue(typeof(ItemT).Name);
        }

        /// <summary>
        /// Add item to queue. It is possible to create a named queue.
        /// It is the same like BotCycle.Add() but not so efficient.
        /// </summary>
        /// <param name="queue_name"></param>
        /// <param name="item"></param>
        static public bool Add(string queue_name, InputItem item)
        {
            if (queue_name == null)
                queue_name = item.GetType().Name;
            InputItemQueue iiq = Session.GetInputItemQueue(queue_name);
            return InputItem.Add2Queue(iiq, BotCycle.GetCurrentInputItemForThisThread(), item);
        }

        /// <summary>
        /// Add item to queue. By default name of queue is name of item type.
        /// It is the same like BotCycle.Add() but not so efficient.
        /// </summary>
        /// <param name="item"></param>
        static public bool Add(InputItem item)
        {
            return Add(null, item);
        }

        /// <summary>
        /// Add item as dynamic object to queue. It is possible to create a named queue.
        /// It is the same like BotCycle.Add() but not so efficient and safe.
        /// </summary>
        /// <typeparam name="ItemT"></typeparam>
        /// <param name="queue_name"></param>
        /// <param name="anonymous_object"></param>
        /// <returns></returns>
        static public bool Add<ItemT>(string queue_name, dynamic anonymous_object) where ItemT : InputItem
        {
            InputItemQueue iiq = Session.GetInputItemQueue(queue_name);
            return InputItem.Add2Queue<ItemT>(iiq, BotCycle.GetCurrentInputItemForThisThread(), anonymous_object);
        }

        /// <summary>
        /// Add item as dynamic object to queue. By default name of queue is name of item type.
        /// It is the same as BotCycle.Add() but not so efficient and safe.
        /// </summary>
        /// <typeparam name="ItemT"></typeparam>
        /// <param name="anonymous_object"></param>
        /// <returns></returns>
        static public bool Add<ItemT>(dynamic anonymous_object) where ItemT : InputItem
        {
            return Add<ItemT>(typeof(ItemT).Name, anonymous_object);
        }

        //static public ItemT WorkItem<ItemT>(params object[] field_value_pairs) where ItemT : WorkItem
        //{
        //    return Cliver.Bot.WorkItem.Create<ItemT>(field_value_pairs);
        //}

        static public ItemT WorkItem<ItemT, ValueT>(ValueT value) where ItemT : SingleValueWorkItem<ValueT>
        {
            return Cliver.Bot.SingleValueWorkItem<ValueT>.Create<ItemT>(value);
        }

        static public ItemT TagItem<ItemT>(params object[] field_value_pairs) where ItemT : TagItem
        {
            return Cliver.Bot.TagItem.Create<ItemT>(field_value_pairs);
        }

        //public ItemT TagItem<ItemT, ValueT>(ValueT value) where ItemT : SingleValueTagItem<ValueT>
        //{
        //    return Cliver.SingleValueTagItem<ValueT>.Create<ItemT>(value);
        //}

        static public WorkItemDictionary<WorkItemT> GetWorkItemDictionary<WorkItemT>() where WorkItemT : WorkItem
        {
            return Session.This.GetWorkItemDictionary_<WorkItemT>();
        }

        static public SingleValueWorkItemDictionary<WorkItemT, ValueT> GetSingleValueWorkItemDictionary<WorkItemT, ValueT>() where WorkItemT : SingleValueWorkItem<ValueT>
        {
            return Session.This.GetSingleValueWorkItemDictionary_<WorkItemT, ValueT>();
        }

        public delegate void OnCreating();
        static public event OnCreating Creating = null;

        public delegate void OnFillStartInputItemQueue(InputItemQueue start_input_item_queue, Type start_input_item_type);
        public static OnFillStartInputItemQueue FillStartInputItemQueue = (InputItemQueue start_input_item_queue, Type start_input_item_type) =>
        {
            Log.Main.Write("Filling queue of " + start_input_item_queue.Name + " with input file.");

            if (!File.Exists(Settings.Input.File))
                throw (new Exception("Input file " + Settings.Input.File + " does not exist."));

            if (Path.GetExtension(Settings.Input.File).StartsWith(".xls", StringComparison.InvariantCultureIgnoreCase))
                throw new Exception("Reading excel is not supported");

            FileReader fr = new FileReader(Settings.Input.File, Settings.Input.FileFormat);
            for (FileReader.Row r = fr.ReadLine(); r != null; r = fr.ReadLine())
                InputItem.Add2QueueBeforeStart(start_input_item_queue, start_input_item_type, r.Headers.ToDictionary(x => x, x => r[x]));

            if (start_input_item_queue.CountOfNew < 1)
                LogMessage.Error("Input queue is empty so nothing is to do. Check your input data.");
        };

        public delegate void OnClosing();
        static public event OnClosing Closing = null;

        public delegate void OnClosed();
        /// <summary>
        /// Used to exit application
        /// </summary>
        static public event OnClosed Closed = null;

        static public Type GetFirstDeclaredInputItemType()
        {
            return (from t in Assembly.GetEntryAssembly().GetTypes() where t.BaseType == typeof(InputItem) select t).FirstOrDefault();
        }

        //public delegate void OnFatalError(string message);
        //static public event OnFatalError FatalError = null;

        static public void FatalErrorClose(string message)
        {
            LogMessage.Error(message);
            Session.State = SessionState.FATAL_ERROR;
            This.Storage.WriteState(State, new { });
            Bot.FatalError(message);
            Session.Close();
        }

        static public void FatalErrorClose(Exception e)
        {
            LogMessage.Error(e);
            Session.State = SessionState.FATAL_ERROR;
            This.Storage.WriteState(State, new { });
            Bot.FatalError(e.Message);
            Session.Close();
        }

        public class FatalException : Exception
        {
            public FatalException(string message)
                : base("FATAL EXCEPTION. " + message)
            {
            }

            public FatalException(string message, Exception exception)
                : base("FATAL EXCEPTION. " + message, exception)
            {
            }

            public FatalException(Exception exception)
                : base("FATAL EXCEPTION. ", exception)
            {
            }
        }
    }
}