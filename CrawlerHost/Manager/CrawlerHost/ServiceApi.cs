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
    public class Service
    {
        public class CommandLineParameters : ProgramRoutines.CommandLineParameters
        {
            public static readonly CommandLineParameters AUTOMATIC = new CommandLineParameters("-automatic");
            public static readonly CommandLineParameters PRODUCTION = new CommandLineParameters("-production");

            public CommandLineParameters(string value) : base(value) { }
        }

        public enum State : int { ENABLED = 1, DISABLED = 2, DEBUG = 3 }

        public enum Command : int { EMPTY = 0, STOP = 1, RESTART = 2, FORCE = 3, DISABLE_AFTER_COMPLETION = 4 }

        public enum SessionState : int { STARTED = 1, _COMPLETED = 25, COMPLETED = 2, _ERROR = 35, ERROR = 3, BROKEN = 4, KILLED = 5 }
    }

    public class ServiceApi
    {
        static ServiceApi()
        {
            lock (DbApi.Connection)
            { 
                try
                {        
                    ServiceId = Log.ProcessName;

                    Record r = DbApi.Connection.Get("SELECT State FROM Services WHERE Id=@Id").GetFirstRecord("@Id", ServiceId);
                    if (r == null)
                        DbApi.Message(DbApi.MessageType.EXIT, "Service id '" + ServiceId + "' does not exist in [Services] table.");

                    if ((Service.State)r["State"] == Service.State.DISABLED)
                        DbApi.Message(DbApi.MessageType.EXIT, "Service id '" + ServiceId + "' is disabled.");
                    
                    string archive = " start_time:" + (r["_LastStartTime"] != null ? ((DateTime)r["_LastStartTime"]).ToString("yyyy-MM-dd HH:mm:ss") : "")
                     + " end_time:" + (r["_LastEndTime"] != null ? ((DateTime)r["_LastEndTime"]).ToString("yyyy-MM-dd HH:mm:ss") : "")
                     + " state:" + (r["_LastSessionState"] != null ? ((Service.SessionState)r["_LastSessionState"]).ToString() : "")
                     + " log:" + r["_LastLog"] + "\n" + r["_Archive"];
                    const int MAX_ARCHIVE_LENGTH = 10000;
                    archive = archive.Substring(0, archive.Length < MAX_ARCHIVE_LENGTH ? archive.Length : MAX_ARCHIVE_LENGTH);
                    if (1 > DbApi.Connection.Get("UPDATE Services SET _LastProcessId=@ProcessId, _LastStartTime=GETDATE(), _LastEndTime=NULL, _LastSessionState=" + (int)Service.SessionState.STARTED + ", _LastLog=@Log, _Archive=@Archive WHERE Id=@Id").Execute(
                        "@ProcessId", Process.GetCurrentProcess().Id, "@Log", Log.WorkDir, "@Archive", archive, "@Id", ServiceId)
                        )
                        throw new Exception("Could not update Services table.");

                    DbApi.Message(DbApi.MessageType.INFORM, "STARTED\r\nCommand line: " + string.Join("|", Environment.GetCommandLineArgs()) + "\nRunning as:" + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                }
                catch (Exception e)
                {
                    Log.Main.Error(e);
                    DbApi.Message(e);
                }
            }
        }

        readonly static public string ServiceId;

        public static void Initialize()
        {
            //to force static constructor
        }

        public static void Complete(bool completed)
        {
            lock (DbApi.Connection)
            {
                if (completed)
                {
                    DbApi.Message(DbApi.MessageType.INFORM, "COMPLETED");
                    if (1 > DbApi.Connection["UPDATE Services SET _LastEndTime=GETDATE(), _LastSessionState=" + (int)Service.SessionState._COMPLETED + ", _NextStartTime=DATEADD(ss, RunTimeSpan, _LastStartTime) WHERE Id=@Id"].Execute("@Id", ServiceId))
                        throw new Exception("Could not update Services table.");
                }
                else
                {
                    DbApi.Message(DbApi.MessageType.ERROR, "UNCOMPLETED");
                    if (1 > DbApi.Connection["UPDATE Services SET _LastEndTime=GETDATE(), _LastSessionState=" + (int)Service.SessionState._ERROR + ", _NextStartTime=DATEADD(ss, RunTimeSpan, _LastStartTime) WHERE Id=@Id"].Execute("@Id", ServiceId))
                        throw new Exception("Could not update Services table.");
                }

                ServiceManager.WaitUntilCheckTime();
            }
        }        
    }
}

