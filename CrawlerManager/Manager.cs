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
using Settings = Cliver.CrawlerHost.Properties.Settings;
using Cliver.Bot;
using Cliver.CrawlerHost;

namespace Cliver.CrawlerHost
{
    static public class Manager
    {
        static public bool Started
        {
            get
            {
                return thread != null;
            }
        }

        static public void Start()
        {
            run_service = true;
            if (thread != null)
                return;
            thread = new Thread(new ThreadStart(service));
            thread.Start();
        }
        static Thread thread = null;

        static public void Stop()
        {
            //if (thread == null)
            //    return;
            //thread.Abort();
            run_service = false;
        }
       static bool run_service = true;

        static void service()
        {
            try
            {
                SysTrayForm.This.Started = true;
                Log.Main.Write("STARTED");
                while (run_service)
                {
                    //RunManager.Reset();
                    manage_crawlers();
                    SysTrayForm.This.RefreshView();
                    //RunManager.WaitOne(Settings.Default.PollIntervalInSecs * 1000);
                    ThreadRoutines.WaitForCondition(() => { if (!run_service) return run_service; return null; }, Settings.Default.PollIntervalInSecs * 1000, 1000);
                }
                Log.Main.Write("STOPPED");
            }
            catch (ThreadAbortException)
            {
                Log.Main.Write("ABORTED");
            }
            catch (Exception e)
            {                
                EmailRoutines.Send(Log.GetExceptionMessage(e));
                LogMessage.Exit(e);
            }
            finally
            {
                thread = null;
                SysTrayForm.This.Started = false;
            }
        }
        static public readonly AutoResetEvent RunManager = new AutoResetEvent(true);
        
        static void service2()
        {
            try
            {
                SysTrayForm.This.CheckingNow = true;
                Log.Main.Write("STARTED2");
                manage_crawlers();
                SysTrayForm.This.RefreshView();
                Log.Main.Write("STOPPED2");
            }
            catch (ThreadAbortException)
            {
                Log.Main.Write("ABORTED2");
            }
            catch (Exception e)
            {
                EmailRoutines.Send(Log.GetExceptionMessage(e));
                LogMessage.Exit(e);
            }
            finally
            {
                thread2 = null;
                SysTrayForm.This.CheckingNow = false;
            }
        }

        static public void CheckNow()
        {
            if (thread2 != null)
                return;
            thread2 = new Thread(new ThreadStart(service2));
            thread2.Start();
        }
        static Thread thread2 = null;

        static string find_file(string parent_directory, string file_name)
        {
            DirectoryInfo di = new DirectoryInfo(parent_directory);
            foreach (FileInfo fi in di.GetFiles())
                if (file_name == fi.Name)
                    return fi.FullName;
            foreach (DirectoryInfo d in di.GetDirectories())
            {
                string f = find_file(d.FullName, file_name);
                if (f != null)
                    return f;
            }
            return null;
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
            DbApi.CrawlerState state = (DbApi.CrawlerState)(int)r["State"];
            switch (state)
            {
                case DbApi.CrawlerState.DISABLED:
                    LogMessage.Error("Crawler '" + crawler_id + "' is disabled.");
                    return false;
                case DbApi.CrawlerState.ENABLED:
                    parameters.Add(Cliver.Bot.CommandLineParameters.PRODUCTION.ToString());
                    parameters.Add(Cliver.Bot.CommandLineParameters.AUTOMATIC.ToString());
                    break;
                case DbApi.CrawlerState.DEBUG:
                    parameters.Add(Cliver.Bot.CommandLineParameters.AUTOMATIC.ToString());
                    break;
                default:
                    throw new Exception("Some case is absent.");
            }

            string crawler_directory;
            crawler_directory = Log.GetAbsolutePath( Settings.Default.CrawlersDirectory);
            if (!Directory.Exists(crawler_directory))
            {
                EmailRoutines.Send("Crawler directory '" + crawler_directory + "' does not exist", crawler_id);
                return false;
            }
            string crawler_file_name = crawler_id + ".exe";
            string crawler_file = find_file(crawler_directory, crawler_file_name);
            if (crawler_file == null)
            {
                EmailRoutines.Send("Crawler file '" + crawler_file_name + "' was not found in " + crawler_directory, crawler_id);
                return false;
            }
            Process p = new Process();
            p.StartInfo.FileName = crawler_file;
            p.StartInfo.Arguments = string.Join(" ", parameters);
            Log.Main.Write("Starting crawler " + crawler_id);
            p.Start();
            Thread.Sleep(2000);
            if (!ProcessRoutines.IsProcessAlive(p))
            {
                EmailRoutines.Send(crawler_id + " could not start.", crawler_id);
                return false;
            }
            running_crawler_ids.Add(crawler_id);
            Log.Main.Write("Process id: " + p.Id);
            return true;
        }
        
