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
using Settings = Cliver.CrawlerManager.Properties.Settings;
using Cliver.Bot;

namespace Cliver.CrawlerManager
{
    static public class CrawlerManager
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
                send_message(Log.GetExceptionMessage(e));
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
                send_message(Log.GetExceptionMessage(e));
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

        static void send_message(string message, string crawler_id = null, bool error = true)
        {
            try
            {
                if (error)
                    Log.Main.Error(message);
                else
                    Log.Main.Inform(message);

                string admin_emails = null;
                if (crawler_id != null)
                    admin_emails = (string)Dbc["SELECT admin_emails FROM crawlers WHERE id=@id"].GetSingleValue("@id", crawler_id);
                if (admin_emails == null)
                    admin_emails = Settings.Default.DefaultAdminEmails;
                if (admin_emails != null)
                    admin_emails = Regex.Replace(admin_emails.Trim(), @"[\s+\,]+", ",", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
                else
                    Log.Main.Error("No email is defined to send messages.");

                MailMessage m = new MailMessage();
                m.From = new MailAddress(Settings.Default.EmailSender);
                m.To.Add(admin_emails);
                string subject = "Crawler Manager:";
                if (crawler_id != null) subject += " " + crawler_id;
                if (error) subject += " error";
                subject += " notification";
                m.Subject = subject;
                m.Body = message;

                System.Net.Mail.SmtpClient c = new SmtpClient(Settings.Default.SmtpHost, Settings.Default.SmtpPort);
                c.EnableSsl = true;
                c.DeliveryMethod = SmtpDeliveryMethod.Network;
                c.UseDefaultCredentials = false;
                c.Credentials = new System.Net.NetworkCredential(Settings.Default.SmtpLogin, Settings.Default.SmtpPassword);

                try
                {
                    c.Send(m);
                }
                catch (Exception e)
                {
                    Log.Main.Error(e);
                }
            }
            catch (Exception e)
            {
                Log.Main.Error(e);
            }
        }

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
            Dictionary<string, object> r = Dbc["SELECT * FROM crawlers WHERE id=@id"].GetFirstRecord("@id", crawler_id);
            if (r == null)
            {
                LogMessage.Error("Crawler does not exist: " + crawler_id);
                return false;
            }

            List<string> parameters = new List<string>();
            CrawlerHost.CrawlerState state = (CrawlerHost.CrawlerState)(byte)r["state"];
            switch (state)
            {
                case CrawlerHost.CrawlerState.ENABLED:
                    parameters.Add("-production");
                    break;
                case CrawlerHost.CrawlerState.DEBUG:
                    parameters.Add("-debug");
                    break;
                default:
                    throw new Exception("Unknown option: " + state);
            }
            parameters.Add("-silently");

            string crawler_directory;
            crawler_directory = Log.GetAbsolutePath( Settings.Default.CrawlersDirectory);
            if (!Directory.Exists(crawler_directory))
            {
                send_message("Crawler directory '" + crawler_directory + "' does not exist", crawler_id);
                return false;
            }
            string crawler_file_name = crawler_id + ".exe";
            string crawler_file = find_file(crawler_directory, crawler_file_name);
            if (crawler_file == null)
            {
                send_message("Crawler file '" + crawler_file_name + "' was not found in " + crawler_directory, crawler_id);
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
                send_message(crawler_id + " could not start.", crawler_id);
                return false;
            }
            running_crawler_ids.Add(crawler_id);
            Log.Main.Write("Process id: " + p.Id);
            return true;
        }
        
