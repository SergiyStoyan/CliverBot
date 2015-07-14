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
        static public T RestoreProduct<T>(Record r) where T : Product
        {
            T product = SerializationRoutines.Json.Get<T>((string)r["data"]);
            typeof(T).GetField("Id").SetValue(product, r["id"]);
            typeof(T).GetField("Url").SetValue(product, r["url"]);
            typeof(T).GetField("CrawlTime").SetValue(product, r["crawl_time"]);
            typeof(T).GetField("ChangeTime").SetValue(product, r["change_time"]);
            return product;
        }
    }
}

