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

namespace Cliver.Bot
{
    public class CrawlerHost
    {
        public enum CrawlerState : byte { ENABLED = 1, DISABLED = 2, DEBUG = 3 }

        public enum CrawlerCommand : byte { EMPTY = 0, STOP = 1, RESTART = 2, FORCE = 3 }

        public enum SessionState : byte { STARTED = 1, _COMPLETED = 25, COMPLETED = 2, _ERROR = 35, ERROR = 3, BROKEN = 4, KILLED = 5 }

        public enum ProductState : byte
        {
            NEW = 1,
            //PARSED = 2,
            //INVALID = 3,
            DELETED = 4
        }

        public enum CrawlerMode : byte { IDLE, PRODUCTION }

        static CrawlerHost()
        {
            CrawlerId = Log.ProcessName;
            lock (CrawlerId)
            {
                try
                {
                    dbc = DbConnection.Create();

                    if (dbc.Get("SELECT * FROM sysobjects WHERE name='crawlers' and xtype='U')").GetFirstRecord() == null)
                    {
                        if (LogMessage.AskYesNo("Crawlers table does not exist in the database " + dbc.Database + ". Do you want to create it?", true))
                            dbc.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='crawlers' and xtype='U') 
CREATE TABLE crawlers
(id nvarchar(50) PRIMARY KEY,
state tinyint NOT NULL,
site nvarchar(50) NOT NULL,
command tinyint NOT NULL,
run_time_span int NOT NULL,
crawl_product_timeout int NOT NULL,
yield_product_timeout int NOT NULL DEFAULT 259200
admin_emails nvarchar(300) NOT NULL,
comment nvarchar(1000),
restart_delay_if_broken int NOT NULL,
_last_session_state tinyint,
_next_start_time datetime NOT NULL,
_session_start_time datetime,
_last_start_time datetime,
_last_end_time datetime,
_last_process_id int,
_last_log nvarchar(500),
_archive ntext,
_products_table nvarchar(60) NOT NULL,
_last_product_time datetime)"//add unique key for _products_table !
                        ).Execute();
                    }

                    Record r = dbc.Get("SELECT _products_table FROM crawlers WHERE id=@id").GetFirstRecord("@id", CrawlerId);
                    if (r == null)
                        LogMessage.Exit("Crawler id '" + CrawlerId + "' does not exist in [crawlers] table");

                    string products_table = (string)r["_products_table"];
                    if (string.IsNullOrWhiteSpace(products_table) || !Regex.IsMatch(products_table, "^products_", RegexOptions.Compiled))
                    {
                        products_table = Regex.Replace(Log.ProcessName, @"\.vshost$", "", RegexOptions.Compiled | RegexOptions.Singleline);
                        products_table = Regex.Replace(products_table, @"[^a-z\d]", "_", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                        products_table = "products_" + products_table;

                        string crawler_id = (string)dbc.Get("SELECT id FROM crawlers WHERE _products_table=@products_table").GetSingleValue("@products_table", products_table);
                        if (crawler_id != null)
                            LogMessage.Exit("Products table '" + products_table + "' already is owned by crawler id '" + crawler_id + "'");
                        if (dbc.Get("UPDATE crawlers SET _products_table=@products_table WHERE id=@id").Execute("@products_table", products_table, "@id", CrawlerId) < 1)
                            throw new Exception("Could not update crawlers table.");
                    }
                    ProductsTable = products_table;

                    dbc.Get(
        @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='" + ProductsTable + @"' and xtype='U') 
CREATE TABLE " + ProductsTable + @"
(id nvarchar(256) PRIMARY KEY,	
crawl_time datetime NOT NULL,	
change_time datetime NOT NULL,	
url nvarchar(512) NOT NULL,	
data ntext NOT NULL,
state tinyint NOT NULL)"
                        ).Execute();

                    Session.Closing += session_Closing;
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
            }
        }

        static DbConnection dbc;
        readonly static public string CrawlerId;
        readonly static public string ProductsTable;

        //public static CrawlerHost This
        //{
        //    get
        //    {
        //        if (This_ == null)
        //            This_ = new CrawlerHost();
        //        return This_;
        //    }
        //}
        //static CrawlerHost This_;

