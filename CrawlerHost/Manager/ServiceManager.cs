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
using System.Windows.Forms;

namespace Cliver.CrawlerHost
{
    public class ServiceManager
    {
        public delegate void OnStateChanged(bool work);
        static public event OnStateChanged StateChanged = null;

        static public bool Work
        {
            get
            {
                return work;
            }
            set
            {
                work = value;
            }
        }
        static bool work = true;

        static internal void Start()
        {
            thread = new Thread(new ThreadStart(service));
            thread.Start();
        }
        static Thread thread = null;

        static void service()
        {
            try
            {
                DbApi.Message(DbApi.MessageType.INFORM, "STATRED");
                actual_work = true;
                while (true)
                {
                    if (actual_work)
                    {
                        CrawlerService.Run();
                        manage_services();
                    }
                    if (actual_work != work)
                    {
                        if (StateChanged != null)
                            StateChanged.BeginInvoke(work, null, null);
                        actual_work = work;
                        if (actual_work)
                            DbApi.Message(DbApi.MessageType.INFORM, "STARTED");
                        else
                            DbApi.Message(DbApi.MessageType.INFORM, "STOPPED");
                    }
                    ThreadRoutines.Wait(Properties.Settings.Default.PollIntervalInMss);
                    //ThreadRoutines.WaitForCondition(() => { if (!work) return work; return null; }, Properties.Settings.Default.PollIntervalInSecs * 1000, 100);
                }
            }
            catch (Exception e)
            {
                DbApi.Message(e);
                email(Log.GetExceptionMessage(e));
            }
            if (StateChanged != null)
                StateChanged.BeginInvoke(false, null, null);
        }
        static bool actual_work = false;