        static void manage_crawlers()
        {
            try
            {
                lock (RunManager)
                {
                    manage_crawlers_();
                }
            }
            catch (Exception e)
            {
                LogMessage.Exit(e);
            }
        }

        static void manage_crawlers_()
        {
            ////////////////////////////////////////////////////////////
            //Killing disabled crawler processes
            ////////////////////////////////////////////////////////////
            Recordset rs = DbApi.Connection["SELECT Id AS crawler_id, _LastStartTime, _LastProcessId, _LastLog, AdminEmails, _LastSessionState FROM Crawlers WHERE _LastSessionState=" + (int)DbApi.SessionState.STARTED + " AND State=" + (int)DbApi.CrawlerState.DISABLED].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];
                Process p = ProcessRoutines.GetProcess((int)r["_LastProcessId"]);
                if (p == null)
                    continue;

                Log.Main.Warning("Killing " + crawler_id + "as disabled");
                p.Kill();
                Thread.Sleep(2000);
                if (ProcessRoutines.IsProcessAlive((int)r["_LastProcessId"]))
                    DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)DbApi.SessionState.KILLED + ", _LastEndTime=GETDATE() WHERE Id=@Id"].Execute("@Id", crawler_id);
                else
                    Log.Main.Error("Could not kill " + crawler_id);
            }

            ////////////////////////////////////////////////////////////
            //Process crawler commands
            ////////////////////////////////////////////////////////////
            rs = DbApi.Connection["SELECT Id AS crawler_id, _LastStartTime, _LastProcessId, _LastLog, AdminEmails, _LastSessionState, Command FROM Crawlers WHERE State<>" + (int)DbApi.CrawlerState.DISABLED + " AND Command<>" + (int)DbApi.CrawlerCommand.EMPTY].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];
                Process p = ProcessRoutines.GetProcess((int)r["_LastProcessId"]);
                DbApi.CrawlerCommand command = (DbApi.CrawlerCommand)(int)r["Command"];
                switch (command)
                {
                    case DbApi.CrawlerCommand.RESTART:
                        if ((int)r["_LastSessionState"] != (int)DbApi.SessionState.STARTED || p == null)
                        {
                            DbApi.Connection["UPDATE Crawlers SET Command=" + (int)DbApi.CrawlerCommand.EMPTY + ", _NextStartTime=DATEADD(ss, -1, GETDATE()) WHERE Id=@Id"].Execute("@Id", crawler_id);
                            break;
                        }
                        Log.Main.Warning("Killing " + crawler_id + " as marked " + command);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ProcessRoutines.IsProcessAlive((int)r["_LastProcessId"]))
                            DbApi.Connection["UPDATE Crawlers SET Command=" + (int)DbApi.CrawlerCommand.FORCE + " WHERE Id=@Id"].Execute("@Id", crawler_id);
                        else
                            Log.Main.Error("Could not kill " + crawler_id);
                        break;
                    case DbApi.CrawlerCommand.STOP:
                        if ((int)r["_LastSessionState"] != (int)DbApi.SessionState.STARTED || p == null)
                            break;
                        Log.Main.Warning("Killing " + crawler_id + " as marked " + command);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ProcessRoutines.IsProcessAlive((int)r["_LastProcessId"]))
                            DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)DbApi.SessionState.KILLED + ", _LastEndTime=GETDATE() WHERE Id=@Id"].Execute("@Id", crawler_id);
                        else
                            Log.Main.Error("Could not kill " + crawler_id);
                        break;
                    case DbApi.CrawlerCommand.FORCE:
                        //processed below				
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
            rs = DbApi.Connection[@"SELECT DATEDIFF(ss, _LastStartTime, GETDATE()) AS duration, Id AS crawler_id, State, _LastStartTime, 
_LastProcessId, _LastLog, AdminEmails, _LastSessionState, CrawlProductTimeout FROM Crawlers 
WHERE _LastSessionState IN (" + (int)DbApi.SessionState.STARTED + ", " + (int)DbApi.SessionState._ERROR + ", " + (int)DbApi.SessionState._COMPLETED + ")"].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];
                string m1 = "\nStarted: " + r["_LastStartTime"] + "\nLog: " + r["_LastLog"];
                DbApi.SessionState _LastSessionState = (DbApi.SessionState)(int)r["_LastSessionState"];
                int duration = (int)r["duration"];
                if (_LastSessionState == DbApi.SessionState._COMPLETED)
                {
                    string m = "Crawler " + crawler_id + " completed successfully.\nTotal duration: " + (new TimeSpan(0, 0, duration)).ToString() + m1;
                    EmailRoutines.Send(m, crawler_id, false);
                    DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)DbApi.SessionState.COMPLETED + " WHERE Id=@Id"].Execute("@Id", crawler_id);
                    continue;
                }

                if (_LastSessionState == DbApi.SessionState._ERROR)
                {
                    EmailRoutines.Send("Crawler " + crawler_id + " exited with error" + m1, crawler_id);
                    DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)DbApi.SessionState.ERROR + " WHERE Id=@Id"].Execute("@Id", crawler_id);
                    continue;
                }

                if (!ProcessRoutines.IsProcessAlive((int)r["_LastProcessId"]))
                {
                    EmailRoutines.Send("Crawler " + crawler_id + " was broken by unknown reason" + m1, crawler_id);
                    DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)DbApi.SessionState.BROKEN + ", _NextStartTime=DATEADD(ss, RestartDelayIfBroken, GETDATE()) WHERE Id=@Id"].Execute("@Id", crawler_id);
                    continue;
                }

                if (duration >= (int)r["CrawlProductTimeout"])
                {
                    int last_crawled_product_elapsed_time = (int)DbApi.Connection["SELECT ISNULL(DATEDIFF(ss, _LastProductTime, GETDATE()), -1) AS duration FROM Crawlers WHERE Id=@Id"].GetSingleValue("@Id", crawler_id);

                    if (last_crawled_product_elapsed_time < 0 || last_crawled_product_elapsed_time > (int)r["CrawlProductTimeout"])
                    {
                        EmailRoutines.Send("Crawler " + crawler_id + " is running but not crawling products during " + last_crawled_product_elapsed_time + " seconds. It will be killed. Total duration: " + (new TimeSpan(0, 0, duration)).ToString() + m1, crawler_id);

                        Process p = ProcessRoutines.GetProcess((int)r["_LastProcessId"]);
                        Log.Main.Warning("Killing " + crawler_id);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ProcessRoutines.IsProcessAlive((int)r["_LastProcessId"]))
                            DbApi.Connection["UPDATE Crawlers SET _LastSessionState=" + (int)DbApi.SessionState.KILLED + ", _NextStartTime=DATEADD(ss, RestartDelayIfBroken, GETDATE()), _LastEndTime=GETDATE() WHERE Id=@Id"].Execute("@Id", crawler_id);
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
            //Starting new sessions
            ////////////////////////////////////////////////////////////
            List<string> remaining_crawler_ids = new List<string>();
            rs = DbApi.Connection[@"SELECT Id AS crawler_id, State, Command, AdminEmails FROM Crawlers 
WHERE (State<>" + (int)DbApi.CrawlerState.DISABLED + " AND GETDATE()>=_NextStartTime AND Command<>" + (int)DbApi.CrawlerCommand.STOP + @") 
            OR Command=" + (int)DbApi.CrawlerCommand.FORCE + " ORDER BY Command, _NextStartTime"].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];

                if ((int)r["Command"] == (int)DbApi.CrawlerCommand.FORCE)
                {
                    Log.Main.Write("Forcing " + crawler_id);
                    if ((int)r["State"] == (int)DbApi.CrawlerState.DISABLED)
                    {
                        Log.Main.Error(crawler_id + " is disabled.");
                        continue;
                    }
                    if (running_crawler_ids.Contains(crawler_id))
                    {
                        Log.Main.Warning(crawler_id + " is running already.");
                        DbApi.Connection["UPDATE Crawlers SET Command=" + (int)DbApi.CrawlerCommand.EMPTY + " WHERE Id=@Id"].Execute("@Id", crawler_id);
                        continue;
                    }
                    if (launch_crawler(crawler_id, running_crawler_ids))
                        DbApi.Connection["UPDATE Crawlers SET Command=" + (int)DbApi.CrawlerCommand.EMPTY + " WHERE Id=@Id"].Execute("@Id", crawler_id);
                    continue;
                }

                if (running_crawler_ids.Contains(crawler_id))
                    continue;
                if (running_crawler_ids.Count >= Settings.Default.CrawlerProcessMaxNumber)
                {
                    remaining_crawler_ids.Add(crawler_id);
                    continue;
                }
                launch_crawler(crawler_id, running_crawler_ids);
            }

            if (remaining_crawler_ids.Count > 0)
                Log.Main.Warning("crawler_process_number reached " + Settings.Default.CrawlerProcessMaxNumber + " so no more crawler will be started.\nCrawlers remaining to start:\n" + string.Join("\r\n", remaining_crawler_ids));

            if (running_crawler_ids.Count > 0)
                Log.Main.Write("Currently running Crawlers: " + running_crawler_ids.Count);
            else Log.Main.Write("Currently no crawler runs.");
        }
    }
}
