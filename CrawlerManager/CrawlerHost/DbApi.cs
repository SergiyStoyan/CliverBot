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
using Settings = Cliver.CrawlerHost.Properties.Settings;
using Cliver.Bot;

namespace Cliver.CrawlerHost
{
    public class DbApi
    {
        public enum CrawlerState : byte { ENABLED = 1, DISABLED = 2, DEBUG = 3 }

        public enum CrawlerCommand : byte { EMPTY = 0, STOP = 1, RESTART = 2, FORCE = 3 }

        public enum SessionState : byte { STARTED = 1, _COMPLETED = 25, COMPLETED = 2, _ERROR = 35, ERROR = 3, BROKEN = 4, KILLED = 5 }

        //public class ProductState
        //{
        //    public const byte NEW = 1;
        //    //PARSED = 2,
        //    //INVALID = 3,
        //    public const byte DELETED = 4;
        //}
        
        //public class ProductState2 :ProductState
        //{
        //    public const byte REPLICATED = 5;
        //    //public const byte INVALID = 6;
        //}

        public enum ProductState : byte
        {
            NEW = 1,
            //    //PARSED = 2,
            //    //INVALID = 3,
            DELETED = 4
        }

        public enum CrawlerMode : byte { IDLE, PRODUCTION }
        
        static DbApi()
        {
            AGAIN:
            try
            {
                Connection = DbConnection.Create(DbApi.ConnectionString);
                create_crawler_tables();
            }
            catch(Exception e)
            {
                if (LogMessage.ShowStumblingMessages)
                {
                    LogMessage.Error("The app could not connect the database. Please create an empty database or locate an existing one and save the respective connection string in settings.");
                    SettingsForm f = new SettingsForm();
                    f.ShowDialog();
                    goto AGAIN;
                }
                LogMessage.Exit(e);
            }
        }
        static public readonly DbConnection Connection;

        static void create_crawler_tables()
        {
            lock (Connection)
            {
                //var scsb = new SqlConnectionStringBuilder(Settings.Default.DbConnectionString);
                //var database = scsb.InitialCatalog;
                //if(database == null)
                //{
                //   //Match m = Regex.Match(scsb.AttachDBFilename, @"[\\\/](?'Name'.*)\.mdf\s*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                //   //if (!m.Success)
                //   //    throw new TerminatingException("Cannot parser database name.");
                //   //database = m.Groups["Name"].Value;
                //   database = scsb.AttachDBFilename;
                //}
                //if(null == Connection.Get(string.Format("select * from master.dbo.sysdatabases where name='{0}'", database)).GetSingleValue())
                //    Connection.Get(string.Format("CREATE DATABASE {0}", database)).Execute();
              

                //if (LogMessage.AskYesNo("Crawlers table does not exist in the database " + Connection.Database + ". Do you want to create it?", true))
                //CREATE TABLE IF NOT EXISTS `crawlers` (
                //  `id` varchar(32) NOT NULL,
                //  `state` enum('enabled','disabled','debug') NOT NULL DEFAULT 'debug',
                //  `site` varchar(64) NOT NULL,
                //  `command` enum('stop','restart') DEFAULT NULL COMMENT 'used while debugging/updating crawler',
                //  `run_time_span` int(11) NOT NULL DEFAULT '86400' COMMENT 'in seconds',
                //  `crawl_product_timeout` int(11) NOT NULL DEFAULT '600' COMMENT 'if no product was crawled for the last specified number of seconds, an error is arisen',
                //  `yield_product_timeout` int(11) NOT NULL DEFAULT '259200' COMMENT 'if no new product was added for the last specified number of seconds, an error is arisen',
                //  `admin_emails` varchar(300) NOT NULL COMMENT 'emails going by  '','' or new line',
                //  `comment` varchar(1000) NOT NULL,
                //  `restart_delay_if_broken` int(11) NOT NULL DEFAULT '1800' COMMENT 'in seconds',
                //  `_last_session_state` enum('started','_completed','completed','_error','error','broken','killed','debug_completed') DEFAULT NULL,
                //  `_next_start_time` datetime NOT NULL,
                //  `_last_start_time` datetime NOT NULL,
                //  `_last_end_time` datetime NOT NULL,
                //  `_last_process_id` int(11) NOT NULL,
                //  `_last_log` varchar(500) NOT NULL,
                //  `_archive` text NOT NULL,
                //  `_last_product_time` datetime NOT NULL COMMENT 'used to monitor crawler activity by manager',
                //  PRIMARY KEY (`id`)
                //) ENGINE=MyISAM DEFAULT CHARSET=latin1; 		
                Connection.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='crawlers' and xtype='U') 
CREATE TABLE crawlers (
    id nvarchar(50) PRIMARY KEY,
    state tinyint DEFAULT (2) NOT NULL,
    site nvarchar(50) NOT NULL,
    command tinyint  DEFAULT (0) NOT NULL,
    run_time_span int DEFAULT (86400) NOT NULL,
    crawl_product_timeout int DEFAULT (600) NOT NULL,
    yield_product_timeout int DEFAULT (259200) NOT NULL, 
    admin_emails nvarchar(300) NOT NULL,
    comment nvarchar(1000),
    restart_delay_if_broken int DEFAULT (1800) NOT NULL,
    _session_start_time datetime,
    _last_session_state tinyint,
    _next_start_time datetime DEFAULT (0) NOT NULL,
    _last_start_time datetime,
    _last_end_time datetime,
    _last_process_id int,
    _last_log nvarchar(500),
    _archive ntext,
    _products_table nvarchar(60) DEFAULT ('') NOT NULL,
    _last_product_time datetime
)"
                    ).Execute();

