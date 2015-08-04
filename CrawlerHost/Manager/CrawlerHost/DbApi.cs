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
        static DbApi()
        {
            ThreadLog.Writing += ThreadLog_Writing;
        AGAIN:
            try
            {
                Connection = DbConnection.Create(DbApi.ConnectionString);
                create_tables();
            }
            catch (Exception e)
            {
                if (ProgramRoutines.IsWebContext)
                    throw e;

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

            Assembly ea = Assembly.GetEntryAssembly();
            if (ea != null)
                entry_assembly_name = Regex.Replace(Assembly.GetEntryAssembly().FullName, @"\,.*", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        static public readonly DbConnection Connection;
        
        //static public Db.CrawlerHostDataContext Context
        //{
        //    get
        //    {
        //        if (_Context == null)
        //            _Context = new Db.CrawlerHostDataContext(DbApi.ConnectionString);
        //        return _Context;
        //    }
        //}
        //static Db.CrawlerHostDataContext _Context;

        static void create_tables()
        {
            lock (Connection)
            {		
                Connection.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Crawlers' and xtype='U') 
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

                Connection.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Messages' and xtype='U')
CREATE TABLE [dbo].[Messages] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [Source]  NVARCHAR (100) NOT NULL,
    [Type]    INT            NOT NULL,
    [Value]   NVARCHAR (MAX) NOT NULL,
    [Time]    DATETIME       NOT NULL,
    [Details] NVARCHAR (MAX) NOT NULL,
    [Mark] INT NULL, 
    PRIMARY KEY NONCLUSTERED ([Id] ASC)
);
"
                    ).Execute();
            }

            Connection.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Services' and xtype='U')
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

        public enum MessageMark
        {
            EMPTY = 0,
            READ = 1,
            IMPORTANT = 2
        }
        
        static void ThreadLog_Writing(Log.MessageType type, string message)
        {
            write2Messages(type, message);
        }

        static void write2Messages(Log.MessageType type, string message)
        {
            lock (Connection)
            {
                if (type == Log.MessageType.LOG)
                    return;

                System.Diagnostics.StackTrace st = new StackTrace(true);
                StackFrame sf;
                string n = null;
                Type dt = null;
                for (int i = 1; ; i++)
                {
                    sf = st.GetFrame(i);
                    if (sf == null)
                        break;
                    MethodBase mb = sf.GetMethod();
                    dt = mb.DeclaringType;
                    n = mb.Name;
                    if (n != "Message" || dt != typeof(DbApi))
                        break;
                }
                string details = dt.ToString() + "::" + n + " \nfile: " + sf.GetFileName() + " \nline: " + sf.GetFileLineNumber().ToString();
                //}
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
                Connection["INSERT INTO Messages (Type,Source,Value,Time,Details) VALUES(@Type,@Source,@Value,GETDATE(),@Details)"].Execute("@Type", (int)type, "@Source", entry_assembly_name, "@Value", message, "@Details", details);
            }
        }
        static readonly string entry_assembly_name;

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

