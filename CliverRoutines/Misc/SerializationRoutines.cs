using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Script.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

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
            t.Created();
            return t;
        }

        static T get<T>(string file, bool ignore_file_content) where T : Serializable, new()
        {
            if (!file.Contains(":"))
                file = Log.GetAppCommonDataDir() + "\\" + file;
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
                file = Log.GetAppCommonDataDir() + "\\" + file;
            Serializable s;
            if (!ignore_file_content && File.Exists(file))
                s = (Serializable)Cliver.SerializationRoutines.Json.Load(serializable_type, file);
            else
                s = (Serializable)Activator.CreateInstance(serializable_type);
            s.__File = file;
            return s;
        }

        [ScriptIgnore]
        public string __File { get; private set; }

        public void Save()
        {
            Saving();
            Cliver.SerializationRoutines.Json.Save(__File, this);
        }

        virtual public void Loaded()
        {

        }

        virtual public void Created()
        {

        }

        virtual public void Saving()
        {

        }
    }

    static public class SerializationRoutines
    {
        static public class Json
        {
            /// <summary>
            /// Serialize object
            /// </summary>
            /// <param name="o"></param>
            /// <returns></returns>
            static public string Serialize(object o)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(o);
            }

            /// <summary>
            /// Deserialize object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="json"></param>
            /// <returns></returns>
            static public T Deserialize<T>(string json)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<T>(json);
            }

            static public object Deserialize(Type type, string json)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize(json, type);
            }

            static public void Save(string file, object o)
            {
                PathRoutines.CreateDirectory(PathRoutines.GetDirFromPath(file));
                File.WriteAllText(file, Serialize(o));
            }

            static public T Load<T>(string file)
            {
                return Deserialize<T>(File.ReadAllText(file));
            }

            static public object Load(Type type, string file)
            {
                return Deserialize(type, File.ReadAllText(file));
            }
        }

        static public class Xml
        {
            /// <summary>
            /// Serialize object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="o"></param>
            /// <returns></returns>
            static public string Serialize2<T>(T o)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                MemoryStream ms = new MemoryStream();
                serializer.Serialize(ms, o);
                StreamReader sr = new StreamReader(ms);
                return sr.ReadToEnd();
            }

            /// <summary>
            /// Deserialize object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="xml"></param>
            /// <returns></returns>
            static public T Deserialize2<T>(string xml)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                StringReader s = new StringReader(xml);
                return (T)serializer.Deserialize(s);
            }

            /// <summary>
            /// Serialize object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="o"></param>
            /// <returns></returns>
            public static string Serialize<T>(T o)
            {
                var serializer = new DataContractSerializer(typeof(T));
                using (var writer = new StringWriter())
                using (var stm = new XmlTextWriter(writer))
                {
                    serializer.WriteObject(stm, o);
                    return writer.ToString();
                }
            }

            /// <summary>
            /// Deserialize object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="xml"></param>
            /// <returns></returns>
            public static T Deserialize<T>(string xml)
            {
                var serializer = new DataContractSerializer(typeof(T));
                using (var reader = new StringReader(xml))
                using (var stm = new XmlTextReader(reader))
                {
                    return (T)serializer.ReadObject(stm);
                }
            }
        }
    }
}

