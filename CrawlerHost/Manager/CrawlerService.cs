//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        03 January 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Configuration;
using Microsoft.Win32;
using System.Text;
using System.Reflection;
using System.Net.Mail;
using Cliver.Bot;
using Cliver.CrawlerHost;

namespace Cliver.CrawlerHost
{
    internal class CrawlerService
    {              
        public static void Run()
        {
            ////////////////////////////////////////////////////////////
            //Killing disabled crawler processes
            ////////////////////////////////////////////////////////////
            Recordset rs = DbApi.Connection[@"SELECT Id AS crawler_id, 
ISNULL(_LastStartTime, 0) AS _LastStartTime, ISNULL(_LastEndTime, 0) AS _LastEndTime, 
_LastProcessId, _LastLog, AdminEmails, _LastSessionState FROM Crawlers WHERE _LastSessionState=" + (int)Crawler.SessionState.STARTED + " AND State=" + (int)Crawler.State.DISABLED].GetRecordset();
            foreach (Record r in rs)
            {
                string crawler_id = (string)r["crawler_id"];
                Process p = ServiceManager.GetProcess((int?)r["_LastProcessId"], crawler_id);
                if (p == null)
                    continue;

                Log.Main.Warning("Killing " + crawler_id + "as disabled");
                p.Kill();
                Thread.Sleep(2000);
                if (ServiceManager.IsProcessAlive((int?)r["_LastProcessId"], crawler_id))
                    Log.Main.Error("Could not kill " + crawler_id);
                else
                    DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)Crawler.SessionState.KILLED + ", _LastEndTime=GETDATE() WHERE Id=@Id"].Execute("@Id", crawler_id);
            }