                Connection.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='messages' and xtype='U')
CREATE TABLE [dbo].[messages] (
    [id]         INT             IDENTITY (1, 1) NOT NULL,
    [crawler_id] NVARCHAR (50)   NOT NULL,
    [type]       TINYINT         NOT NULL,
    [message]    NVARCHAR (MAX) NOT NULL,
    [time]       DATETIME        NOT NULL,
    [source]     NVARCHAR(MAX) NOT NULL, 
    PRIMARY KEY NONCLUSTERED ([id] ASC),
    CONSTRAINT [FK_messages_To_crawlers] FOREIGN KEY ([crawler_id]) REFERENCES [dbo].[crawlers] ([id])
);"
                    ).Execute();
            }
        }

        internal static string CreateProductsTableForCrawler(string crawler_id)
        {
            lock (Connection)
            {
                string products_table = Regex.Replace(Log.ProcessName, @"\.vshost$", "", RegexOptions.Compiled | RegexOptions.Singleline);
                products_table = Regex.Replace(products_table, @"[^a-z\d]", "_", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                products_table = "products_" + products_table;

                string crawler_id2 = (string)Connection.Get("SELECT id FROM crawlers WHERE _products_table=@products_table").GetSingleValue("@products_table", products_table);
                if (crawler_id2 != null && crawler_id2 != crawler_id)
                    LogMessage.Exit("Products table '" + products_table + "' is already owned by crawler '" + crawler_id2 + "'");
                if (Connection.Get("UPDATE crawlers SET _products_table=@products_table WHERE id=@id").Execute("@products_table", products_table, "@id", crawler_id) < 1)
                    throw new Exception("Could not update crawlers table.");

                Connection.Get(
    @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='" + products_table + @"' and xtype='U') 
CREATE TABLE " + products_table + @"
(id nvarchar(256) PRIMARY KEY NONCLUSTERED,	
crawl_time datetime NOT NULL,	
change_time datetime NOT NULL,	
url nvarchar(512) NOT NULL,	
data ntext NOT NULL,
state tinyint NOT NULL)"
                    ).Execute();

                return products_table;
            }
        }

        public enum MessageType
        {
            INFORM = 1,
            WARNING = 2,
            ERROR = 3
        }
        
        static public void Message(MessageType type, string crawler_id, string message, string source = null)
        {
            if (source == null)
            {
                System.Diagnostics.StackTrace st = new StackTrace(true);
                StackFrame sf = st.GetFrame(1);
                var m = sf.GetMethod();
                source = m.DeclaringType.ToString() + "\nmethod: " + m.Name + "\nfile: " + sf.GetFileName() + "\nline: " + sf.GetFileLineNumber().ToString();
            }
            if (1 > Connection["INSERT INTO messages (crawler_id,type,message,time,source) VALUES (@crawler_id,@type,CAST(@message AS nvarchar(MAX)),GETDATE(),CAST(@source AS nvarchar(MAX)))"].Execute("@crawler_id", crawler_id, "@type", (int)type, "@message", message, "@source", source))
                throw new Exception("Cannot add to 'crawler_messages': " + message);
        }

        public static string ConnectionString
        {
            get
            {
                return RegistryRoutines.GetString(DbConnectionString_registry_name);
            }
            internal set 
            {
                RegistryRoutines.SetValue(DbConnectionString_registry_name, value);
            }
        }
        const string DbConnectionString_registry_name = @"CrawlerHostDbConnectionString";
    }
}

