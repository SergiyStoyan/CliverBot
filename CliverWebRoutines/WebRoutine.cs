//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        16 October 2007
//Copyright: (C) 2006-2007, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;
//using MSHTML;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace Cliver.Bot
{
    /// <summary>
    /// Defines base methods for web methods for a sinlge thread
    /// </summary>
    public class WebRoutine
    {
        static WebRoutine()
        {
            Session.Closing += clear_session;
        }

        public bool UseCache = Settings.General.UseFilesFromCache;
        
        /// <summary>
        /// Used to suspend bot thread until user allow (e.g. until he have filled/submitted fields in IE)
        /// </summary>
        public void WaitUntilUserAllow()
        {
            Wait2Continue.Reset();
            Wait2Continue.WaitOne();
        }
        internal ManualResetEvent Wait2Continue = new ManualResetEvent(false);

        //*********************************************************************************************
        //*****************************************WEB ROUTINE*****************************************
        //*********************************************************************************************  

        /// <summary>
        /// Maximal redirection count
        /// </summary>
        public int MaxAutoRedirectionCount = Properties.Web.Default.MaxHttpRedirectionCount;//-1;

        /// <summary>
        /// page counter in order to Log pages
        /// </summary>
        internal static int page_number = 0;

        internal int get_next_page_number()
        {
            lock (static_lock_object)
            {
                return page_number++;
            }
        }
        static object static_lock_object = new object();

        static void clear_session()
        {
            lock (static_lock_object)
            {
                page_number = 0;
            }
        }

        /// <summary>
        /// usually it is name of queue of item processed currently
        /// </summary>
        public string CycleIdentifier = null;

        internal string cycle_identifier
        {
            get
            {
                if (!string.IsNullOrEmpty(CycleIdentifier))
                    return CycleIdentifier;
                string iiqn = BotCycle.GetCurrentInputItemQueueNameThisThread();
                if (iiqn == null)//it can be so while using iebrowser
                    return "undefined";
                return iiqn;
            }
        }

        /// <summary>
        /// Status of web transfer
        /// </summary>
        public WebRoutineStatus Status
        {
            get
            {
                return web_routine_status;
            }
        }
        internal WebRoutineStatus web_routine_status = WebRoutineStatus.UNDEFINED;

        ///// <summary>
        ///// Name of downloaded and saved file
        ///// </summary>
        //public string SavedFile
        //{
        //    get
        //    {
        //        return saved_file;
        //    }
        //}
        //internal string saved_file = null;

        //Error message for the last web request if it was not successful
        public string ErrorMessage = null;

        /// <summary>
        /// Downloaded page
        /// </summary>
        public string HtmlResult
        {
            get
            {
                if (html_result == null && BinaryResult != null)
                    html_result = Encoding.UTF8.GetString(BinaryResult, 0, BinaryResult.Length);
                return html_result;
            }
        }
        protected string html_result = null;

        /// <summary>
        /// Downloaded binary
        /// </summary>
        public byte[] BinaryResult
        {
            get
            {
                return binary_result;
            }
        }
        protected byte[] binary_result = null;

        /// <summary>
        /// Last responsed url
        /// </summary>
        public string ResponseUrl = "";

        /// <summary>
        /// Cached file if used in the last request
        /// </summary>
        public string CachedFile = "";

        public HtmlAgilityPack.HtmlDocument HtmlDocument
        {
            get
            {
                if (HtmlDocument_ == null)
                {
                    HtmlDocument_ = new HtmlAgilityPack.HtmlDocument();
                    HtmlDocument_.LoadHtml(this.HtmlResult);
                    if (HtmlDocument_.ParseErrors != null)
                        foreach (HtmlAgilityPack.HtmlParseError e in HtmlDocument_.ParseErrors)
                            Log.Error("Html parser error: " + e.Reason);
                }
                return HtmlDocument_;
            }
        }
        HtmlAgilityPack.HtmlDocument HtmlDocument_ = null;

        public Proxy Proxy = null;

        /// <summary>
        /// Flag whether proxy should be automatically changed before each request
        /// </summary>
        public bool AutoRotateProxy = true;

        public int MaxAttemptCount = Properties.Web.Default.MaxAttemptCount;

        ///// <summary>
        ///// get next proxy from proxy queue
        ///// </summary>
        public void ChangeProxy()
        {
            this.Proxy = Proxies.Next();
        }

        internal static object static_lock_variable = new object();

        /// <summary>
        /// Defines whether to use cookies from IE contaner
        /// </summary>
        //public bool UseIeCookies = false;

        internal DateTime next_request_time = DateTime.Now;

        internal void init_loading(string url)
        {
            ErrorMessage = null;
            html_result = null;
            binary_result = null;
            CachedFile = null;
            HtmlDocument_ = null;
        }

        internal void init_loading_progress(string url)
        {
            if (url != null)
                Log.Write("Downloading:" + url);
            show_progress(100, 0);
            if (url != null)
            {
                delay(url);
                if (Visible && OnLoading != null)
                    OnLoading.Invoke("URL:" + url);
            }
            web_routine_status = WebRoutineStatus.UNDEFINED;
        }
        
        public delegate void LoadingHandler(string m);
        public LoadingHandler OnLoading = null;

        public delegate void ProgressHandler(int max, int point);
        public ProgressHandler OnProgress = null;

        public bool Visible = true;

        internal void show_progress(int content_length, int received_length)
        {
            if (Visible && OnProgress != null)
                OnProgress.Invoke(content_length, received_length);
        }

        internal void delay(string url)
        {
            if (Properties.Web.Default.CrawlTimeIntervalInMss <= 0)
                return;

            DateTime old_time = DateTime.Now - TimeSpan.FromMilliseconds(Properties.Web.Default.CrawlTimeIntervalInMss);
            lock (last_domains)
            {
                while (last_domains.Count > 0)
                {
                    if ((DateTime)last_domains[0] > old_time)
                        break;
                    last_domains.RemoveAt(0);
                }
            }

            string domain = Regex.Replace(url, @"[^/]*?://(?:[^/]+\.)?([^\./]+\.[^\./]+)(/.*|$)", "$1", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

            object to = last_domains[domain];
            if (to != null)
            {
                TimeSpan wait_time = (DateTime)to - old_time;
                lock (last_domains)
                {
                    last_domains.Remove(domain);
                    last_domains.Add(domain, DateTime.Now + wait_time);
                }
                Thread.Sleep(wait_time);
            }
            else
            {
                lock (last_domains)
                {
                    last_domains.Add(domain, DateTime.Now);
                }
            }
        }
        static OrderedDictionary last_domains = new OrderedDictionary();

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************
                
        public List<string> ExtractLinks(Regex filter = null)
        {
            HtmlAgilityPack.HtmlNodeCollection ns= HtmlDocument.DocumentNode.SelectNodes("/./a");
            if (filter == null)
                return (from n in ns select n.Attributes["href"].Value).ToList();

            return (from n in ns where filter.IsMatch(n.Attributes["href"].Value) select n.Attributes["href"].Value).ToList();
        }
    }


    /// <summary>
    /// Status of web transport
    /// </summary>
    public enum WebRoutineStatus
    {
        UNDEFINED,
        OK,//do not change! It is used by Cache
        BAD_PROXY,
        TIMEOUT, //download timeout
        UNACCEPTABLE_CONTENT_TYPE,
        DOWNLOAD_ERROR,
        EXCEPTION,
        FILE_TRUNCATED,
        ABORTED,
        REDIRECTION,
        CACHED
    }
}
