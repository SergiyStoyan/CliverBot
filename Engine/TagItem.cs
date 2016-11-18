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
    /// Intended to be contained by many InputItem objects as a property, to save memory.
    /// It is logged/restored just as InputItem's.
    /// </summary>
    public class TagItem : Item
    {
        new static public ItemT Create<ItemT>(object anonymous_object) where ItemT : TagItem
        {
            return Item.Create<ItemT>(anonymous_object);
        }

        new internal static TagItem Restore(Type item_type, string item_seed, int item_id)
        {
            return (TagItem)Item.Restore(item_type, item_seed, item_id);
        }
    }

    //public class SingleValueTagItem<ValueT> : TagItem
    //{
    //    public readonly ValueT __Value;

    //    static public ItemT Create<ItemT>(ValueT value) where ItemT : SingleValueTagItem<ValueT>
    //    {
    //        return Item.Create<ItemT>("__Value", value);
    //    }
    //}
}

