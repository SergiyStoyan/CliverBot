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
        //public class CrawlerState : Enum<int>
        //{
        //    public static readonly CrawlerState ENABLED = new CrawlerState(1);
        //    public static readonly CrawlerState DISABLED = new CrawlerState(2);
        //    public static readonly CrawlerState DEBUG = new CrawlerState(3);

        //    public CrawlerState(int value) : base(value) { }
        //}
        public enum CrawlerState : int { ENABLED = 1, DISABLED = 2, DEBUG = 3 }

        public enum CrawlerCommand : int { EMPTY = 0, STOP = 1, RESTART = 2, FORCE = 3 }

        public enum SessionState : int { STARTED = 1, _COMPLETED = 25, COMPLETED = 2, _ERROR = 35, ERROR = 3, BROKEN = 4, KILLED = 5 }

        //public class ProductState : Enum<int>
        //{
        //    public static readonly ProductState NEW = new ProductState(1);
        //    public static readonly ProductState DELETED = new ProductState(4);

        //    protected ProductState(int value) : base(value) { }
        //}
        public enum ProductState : int { NEW = 1, DELETED = 4 }

        public enum CrawlerMode : int { IDLE, PRODUCTION }
        
        static DbApi()
        {
            AGAIN:
            try
            {
                //Database = new CliverCrawlerHostEntities(DbApi.ConnectionString);
                Connection = DbConnection.Create(DbApi.ConnectionString);
                create_tables();
            }
            catch(Exception e)
            {
                if (!LogMessage.DisableStumblingDialogs)
                {
                    string connection_string = DbApi.ConnectionString;
                    if (string.IsNullOrWhiteSpace(connection_string))
                        connection_string = Properties.Settings.Default.DbConnectionString;
                    string message = e.Message + "\r\n\r\nThe app could not connect the database. Please create an empty database or locate an existing one and save the respective connection string in settings.";
                    DbConnectionSettingsForm f = new DbConnectionSettingsForm(message, connection_string);
                    f.ShowDialog();
                    if (f.ConnectionString == null)
                    {
                        Log.Error(e);
                        LogMessage.Exit("The app cannot work and will exit.");
                    }
                    DbApi.ConnectionString = f.ConnectionString;
                    goto AGAIN;
                }
                LogMessage.Exit(e);
            }
        }
        static public readonly DbConnection Connection;
        //static public readonly CliverCrawlerHostEntities Database = new CliverCrawlerHostEntities();

        static void create_tables()
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
                //CREATE TABLE IF NOT EXISTS `Crawlers` (
                //  `Id` varchar(32) NOT NULL,
                //  `State` enum('enabled','disabled','debug') NOT NULL DEFAULT 'debug',
                //  `Site` varchar(64) NOT NULL,
                //  `Command` enum('stop','restart') DEFAULT NULL COMMENT 'used while debugging/updating crawler',
                //  `RunTimeSpan` int(11) NOT NULL DEFAULT '86400' COMMENT 'in seconds',
                //  `CrawlProductTimeout` int(11) NOT NULL DEFAULT '600' COMMENT 'if no product was crawled for the last specified number of seconds, an error is arisen',
                //  `YieldProductTimeout` int(11) NOT NULL DEFAULT '259200' COMMENT 'if no new product was added for the last specified number of seconds, an error is arisen',
                //  `AdminEmails` varchar(300) NOT NULL COMMENT 'emails going by  '','' or new line',
                //  `Comment` varchar(1000) NOT NULL,
                //  `RestartDelayIfBroken` int(11) NOT NULL DEFAULT '1800' COMMENT 'in seconds',
                //  `_LastSessionState` enum('started','_completed','completed','_error','error','broken','killed','debug_completed') DEFAULT NULL,
                //  `_NextStartTime` datetime NOT NULL,
                //  `_LastStartTime` datetime NOT NULL,
                //  `_LastEndTime` datetime NOT NULL,
                //  `_LastProcessId` int(11) NOT NULL,
                //  `_LastLog` varchar(500) NOT NULL,
                //  `_Archive` text NOT NULL,
                //  `_LastProductTime` datetime NOT NULL COMMENT 'used to monitor crawler activity by manager',
                //  PRIMARY KEY (`Id`)
                //) ENGINE=MyISAM DEFAULT CHARSET=latin1; 		
                Connection.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Crawlers' and xtype='U') 
