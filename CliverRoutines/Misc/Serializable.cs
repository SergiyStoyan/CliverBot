using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Script.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Reflection;

namespace Cliver
{
    public class Serializable
    {
        static public T Load<T>(string file) where T : Serializable, new()
        {
            T t = get<T>(file, false);
            t.Loaded();
            return t;
        }

        static public T Create<T>(string file) where T : Serializable, new()
        {
            T t = get<T>(file, true);
            t.Loaded();
            return t;
        }

        static T get<T>(string file, bool ignore_file_content) where T : Serializable, new()
        {
            if (!file.Contains(":"))
                file = Log.AppCommonDataDir + "\\" + file;
            T s;
            if (!ignore_file_content && File.Exists(file))
                s = Cliver.SerializationRoutines.Json.Load<T>(file);
            else
                s = new T();
            s.__File = file;
            return s;
        }

        static public Serializable Load(Type serializable_type, string file)
        {
            Serializable t = get(serializable_type, file, false);
            t.Loaded();
            return t;
        }

        static public Serializable Create(Type serializable_type, string file)
        {
            Serializable t = get(serializable_type, file, true);
            t.Loaded();
            return t;
        }

        static Serializable get(Type serializable_type, string file, bool ignore_file_content)
        {
            if (!file.Contains(":"))
                file = Log.AppCommonDataDir + "\\" + file;
            Serializable s;
            if (!ignore_file_content && File.Exists(file))
                s = (Serializable)Cliver.SerializationRoutines.Json.Load(serializable_type, file);
            else
                s = (Serializable)Activator.CreateInstance(serializable_type);
            s.__File = file;
            return s;
        }

        //[ScriptIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string __File { get; private set; }

        public void Save(string file = null)
        {
            lock (this)
            {
                if (file != null)
                    __File = file;
                Saving();
                Cliver.SerializationRoutines.Json.Save(__File, this);
            }
        }

        virtual public void Loaded()
        {

        }

        virtual public void Saving()
        {

        }
    }
}