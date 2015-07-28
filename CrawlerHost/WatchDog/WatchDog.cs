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
            List<Report> reports = GetReports();
        }

        public static List<Report> GetReports(string db_connection_string/*used by ProductOffice as it cannot read registry for some reason*/ = null)
        {
            DbConnection dbc;
            if (db_connection_string == null)
                dbc = DbApi.Connection;
            else
                dbc = DbConnection.Create(db_connection_string);

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
                        report.MessageType = DbApi.MessageType.WARNING;
                        report.Value = "LONG WORK";
                        report.Description = "Works longer than its RunTimeSpane";
                        continue;
                    }
                    report.MessageType = DbApi.MessageType.ERROR;
                    report.Value = "NO START";
                    report.Description = "Not started within its RunTimeSpan";
                    continue;
                }
                Record end_message = dbc["SELECT * FROM Messages WHERE Source=@Source AND (Value LIKE '" + CrawlerApi.MessageMark.ABORTED + "%' OR Value LIKE '" + CrawlerApi.MessageMark.UNCOMPLETED + "%' OR Value LIKE '" + CrawlerApi.MessageMark.COMPLETED + "%') ORDER BY Time DESC"].GetFirstRecord("@Source", crawler["Id"]);
                if (end_message == null)
                {
                    if (state == Crawler.SessionState.KILLED)
                    {
                        report.MessageType = DbApi.MessageType.ERROR;
                        report.Value = "KILLED";
                        report.Description = "KIlled by Manager.";
                        continue;
                    }
                    report.MessageType = DbApi.MessageType.INFORM;
                    report.Value = "RUNNING";
                    report.Description = "Running";
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + CrawlerApi.MessageMark.ABORTED))
                {
                    report.MessageType = DbApi.MessageType.ERROR;
                    report.Value = "ABORTED";
                    report.Description = "Last session is " + CrawlerApi.MessageMark.ABORTED;
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + CrawlerApi.MessageMark.UNCOMPLETED))
                {
                    report.MessageType = DbApi.MessageType.WARNING;
                    report.Value = "UNCOMPLETED";
                    report.Description = "Last session is " + CrawlerApi.MessageMark.UNCOMPLETED;
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + CrawlerApi.MessageMark.COMPLETED))
                {
                    report.MessageType = DbApi.MessageType.INFORM;
                    report.Value = "COMPLETED";
                    report.Description = "Completed";
                    continue;
                }
                report.MessageType = DbApi.MessageType.ERROR;
                report.Value = "SYSTEM ERROR";
                report.Description = "Unknown MessageMark";
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
                        report.MessageType = DbApi.MessageType.WARNING;
                        report.Value = "LONG WORK";
                        report.Description = "Works longer than its RunTimeSpane";
                        continue;
                    }
                    report.MessageType = DbApi.MessageType.ERROR;
                    report.Value = "NO START";
                    report.Description = "Not started within its RunTimeSpan";
                    continue;
                }
                Record end_message = dbc["SELECT * FROM Messages WHERE Source=@Source AND (Value LIKE '" + Service.MessageMark.ABORTED + "%' OR Value LIKE '" + Service.MessageMark.ERROR + "%' OR Value LIKE '" + Service.MessageMark.COMPLETED + "%') ORDER BY Time DESC"].GetFirstRecord("@Source", service["Id"]);
                if (end_message == null)
                {
                    report.MessageType = DbApi.MessageType.INFORM;
                    report.Value = "RUNNING";
                    report.Description = "Running";
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + Service.MessageMark.ABORTED))
                {
                    report.MessageType = DbApi.MessageType.ERROR;
                    report.Value = "ABORTED";
                    report.Description = "Last session is " + Service.MessageMark.ABORTED;
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + Service.MessageMark.ERROR))
                {
                    report.MessageType = DbApi.MessageType.ERROR;
                    report.Value = "ERRORS";
                    report.Description = "Last session has errors";
                    continue;
                }
                if (Regex.IsMatch((string)end_message["Value"], @"^" + Service.MessageMark.COMPLETED))
                {
                    report.MessageType = DbApi.MessageType.INFORM;
                    report.Value = "COMPLETED";
                    report.Description = "Completed";
                    continue;
                }
                report.MessageType = DbApi.MessageType.ERROR;
                report.Value = "SYSTEM ERROR";
                report.Description = "Unknown MessageMark";
            }

            return reports;
        }
    }
}