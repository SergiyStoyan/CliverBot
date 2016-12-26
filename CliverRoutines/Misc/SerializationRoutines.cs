
//#define UseNetJsonSerialization //for legacy apps

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
#if UseNetJsonSerialization
using System.Web.Script.Serialization;
#else
using Newtonsoft.Json;
#endif
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Cliver
{
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
#if UseNetJsonSerialization
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(o);
#else
                return JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.Indented);
#endif
            }

            /// <summary>
            /// Deserialize object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="json"></param>
            /// <returns></returns>
            static public T Deserialize<T>(string json)
            {
#if UseNetJsonSerialization
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<T>(json);
#else
                return JsonConvert.DeserializeObject<T>(json);
#endif
            }

            static public object Deserialize(Type type, string json)
            {
#if UseNetJsonSerialization
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize(json, type);
#else
                return JsonConvert.DeserializeObject(json, type);
#endif
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

