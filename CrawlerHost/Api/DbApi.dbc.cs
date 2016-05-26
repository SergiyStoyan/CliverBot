using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
//using System.Data.Odbc;
//using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;
using Cliver.Bot;

namespace Cliver.CrawlerHost
{
    public class DbApi : MsSqlConnection
    {
        static public DbApi Create()
        {
            return new DbApi(GetConnectionString());
        }

        DbApi(string connection_string)
            : base(connection_string)
        {
            Assembly ea = Assembly.GetEntryAssembly();
            if (ea != null)//can be null if web context
                entry_assembly_name = Regex.Replace(Assembly.GetEntryAssembly().FullName, @"\,.*", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            try
            {
                if (!ProgramRoutines.IsWebContext)
                    Log.Main.Write("DbApi ConnectionString: " + ConnectionString);
                create_tables();
            }
            catch (Exception e)
            {
                string m = "The app could not connect the database with string:\r\n" + ConnectionString + "\r\nBe sure " + Cliver.CrawlerHost.Api.CrawlerHost_CONGIG_FILE_NAME + " file exists and is correct.\r\n\r\n" + e.Message;
                if (!ProgramRoutines.IsWebContext)
                    LogMessage.Exit(m);
                else
                    throw new Exception(m);
            } 
            ThreadLog.Writing += ThreadLog_Writing;  
        }

        public static string GetConnectionString()
        {
            return Cliver.CrawlerHost.Api.GetConnectionString(DATABASE_CONNECTION_STRING_NAME);
        }
        public const string DATABASE_CONNECTION_STRING_NAME = "CliverCrawlerHostConnectionString";

        void create_tables()
        {
            lock (this)
            {
                this.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Crawlers' and xtype='U') 
CREATE TABLE [dbo].[Crawlers] (
    [Id]                   NVARCHAR (50)   NOT NULL,/*crawler process/assembly name*/
    [State]                INT             DEFAULT ((2)) NOT NULL,
    [Site]                 NVARCHAR (50)   NOT NULL,
    [Command]              INT             DEFAULT ((0)) NOT NULL,
    [RunTimeSpan]          INT             DEFAULT ((86400)) NOT NULL,/*in seconds*/
    [CrawlProductTimeout]  INT             DEFAULT ((600)) NOT NULL,/*in seconds. If no product was crawled for the last specified number of seconds, an error is arisen.*/
    [YieldProductTimeout]  INT             DEFAULT ((259200)) NOT NULL,/*in seconds. If no new product was added for the last specified number of seconds, an error is arisen.*/
    [AdminEmails]          NVARCHAR (300)  NOT NULL,/*emails going by , or new line*/
    [Comment]              NVARCHAR (1000) DEFAULT (NULL) NULL,
    [RestartDelayIfBroken] INT             DEFAULT ((1800)) NOT NULL,/*in seconds*/
    [_SessionStartTime]    DATETIME        DEFAULT (NULL) NULL,
    [_LastSessionState]    INT             DEFAULT (NULL) NULL,
    [_NextStartTime]       DATETIME        DEFAULT ((0)) NOT NULL,
    [_LastStartTime]       DATETIME        DEFAULT (NULL) NULL,
    [_LastEndTime]         DATETIME        DEFAULT (NULL) NULL,
    [_LastProcessId]       INT             DEFAULT (NULL) NULL,
    [_LastLog]             NVARCHAR (500)  DEFAULT (NULL) NULL,
    [_Archive]             NTEXT           DEFAULT (NULL) NULL,
    [_ProductsTable]       NVARCHAR (100)  DEFAULT ('') NOT NULL,
    [_LastProductTime]     DATETIME        DEFAULT (NULL) NULL,/*used to monitor crawler activity by manager*/
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
"
                    ).Execute();

                this.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Messages' and xtype='U')
CREATE TABLE [dbo].[Messages] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [Source]  NVARCHAR (100) NOT NULL,
    [Type]    INT            NOT NULL,
    [Value]   NVARCHAR (MAX) NOT NULL,
    [Time]    DATETIME       NOT NULL,
    [Details] NVARCHAR (MAX) NULL,
    [Mark] INT NULL, 
    PRIMARY KEY NONCLUSTERED ([Id] ASC)
);
"
                    ).Execute();

                this.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Services' and xtype='U')
CREATE TABLE [dbo].[Services] (
    [Id]                   NVARCHAR (50)  NOT NULL,/*service process/assembly name*/
    [State]                INT            DEFAULT ((2)) NOT NULL,
    [ExeFolder]            NVARCHAR (MAX) NULL,
    [Command]              INT            DEFAULT ((0)) NOT NULL,
    [RunTimeSpan]          INT            DEFAULT ((86400)) NOT NULL,/*in seconds*/
    [RunTimeout]           INT            DEFAULT ((1800)) NOT NULL,/*in seconds*/
    [AdminEmails]          NVARCHAR (MAX) NOT NULL,/*emails going by , or new line*/
    [Comment]              NVARCHAR (MAX) DEFAULT (NULL) NULL,
    [RestartDelayIfBroken] INT            DEFAULT ((86400)) NOT NULL,/*in seconds*/
    [_LastSessionState]    INT            DEFAULT (NULL) NULL,
    [_NextStartTime]       DATETIME       DEFAULT ((0)) NOT NULL,
    [_LastStartTime]       DATETIME       DEFAULT (NULL) NULL,
    [_LastEndTime]         DATETIME       DEFAULT (NULL) NULL,
    [_LastProcessId]       INT            DEFAULT (NULL) NULL,
    [_LastLog]             NVARCHAR (MAX) DEFAULT (NULL) NULL,
    [_Archive]             NTEXT          DEFAULT (NULL) NULL,
    [_Data]                NVARCHAR (MAX) DEFAULT (NULL) NULL,/*json data used by service*/
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
"
                    ).Execute();
            }
        }

        public string CreateProductsTableForCrawler(string crawler_id)
        {
            lock (this)
            {
                string products_table = Regex.Replace(Log.EntryAssemblyName, @"\.vshost$", "", RegexOptions.Compiled | RegexOptions.Singleline);
                products_table = Regex.Replace(products_table, @"[^a-z\d]", "_", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                products_table = "Products_" + products_table;

                string crawler_id2 = (string)this.Get("SELECT Id FROM Crawlers WHERE _ProductsTable=@products_table").GetSingleValue("@products_table", products_table);
                if (crawler_id2 != null && crawler_id2 != crawler_id)
                    LogMessage.Exit("Products table '" + products_table + "' is already owned by crawler '" + crawler_id2 + "'");
                if (this.Get("UPDATE Crawlers SET _ProductsTable=@products_table WHERE Id=@Id").Execute("@products_table", products_table, "@Id", crawler_id) < 1)
                    throw new Exception("Could not update Crawlers table.");

                this.Get(
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

        public enum MessageMark
        {
            EMPTY = 0,
            READ = 1,
            IMPORTANT = 2
        }
        
        void ThreadLog_Writing(Log.MessageType type, string message, string details)
        {
            write2Messages(type, message, details);
        }

        void write2Messages(Log.MessageType type, string message, string details)
        {
            lock (this)
            {
                if (type == Log.MessageType.LOG)
                    return;
                this["INSERT INTO Messages (Type,Source,Value,Time,Details) VALUES(@Type,@Source,@Value,GETDATE(),@Details)"].Execute("@Type", (int)type, "@Source", entry_assembly_name, "@Value", message, "@Details", details);
            }
        }
        readonly string entry_assembly_name;
    }
}