CREATE TABLE Crawlers (
    Id nvarchar(50) PRIMARY KEY,
    State int DEFAULT (2) NOT NULL,
    Site nvarchar(50) NOT NULL,
    Command int  DEFAULT (0) NOT NULL,
    RunTimeSpan int DEFAULT (86400) NOT NULL,
    CrawlProductTimeout int DEFAULT (600) NOT NULL,
    YieldProductTimeout int DEFAULT (259200) NOT NULL, 
    AdminEmails nvarchar(300) NOT NULL,
    Comment nvarchar(1000),
    RestartDelayIfBroken int DEFAULT (1800) NOT NULL,
    _SessionStartTime datetime,
    _LastSessionState int,
    _NextStartTime datetime DEFAULT (0) NOT NULL,
    _LastStartTime datetime,
    _LastEndTime datetime,
    _LastProcessId int,
    _LastLog nvarchar(500),
    _Archive ntext,
    _ProductsTable nvarchar(100) DEFAULT ('') NOT NULL,
    _LastProductTime datetime
)"
                    ).Execute();

                Connection.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='messages' and xtype='U')
CREATE TABLE [dbo].[Messages] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [CrawlerId] NVARCHAR (50)   NOT NULL,
    [Type]       INT         NOT NULL,
    [Message]    NVARCHAR (MAX) NOT NULL,
    [Time]       DATETIME        NOT NULL,
    [Source]     NVARCHAR(MAX) NOT NULL, 
    PRIMARY KEY NONCLUSTERED ([Id] ASC),
    CONSTRAINT [FK_messages_To_crawlers] FOREIGN KEY ([CrawlerId]) REFERENCES [dbo].[Crawlers] ([Id])
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
                products_table = "Products_" + products_table;

                string crawler_id2 = (string)Connection.Get("SELECT Id FROM Crawlers WHERE _ProductsTable=@products_table").GetSingleValue("@products_table", products_table);
                if (crawler_id2 != null && crawler_id2 != crawler_id)
                    LogMessage.Exit("Products table '" + products_table + "' is already owned by crawler '" + crawler_id2 + "'");
                if (Connection.Get("UPDATE Crawlers SET _ProductsTable=@products_table WHERE Id=@Id").Execute("@products_table", products_table, "@Id", crawler_id) < 1)
                    throw new Exception("Could not update Crawlers table.");

                Connection.Get(
    @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='" + products_table + @"' and xtype='U') 
CREATE TABLE " + products_table + @"
(Id nvarchar(256) PRIMARY KEY NONCLUSTERED,	
CrawlTime datetime NOT NULL,	
ChangeTime datetime NOT NULL,	
Url nvarchar(512) NOT NULL,	
Data ntext NOT NULL,
State tinyint NOT NULL)"
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
            //{
            //    Message m = new CrawlerHost.Message();
            //    m.CrawlerId = crawler_id;
            //    m.Source = source;
            //    m.Time = DateTime.Now;
            //    m.Type = (int)type;
            //    m.Value = message;
            //    Database.Messages.Add(m);
            //}
            //if (1 > Database.SaveChanges())
            //    throw new Exception("Cannot add to 'crawler_messages': " + message);
            Connection["INSERT INTO Messages (Type,CrawlerId,Value,Time,Source) VALUES(@Type,@CrawlerId,@Value, GETDATE(),@Source)"].Execute("@Type", (int)type, "@CrawlerId", crawler_id, "@Value", message, "@Source", source);
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

