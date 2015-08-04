using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.Bot;
using System.Text.RegularExpressions;
using Cliver.CrawlerHost;

namespace Cliver.CrawlerHostWatchDog
{
    public class WatchDog : Service
    {
        override protected void Do()
        {
            //GetReportsIntoTempTable((System.Data.Common.DbConnection)DbApi.Connection.NativeConnection);
        }

        /// <summary>
        /// used by ProductOffice
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static string GetReportsTempTable(System.Data.Common.DbConnection connection)
        {
            string table = "#WatchDogReports";
            DbConnection   dbc = DbConnection.CreateFromNativeConnection(connection);
            List<Report> reports = get_reports(dbc);
            dbc.Get(@"CREATE TABLE " + table + @" (
    [Source]                   NVARCHAR (100)   NOT NULL,
    [SourceType]                NVARCHAR (100)            NOT NULL,
    [MessageType]              NVARCHAR (100)            NOT NULL,
    [Value]          NVARCHAR (300)  NOT NULL,
    [Details]              NVARCHAR (1000) NOT NULL
);").Execute();
            foreach(Report r in reports)
                dbc[@"INSERT INTO #WatchDogReports (Source,SourceType,MessageType,Value,Details) VALUES(@Source,@SourceType,@MessageType,@Value,@Details);"].Execute("@Source", r.Source, "@SourceType", r.SourceType.ToString(), "@MessageType", r.MessageType.ToString(), "@Value", r.Value, "@Details", r.Details);

            return table;
        }

        static List<Report> get_reports(DbConnection dbc)
        {            
            List<Report> reports = new List<Report>();

            foreach (Record crawler in dbc.Get("SELECT * FROM Crawlers WHERE State<>" + (int)Crawler.State.DISABLED).GetRecordset())
            {
                Report report = new Report();
                reports.Add(report);
                report.Source = (string)crawler["Id"];
                report.SourceType = ReportSourceType.CRAWLER;
                DateTime earliest_start_time = DateTime.Now.AddSeconds(-(int)crawler["RunTimeSpan"]);
                Record start_message = dbc["SELECT * FROM Messages WHERE Source=@Source AND Value LIKE '" + CrawlerApi.MessageMark.STARTED + "%' ORDER BY Time DESC"].GetFirstRecord("@Source", crawler["Id"]);
                Crawler.SessionState state =  (Crawler.SessionState)(int)dbc["SELECT _LastSessionState FROM Crawlers WHERE Id=@Id"].GetSingleValue("@Id", crawler["Id"]);
                if (start_message == null || (DateTime)start_message["Time"] < earliest_start_time)
                {
                    if (state == Crawler.SessionState.STARTED)
                    {
                        report.MessageType = Log.MessageType.WARNING;
                        report.Value = "LONG WORK";
                        report.Details = "Works longer than its RunTimeSpane";
                        continue;
                    }
                    report.MessageType = Log.MessageType.ERROR;
                    report.Value = "NO START";
                    report.Details = "Not started within its RunTimeSpan";
                    continue;
                }
                Record end_message = dbc["SELECT * FROM Messages WHERE Source=@Source AND (Value LIKE '" + CrawlerApi.MessageMark.ABORTED + "%' OR Value LIKE '" + CrawlerApi.MessageMark.UNCOMPLETED + "%' OR Value LIKE '" + CrawlerApi.MessageMark.COMPLETED + "%') ORDER BY Time DESC"].GetFirstRecord("@Source", crawler["Id"]);
                if (end_message == null)
                {
                    if (state == Crawler.SessionState.KILLED)
                    {
                        report.MessageType = Log.MessageType.ERROR;
                        report.Value = "KILLED";
                        report.Details = "KIlled by Manager.";
                        continue;
                    }
                    report.MessageType = Log.MessageType.INFORM;
                    report.Value = "RUNNING";
                    report.Details = "Running";
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + CrawlerApi.MessageMark.ABORTED))
                {
                    report.MessageType = Log.MessageType.ERROR;
                    report.Value = "ABORTED";
                    report.Details = "Last session is " + CrawlerApi.MessageMark.ABORTED;
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + CrawlerApi.MessageMark.UNCOMPLETED))
                {
                    report.MessageType = Log.MessageType.WARNING;
                    report.Value = "UNCOMPLETED";
                    report.Details = "Last session is " + CrawlerApi.MessageMark.UNCOMPLETED;
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + CrawlerApi.MessageMark.COMPLETED))
                {
                    report.MessageType = Log.MessageType.INFORM;
                    report.Value = "COMPLETED";
                    report.Details = "Completed";
                    continue;
                }
                report.MessageType = Log.MessageType.ERROR;
                report.Value = "SYSTEM ERROR";
                report.Details = "Unknown MessageMark";
            }

            foreach (Record service in dbc.Get("SELECT * FROM Services WHERE State<>" + (int)Service.State.DISABLED).GetRecordset())
            {
                Report report = new Report();
                reports.Add(report);
                report.Source = (string)service["Id"];
                report.SourceType = ReportSourceType.SERVICE;
                DateTime earliest_start_time = DateTime.Now.AddSeconds(-(int)service["RunTimeSpan"]);
                Record start_message = dbc["SELECT * FROM Messages WHERE Source=@Source AND Value LIKE '" + Service.MessageMark.STARTED + "%' ORDER BY Time DESC"].GetFirstRecord("@Source", service["Id"]);
                if (start_message == null || (DateTime)start_message["Time"] < earliest_start_time)
                {
                    Service.SessionState state = (Service.SessionState)(int)dbc["SELECT _LastSessionState FROM Services WHERE Id=@Id"].GetSingleValue("@Id", service["Id"]);
                    if (state == SessionState.STARTED)
                    {
                        report.MessageType = Log.MessageType.WARNING;
                        report.Value = "LONG WORK";
                        report.Details = "Works longer than its RunTimeSpane";
                        continue;
                    }
                    report.MessageType = Log.MessageType.ERROR;
                    report.Value = "NO START";
                    report.Details = "Not started within its RunTimeSpan";
                    continue;
                }
                Record end_message = dbc["SELECT * FROM Messages WHERE Source=@Source AND (Value LIKE '" + Service.MessageMark.ABORTED + "%' OR Value LIKE '" + Service.MessageMark.ERROR + "%' OR Value LIKE '" + Service.MessageMark.COMPLETED + "%') ORDER BY Time DESC"].GetFirstRecord("@Source", service["Id"]);
                if (end_message == null)
                {
                    report.MessageType = Log.MessageType.INFORM;
                    report.Value = "RUNNING";
                    report.Details = "Running";
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + Service.MessageMark.ABORTED))
                {
                    report.MessageType = Log.MessageType.ERROR;
                    report.Value = "ABORTED";
                    report.Details = "Last session is " + Service.MessageMark.ABORTED;
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + Service.MessageMark.ERROR))
                {
                    report.MessageType = Log.MessageType.ERROR;
                    report.Value = "ERRORS";
                    report.Details = "Last session has errors";
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + Service.MessageMark.COMPLETED))
                {
                    report.MessageType = Log.MessageType.INFORM;
                    report.Value = "COMPLETED";
                    report.Details = "Completed";
                    continue;
                }
                report.MessageType = Log.MessageType.ERROR;
                report.Value = "SYSTEM ERROR";
                report.Details = "Unknown MessageMark";
            }

            return reports;
        }
    }
}