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
using Cliver.Bot;

namespace Cliver.CrawlerHost
{
    public abstract class Product
    {
        public readonly string Id;
        public readonly string Url;

        public Product(string id, string url)
        {
            Id = id;
            Url = url;
        }

        static public void ValidateProductClass(Type type)
        {
            Type[] ALLOWED_TYPES = new Type[] { typeof(int), typeof(int[]), typeof(string), typeof(string[]) };
            Dictionary<string, FieldInfo> declared_field_infos = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToDictionary(x => x.Name, x => x);
            FieldInfo fi = declared_field_infos.Values.FirstOrDefault(x => !ALLOWED_TYPES.Contains(x.FieldType));
            if (fi != null)
                throw new TerminatingException("Product class " + type + " contains field " + fi.Name + " that has not allowed type: " + fi.FieldType.ToString() + ".\r\n Allowed types: " + string.Join("\r\n", ALLOWED_TYPES.Select(r => r.ToString())));
        }

        internal Dictionary<string, object> GetDeclaredField2Values()
        {
            if (Id == null)
                throw new Exception("'Id' is empty");
            if (Url == null)
                throw new Exception("'Url' is empty");

            //return get_declared_field_infos().ToDictionary(x => x.Key, x =>
            //{
            //    if (x.Value.FieldType == typeof(string[]))
            //        return (string[])x.Value.GetValue(this);
            //    if (x.Value.FieldType == typeof(int[]))
            //        return (int[])x.Value.GetValue(this);
            //    return x.Value.GetValue(this);
            //}
            //);
            return get_declared_field_infos().ToDictionary(x => x.Key, x => x.Value.GetValue(this));
        }

        Dictionary<string, FieldInfo> get_declared_field_infos()
        {
            if (declared_field_infos == null)
                declared_field_infos = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToDictionary(x => x.Name, x => x);
            return declared_field_infos;
        }
        Dictionary<string, FieldInfo> declared_field_infos = null;

        internal bool IsValid()
        {
            Validate();
            if (errors.Count > 0)
                throw new ProcessorException(ProcessorExceptionType.ERROR, "Product error: " + string.Join("\r\n", errors));
            if (warnings.Count > 0)
                Log.Warning(string.Join("\r\n", warnings));
            return errors.Count < 1;
        }

        virtual public void Validate()
        {
                if (Id == null)
                    Error("'Id' is empty");
                if (Url == null)
                    Error("'Url' is empty");
            foreach (KeyValuePair<string, object> f2v in GetDeclaredField2Values())
            {
                if(!(f2v.Value is string))
                    continue;
                if (f2v.Value == null)
                    Warning("'" + f2v.Key + "' is empty");
            }
        }

        public void Error(string message)
        {
            errors.Add(message);
        }
        readonly List<string> errors = new List<string>();

        public void Warning(string message)
        {
            warnings.Add(message);
        }
        readonly List<string> warnings = new List<string>();
    }

    //public class ProductDictionary : Product
    //{
    //    public ProductDictionary(string id, string url):base( id,  url)
    //    {
    //    }

    //    public ProductDictionary(params string[] key_value_pairs)
    //    {
    //        for (int i = 0; i < key_value_pairs.Length; i += 2)
    //            this[(string)key_value_pairs[i]] = FileWriter.PrepareField(key_value_pairs[i + 1], FileWriter.FieldFormat.DB_TABLE);
    //    }

    //    Dictionary<string, object> container = new Dictionary<string, object>();

    //    override public Dictionary<string, object> GetField2Values()
    //    {
    //        return container;
    //    }

    //    override public object this[string field]
    //    {
    //        set
    //        {
    //            Type field_type = value.GetType();
    //            Type at = ALLOWED_TYPES.FirstOrDefault(x => x == field_type);
    //            if (at == null)
    //                throw new Exception("Product class " + this.GetType() + " cannot contain field " + field + " of a prohibited type: " + field_type);

    //            container[field] = value;
    //        }
    //        get
    //        {
    //            return container[field];
    //        }
    //    }

    //    override public IEnumerable<string> Fields
    //    {
    //        get
    //        {
    //            return container.Keys.AsEnumerable();
    //        }
    //    }
    //}
}

