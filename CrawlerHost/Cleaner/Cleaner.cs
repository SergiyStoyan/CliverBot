using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.Bot;
using System.Text.RegularExpressions;
using Cliver.CrawlerHost;

namespace Cliver.CrawlerHostCleaner
{
    class Cleaner
    {
        public static void Do()
        {
            //EmailRoutines.Send

            DbApi.Connection.Get("DELETE FROM Messages WHERE Time<@OldTime").Execute("@OldTime", DateTime.Now.AddDays(-Properties.Settings.Default.DeleteMessagesOlderThanDays));

            Recordset ps = DbApi.Connection.Get(@"SELECT name FROM sysobjects WHERE name LIKE 'PRODUCTS_%' and xtype='U'").GetRecordset();
            foreach (Record p in ps)
            {
                if (null != DbApi.Connection["SELECT Id FROM Crawlers WHERE _ProductsTable=@ProductsTable"].GetSingleValue("@ProductsTable", p["name"]))
                    continue;
                DbApi.Message(DbApi.MessageType.WARNING, "Deleting table '" + p["name"] + "' as not crawler owning it.");
                DbApi.Connection.Get("DROP TABLE @ProductsTable").Execute("@ProductsTable", p["name"]);
            }
        }
    }
}