        /// <summary>
        /// It should be invoked in ACustomSession:Init() to perform static constructor
        /// </summary>
        static public void InitSession()
        {
            lock (CrawlerId)
            {
                try
                {
                    if (mode != CrawlerMode.IDLE)
                        return;
                    mode = CrawlerMode.PRODUCTION;

                    Record r = dbc.Get("SELECT * FROM crawlers WHERE id=@id").GetFirstRecord("@id", CrawlerId);
                    if (r == null)
                        LogMessage.Exit("Crawler id '" + CrawlerId + "' does not exist in [crawlers] table");

                    string archive = "session_start_time="
                        + (r["_session_start_time"] != null ? ((DateTime)r["_session_start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "")
                        + " start_time=" + (r["_last_start_time"] != null ? ((DateTime)r["_last_start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "")
                        + " end_time=" + (r["_last_end_time"] != null ? ((DateTime)r["_last_end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "")
                        + " state=" + (r["_last_session_state"] != null ? ((SessionState)r["_last_session_state"]).ToString() : "")
                        + " log=" + r["_last_log"] + "\n" + r["_archive"];
                    const int MAX_ARCHIVE_LENGTH = 10000;
                    archive = archive.Substring(0, archive.Length < MAX_ARCHIVE_LENGTH ? archive.Length : MAX_ARCHIVE_LENGTH);
                    if (dbc.Get("UPDATE crawlers SET _session_start_time=@session_start_time, _last_process_id=@process_id, _last_start_time=GETDATE(), _last_end_time=NULL, _last_session_state=" + (byte)SessionState.STARTED + ", _last_log=@Log, _archive=@archive WHERE id=@id").Execute(
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
            lock (CrawlerId)
            {
                switch (mode)
                {
                    case CrawlerMode.PRODUCTION:
                        if (completed)
                        {
                            Log.Main.Inform("Deleted marked old products: " + dbc["DELETE FROM " + ProductsTable + " WHERE state=" + (byte)ProductState.DELETED].Execute());
                            Log.Main.Inform("Marked as deleted old products: " + dbc["UPDATE " + ProductsTable + " SET state=" + (byte)ProductState.DELETED + " WHERE crawl_time<@session_start_time"].Execute("@session_start_time", Session.This.StartTime));

                            if (dbc["UPDATE crawlers SET _last_end_time=GETDATE(), _last_session_state=" + (byte)SessionState._COMPLETED + ", _next_start_time=DATEADD(ss, run_time_span, _last_start_time) WHERE id=@id"].Execute("@id", CrawlerId) < 1)
                                throw new Exception("Could not update crawlers table.");

                            break;
                        }

                        if (dbc["UPDATE crawlers SET _last_end_time=GETDATE(), _last_session_state=" + (byte)SessionState._ERROR + ", _next_start_time=DATEADD(ss, restart_delay_if_broken, _last_start_time) WHERE id=@id"].Execute("@id", CrawlerId) < 1)
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
        
        public static void SaveProduct(string id, string data, string url)
        {
            InitSession();

            if (id == null)
                throw (new Exception("id cannot be NULL"));

            if (data == null)
                data = "";

            if (url == null)
                throw (new Exception("url cannot be NULL"));

            if (dbc["SELECT id FROM " + ProductsTable + " WHERE id=@id"].GetFirstRecord("@id", id) != null)
            {
                if (dbc["UPDATE " + ProductsTable + " SET data=@data WHERE id=@id"].Execute("@data", data, "@id", id) > 0)
                    dbc["UPDATE " + ProductsTable + " SET crawl_time=GETDATE(), change_time=GETDATE(), state=@state WHERE id=@id"].Execute("@state", ProductState.NEW, "@id", id);
                else
                    dbc["UPDATE " + ProductsTable + " SET crawl_time=GETDATE() WHERE id=@id"].Execute("@id", id);
            }
            else
            {
                dbc["INSERT INTO " + ProductsTable + " (id, crawl_time, change_time, url, data, state) VALUES (@id, GETDATE(), GETDATE(), @url, @data, @state)"].Execute("@id", id, "@url", url, "@data", data, "@state", ProductState.NEW);
            }

            if (DateTime.Now > time_2_update_last_product_time)
            {//it is used because getting MIN() from products table is very slow when the table is large
                if (dbc["UPDATE crawlers SET _last_product_time=GETDATE() WHERE id=@id"].Execute("@id", CrawlerId) < 1)
                    throw new Exception("Could not update _last_product_time.");
                time_2_update_last_product_time = DateTime.Now.AddSeconds(UPDATE_LAST_PRODUCT_TIME_SPAN_IN_SECS);
            }
        }
        static DateTime time_2_update_last_product_time = DateTime.Now;
        const int UPDATE_LAST_PRODUCT_TIME_SPAN_IN_SECS = 100;

        static public bool SaveProductAsJson<T>(string id, T product, string url) where T : Product
        {
            if (!product.IsValid())
                return false;
            SaveProduct(id, SerializationRoutines.Json.Get(product.__GetField2Values()), url);
            return true;
        }

        static public bool SaveProductAsXml<T>(string id, T product, string url) where T : Product
        {
            if (!product.IsValid())
                return false;

            XmlDocument xd = new XmlDocument();
            {
                XmlNode xn = xd.CreateNode(XmlNodeType.Element, "Product", null);
                xd.AppendChild(xn);
                XmlAttribute xa = xd.CreateAttribute("id");
                xa.Value = id;
                xn.Attributes.Append(xa);
                xa = xd.CreateAttribute("url");
                xa.Value = url;
                xn.Attributes.Append(xa);
            }

            foreach (string field in product.Fields)
            {
                XmlNode xn = xd.CreateNode(XmlNodeType.Element, "Field", null);
                xd.DocumentElement.AppendChild(xn);
                XmlAttribute xa = xd.CreateAttribute("name");
                xa.Value = field;
                xn.Attributes.Append(xa);
                xa = xd.CreateAttribute("value");
                xa.Value = (string)product[field];
                xn.Attributes.Append(xa);
            }

            //save_product(id, SerializationRoutines.Xml.Get(product.__GetField2Values()), url);
            SaveProduct(id, xd.OuterXml, url);
            return true;
        }
    }
}

