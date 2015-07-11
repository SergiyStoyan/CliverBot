using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
//using System.Data.Odbc;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Cliver.Bot
{
    public abstract class Product
    {
        protected static readonly Type[] __ALLOWED_TYPES = new Type[] { typeof(int), typeof(string) };

        abstract public Dictionary<string, object> __GetField2Values();

        virtual public bool IsValid()
        {
            foreach (string field in this.Fields)
                if (!validate_value(field))
                    return false;
            return true;
        }

        virtual protected bool validate_value(string field)
        {
            if (this[field] == null)
                __errors.Add("'" + field + "' is empty");
            return true;
        }

        public bool __IsError
        {
            get
            {
                return __errors.Count > 0;
            }
        }
        public string __GetErrorsAsString()
        {
            return string.Join("\r\n", __errors);
        }
        List<string> __errors = new List<string>();

        abstract public object this[string field]
        {
            get;
            set;
        }

        abstract public IEnumerable<string> Fields
        {
            get;
        }
    }

    public class ProductDictionary : Product
    {
        public ProductDictionary()
        {
        }

        public ProductDictionary(params string[] key_value_pairs)
        {
            for (int i = 0; i < key_value_pairs.Length; i += 2)
                this[(string)key_value_pairs[i]] = FileWriter.PrepareField(key_value_pairs[i + 1], FileWriter.FieldFormat.DB_TABLE);
        }

        Dictionary<string, object> container = new Dictionary<string, object>();

        override public Dictionary<string, object> __GetField2Values()
        {
            return container;
        }

        override public object this[string field]
        {
            set
            {
                Type field_type = value.GetType();
                Type at = __ALLOWED_TYPES.FirstOrDefault(x => x == field_type);
                if (at == null)
                    throw new Exception("Product class " + this.GetType() + " cannot contain field " + field + " of a prohibited type: " + field_type);

                container[field] = value;
            }
            get
            {
                return container[field];
            }
        }

        override public IEnumerable<string> Fields
        {
            get
            {
                return container.Keys.AsEnumerable();
            }
        }
    }

    public class ProductClass : Product
    {
        public ProductClass()
        {
        }

        public ProductClass(params string[] key_value_pairs)
        {
            Dictionary<string, FieldInfo> n2fis = __get_field_infos();
            for (int i = 0; i < key_value_pairs.Length; i += 2)
            {
                string v = FileWriter.PrepareField(key_value_pairs[i + 1], FileWriter.FieldFormat.DB_TABLE);
                n2fis[key_value_pairs[i]].SetValue(this, v);
            }
        }

        private Dictionary<string, FieldInfo> __get_field_infos()
        {
            if (__field_name2fis == null)
            {
                __field_name2fis = (from x in this.GetType().GetFields() where !x.IsStatic && !x.IsPrivate select x).ToDictionary(x => x.Name, x => x);
                FieldInfo fi = __field_name2fis.Values.FirstOrDefault(x => !__ALLOWED_TYPES.Contains(x.FieldType));
                if (fi != null)
                    throw new Exception("Product class " + this.GetType() + " contains field " + fi.Name + " of a prohibited type: " + fi.GetType());
            }
            return __field_name2fis;
        }
        private static Dictionary<string, FieldInfo> __field_name2fis = null;

        override public Dictionary<string, object> __GetField2Values()
        {
            return __get_field_infos().Values.ToDictionary(x => x.Name, x => x.GetValue(this));
        }

        override public object this[string field]
        {
            set
            {
                __get_field_infos()[field].SetValue(this, value);
            }
            get
            {
                return __get_field_infos()[field].GetValue(this);
            }
        }

        override public IEnumerable<string> Fields
        {
            get
            {
                return __get_field_infos().Keys.AsEnumerable();
            }
        }
    }
}

