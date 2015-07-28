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
    public class Report
    {
        public string Source{ get; internal set; }
        public ReportSourceType SourceType { get; internal set; }
        public DbApi.MessageType MessageType { get; internal set; }
        public string Value { get; internal set; }
        public string Details { get; internal set; }
        public string Log { get; internal set; }
    }
}