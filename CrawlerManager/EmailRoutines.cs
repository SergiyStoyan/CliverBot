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
    static public class EmailRoutines
    {
        static public void Send(string message, string crawler_id = null, bool error = true)
        {
            try
            {
                if (error)
                    Log.Main.Error(message);
                else
                    Log.Main.Inform(message);

                string AdminEmails = null;
                if (crawler_id != null)
                    AdminEmails = (string)DbApi.Connection["SELECT AdminEmails FROM Crawlers WHERE Id=@Id"].GetSingleValue("@Id", crawler_id);
                if (AdminEmails == null)
                    AdminEmails = Settings.Default.DefaultAdminEmails;
                if (AdminEmails != null)
                    AdminEmails = Regex.Replace(AdminEmails.Trim(), @"[\s+\,]+", ",", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
                else
                    Log.Main.Error("No email is defined to send messages.");

                MailMessage m = new MailMessage();
                m.From = new MailAddress(Settings.Default.EmailSender);
                m.To.Add(AdminEmails);
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
    }
}
