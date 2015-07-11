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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Cliver.Bot
{
    /// <summary>
    /// Items that are ordered in queues and processed one by one in BotCycle.
    /// </summary>
    abstract public class InputItem : Item
    {
        /// <summary>
        /// Method to process custom InputItem. 
        /// When it is not defined within custom InputItem class, it must be defined in CustomBot as a function with InputItem Type parameter, where the function name is "PROCESSOR".
        /// </summary>
        /// <param name="bc"></param>
        virtual public void PROCESSOR(BotCycle bc)
        {
            //it will be invoked by default if no overriding PROCESSOR implementation
            bc.Bot.PROCESSOR(this);
        }

        static internal bool Add2Queue<ItemT>(InputItemQueue queue, InputItem parent_item, object anonymous_object) where ItemT : InputItem
        {
            ItemT item = Item.Create<ItemT>(anonymous_object);
            item.set_parent_members(parent_item);
            item.set_tag_item_members();
            return item.add2queue(queue);
        }

        /// <summary>
        /// Must be used only for InputItems created by own constructor
        /// if InputItem was created by own constructor, its base parameters should be set
        /// </summary>
        /// <typeparam name="ItemT"></typeparam>
        /// <param name="parent_item"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        static internal bool Add2Queue<ItemT>(InputItemQueue queue, InputItem parent_item, ItemT item) where ItemT : InputItem
        {
            item.set_parent_members(parent_item);
            item.set_tag_item_members();
            typeof(Item).GetField("__Id", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(item, Session.This.GetNewItemId());
            return item.add2queue(queue);
        }

        /// <summary>
        /// Must be used ONLY before bot cycle started as no parent item is accepted. Usually used while feeding by input file.
        /// </summary>
        /// <param name="item_type"></param>
        /// <param name="field_value_pairs"></param>
        /// <returns></returns>
        static internal bool Add2QueueBeforeStart(InputItemQueue queue, Type item_type, Dictionary<string, string> field2value)
        {
            InputItem item = (InputItem)FormatterServices.GetUninitializedObject(item_type);
            Dictionary<string, FieldInfo> serialized_field_name2serialized_field_fis = item_type2serialized_field_name2serialized_field_fis[item_type];
            foreach (string field in field2value.Keys)
                try
                {
                    FieldInfo fi = serialized_field_name2serialized_field_fis[field];
                    fi.SetValue(item, field2value[field]);
                }
                catch (Exception e)
                {
                    throw new Exception("Field '" + field + "' does not exist.\n" + e.Message);
                }
            typeof(Item).GetField("__Id", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(item, Session.This.GetNewItemId());
            return item.add2queue(queue);
        }

        public readonly InputItemQueue __Queue = null;
        internal readonly InputItem __ParentItem = null;

        bool add2queue(InputItemQueue queue)
        {
            __State = InputItemState.NEW;
            this.GetType().GetField("__Queue", BindingFlags.Instance | BindingFlags.Public).SetValue(this, queue);
            return __Queue.Enqueue(this);
        }

        void set_tag_item_members()
        {
            Dictionary<string, TagItem> tag_item_name2tag_items = GetTagItemName2TagItems();
            foreach (TagItem ti in tag_item_name2tag_items.Values)
            {
                if (ti.__Id >= 0)
                    continue;
                typeof(Item).GetField("__Id", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ti, Session.This.GetNewItemId());
            }
        }

        void set_parent_members(InputItem parent_item)
        {
            lock (item_type2parent_field_fis)
            {
                this.GetType().GetField("__ParentItem", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, parent_item);

                foreach (FieldInfo fi in item_type2parent_field_fis[this.GetType()])
                {
                    for (InputItem ii = __ParentItem; ii != null; ii = ii.__ParentItem)
                    {
                        if (fi.FieldType != ii.GetType())
                            continue;
                        fi.SetValue(this, ii);
                        break;
                    }
                    //if (fi.GetValue(this) == null)
                    //    throw new Exception("Field " + fi.Name + " of " + fi.FieldType + " type is not parent for " + this.GetType());
                }
            }
        }
        static Dictionary<Type, List<FieldInfo>> item_type2parent_field_fis = new Dictionary<Type, List<FieldInfo>>();

        /// <summary>
        /// Key may deffer from Seed as some fields may be excluded from Key 
        /// </summary>
        /// <returns></returns>
        internal string GetKey()
        {
            return get_as_string(false, item_type2key_field_name2key_field_fis[this.GetType()]);
        }
        static Dictionary<Type, Dictionary<string, FieldInfo>> item_type2key_field_name2key_field_fis = new Dictionary<Type, Dictionary<string, FieldInfo>>();

        internal Dictionary<string, TagItem> GetTagItemName2TagItems()
        {
            List<FieldInfo> tag_item_fis = item_type2tag_item_fis[this.GetType()];
            Dictionary<string, TagItem> tag_item_name2tag_items = new Dictionary<string, TagItem>();
            foreach (FieldInfo fi in tag_item_fis)
                tag_item_name2tag_items[fi.Name] = (TagItem)fi.GetValue(this);
            return tag_item_name2tag_items;
        }
        static Dictionary<Type, List<FieldInfo>> item_type2tag_item_fis = new Dictionary<Type,List<FieldInfo>>();

        internal static InputItem Restore(InputItemQueue queue, Type item_type, string item_seed, int item_id, InputItem parent_item, Dictionary<string, TagItem> field2tag_items)
        {
            InputItem item = (InputItem)Item.Restore(item_type,  item_seed,  item_id);
            item.set_parent_members(parent_item);
            foreach (KeyValuePair<string, TagItem> kv in field2tag_items)
                item_type.GetField(kv.Key).SetValue(item, kv.Value);
            item.add2queue(queue);
            return item;
        }

        internal InputItemState __State
        {
            get
            {
                return __state;
            }
            set
            {
                this.__state = value;

                switch (Session.This.SourceType)
                {
                    case ItemSourceType.DB:
                        return;
                    case ItemSourceType.FILE:
                        if (__state != InputItemState.NEW)
                            Session.This.LogInputItem(this);
                        return;
                    default:
                        throw new Exception("Undefined SourceType: " + Session.This.SourceType.ToString());
                }
            }
        }
        InputItemState __state;

        new internal static void Initialize(List<Type> item_types)
        {
            Item.Initialize(item_types);
            foreach (Type item_type in item_types)
            {
                //create dictionaries
                item_type2parent_field_fis[item_type] = (from x in item_type.GetFields() where !x.IsStatic && x.FieldType.IsSubclassOf(typeof(InputItem)) select x).ToList();
                item_type2tag_item_fis[item_type] = (from x in item_type.GetFields() where x.FieldType.BaseType == typeof(TagItem) select x).ToList();

                //fill item_type2key_field_fis
                Dictionary<string, FieldInfo> key_field2key_field_fis = (from x in item_type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) where x.GetCustomAttributes(typeof(KeyField), false).FirstOrDefault() != null select x).ToDictionary(x => x.Name, x => x);
                List<string> ns = (from x in key_field2key_field_fis.Values where x.FieldType.IsSubclassOf(typeof(Item)) || x.DeclaringType != item_type select x.Name).ToList();
                if (ns.Count > 0)
                    throw new Exception("InputItem derivative " + item_type + " cannot use attribute " + typeof(KeyField) + " for the following fields: " + string.Join(", ", ns));
                List<FieldInfo> not_key_field_fis = (from x in item_type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) where x.GetCustomAttributes(typeof(NotKeyField), false).FirstOrDefault() != null select x).ToList();
                ns = (from x in not_key_field_fis where x.FieldType.IsSubclassOf(typeof(Item)) || x.DeclaringType != item_type select x.Name).ToList();
                if (ns.Count > 0)
                    throw new Exception("InputItem derivative " + item_type + " cannot use attribute " + typeof(NotKeyField) + " for the following fields: " + string.Join(", ", ns));
                if (key_field2key_field_fis.Count > 0)
                {
                    if (not_key_field_fis.Count > 0)
                        throw new Exception("InputItem derivative " + item_type + " cannot use attributes " + typeof(KeyField) + " and " + typeof(NotKeyField) + " at the same time");
                }
                else
                {
                    key_field2key_field_fis = (from x in item_type.GetFields() where !x.IsStatic && !x.FieldType.IsSubclassOf(typeof(Item)) select x).ToDictionary(x => x.Name, x => x);
                    if (not_key_field_fis.Count > 0)
                        key_field2key_field_fis = (from f in key_field2key_field_fis.Values where !not_key_field_fis.Contains(f) select f).ToDictionary(x => x.Name, x => x);
                }
                if (key_field2key_field_fis.Count < 1)
                    throw new Exception("No key field was found for " + item_type);
                item_type2key_field_name2key_field_fis[item_type] = key_field2key_field_fis;
            }
        }

        public class KeyField : Attribute
        {
        }

        public class NotKeyField : Attribute
        {
        }
    }

    internal enum InputItemState : uint
    {
        NEW = 1,
        COMPLETED = 2,
        ERROR = 3,
        ERROR_RESTORE_AS_NEW = 4
    }
}

