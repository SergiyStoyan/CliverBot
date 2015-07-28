//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Configuration;
using System.Web;
using System.Net.NetworkInformation;
using Cliver.Bot;
using System.Net.Mail;

namespace Cliver.CrawlerHost
{
        public enum ReportSourceType
        {
            MANAGER,
            SERVICE,
            CRAWLER
        }

    public static class Mailer
    {

        static public void Send(string message, ReportSourceType source_type, string source_id = null, bool error = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Properties.Settings.Default.SmtpHost))
                {
                    Log.Main.Warning("Message not emailed as SmtpHost is not specified:\n" + (error ? "ERROR:" : "" + source_id) + ":\n" + message);
                    return;
                }

                if (error)
                    Log.Main.Error(message);
                else
                    Log.Main.Inform(message);

                string AdminEmails = null;
                if (source_id != null)
                    switch (source_type)
                    {
                        case ReportSourceType.CRAWLER:
                            AdminEmails = (string)DbApi.Connection["SELECT AdminEmails FROM Crawlers WHERE Id=@Id"].GetSingleValue("@Id", source_id);
                            break;
                        case ReportSourceType.SERVICE:
                            AdminEmails = (string)DbApi.Connection["SELECT AdminEmails FROM Services WHERE Id=@Id"].GetSingleValue("@Id", source_id);
                            break;
                        case ReportSourceType.MANAGER:
                            break;
                        default:
                            throw new Exception("Option is not defined.");
                    }
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
                if (source_id != null)
                    subject += " " + source_id;
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
     
    }
}