        static void manage_services()
        {
            ////////////////////////////////////////////////////////////
            //Killing disabled service processes
            ////////////////////////////////////////////////////////////
            Recordset rs = DbApi.Connection[@"SELECT Id, 
ISNULL(_LastStartTime, 0) AS _LastStartTime, ISNULL(_LastEndTime, 0) AS _LastEndTime, 
_LastProcessId, _LastLog, AdminEmails, _LastSessionState 
FROM Services 
WHERE _LastEndTime IS NULL AND State=" + (int)Service.State.DISABLED].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string service_id = (string)r["Id"];
                Process p = GetProcess((int?)r["_LastProcessId"], service_id);
                if (p == null)
                    continue;

                Log.Main.Warning("Killing " + service_id + "as disabled");
                p.Kill();
                Thread.Sleep(2000);
                if (IsProcessAlive((int?)r["_LastProcessId"], service_id))
                    Log.Main.Error("Could not kill " + service_id);
                else
                    DbApi.Connection["UPDATE Services SET _LastSessionState=" + (int)Service.SessionState.KILLED + ", _LastEndTime=GETDATE() WHERE Id=@Id"].Execute("@Id", service_id);
            
            }

            ////////////////////////////////////////////////////////////
            //Process service commands
            ////////////////////////////////////////////////////////////
            rs = DbApi.Connection[@"SELECT Id, 
ISNULL(_LastStartTime, 0) AS _LastStartTime, ISNULL(_LastEndTime, 0) AS _LastEndTime, 
_LastProcessId, _LastLog, AdminEmails, _LastSessionState, Command 
FROM Services 
WHERE State<>" + (int)Service.State.DISABLED + " AND Command<>" + (int)Service.Command.EMPTY].GetRecordset();
            foreach (Dictionary<string, object> r in rs)
            {
                string service_id = (string)r["Id"];
                Process p = GetProcess((int?)r["_LastProcessId"], service_id);
               Service.Command command = (Service.Command)(int)r["Command"];
                switch (command)
                {
                    case Service.Command.RESTART:
                        if (p == null)
                        {
                            DbApi.Connection["UPDATE Services SET Command=" + (int)Service.Command.EMPTY + ", _NextStartTime=DATEADD(ss, -1, GETDATE()) WHERE Id=@Id"].Execute("@Id", service_id);
                            break;
                        }
                        Log.Main.Warning("Killing " + service_id + " as marked " + command);
                        p.Kill();
                        Thread.Sleep(2000);
                if (IsProcessAlive((int?)r["_LastProcessId"], service_id))
                            DbApi.Connection["UPDATE Services SET Command=" + (int)Service.Command.FORCE + " WHERE Id=@Id"].Execute("@Id", service_id);
                        else
                            Log.Main.Error("Could not kill " + service_id);
                        break;
                    case Service.Command.STOP:
                        if (p == null)
                            break;
                        Log.Main.Warning("Killing " + service_id + " as marked " + command);
                        p.Kill();
                        Thread.Sleep(2000);
                if (IsProcessAlive((int?)r["_LastProcessId"], service_id))
                            DbApi.Connection["UPDATE Services SET_LastSessionState=" + (int)Service.SessionState.KILLED + ", _LastEndTime=GETDATE() WHERE Id=@Id"].Execute("@Id", service_id);
                        else
                            Log.Main.Error("Could not kill " + service_id);
                        break;
                    case Service.Command.FORCE:
                        //processed below				
                        break;
                    case Service.Command.DISABLE_AFTER_COMPLETION:
                        if (p == null)
                            DbApi.Connection["UPDATE Services SET _LastEndTime=GETDATE(), state=" + Service.State.DISABLED + ", command" + Service.Command.EMPTY + " WHERE Id=@Id"].Execute("@Id", service_id);
                        break;
                    default:
                        throw new Exception("Service command " + command + " is not defined.");
                }
            }
            
            ////////////////////////////////////////////////////////////
            //Checking previously started services
            ////////////////////////////////////////////////////////////
            rs = DbApi.Connection[@"SELECT DATEDIFF(ss, ISNULL(_LastStartTime, 0), GETDATE()) AS duration, Id, State, 
ISNULL(_LastStartTime, 0) AS _LastStartTime, ISNULL(_LastEndTime, 0) AS _LastEndTime, 
_LastProcessId, _LastLog, AdminEmails, _LastSessionState, RunTimeout 
FROM Services 
WHERE _LastSessionState IN (" + (int)Service.SessionState.STARTED + ", " + (int)Service.SessionState._ERROR + ", " + (int)Service.SessionState._COMPLETED + ")"].GetRecordset();
            foreach (Record r in rs)
            {
                string service_id = (string)r["Id"];
                string m1 = "\nStarted: " + r["_LastStartTime"] + "\nLog: " + r["_LastLog"];
                int duration = (int)r["duration"];
                Service.SessionState _LastSessionState = (Service.SessionState)(int)r["_LastSessionState"];
                if (_LastSessionState == Service.SessionState._COMPLETED)
                {
                    string m = "Service " + service_id + " completed successfully.\nTotal duration: " + (new TimeSpan(0, 0, duration)).ToString() + m1;
                    email(m, service_id, false);
                    DbApi.Connection["UPDATE Services SET _LastSessionState=" + (int)Service.SessionState.COMPLETED + " WHERE Id=@Id"].Execute("@Id", service_id);
                    continue;
                }

                if (_LastSessionState ==Service.SessionState._ERROR)
                {
                    email("Service " + service_id + " exited with error" + m1, service_id);
                    DbApi.Connection["UPDATE Services SET _LastSessionState=" + (int)Service.SessionState.ERROR + " WHERE Id=@Id"].Execute("@Id", service_id);
                    continue;
                }
                
                Process p = GetProcess((int?)r["_LastProcessId"], service_id);
                if (p==null)
                {
                    email("Service " + service_id + " was broken by unknown reason" + m1, service_id);
                    DbApi.Connection["UPDATE Services SET _LastSessionState=" + (int)Service.SessionState.BROKEN + ", _NextStartTime=DATEADD(ss, RestartDelayIfBroken, GETDATE()) WHERE Id=@Id"].Execute("@Id", service_id);
                    continue;
                }

                if (duration >= (int)r["RunTimeout"])
                {                  
                        email("Service " + service_id + " is running " + (new TimeSpan(0, 0, duration)).ToString() + " seconds. It will be killed." + m1, service_id);

                        p = GetProcess((int?)r["_LastProcessId"], service_id);
                        Log.Main.Warning("Killing " + service_id);
                        p.Kill();
                        Thread.Sleep(2000);
                        if (!IsProcessAlive((int?)r["_LastProcessId"], service_id))
                            DbApi.Connection["UPDATE Services SET _LastSessionState=" + (int)Service.SessionState.KILLED + ", _NextStartTime=DATEADD(ss, RestartDelayIfBroken, GETDATE()), _LastEndTime=GETDATE() WHERE Id=@Id"].Execute("@Id", service_id);
                        else
                            Log.Main.Error("Could not kill " + service_id);
                        continue;                    
                }
            }

            ////////////////////////////////////////////////////////////
            //Starting services
            ////////////////////////////////////////////////////////////
            rs = DbApi.Connection[@"SELECT Id, State, Command, AdminEmails,_LastProcessId,ExeFolder FROM Services 
WHERE (State<>" + (int)Service.State.DISABLED + " AND GETDATE()>=_NextStartTime AND Command<>" + (int)Service.Command.STOP + @") 
            OR Command=" + (int)Service.Command.FORCE + " ORDER BY Command, _NextStartTime"].GetRecordset();
            foreach (Record r in rs)
            {
                string service_id = (string)r["Id"];
                Process p = GetProcess((int?)r["_LastProcessId"], service_id);
                if ((int)r["Command"] == (int)Service.Command.FORCE)
                {
                    Log.Main.Write("Forcing " + service_id);
                    if ((int)r["State"] == (int)Service.State.DISABLED)
                    {
                        Log.Main.Error(service_id + " is disabled.");
                        continue;
                    }
                    if (p != null)
                    {
                        Log.Main.Warning(service_id + " is running already.");
                        DbApi.Connection["UPDATE Services SET Command=" + (int)Service.Command.EMPTY + " WHERE Id=@Id"].Execute("@Id", service_id);
                        continue;
                    }
                    if (launch_service(r))
                        DbApi.Connection["UPDATE Services SET Command=" + (int)Service.Command.EMPTY + " WHERE Id=@Id"].Execute("@Id", service_id);
                    continue;
                }

                if (p != null)
                    continue;

                launch_service(r);
            }
        }

        static bool launch_service(Record r)
        {
            string service_id = (string)r["Id"];
            List<string> parameters = new List<string>();
            switch ((Service.State)r["State"])
            {
                case Service.State.DISABLED:
                    LogMessage.Error("Service '" + service_id + "' is disabled.");
                    return false;
                case Service.State.ENABLED:
                    parameters.Add(Service.CommandLineParameters.PRODUCTION.ToString());
                    parameters.Add(Service.CommandLineParameters.AUTOMATIC.ToString());
                    break;
                case Service.State.DEBUG:
                    parameters.Add(Service.CommandLineParameters.AUTOMATIC.ToString());
                    break;
                default:
                    throw new Exception("Case " + r["State"] + " is absent.");
            }

            string service_directory;
            if (!string.IsNullOrWhiteSpace((string)r["ExeFolder"]))
                service_directory = (string)r["ExeFolder"];
            else
                service_directory = Properties.Settings.Default.ServiceDirectory;
            service_directory = Log.GetAbsolutePath(service_directory);
            if (!Directory.Exists(service_directory))
            {
                email("Service directory '" + service_directory + "' does not exist", service_id);
                return false;
            }
            string service_file_name = service_id + ".exe";
            string service_file = FindFile(service_directory, service_file_name);
            if (service_file == null)
            {
                email("Service file '" + service_file_name + "' was not found in " + service_directory, service_id);
                return false;
            }
            Process p = new Process();
            p.StartInfo.FileName = service_file;
            p.StartInfo.Arguments = string.Join(" ", parameters);
            Log.Main.Write("Starting service " + service_id);
            p.Start();
            ThreadRoutines.Wait(Properties.Settings.Default.ServiceCheckDurationInMss);
            if (!IsProcessAlive(p.Id, service_id))
            {
                DbApi.Connection["UPDATE Services SET _NextStartTime=DATEADD(ss, RestartDelayIfBroken, GETDATE()) WHERE Id=@Id"].Execute("@Id", service_id);

                email(service_id + " could not start.", service_id);
                return false;
            }
            Log.Main.Write("Process id: " + p.Id);
            return true;
        }
        
        internal static string FindFile(string parent_directory, string file_name)
        {
            DirectoryInfo di = new DirectoryInfo(parent_directory);
            foreach (FileInfo fi in di.GetFiles())
                if (file_name == fi.Name)
                    return fi.FullName;
            foreach (DirectoryInfo d in di.GetDirectories())
            {
                string f = FindFile(d.FullName, file_name);
                if (f != null)
                    return f;
            }
            return null;
        }

        internal static bool IsProcessAlive(int? id, string name)
        {
            try
            {
                if (id == null)
                    return false;
                Process p = Process.GetProcessById((int)id);
                if (p == null)
                    return false;
                if (!Regex.IsMatch(name, p.ProcessName, RegexOptions.IgnoreCase))
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static Process GetProcess(int? id, string name)
        {
            try
            {
                if (id == null)
                    return null;
                Process p = Process.GetProcessById((int)id);
                if (p == null)
                    return null;
                if (!Regex.IsMatch(name, p.ProcessName, RegexOptions.IgnoreCase))
                    return null;
                return p;
            }
            catch
            {
                return null;
            }
        }

        static public void email(string message, string service_id = null, bool error = true)
        {
            return;
            try
            {
                if (error)
                    Log.Main.Error(message);
                else
                    Log.Main.Inform(message);

                string AdminEmails = null;
                if (service_id != null)
                    AdminEmails = (string)DbApi.Connection["SELECT AdminEmails FROM Services WHERE Id=@Id"].GetSingleValue("@Id", service_id);
                if (AdminEmails == null)
                    AdminEmails = Properties.Settings.Default.DefaultAdminEmails;
                if (AdminEmails != null)
                    AdminEmails = Regex.Replace(AdminEmails.Trim(), @"[\s+\,]+", ",", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
                else
                    Log.Main.Error("No email is defined to send messages.");

                MailMessage m = new MailMessage();
                m.From = new MailAddress(Properties.Settings.Default.EmailSender);
                m.To.Add(AdminEmails);
                string subject = "Crawler Host Service Manager:";
                if (service_id != null)
                    subject += " " + service_id;
                if (error) subject += " error";
                subject += " notification";
                m.Subject = subject;
                m.Body = message;

                System.Net.Mail.SmtpClient c = new SmtpClient(Properties.Settings.Default.SmtpHost, Properties.Settings.Default.SmtpPort);
                c.EnableSsl = true;
                c.DeliveryMethod = SmtpDeliveryMethod.Network;
                c.UseDefaultCredentials = false;
                c.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.SmtpLogin, Properties.Settings.Default.SmtpPassword);

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

        public static void WaitUntilCheckTime()
        {
            long duration = (long)(Process.GetCurrentProcess().StartTime.AddMilliseconds(Properties.Settings.Default.ServiceCheckDurationInMss + 500) - DateTime.Now).TotalMilliseconds;
            if (duration > 0)
                ThreadRoutines.Wait(duration);
        }
    }
}