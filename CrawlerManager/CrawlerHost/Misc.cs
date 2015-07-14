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
using System.Xml;
using Cliver.CrawlerHost;
using Cliver.Bot;

namespace Cliver.CrawlerHost
{
    public class Misc
    {
        static public T RestoreProduct<T>(string id, string url, string data) where T : Product
        {
            T product = SerializationRoutines.Json.Get<T>(data);
            typeof(T).GetField("Id").SetValue(product, id);
            typeof(T).GetField("Url").SetValue(product, url);
            return product;
        }
    }
}

