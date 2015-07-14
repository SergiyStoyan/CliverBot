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
    public class SessionApi
    {
        public enum CrawlerMode : byte { IDLE, PRODUCTION }
        
        static SessionApi()
        {
            lock (DbApi.Dbc)
            { 
                try
                {        
                    CrawlerId = Log.ProcessName;

                    Record r = DbApi.Dbc.Get("SELECT _products_table FROM crawlers WHERE id=@id").GetFirstRecord("@id", CrawlerId);
                    if (r == null)
                        LogMessage.Exit("Crawler id '" + CrawlerId + "' does not exist in [crawlers] table.");

                    ProductsTable = DbApi.CreateProductsTableForCrawler(CrawlerId);

                    Session.Closing += session_Closing;

                    InitSession();
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
            }
        }

        readonly static public string CrawlerId;
        readonly static public string ProductsTable;
        
        public static void InitSession()
        {
            lock (DbApi.Dbc)
            { 
                try
                {
                    if (mode != CrawlerMode.IDLE)
                        return;
                    mode = CrawlerMode.PRODUCTION;

                    Record r = DbApi.Dbc.Get("SELECT * FROM crawlers WHERE id=@id").GetFirstRecord("@id", CrawlerId);
                    if (r == null)
                        LogMessage.Exit("Crawler id '" + CrawlerId + "' does not exist in [crawlers] table");

                    string archive = "session_start_time="
                        + (r["_session_start_time"] != null ? ((DateTime)r["_session_start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "")
                        + " start_time=" + (r["_last_start_time"] != null ? ((DateTime)r["_last_start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "")
                        + " end_time=" + (r["_last_end_time"] != null ? ((DateTime)r["_last_end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "")
                        + " state=" + (r["_last_session_state"] != null ? ((DbApi.SessionState)r["_last_session_state"]).ToString() : "")
                        + " log=" + r["_last_log"] + "\n" + r["_archive"];
                    const int MAX_ARCHIVE_LENGTH = 10000;
                    archive = archive.Substring(0, archive.Length < MAX_ARCHIVE_LENGTH ? archive.Length : MAX_ARCHIVE_LENGTH);
                    if (DbApi.Dbc.Get("UPDATE crawlers SET _session_start_time=@session_start_time, _last_process_id=@process_id, _last_start_time=GETDATE(), _last_end_time=NULL, _last_session_state=" + (byte)DbApi.SessionState.STARTED + ", _last_log=@Log, _archive=@archive WHERE id=@id").Execute(
                        "@session_start_time", Session.This.StartTime, "@process_id", Process.GetCurrentProcess().Id, "@Log", Log.SessionDir, "@archive", archive, "@id", CrawlerId) < 1
                        )
                        throw new Exception("Could not update crawlers table.");
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
            }
        }

        static CrawlerMode mode;

        static void session_Closing()
        {
            stop(!Session.This.IsUnprocessedInputItem && !Session.This.IsItemToRestore);
        }

        internal static void stop(bool completed)
        {
            lock (DbApi.Dbc)
            { 
                switch (mode)
                {
                    case CrawlerMode.PRODUCTION:
                        if (completed)
                        {
                            Log.Main.Inform("Deleted marked old products: " + DbApi.Dbc["DELETE FROM " + ProductsTable + " WHERE state=" + (byte)DbApi.ProductState.DELETED].Execute());
                            Log.Main.Inform("Marked as deleted old products: " + DbApi.Dbc["UPDATE " + ProductsTable + " SET state=" + (byte)DbApi.ProductState.DELETED + " WHERE crawl_time<@session_start_time"].Execute("@session_start_time", Session.This.StartTime));

                            if (DbApi.Dbc["UPDATE crawlers SET _last_end_time=GETDATE(), _last_session_state=" + (byte)DbApi.SessionState._COMPLETED + ", _next_start_time=DATEADD(ss, run_time_span, _last_start_time) WHERE id=@id"].Execute("@id", CrawlerId) < 1)
                                throw new Exception("Could not update crawlers table.");

                            break;
                        }

                        if (DbApi.Dbc["UPDATE crawlers SET _last_end_time=GETDATE(), _last_session_state=" + (byte)DbApi.SessionState._ERROR + ", _next_start_time=DATEADD(ss, restart_delay_if_broken, _last_start_time) WHERE id=@id"].Execute("@id", CrawlerId) < 1)
                            throw new Exception("Could not update crawlers table.");

                        break;
                    case CrawlerMode.IDLE:
                        break;
                    default:
                        throw new Exception("Unknown mode: " + mode);
                }

            }
            mode = CrawlerMode.IDLE;
        }
        
        public static void SaveProduct(string id, string url, string data)
        {
            lock (DbApi.Dbc)
            {
                //InitSession();

                if (id == null)
                    throw (new Exception("id cannot be NULL"));

                if (data == null)
                    data = "";

                if (url == null)
                    throw (new Exception("url cannot be NULL"));

                if (DbApi.Dbc["SELECT id FROM " + ProductsTable + " WHERE id=@id"].GetFirstRecord("@id", id) != null)
                {
                    if (DbApi.Dbc["UPDATE " + ProductsTable + " SET data=@data WHERE id=@id"].Execute("@data", data, "@id", id) > 0)
                        DbApi.Dbc["UPDATE " + ProductsTable + " SET crawl_time=GETDATE(), change_time=GETDATE(), state=@state WHERE id=@id"].Execute("@state", DbApi.ProductState.NEW, "@id", id);
                    else
                        DbApi.Dbc["UPDATE " + ProductsTable + " SET crawl_time=GETDATE() WHERE id=@id"].Execute("@id", id);
                }
                else
                {
                    DbApi.Dbc["INSERT INTO " + ProductsTable + " (id, crawl_time, change_time, url, data, state) VALUES (@id, GETDATE(), GETDATE(), @url, @data, @state)"].Execute("@id", id, "@url", url, "@data", data, "@state", DbApi.ProductState.NEW);
                }

                if (DateTime.Now > time_2_update_last_product_time)
                {//it is used because getting MIN() from products table is very slow when the table is large
                    if (DbApi.Dbc["UPDATE crawlers SET _last_product_time=GETDATE() WHERE id=@id"].Execute("@id", CrawlerId) < 1)
                        throw new Exception("Could not update _last_product_time.");
                    time_2_update_last_product_time = DateTime.Now.AddSeconds(UPDATE_LAST_PRODUCT_TIME_SPAN_IN_SECS);
                }
            }
        }
        static DateTime time_2_update_last_product_time = DateTime.Now;
        const int UPDATE_LAST_PRODUCT_TIME_SPAN_IN_SECS = 100;

        static public bool SaveProductAsJson<T>(T product) where T : Product
        {
            if (!product.IsValid())
                return false;
            SaveProduct(product.Id, product.Url, SerializationRoutines.Json.Get(product.GetDeclaredField2Values()));
            return true;
        }

        //static public bool SaveProductAsXml<T>(string id, T product, string url) where T : Product
        //{
        //    product.Prepare();
        //    if (!product.IsValid())
        //        return false;

        //    XmlDocument xd = new XmlDocument();
        //    {
        //        XmlNode xn = xd.CreateNode(XmlNodeType.Element, "Product", null);
        //        xd.AppendChild(xn);
        //        XmlAttribute xa = xd.CreateAttribute("id");
        //        xa.Value = id;
        //        xn.Attributes.Append(xa);
        //        xa = xd.CreateAttribute("url");
        //        xa.Value = url;
        //        xn.Attributes.Append(xa);
        //    }

        //    foreach (string field in product.Fields)
        //    {
        //        XmlNode xn = xd.CreateNode(XmlNodeType.Element, "Field", null);
        //        xd.DocumentElement.AppendChild(xn);
        //        XmlAttribute xa = xd.CreateAttribute("name");
        //        xa.Value = field;
        //        xn.Attributes.Append(xa);
        //        xa = xd.CreateAttribute("value");
        //        xa.Value = (string)product[field];
        //        xn.Attributes.Append(xa);
        //    }

        //    //save_product(id, SerializationRoutines.Xml.Get(product.__GetField2Values()), url);
        //    SaveProduct(id, xd.OuterXml, url);
        //    return true;
        //}
    }
}

