using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;

namespace Cliver.BotWeb
{
    public partial class Settings
    {
        public static readonly WebClass Web;

        public class WebClass : Cliver.Settings
        {
            public string TextModeDownloadableContentTypePattern = "text|json";
            public string TextModeHttpRequestAcceptHeader = "text/xml,text/html;q=0.9,text/plain;q=0.8";
            public int CrawlTimeIntervalInMss = 5000;
            public int HttpRequestTimeoutInSeconds = 30;
            public int MaxDownloadedFileLength = 1000000;
            public int MaxHttpRedirectionCount = 20;
            public int MaxAttemptCount = 1;
            public string HttpUserAgent = "CliverBot 4.*";
            public bool UseFilesFromCache = true;
            public bool LogDownloadedFiles = true;
            public bool LogPostRequestParameters = true;
        }
    }
}