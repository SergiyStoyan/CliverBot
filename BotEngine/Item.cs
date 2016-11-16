//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************

#define SERIALIZE_TO_JSON1 //not completed

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
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Cliver.Bot
{
    /// <summary>
    /// Item derivative can define readonly public non-static fields that are value or string type
    /// </summary>
    public class Item
    {
        static internal ItemT Create<ItemT>(object anonymous_object) where ItemT : Item
        {
            //ItemT item = (ItemT)Activator.CreateInstance(typeof(ItemT));
            ItemT item = (ItemT)FormatterServices.GetUninitializedObject(typeof(ItemT));
            foreach (PropertyInfo pi in anonymous_object.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                object o = pi.GetValue(anonymous_object, null);
                FieldInfo fi = item.GetType().GetField(pi.Name);
                if (fi == null)
                    LogMessage.Exit("Field " + pi.Name + " is absent in " + item.GetType());
                try
                {
                    fi.SetValue(item, o);
                }
                catch (Exception e)
                {
                    LogMessage.Exit("Could not set " + pi.Name + " in " + item.GetType() + "\n" + Log.GetExceptionMessage(e));
                }
            }
            typeof(Item).GetField("__Id", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(item, Session.This.GetNewItemId());
            return item;
        }
        internal readonly int __Id = -1;

        internal static Item Restore(Type item_type, string item_seed, int item_id)
        {
#if SERIALIZE_TO_JSON
            lock (serializer)
            {
                Item item = (Item)serializer.Deserialize(item_seed, item_type);
                ConstructorInfo ci;
                if (item_types2constructor_info.TryGetValue(item_type, out ci))
                {
                    ci.Invoke(item, new object[] { });
                }
                typeof(Item).GetField("__Id", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(item, item_id);
                return item;
            }
#else
            lock (item_types2serialized_field_names2serialized_field_fi)
            {
                //Item item = (Item)Activator.CreateInstance(item_type);
                Item item = (Item)FormatterServices.GetUninitializedObject(item_type);
                item.restore_from_string(SERIALIZE_WITH_NAMES, item_seed, item_types2serialized_field_names2serialized_field_fi[item_type]);  ConstructorInfo ci;
                if (item_types2constructor_info.TryGetValue(item_type, out ci))
                {
                    ci.Invoke(item, new object[] { });
                }
                typeof(Item).GetField("__Id", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(item, item_id);
                return item;
            }
#endif          
        }

        //static protected Dictionary<string, FieldInfo> GetField2FieldInfos<ItemT>()
        //{
        //    return item_type2field_name2field_fis[typeof(ItemT)];
        //}
        //static Dictionary<Type, Dictionary<string, FieldInfo>> item_type2field_name2field_fis = new Dictionary<Type, Dictionary<string, FieldInfo>>();

        /// <summary>
        /// Seed is Item data serialized as string that is used for logging/restoring Item 
        /// </summary>
        /// <returns></returns>
        internal string GetSeed()
        {
#if SERIALIZE_TO_JSON          
            lock (serializer)
            {
                return serializer.Serialize(this);
            }
#else
            return get_as_string(SERIALIZE_WITH_NAMES, item_types2serialized_field_names2serialized_field_fi[this.GetType()]);  
#endif
        }
#if SERIALIZE_TO_JSON
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
#else
        protected const bool SERIALIZE_WITH_NAMES = false;
        protected static Dictionary<Type, Dictionary<string, FieldInfo>> item_types2serialized_field_names2serialized_field_fi = new Dictionary<Type, Dictionary<string, FieldInfo>>();
#endif
        protected static Dictionary<Type, ConstructorInfo> item_types2constructor_info = new Dictionary<Type, ConstructorInfo>();

        internal static void Initialize(List<Type> item_types)
        {
            foreach (Type item_type in item_types)
            {
                validate_fields(item_type);
                //item_type2field_name2field_fis[item_type] = (from x in item_type.GetFields() where !x.IsStatic select x).ToDictionary(x => x.Name, x => x);
                if (item_type.IsSubclassOfRawGeneric(typeof(SingleValueWorkItem<>)))
                    item_types2serialized_field_names2serialized_field_fi[item_type] = (from x in item_type.GetFields(BindingFlags.Instance | BindingFlags.Public) where x.Name == "__Value" && (x.GetCustomAttributes(typeof(ConstructedField)).FirstOrDefault() == null) select x).ToDictionary(x => x.Name, x => x);
                else
                    item_types2serialized_field_names2serialized_field_fi[item_type] = (from x in item_type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) where !x.FieldType.IsSubclassOf(typeof(Item)) && (x.GetCustomAttributes(typeof(ConstructedField)).FirstOrDefault() == null) select x).ToDictionary(x => x.Name, x => x);
                //item_type2parameter_count_in_constructor[item_type] = (from x in item_type.GetConstructors() select x.GetParameters().Length).Min();

                //ConstructorInfo ci = item_type.GetConstructor(item_types2serialized_field_names2serialized_field_fi[item_type].Values.Select(x=>x.FieldType).ToArray());
                ConstructorInfo ci = item_type.GetConstructor(new Type[]{});
                if (ci != null)
                   item_types2constructor_info[item_type] = ci;
            }
        }
        //protected static Dictionary<Type, int> item_type2parameter_count_in_constructor = new Dictionary<Type, int>();

        static void validate_fields(Type item_type)
        {
            List<string> fs = (from x in item_type.GetFields(BindingFlags.Public | BindingFlags.Instance) where !x.IsInitOnly select x.Name).ToList();
            if (fs.Count > 0)
                throw new Exception("The following fields in " + item_type + " must be readonly: " + fs.Aggregate((total, x) => total + ", " + x));

            fs = (from x in item_type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) select x.Name).ToList();
            if (fs.Count > 0)
                throw new Exception("The following fields in " + item_type + " must be public: " + fs.Aggregate((total, x) => total + ", " + x));

            List<string> base_fs = (from y in item_type.BaseType.GetFields() select y.Name).ToList();
            fs = (from x in item_type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly) where !base_fs.Contains(x.Name) select x.Name).ToList();
            if (fs.Count > 0)
                throw new Exception("The following fields in " + item_type + " cannot be static: " + fs.Aggregate((total, x) => total + ", " + x));

            fs = (from x in item_type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                  where !(x.FieldType.IsValueType || x.FieldType == typeof(string)) && !ALLOWED_PARENT_ITEM_TYPES.Contains(x.FieldType.BaseType) && (x.GetCustomAttributes(typeof(ConstructedField)).FirstOrDefault() == null)
                  select x.Name).ToList();
            if (fs.Count > 0)
                throw new Exception("The following fields in " + item_type + " have not supported type: " + fs.Aggregate((total, x) => total + ", " + x));
        }
        static readonly Type[] ALLOWED_PARENT_ITEM_TYPES = new Type[] { typeof(InputItem), typeof(TagItem)/*, typeof(SingleValueTagItem<>)*/ };

        //string CreateCacheKey()
        //{
        //    return CreateCacheKey(this);
        //}

        //public static string CreateCacheKey(object o, string property_name = null)
        //{
        //    var sb = new StringBuilder();
        //    if (o.GetType().IsValueType || o is string)
        //        sb.AppendFormat("{0}_{1}|", property_name, o);
        //    else
        //    {
        //        foreach (PropertyInfo p in o.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly))
        //        {
        //            if (!typeof(IEnumerable<object>).IsAssignableFrom(p.PropertyType))
        //            {
        //                sb.AppendFormat("{0}{1}_{2}|", property_name, p.Name, p.GetValue(o, null));
        //                continue;
        //            }
        //            MethodInfo get = p.GetGetMethod();
        //            if (!get.IsStatic && get.GetParameters().Length == 0)
        //            {
        //                var collection = (IEnumerable<object>)get.Invoke(o, null);
        //                if (collection == null)
        //                    continue;
        //                foreach (object ob in collection)
        //                    sb.Append(CreateCacheKey(ob, p.Name));
        //            }
        //        }
        //    }        
        //    return sb.ToString();
        //}

        internal protected string get_as_string(bool with_names, Dictionary<string, FieldInfo> ns2fi)
        {
            Type item_type = this.GetType();
            List<string> ss = new List<string>();
            if (with_names)
            {
                foreach (FieldInfo fi in ns2fi.Values)
                    ss.Add(fi.Name + "=" + Regex.Replace(fi.GetValue(this).ToString(), @"\|", @"\|"));
            }
            else
            {
                foreach (FieldInfo fi in ns2fi.Values)
                {
                    object o = fi.GetValue(this);
                    if (o == null)
                        o = "";
                    ss.Add(Regex.Replace(o.ToString(), @"\|", @"\|"));
                }
            }
            return string.Join("|", ss);
        }

        protected void restore_from_string(bool with_names, string item_seed, Dictionary<string, FieldInfo> ns2fi)
        {
            Dictionary<FieldInfo, string> fis2v = new Dictionary<FieldInfo, string>();
            if (with_names)
            {
                foreach (string n2v in Regex.Split(item_seed, @"(?<=^|[^\\])\|"))
                {
                    string[] ss = n2v.Split('=');
                    fis2v[ns2fi[ss[0]]] = Regex.Replace(ss[1], @"\\\|", "|");
                }
            }
            else
            {
                int i = 0;
                string[] vs = Regex.Split(item_seed, @"(?<=^|[^\\])\|");
                foreach (FieldInfo fi in ns2fi.Values)
                    fis2v[fi] = Regex.Replace(vs[i++], @"\\\|", "|");
            }

            foreach (FieldInfo fi in ns2fi.Values)
                fi.SetValue(this, Convert.ChangeType(fis2v[fi], fi.FieldType));
        }

        /// <summary>
        /// Such a field is expected to be set within a custom constructor so it is not initialized and restored by BotEngine implicitly
        /// </summary>
        public class ConstructedField : Attribute
        {
        }
    }
}

