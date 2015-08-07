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
    public abstract class Service
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
        
        protected Service()
        {
            lock (DbApi.Connection)
            {
                try
                {
                    ServiceId = Log.ProcessName;

                    ThreadLog.Exitig += ThreadLog_Exitig;

                    Record r = DbApi.Connection.Get("SELECT State FROM Services WHERE Id=@Id").GetFirstRecord("@Id", ServiceId);
                    if (r == null)
                       LogMessage.Exit("Service id '" + ServiceId + "' does not exist in [Services] table.");

                    if ((Service.State)r["State"] == Service.State.DISABLED)
                        LogMessage.Exit("Service id '" + ServiceId + "' is disabled.");

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
                }
                catch (Exception e)
                {
                    LogMessage.Error(e);
                }
            }
        }

        readonly public string ServiceId;

        public class MessageMark
        {
            public const string STARTED = "STARTED";
            public const string COMPLETED = "COMPLETED";
            public const string ABORTED = "ABORTED";
            public const string ERROR = "THERE_WAS_ERROR";
        }

        void ThreadLog_Exitig(string message)
        {
            LogMessage.Error(MessageMark.ABORTED);
            DbApi.Connection["UPDATE Services SET _LastEndTime=GETDATE(), _LastSessionState=" + (int)Service.SessionState._ERROR + ", _NextStartTime=DATEADD(ss, RunTimeSpan, _LastStartTime) WHERE Id=@Id"].Execute("@Id", ServiceId);
        }

        void complete()
        {
            lock (DbApi.Connection)
            {
                if (ThreadLog.TotalErrorCount > 0)
                {
                    LogMessage.Error2(MessageMark.ERROR);
                    DbApi.Connection["UPDATE Services SET _LastEndTime=GETDATE(), _LastSessionState=" + (int)Service.SessionState._ERROR + ", _NextStartTime=DATEADD(ss, RunTimeSpan, _LastStartTime) WHERE Id=@Id"].Execute("@Id", ServiceId);
                }
                else
                {
                   LogMessage.Inform(MessageMark.COMPLETED);
                    DbApi.Connection["UPDATE Services SET _LastEndTime=GETDATE(), _LastSessionState=" + (int)Service.SessionState._COMPLETED + ", _NextStartTime=DATEADD(ss, RunTimeSpan, _LastStartTime) WHERE Id=@Id"].Execute("@Id", ServiceId);
                }

                ServiceManager.WaitUntilCheckTime();
            }
        }

        abstract protected void Do();

        public static void Run()
        {
            try
            {
                LogMessage.DisableStumblingDialogs = ProgramRoutines.IsParameterSet(CommandLineParameters.AUTOMATIC);
                Log.LOGGING_MODE = Log.LoggingMode.ONLY_LOG;
                LogMessage.Output2Console = true;
                ProcessRoutines.RunSingleProcessOnly();
                
                LogMessage.Inform(MessageMark.STARTED + " \r\nCommand line: " + string.Join("|", Environment.GetCommandLineArgs()) + " \r\nRunning as:" + System.Security.Principal.WindowsIdentity.GetCurrent().Name);

                Assembly service_assembly = Assembly.GetEntryAssembly();
                List<Type> service_types = (from t in service_assembly.GetExportedTypes() where t.IsSubclassOf(typeof(Service)) select t).ToList();
                if (service_types.Count < 1)
                    LogMessage.Exit("Could not find Service implementation in the entry assembly: " + service_assembly.FullName);
                if (service_types.Count > 1)
                    LogMessage.Exit("Found more than one Service implementations in the entry assembly: " + service_assembly.FullName);

                Service service = (Service)Activator.CreateInstance(service_types[0]);
                service.Do();
                service.complete();
            }
            catch(Exception e)
            {
                LogMessage.Exit(e);
            }
        }
    }
}

