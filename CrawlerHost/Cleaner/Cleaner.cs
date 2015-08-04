using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.Bot;
using System.Text.RegularExpressions;
using Cliver.CrawlerHost;
using System.IO;

namespace Cliver.CrawlerHostCleaner
{
    public class Cleaner : CrawlerHost.Service
    {
        override protected void Do()
        {
            //EmailRoutines.Send

            DateTime old_time = DateTime.Now.AddDays(-Properties.Settings.Default.DeleteMessagesOlderThanDays);
            int c = DbApi.Connection.Get("DELETE FROM Messages WHERE Time<@OldTime").Execute("@OldTime", old_time);
            if(c > 0)
                Log.Inform("Messages older than " + old_time.ToShortDateString() + " have been cleaned up.");

            Recordset ps = DbApi.Connection.Get(@"SELECT name FROM sysobjects WHERE name LIKE 'Products_%' and xtype='U'").GetRecordset();
            foreach (Record p in ps)
            {
                if (null != DbApi.Connection["SELECT Id FROM Crawlers WHERE _ProductsTable=@ProductsTable"].GetSingleValue("@ProductsTable", p["name"]))
                    continue;
                Log.Warning("Deleting table '" + p["name"] + "' as not crawler owning it.");
                DbApi.Connection.Get("DROP TABLE @ProductsTable").Execute("@ProductsTable", p["name"]);
            }

            if (!string.IsNullOrWhiteSpace(Cliver.Bot.Properties.Log.Default.PreWorkDir))
                clear_files_older_than(new DirectoryInfo(Cliver.Bot.Properties.Log.Default.PreWorkDir), DateTime.Now.AddDays(-Properties.Settings.Default.DeleteLogsOlderThanDays));
            else
                Log.Error("Log directory does not exists: " + Cliver.Bot.Properties.Log.Default.PreWorkDir);
        }

        void clear_files_older_than(DirectoryInfo pdi, DateTime old_time)
        {
            foreach (FileInfo fi in pdi.GetFiles())
                if (fi.LastWriteTime < old_time)
                {
                    Log.Main.Write("Deleting: " + fi.FullName);
                    fi.Delete();
                }
            foreach (DirectoryInfo di in pdi.GetDirectories())
            {
                clear_files_older_than(di, old_time);
                if (di.GetFiles().Length < 1 && di.GetDirectories().Length < 1)
                {
                    Log.Main.Write("Deleting: " + di.FullName);
                    di.Delete();
                }
            }
        }
    }
}