            ////////////////////////////////////////////////////////////
            //Process crawler commands
            ////////////////////////////////////////////////////////////
            rs = DbApi.Connection[@"SELECT Id AS crawler_id, 
ISNULL(_LastStartTime, 0) AS _LastStartTime, ISNULL(_LastEndTime, 0) AS _LastEndTime, 
    _LastProcessId, _LastLog, AdminEmails, _LastSessionState, Command FROM Crawlers WHERE State<>" + (int)Crawler.State.DISABLED + " AND Command<>" + (int)Crawler.Command.EMPTY].GetRecordset();
            foreach (Record r in rs)
            {
                string crawler_id = (string)r["crawler_id"];
                Process p = ServiceManager.GetProcess((int?)r["_LastProcessId"], crawler_id);
                Crawler.Command command = (Crawler.Command)(int)r["Command"];
                switch (command)
                {
                    case Crawler.Command.RESTART:
                        if (p == null)
                        {
                            DbApi.Connection["UPDATE Crawlers SET Command=" + (int)Crawler.Command.EMPTY + ", _NextStartTime=DATEADD(ss, -1, GETDATE()) WHERE Id=@Id"].Execute("@Id", crawler_id);
                            break;
                        }
                        Log.Main.Warning("Killing " + crawler_id + " as marked " + command);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ServiceManager.IsProcessAlive((int?)r["_LastProcessId"], crawler_id))
                            DbApi.Connection["UPDATE Crawlers SET Command=" + (int)Crawler.Command.FORCE + " WHERE Id=@Id"].Execute("@Id", crawler_id);
                        else
                            Log.Main.Error("Could not kill " + crawler_id);
                        break;
                    case Crawler.Command.STOP:
                        if (p == null)
                            break;
                        Log.Main.Warning("Killing " + crawler_id + " as marked " + command);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ServiceManager.IsProcessAlive((int?)r["_LastProcessId"], crawler_id))
                            DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)Crawler.SessionState.KILLED + ", _LastEndTime=GETDATE() WHERE Id=@Id"].Execute("@Id", crawler_id);
                        else
                            Log.Main.Error("Could not kill " + crawler_id);
                        break;
                    case Crawler.Command.FORCE:
                        //processed below				
                        break;
                    case Crawler.Command.RESTART_WITH_CLEAR_SESSION:
                        if (p != null)
                        {
                            Log.Main.Warning("Killing " + crawler_id + " as marked " + command);
                            p.Kill();
                            Thread.Sleep(2000);
                            if (ServiceManager.IsProcessAlive((int?)r["_LastProcessId"], crawler_id))
                                Log.Main.Error("Could not kill " + crawler_id);
                            break;
                        }
                        clear_session(crawler_id);
                        DbApi.Connection["UPDATE Crawlers SET Command=" + (int)Crawler.Command.FORCE + " WHERE Id=@Id"].Execute("@Id", crawler_id);
                        break;
                    default:
                        throw new Exception("Crawler command " + command + " is not defined.");
                }
            }

            ////////////////////////////////////////////////////////////
            //Checking previously started sessions
            ////////////////////////////////////////////////////////////
            List<string> running_crawler_ids = new List<string>();
            List<string> running_crawler_notifications = new List<string>();
            rs = DbApi.Connection[@"SELECT DATEDIFF(ss, ISNULL(_LastStartTime, 0), GETDATE()) AS duration, Id AS crawler_id, State, 
ISNULL(_LastStartTime, 0) AS _LastStartTime, ISNULL(_LastEndTime, 0) AS _LastEndTime, 
_LastProcessId, _LastLog, AdminEmails, _LastSessionState, CrawlProductTimeout 
FROM Crawlers 
WHERE _LastSessionState IN (" + (int)Crawler.SessionState.STARTED + ", " + (int)Crawler.SessionState._ERROR + ", " + (int)Crawler.SessionState._COMPLETED + ")"].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];
                string m1 = "\nStarted: " + r["_LastStartTime"] + "\nLog: " + r["_LastLog"];
                Crawler.SessionState _LastSessionState = (Crawler.SessionState)(int)r["_LastSessionState"];
                int duration = (int)r["duration"];
                if (_LastSessionState == Crawler.SessionState._COMPLETED)
                {
                    string m = "Crawler " + crawler_id + " completed successfully.\nTotal duration: " + (new TimeSpan(0, 0, duration)).ToString() + m1;
                    EmailRoutine.Send(m, EmailRoutine.SourceType.CRAWLER, crawler_id, false);
                    DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)Crawler.SessionState.COMPLETED + " WHERE Id=@Id"].Execute("@Id", crawler_id);
                    continue;
                }

                if (_LastSessionState == Crawler.SessionState._ERROR)
                {
                    EmailRoutine.Send("Crawler " + crawler_id + " exited with error" + m1, EmailRoutine.SourceType.CRAWLER, crawler_id);
                    DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)Crawler.SessionState.ERROR + " WHERE Id=@Id"].Execute("@Id", crawler_id);
                    continue;
                }

                if (!ServiceManager.IsProcessAlive((int?)r["_LastProcessId"], crawler_id))
                {
                    EmailRoutine.Send("Crawler " + crawler_id + " was broken by unknown reason", EmailRoutine.SourceType.CRAWLER, crawler_id);
                    DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)Crawler.SessionState.BROKEN + ", _NextStartTime=DATEADD(ss, RestartDelayIfBroken, GETDATE()) WHERE Id=@Id"].Execute("@Id", crawler_id);
                    continue;
                }

                if (duration >= (int)r["CrawlProductTimeout"])
                {
                    int last_crawled_product_elapsed_time = (int)DbApi.Connection["SELECT ISNULL(DATEDIFF(ss, _LastProductTime, GETDATE()), -1) AS duration FROM Crawlers WHERE Id=@Id"].GetSingleValue("@Id", crawler_id);

                    if (last_crawled_product_elapsed_time < 0 || last_crawled_product_elapsed_time > (int)r["CrawlProductTimeout"])
                    {
                        EmailRoutine.Send("Crawler " + crawler_id + " is running but not crawling products during " + last_crawled_product_elapsed_time + " seconds. It will be killed. Total duration: " + (new TimeSpan(0, 0, duration)).ToString() + m1, EmailRoutine.SourceType.CRAWLER, crawler_id);
                        
                        Process p = ServiceManager.GetProcess((int?)r["_LastProcessId"], crawler_id);
                        Log.Main.Warning("Killing " + crawler_id);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ServiceManager.IsProcessAlive((int?)r["_LastProcessId"], crawler_id))
                            DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)Crawler.SessionState.KILLED + ", _NextStartTime=DATEADD(ss, RestartDelayIfBroken, GETDATE()), _LastEndTime=GETDATE() WHERE Id=@Id"].Execute("@Id", crawler_id);
                        else
                            Log.Main.Error("Could not kill " + crawler_id);
                        continue;
                    }
                }

                running_crawler_ids.Add(crawler_id);
                running_crawler_notifications.Add(crawler_id + ", process id: " + r["_LastProcessId"]);
            }
            if (running_crawler_notifications.Count > 0)
                Log.Main.Write("Already running: " + string.Join("\r\n", running_crawler_notifications));

            ////////////////////////////////////////////////////////////
            //Starting new crawlers
            ////////////////////////////////////////////////////////////
            List<string> remaining_crawler_ids = new List<string>();
            rs = DbApi.Connection[@"SELECT Id AS crawler_id, State, Command, AdminEmails FROM Crawlers 
WHERE (State<>" + (int)Crawler.State.DISABLED + " AND GETDATE()>=_NextStartTime AND Command<>" + (int)Crawler.Command.STOP + @") 
            OR Command=" + (int)Crawler.Command.FORCE + " ORDER BY Command, _NextStartTime"].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];

                if ((int)r["Command"] == (int)Crawler.Command.FORCE)
                {
                    Log.Main.Write("Forcing " + crawler_id);
                    if ((int)r["State"] == (int)Crawler.State.DISABLED)
                    {
                        Log.Main.Error(crawler_id + " is disabled.");
                        continue;
                    }
                    if (running_crawler_ids.Contains(crawler_id))
                    {
                        Log.Main.Warning(crawler_id + " is running already.");
                        DbApi.Connection["UPDATE Crawlers SET Command=" + (int)Crawler.Command.EMPTY + " WHERE Id=@Id"].Execute("@Id", crawler_id);
                        continue;
                    }
                    if (launch_crawler(crawler_id, running_crawler_ids))
                        DbApi.Connection["UPDATE Crawlers SET Command=" + (int)Crawler.Command.EMPTY + " WHERE Id=@Id"].Execute("@Id", crawler_id);
                    continue;
                }

                if (running_crawler_ids.Contains(crawler_id))
                    continue;
                if (running_crawler_ids.Count >= Properties.Settings.Default.CrawlerProcessMaxNumber)
                {
                    remaining_crawler_ids.Add(crawler_id);
                    continue;
                }
                launch_crawler(crawler_id, running_crawler_ids);
            }

            if (remaining_crawler_ids.Count > 0)
                Log.Main.Warning("crawler_process_number reached " + Properties.Settings.Default.CrawlerProcessMaxNumber + " so no more crawler will be started.\nCrawlers remaining to start:\n" + string.Join("\r\n", remaining_crawler_ids));

            if (running_crawler_ids.Count > 0)
                Log.Main.Write("Currently running Crawlers: " + running_crawler_ids.Count);
            else Log.Main.Write("Currently no crawler runs.");
        }

        static bool launch_crawler(string crawler_id, List<string> running_crawler_ids)
        {
            Dictionary<string, object> r = DbApi.Connection["SELECT * FROM Crawlers WHERE Id=@Id"].GetFirstRecord("@Id", crawler_id);
            if (r == null)
            {
                LogMessage.Error("Crawler '" + crawler_id + "' does not exist.");
                return false;
            }

            List<string> parameters = new List<string>();
            Crawler.State state = (Crawler.State)(int)r["State"];
            switch (state)
            {
                case Crawler.State.DISABLED:
                    LogMessage.Error("Crawler '" + crawler_id + "' is disabled.");
                    return false;
                case Crawler.State.ENABLED:
                    parameters.Add(Cliver.Bot.CommandLineParameters.PRODUCTION.ToString());
                    parameters.Add(Cliver.Bot.CommandLineParameters.AUTOMATIC.ToString());
                    break;
                case Crawler.State.DEBUG:
                    parameters.Add(Cliver.Bot.CommandLineParameters.AUTOMATIC.ToString());
                    break;
                default:
                    throw new Exception("Some case is absent.");
            }

            string crawler_directory;
            crawler_directory = Log.GetAbsolutePath(Cliver.CrawlerHost.Properties.Settings.Default.CrawlersDirectory);
            if (!Directory.Exists(crawler_directory))
            {
                EmailRoutine.Send("Crawler directory '" + crawler_directory + "' does not exist", EmailRoutine.SourceType.CRAWLER, crawler_id);
                return false;
            }
            string crawler_file_name = crawler_id + ".exe";
            string crawler_file = ServiceManager.FindFile(crawler_directory, crawler_file_name);
            if (crawler_file == null)
            {
                EmailRoutine.Send("Crawler file '" + crawler_file_name + "' was not found in " + crawler_directory, EmailRoutine.SourceType.CRAWLER, crawler_id);
                return false;
            }
            Process p = new Process();
            p.StartInfo.FileName = crawler_file;
            p.StartInfo.Arguments = string.Join(" ", parameters);
            Log.Main.Write("Starting crawler " + crawler_id);
            p.Start();
            ThreadRoutines.Wait(Properties.Settings.Default.ServiceCheckDurationInMss);
            if (!ServiceManager.IsProcessAlive(p.Id, crawler_id))
            {
                DbApi.Connection["UPDATE Crawlers SET _NextStartTime=DATEADD(ss, RestartDelayIfBroken, GETDATE()) WHERE Id=@Id"].Execute("@Id", crawler_id);

                EmailRoutine.Send(crawler_id + " could not start.", EmailRoutine.SourceType.CRAWLER, crawler_id);
                return false;
            }
            running_crawler_ids.Add(crawler_id);
            Log.Main.Write("Process id: " + p.Id);
            return true;
        }

        static void clear_session(string crawler_id)
        {
            try
            {
                Directory.Delete(Cliver.Bot.Properties.Log.Default.PreWorkDir + @"\" + Log.WorkDirPrefix, true);
            }
            catch(Exception e)
            {
                Log.Main.Error(e);
            }
        }
    }
}
