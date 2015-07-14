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
            Dictionary<string, object> r = DbApi.Dbc["SELECT * FROM crawlers WHERE id=@id"].GetFirstRecord("@id", crawler_id);
            if (r == null)
            {
                LogMessage.Error("Crawler '" + crawler_id + "' does not exist.");
                return false;
            }

            List<string> parameters = new List<string>();
            DbApi.CrawlerState state = (DbApi.CrawlerState)(byte)r["state"];
            if (state != DbApi.CrawlerState.ENABLED)
            {
                LogMessage.Error("Crawler '" + crawler_id + "' is not enabled.");
                return false;
            }
            parameters.Add(Cliver.Bot.CommandLineParameters.SILENTLY);

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
            Recordset rs = DbApi.Dbc["SELECT id AS crawler_id, _last_start_time, _last_process_id, _last_log, admin_emails, _last_session_state FROM crawlers WHERE _last_session_state=" + (byte)DbApi.SessionState.STARTED + " AND state=" + (byte)DbApi.CrawlerState.DISABLED].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];
                Process p = ProcessRoutines.GetProcess((int)r["_last_process_id"]);
                if (p == null)
                    continue;

                Log.Main.Warning("Killing " + crawler_id + "as disabled");
                p.Kill();
                Thread.Sleep(2000);
                if (ProcessRoutines.IsProcessAlive((int)r["_last_process_id"]))
                    DbApi.Dbc["UPDATE crawlers SET _last_session_state=" + (byte)DbApi.SessionState.KILLED + ", _last_end_time=GETDATE() WHERE id=@id"].Execute("@id", crawler_id);
                else
                    Log.Main.Error("Could not kill " + crawler_id);
            }

            ////////////////////////////////////////////////////////////
            //Process crawler commands
            ////////////////////////////////////////////////////////////
            rs = DbApi.Dbc["SELECT id AS crawler_id, _last_start_time, _last_process_id, _last_log, admin_emails, _last_session_state, command FROM crawlers WHERE state<>" + (byte)DbApi.CrawlerState.DISABLED + " AND command<>" + (byte)DbApi.CrawlerCommand.EMPTY].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];
                Process p = ProcessRoutines.GetProcess((int)r["_last_process_id"]);
                DbApi.CrawlerCommand command = (DbApi.CrawlerCommand)(byte)r["command"];
                switch (command)
                {
                    case DbApi.CrawlerCommand.RESTART:
                        if ((byte)r["_last_session_state"] != (byte)DbApi.SessionState.STARTED || p == null)
                        {
                            DbApi.Dbc["UPDATE crawlers SET command=" + (byte)DbApi.CrawlerCommand.EMPTY + ", _next_start_time=DATEADD(ss, -1, GETDATE()) WHERE id=@id"].Execute("@id", crawler_id);
                            break;
                        }
                        Log.Main.Warning("Killing " + crawler_id + " as marked " + command);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ProcessRoutines.IsProcessAlive((int)r["_last_process_id"]))
                            DbApi.Dbc["UPDATE crawlers SET command=" + (byte)DbApi.CrawlerCommand.FORCE + " WHERE id=@id"].Execute("@id", crawler_id);
                        else
                            Log.Main.Error("Could not kill " + crawler_id);
                        break;
                    case DbApi.CrawlerCommand.STOP:
                        if ((byte)r["_last_session_state"] != (byte)DbApi.SessionState.STARTED || p == null)
                            break;
                        Log.Main.Warning("Killing " + crawler_id + " as marked " + command);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ProcessRoutines.IsProcessAlive((int)r["_last_process_id"]))
                            DbApi.Dbc["UPDATE crawlers SET _last_session_state=" + (byte)DbApi.SessionState.KILLED + ", _last_end_time=GETDATE() WHERE id=@id"].Execute("@id", crawler_id);
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
            rs = DbApi.Dbc[@"SELECT DATEDIFF(ss, _last_start_time, GETDATE()) AS duration, id AS crawler_id, state, _last_start_time, 
_last_process_id, _last_log, admin_emails, _last_session_state, crawl_product_timeout FROM crawlers 
WHERE _last_session_state IN (" + (byte)DbApi.SessionState.STARTED + ", " + (byte)DbApi.SessionState._ERROR + ", " + (byte)DbApi.SessionState._COMPLETED + ")"].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];
                string m1 = "\nStarted: " + r["_last_start_time"] + "\nLog: " + r["_last_log"];
                DbApi.SessionState _last_session_state = (DbApi.SessionState)(byte)r["_last_session_state"];
                int duration = (int)r["duration"];
                if (_last_session_state == DbApi.SessionState._COMPLETED)
                {
                    string m = "Crawler " + crawler_id + " completed successfully.\nTotal duration: " + (new TimeSpan(0, 0, duration)).ToString() + m1;
                    EmailRoutines.Send(m, crawler_id, false);
                    DbApi.Dbc["UPDATE crawlers SET _last_session_state=" + (byte)DbApi.SessionState.COMPLETED + " WHERE id=@id"].Execute("@id", crawler_id);
                    continue;
                }

                if (_last_session_state == DbApi.SessionState._ERROR)
                {
                    EmailRoutines.Send("Crawler " + crawler_id + " exited with error" + m1, crawler_id);
                    DbApi.Dbc["UPDATE crawlers SET _last_session_state=" + (byte)DbApi.SessionState.ERROR + " WHERE id=@id"].Execute("@id", crawler_id);
                    continue;
                }

                if (!ProcessRoutines.IsProcessAlive((int)r["_last_process_id"]))
                {
                    EmailRoutines.Send("Crawler " + crawler_id + " was broken by unknown reason" + m1, crawler_id);
                    DbApi.Dbc["UPDATE crawlers SET _last_session_state=" + (byte)DbApi.SessionState.BROKEN + ", _next_start_time=DATEADD(ss, restart_delay_if_broken, GETDATE()) WHERE id=@id"].Execute("@id", crawler_id);
                    continue;
                }

                if (duration >= (int)r["crawl_product_timeout"])
                {
                    int last_crawled_product_elapsed_time = (int)DbApi.Dbc["SELECT ISNULL(DATEDIFF(ss, _last_product_time, GETDATE()), -1) AS duration FROM crawlers WHERE id=@id"].GetSingleValue("@id", crawler_id);

                    if (last_crawled_product_elapsed_time < 0 || last_crawled_product_elapsed_time > (int)r["crawl_product_timeout"])
                    {
                        EmailRoutines.Send("Crawler " + crawler_id + " is running but not crawling products during " + last_crawled_product_elapsed_time + " seconds. It will be killed. Total duration: " + (new TimeSpan(0, 0, duration)).ToString() + m1, crawler_id);

                        Process p = ProcessRoutines.GetProcess((int)r["_last_process_id"]);
                        Log.Main.Warning("Killing " + crawler_id);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ProcessRoutines.IsProcessAlive((int)r["_last_process_id"]))
                            DbApi.Dbc["UPDATE crawlers SET _last_session_state=" + (byte)DbApi.SessionState.KILLED + ", _next_start_time=DATEADD(ss, restart_delay_if_broken, GETDATE()), _last_end_time=GETDATE() WHERE id=@id"].Execute("@id", crawler_id);
                        else
                            Log.Main.Error("Could not kill " + crawler_id);
                        continue;
                    }
                }

                running_crawler_ids.Add(crawler_id);
                running_crawler_notifications.Add(crawler_id + ", process id: " + r["_last_process_id"]);
            }
            if (running_crawler_notifications.Count > 0)
                Log.Main.Write("Already running: " + string.Join("\r\n", running_crawler_notifications));

            ////////////////////////////////////////////////////////////
            //Starting new sessions
            ////////////////////////////////////////////////////////////
            List<string> remaining_crawler_ids = new List<string>();
            rs = DbApi.Dbc[@"SELECT id AS crawler_id, state, command, admin_emails FROM crawlers 
WHERE (state<>" + (byte)DbApi.CrawlerState.DISABLED + " AND GETDATE()>=_next_start_time AND command<>" + (byte)DbApi.CrawlerCommand.STOP + @") 
            OR command=" + (byte)DbApi.CrawlerCommand.FORCE + " ORDER BY command, _next_start_time"].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];

                if ((byte)r["command"] == (byte)DbApi.CrawlerCommand.FORCE)
                {
                    Log.Main.Write("Forcing " + crawler_id);
                    if ((byte)r["state"] == (byte)DbApi.CrawlerState.DISABLED)
                    {
                        Log.Main.Error(crawler_id + " is disabled.");
                        continue;
                    }
                    if (running_crawler_ids.Contains(crawler_id))
                    {
                        Log.Main.Warning(crawler_id + " is running already.");
                        DbApi.Dbc["UPDATE crawlers SET command=" + (byte)DbApi.CrawlerCommand.EMPTY + " WHERE id=@id"].Execute("@id", crawler_id);
                        continue;
                    }
                    if (launch_crawler(crawler_id, running_crawler_ids))
                        DbApi.Dbc["UPDATE crawlers SET command=" + (byte)DbApi.CrawlerCommand.EMPTY + " WHERE id=@id"].Execute("@id", crawler_id);
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
                Log.Main.Write("Currently running crawlers: " + running_crawler_ids.Count);
            else Log.Main.Write("Currently no crawler runs.");
        }
    }
}