        static internal readonly DbConnection Dbc = DbConnection.Create(DbConnection.GetPreparedDbConnectionString(Settings.Default.DbConnectionString_, Settings.Default.DbRelativePath_));

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
            Recordset rs = Dbc["SELECT id AS crawler_id, _last_start_time, _last_process_id, _last_log, admin_emails, _last_session_state FROM crawlers WHERE _last_session_state=" + (byte)CrawlerHost.SessionState.STARTED + " AND state=" + (byte)CrawlerHost.CrawlerState.DISABLED].GetRecordset();
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
                    Dbc["UPDATE crawlers SET _last_session_state=" + (byte)CrawlerHost.SessionState.KILLED + ", _last_end_time=GETDATE() WHERE id=@id"].Execute("@id", crawler_id);
                else
                    Log.Main.Error("Could not kill " + crawler_id);
            }

            ////////////////////////////////////////////////////////////
            //Process crawler commands
            ////////////////////////////////////////////////////////////
            rs = Dbc["SELECT id AS crawler_id, _last_start_time, _last_process_id, _last_log, admin_emails, _last_session_state, command FROM crawlers WHERE state<>" + (byte)CrawlerHost.CrawlerState.DISABLED + " AND command<>" + (byte)CrawlerHost.CrawlerCommand.EMPTY].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];
                Process p = ProcessRoutines.GetProcess((int)r["_last_process_id"]);
                CrawlerHost.CrawlerCommand command = (CrawlerHost.CrawlerCommand)(byte)r["command"];
                switch (command)
                {
                    case CrawlerHost.CrawlerCommand.RESTART:
                        if ((byte)r["_last_session_state"] != (byte)CrawlerHost.SessionState.STARTED || p == null)
                        {
                            Dbc["UPDATE crawlers SET command=" + (byte)CrawlerHost.CrawlerCommand.EMPTY + ", _next_start_time=DATEADD(ss, -1, GETDATE()) WHERE id=@id"].Execute("@id", crawler_id);
                            break;
                        }
                        Log.Main.Warning("Killing " + crawler_id + " as marked " + command);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ProcessRoutines.IsProcessAlive((int)r["_last_process_id"]))
                            Dbc["UPDATE crawlers SET command=" + (byte)CrawlerHost.CrawlerCommand.FORCE + " WHERE id=@id"].Execute("@id", crawler_id);
                        else
                            Log.Main.Error("Could not kill " + crawler_id);
                        break;
                    case CrawlerHost.CrawlerCommand.STOP:
                        if ((byte)r["_last_session_state"] != (byte)CrawlerHost.SessionState.STARTED || p == null)
                            break;
                        Log.Main.Warning("Killing " + crawler_id + " as marked " + command);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ProcessRoutines.IsProcessAlive((int)r["_last_process_id"]))
                            Dbc["UPDATE crawlers SET _last_session_state=" + (byte)CrawlerHost.SessionState.KILLED + ", _last_end_time=GETDATE() WHERE id=@id"].Execute("@id", crawler_id);
                        else
                            Log.Main.Error("Could not kill " + crawler_id);
                        break;
                    case CrawlerHost.CrawlerCommand.FORCE:
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
            rs = Dbc[@"SELECT DATEDIFF(ss, _last_start_time, GETDATE()) AS duration, id AS crawler_id, state, _last_start_time, 
_last_process_id, _last_log, admin_emails, _last_session_state, crawl_product_timeout FROM crawlers 
WHERE _last_session_state IN (" + (byte)CrawlerHost.SessionState.STARTED + ", " + (byte)CrawlerHost.SessionState._ERROR + ", " + (byte)CrawlerHost.SessionState._COMPLETED + ")"].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];
                string m1 = "\nStarted: " + r["_last_start_time"] + "\nLog: " + r["_last_log"];
                CrawlerHost.SessionState _last_session_state = (CrawlerHost.SessionState)(byte)r["_last_session_state"];
                int duration = (int)r["duration"];
                if (_last_session_state == CrawlerHost.SessionState._COMPLETED)
                {
                    string m = "Crawler " + crawler_id + " completed successfully.\nTotal duration: " + (new TimeSpan(0, 0, duration)).ToString() + m1;
                    send_message(m, crawler_id, false);
                    Dbc["UPDATE crawlers SET _last_session_state=" + (byte)CrawlerHost.SessionState.COMPLETED + " WHERE id=@id"].Execute("@id", crawler_id);
                    continue;
                }

                if (_last_session_state == CrawlerHost.SessionState._ERROR)
                {
                    send_message("Crawler " + crawler_id + " exited with error" + m1, crawler_id);
                    Dbc["UPDATE crawlers SET _last_session_state=" + (byte)CrawlerHost.SessionState.ERROR + " WHERE id=@id"].Execute("@id", crawler_id);
                    continue;
                }

                if (!ProcessRoutines.IsProcessAlive((int)r["_last_process_id"]))
                {
                    send_message("Crawler " + crawler_id + " was broken by unknown reason" + m1, crawler_id);
                    Dbc["UPDATE crawlers SET _last_session_state=" + (byte)CrawlerHost.SessionState.BROKEN + ", _next_start_time=DATEADD(ss, restart_delay_if_broken, GETDATE()) WHERE id=@id"].Execute("@id", crawler_id);
                    continue;
                }

                if (duration >= (int)r["crawl_product_timeout"])
                {
                    int last_crawled_product_elapsed_time = (int)Dbc["SELECT ISNULL(DATEDIFF(ss, _last_product_time, GETDATE()), -1) AS duration FROM crawlers WHERE id=@id"].GetSingleValue("@id", crawler_id);

                    if (last_crawled_product_elapsed_time < 0 || last_crawled_product_elapsed_time > (int)r["crawl_product_timeout"])
                    {
                        send_message("Crawler " + crawler_id + " is running but not crawling products during " + last_crawled_product_elapsed_time + " seconds. It will be killed. Total duration: " + (new TimeSpan(0, 0, duration)).ToString() + m1, crawler_id);

                        Process p = ProcessRoutines.GetProcess((int)r["_last_process_id"]);
                        Log.Main.Warning("Killing " + crawler_id);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!ProcessRoutines.IsProcessAlive((int)r["_last_process_id"]))
                            Dbc["UPDATE crawlers SET _last_session_state=" + (byte)CrawlerHost.SessionState.KILLED + ", _next_start_time=DATEADD(ss, restart_delay_if_broken, GETDATE()), _last_end_time=GETDATE() WHERE id=@id"].Execute("@id", crawler_id);
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
            rs = Dbc[@"SELECT id AS crawler_id, state, command, admin_emails FROM crawlers 
WHERE (state<>" + (byte)CrawlerHost.CrawlerState.DISABLED + " AND GETDATE()>=_next_start_time AND command<>" + (byte)CrawlerHost.CrawlerCommand.STOP + @") 
            OR command=" + (byte)CrawlerHost.CrawlerCommand.FORCE + " ORDER BY command, _next_start_time"].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string crawler_id = (string)r["crawler_id"];

                if ((byte)r["command"] == (byte)CrawlerHost.CrawlerCommand.FORCE)
                {
                    Log.Main.Write("Forcing " + crawler_id);
                    if ((byte)r["state"] == (byte)CrawlerHost.CrawlerState.DISABLED)
                    {
                        Log.Main.Error(crawler_id + " is disabled.");
                        continue;
                    }
                    if (running_crawler_ids.Contains(crawler_id))
                    {
                        Log.Main.Warning(crawler_id + " is running already.");
                        Dbc["UPDATE crawlers SET command=" + (byte)CrawlerHost.CrawlerCommand.EMPTY + " WHERE id=@id"].Execute("@id", crawler_id);
                        continue;
                    }
                    if (launch_crawler(crawler_id, running_crawler_ids))
                        Dbc["UPDATE crawlers SET command=" + (byte)CrawlerHost.CrawlerCommand.EMPTY + " WHERE id=@id"].Execute("@id", crawler_id);
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
