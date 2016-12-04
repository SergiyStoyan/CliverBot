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
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Linq;
using System.Reflection;

namespace Cliver.Bot
{
    /// <summary>
    /// Item derivative that is designed to be value in [string]=>[WorkItem derivative] dictionary.
    /// It is logged/restored just as InputItem's.
    /// </summary>
    public class WorkItem : Item
    {
        new static internal ItemT Create<ItemT>(dynamic anonymous_object) where ItemT : WorkItem
        {
            return Item.Create<ItemT>(anonymous_object);
        }

        new internal static WorkItem Restore(Type item_type, ArrayList item_seed, int item_id)
        {
            return (WorkItem)Item.Restore(item_type, item_seed, item_id);
        }
    }

    /// <summary>
    /// Single value WorkItem so it cannot contain any member except the one declared by generic.
    /// Derivatives of this class are used as value in [string]=>[SingleValueWorkItem derivative] dictionary.
    /// It is logged/restored just as InputItem's.
    /// </summary>
    /// <typeparam name="ValueT">type of its value</typeparam>
    public class SingleValueWorkItem<ValueT> : WorkItem
    {
        public readonly ValueT __Value;

        static public ItemT Create<ItemT>(ValueT value) where ItemT : SingleValueWorkItem<ValueT>
        {
            return Item.Create<ItemT>(new {__Value = value});
        }
    }
}

