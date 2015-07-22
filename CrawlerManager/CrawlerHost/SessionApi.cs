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
        public enum CrawlerMode : int { IDLE, PRODUCTION }
        
        static SessionApi()
        {
            lock (DbApi.Connection)
            { 
                try
                {        
                    CrawlerId = Log.ProcessName;

                    Record r = DbApi.Connection.Get("SELECT _ProductsTable FROM Crawlers WHERE Id=@Id").GetFirstRecord("@Id", CrawlerId);
                    if (r == null)
                        LogMessage.Exit("Crawler id '" + CrawlerId + "' does not exist in [Crawlers] table.");

                    ProductsTable = DbApi.CreateProductsTableForCrawler(CrawlerId);

                    Session.Closing += session_Closing;

                    string archive = "session_start_time:"
                     + (r["_SessionStartTime"] != null ? ((DateTime)r["_SessionStartTime"]).ToString("yyyy-MM-dd HH:mm:ss") : "")
                     + " start_time:" + (r["_LastStartTime"] != null ? ((DateTime)r["_LastStartTime"]).ToString("yyyy-MM-dd HH:mm:ss") : "")
                     + " end_time:" + (r["_LastEndTime"] != null ? ((DateTime)r["_LastEndTime"]).ToString("yyyy-MM-dd HH:mm:ss") : "")
                     + " state:" + (r["_LastSessionState"] != null ? ((DbApi.SessionState)r["_LastSessionState"]).ToString() : "")
                     + " log:" + r["_LastLog"] + "\n" + r["_Archive"];
                    const int MAX_ARCHIVE_LENGTH = 10000;
                    archive = archive.Substring(0, archive.Length < MAX_ARCHIVE_LENGTH ? archive.Length : MAX_ARCHIVE_LENGTH);
                    if (DbApi.Connection.Get("UPDATE Crawlers SET _SessionStartTime=@SessionStartTime, _LastProcessId=@ProcessId, _LastStartTime=GETDATE(), _LastEndTime=NULL, _LastSessionState=" + (int)DbApi.SessionState.STARTED + ", _LastLog=@Log, _Archive=@Archive WHERE Id=@Id").Execute(
                        "@SessionStartTime", Session.This.StartTime, "@ProcessId", Process.GetCurrentProcess().Id, "@Log", Log.SessionDir, "@Archive", archive, "@Id", CrawlerId) < 1
                        )
                        throw new Exception("Could not update Crawlers table.");

                    DbApi.Message(DbApi.MessageType.INFORM, "STARTED\r\nCommand line: " + string.Join("|", Environment.GetCommandLineArgs()) + "\nRunning as:" + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                }
                catch (Exception e)
                {
                    LogMessage.Exit(e);
                }
            }
        }

        readonly static public string CrawlerId;
        readonly static public string ProductsTable;

        public static void Initialize()
        {
            //to force static constructor
        }

        static CrawlerMode mode;

        static void session_Closing()
        {
            stop(!Session.This.IsUnprocessedInputItem && !Session.This.IsItemToRestore);
        }

        internal static void stop(bool completed)
        {            
            lock (DbApi.Connection)
            { 
                switch (mode)
                {
                    case CrawlerMode.PRODUCTION:
                        if (completed)
                        {
                            Log.Main.Inform("Deleted marked old products: " + DbApi.Connection["DELETE FROM " + ProductsTable + " WHERE State=" + DbApi.ProductState.DELETED].Execute());
                            Log.Main.Inform("Marked as deleted old products: " + DbApi.Connection["UPDATE " + ProductsTable + " SET State=" + DbApi.ProductState.DELETED + " WHERE CrawlTime<@session_start_time"].Execute("@session_start_time", Session.This.StartTime));

                            if (DbApi.Connection["UPDATE Crawlers SET _LastEndTime=GETDATE(), _LastSessionState=" + (int)DbApi.SessionState._COMPLETED + ", _NextStartTime=DATEADD(ss, RunTimeSpan, _LastStartTime) WHERE Id=@Id"].Execute("@Id", CrawlerId) < 1)
                                throw new Exception("Could not update Crawlers table.");

                            DbApi.Message(DbApi.MessageType.INFORM, "COMPLETED");
                            break;
                        }

                        if (DbApi.Connection["UPDATE Crawlers SET _LastEndTime=GETDATE(), _LastSessionState=" + (int)DbApi.SessionState._ERROR + ", _NextStartTime=DATEADD(ss, RestartDelayIfBroken, _LastStartTime) WHERE Id=@Id"].Execute("@Id", CrawlerId) < 1)
                            throw new Exception("Could not update Crawlers table.");

                            DbApi.Message(DbApi.MessageType.INFORM, "QUITED");
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
            lock (DbApi.Connection)
            {
                if (id == null)
                    throw (new Exception("id cannot be NULL"));

                if (data == null)
                    data = "";

                if (url == null)
                    throw (new Exception("url cannot be NULL"));

                if (DbApi.Connection["SELECT Id FROM " + ProductsTable + " WHERE Id=@Id"].GetFirstRecord("@Id", id) != null)
                {
                    if (DbApi.Connection["UPDATE " + ProductsTable + " SET Data=@Data WHERE Id=@Id"].Execute("@Data", data, "@Id", id) > 0)
                        DbApi.Connection["UPDATE " + ProductsTable + " SET CrawlTime=GETDATE(), ChangeTime=GETDATE(), State=@State WHERE Id=@Id"].Execute("@State", DbApi.ProductState.NEW, "@Id", id);
                    else
                        DbApi.Connection["UPDATE " + ProductsTable + " SET CrawlTime=GETDATE() WHERE Id=@Id"].Execute("@Id", id);
                }
                else
                {
                    DbApi.Connection["INSERT INTO " + ProductsTable + " (Id, CrawlTime, ChangeTime, Url, Data, State) VALUES (@Id, GETDATE(), GETDATE(), @Url, @Data, @State)"].Execute("@Id", id, "@Url", url, "@Data", data, "@State", DbApi.ProductState.NEW);
                }

                if (DateTime.Now > time_2_update_last_product_time)
                {//it is used because getting MIN() from products table is very slow when the table is large
                    if (DbApi.Connection["UPDATE Crawlers SET _LastProductTime=GETDATE() WHERE Id=@Id"].Execute("@Id", CrawlerId) < 1)
                        throw new Exception("Could not update _LastProductTime.");
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
        //        XmlAttribute xa = xd.CreateAttribute("Id");
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

