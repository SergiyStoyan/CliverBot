//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;
using MSHTML;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace Cliver.Bot
{
    /// <summary>
    /// Defines methods for downloading web pages, using IE browser
    /// </summary>
    public partial class IeRoutine : WebRoutine
    {
        public IeRoutine(WebBrowser browser)
        {
            IeRoutines.Invoke(browser, () =>
            {
                if (_CloseWebBrowserDialogsAutomatically)
                {
                    browser.ScriptErrorsSuppressed = true;
                    WindowInterceptor.AddOwnerWindow(browser.Handle);
                }
                browser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(Browser_Navigating);
                browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
                browser.NewWindow += new System.ComponentModel.CancelEventHandler(browser_NewWindow);
                browser.ProgressChanged += new WebBrowserProgressChangedEventHandler(browser_ProgressChanged);
                //browser.ScriptErrorsSuppressed = Properties.Web.Default.flagWebBrowserScriptErrorsSuppressed;   
                this.browser = browser;

                //needed to make google (and probably other sites) work correctly
                //if (!InternetExplorerBrowserEmulation.IsBrowserEmulationSet())
                //    InternetExplorerBrowserEmulation.SetBrowserEmulationVersion();
            });
        }
        //public WebBrowser Browser
        //{
        //    get { return browser; }
        //}

        ~IeRoutine()
        {
            //if (browser != null)
            //{
            //    browser.Stop();
            //    browser.SuspendLayout();
            //    browser.Dispose();
            //    browser = null;
            //}
        }

        internal void CloseIE()
        {
            try
            {
                IeRoutines.Invoke(browser, () =>
                {
                    if (browser != null)
                    {
                        GetDoc("about:blank");
                        browser.Dispose();
                        browser = null;
                    }
                });
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception error)
            {
                Log.Error(error);
            }
        }

        //*********************************************************************************************
        //*****************************************IE ROUTINE*****************************************
        //*********************************************************************************************           


        /// <summary>
        /// Downloaded managed DOM document
        /// </summary>
        public HtmlDocument HtmlDoc;

        /// <summary>
        /// Downloaded mshtml DOM documents of frames (if exists)
        /// </summary>
        public HtmlFrames HtmlFrameDocs = new HtmlFrames();
        public class HtmlFrames
        {
            Dictionary<string, HtmlDocument> html_frame_docs = new System.Collections.Generic.Dictionary<string, HtmlDocument>();
            Hashtable is_saved = new Hashtable();

            /// <summary>
            /// get/set frame's managed DOM document
            /// </summary>
            /// <param name="full_name">full name includes parent frame names joined by "&gt;", e.g. "main&gt;menu"</param>
            /// <returns></returns>
            public HtmlDocument this[string full_name]
            {
                set
                {
                    html_frame_docs[full_name] = value;
                }
                get
                {
                    return (HtmlDocument)html_frame_docs[full_name];
                }
            }

            public ICollection FullNames
            {
                get { return html_frame_docs.Keys; }
            }

            internal bool IsSaved(string full_name)
            {
                object o = is_saved[full_name];
                if (o == null)
                    return false;

                return (bool)o;
            }

            internal void Saved(string full_name)
            {
                is_saved[full_name] = true;
            }

            /// <summary>
            /// it is 0 if no frames
            /// </summary>
            public int Count
            {
                get
                {
                    return html_frame_docs.Count;
                }
            }

            internal void Clear()
            {
                html_frame_docs.Clear();
                is_saved.Clear();
            }
        }

        /// <summary>
        /// Downloaded Windows.Forms.HtmlDocument
        /// </summary>
        //public HtmlDocument HtmlDoc2;

        internal WebBrowser browser = null;

        /// <summary>
        /// Stops bot thread until WebBrowser has completed document.
        /// </summary>
        //ManualResetEvent WaitUntilDocumentCompleted = new ManualResetEvent(true);

        bool IE_result = false;

        /// <summary>
        /// Beging getting document by the url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool Navigate(string url)
        {
            try
            {
                IE_result = false;
                lock (completed_urls)
                {
                    completed_urls.Clear();
                }
                init_loading(url);
                init_loading_progress(url);
                IeRoutines.Invoke(browser, () => { browser.Navigate(url); });
                IE_result = true;
                return IE_result;
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception error)
            {
                web_routine_status = WebRoutineStatus.EXCEPTION;
                Log.Error(error.Message + "\nURL: " + url + "\nIE using");
            }
            IE_result = false;
            return IE_result;
        }

        /// <summary>
        /// Set/unset IE dialog interception
        /// </summary>
        public bool CloseWebBrowserDialogsAutomatically
        {
            private get
            {
                return _CloseWebBrowserDialogsAutomatically;
            }
            set
            {
                _CloseWebBrowserDialogsAutomatically = value;
                if (_CloseWebBrowserDialogsAutomatically)
                    WindowInterceptor.AddOwnerWindow(browser.Handle);
                else
                    WindowInterceptor.Stop();
            }
        }
        bool _CloseWebBrowserDialogsAutomatically = Properties.Browser.Default.CloseWebBrowserDialogsAutomatically;

        void browser_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (Properties.Web.Default.MaxDownloadedFileLength > 0
                && e.CurrentProgress > Properties.Web.Default.MaxDownloadedFileLength
                && e.MaximumProgress > e.CurrentProgress
                )
            {
                web_routine_status = WebRoutineStatus.FILE_TRUNCATED;
                Log.Write("TRUNCATED. URL:" + browser.Url.ToString());
                //Log.Write("EXCEEDED MaxDownloadedFileLength. URL:" + browser.Url.ToString());
                browser.Stop();
            }
            show_progress((int)e.MaximumProgress, (int)e.CurrentProgress);
        }

        public bool GetDocHavingPixel(string url, int timeout_in_mss, Point p, Color c)
        {
            return GetDoc(url, timeout_in_mss, () =>
            {
                if (browser.GetPixelColor(p) == c)
                    return c;
                return null;
            });
        }

        /// <summary>
        /// Get complete document by the url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="return_if_completed_url_matches"></param>
        /// <param name="timeout_in_mss"></param>
        /// <returns></returns>
        public bool GetDoc(string url, Regex return_if_completed_url_matches, int timeout_in_mss = -1)
        {
            return GetDoc(url, timeout_in_mss, () =>
            {
                lock (completed_urls)
                {
                    foreach (string u in completed_urls.Keys)
                    {
                        if (return_if_completed_url_matches.IsMatch(u))
                            return u;
                    }
                }
                return null;
            });
        }

        /// <summary>
        /// Get complete document by the url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout_in_mss"></param>
        /// <param name="return_if_exists"></param>
        /// <returns></returns>
        public bool GetDoc(string url, int timeout_in_mss = -1, Func<object> return_if_exists = null)
        {
            try
            {
                IE_result = false;

                if (UseCache && Cache.GetCachedFile(url, null, out binary_result, out ResponseUrl, out CachedFile))
                {
                    web_routine_status = WebRoutineStatus.CACHED;
                    Log.Write("From cache: " + url);
                    IE_result = true;
                    return true;
                }

                if (Navigate(url))
                {
                    if (return_if_exists == null)
                        IE_result = WaitForCompletion(timeout_in_mss);
                    else
                    {
                        if (timeout_in_mss < 0) timeout_in_mss = Properties.Browser.Default.PageCompletedTimeoutInSeconds * 1000;
                        IE_result = (IeRoutines.WaitForCondition(browser, return_if_exists, timeout_in_mss) != null);
                    }
                    browser.Invoke(() => { HtmlDoc = browser.Document; });
                }
                return IE_result;
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception error)
            {
                web_routine_status = WebRoutineStatus.EXCEPTION;
                Log.Error(error.Message + "\nURL: " + url + "\nIE using");
            }
            IE_result = false;
            return IE_result;
        }

        /// <summary>
        /// Used by bot thread cycle if navigating by click/submit
        /// </summary>
        public bool WaitForCompletion(int timeout_in_mss = -1)
        {
            if (timeout_in_mss < 0)
                timeout_in_mss = Properties.Browser.Default.PageCompletedTimeoutInSeconds * 1000;
            return browser.WaitForCompletion(timeout_in_mss);
        }

        /// <summary>
        /// Begging getting the previous page
        /// </summary>
        public void NavigateBack()
        {
            GetBack(0);
        }

        /// <summary>
        /// Get the previous page
        /// </summary>
        public bool GetBack(int timeout_in_mss = -1)
        {
            return browser.GetBack(timeout_in_mss);
        }

        /// <summary>
        /// Used by bot thread cycle wait while browser downloading document
        /// </summary>
        //public void WaitUntilIEDocumentCompleted2()
        //{
        //    DateTime t = DateTime.Now.Add(new TimeSpan(0, 0, Properties.Web.Default.WebBrowserPageCompletedTimeoutInSeconds));
        //    while (navigating_count > 0)
        //    {
        //        Application.DoEvents();
        //        if (DateTime.Now > t)
        //        {
        //            Log.Error("Browser document completed timeout");
        //            web_routine_status = WebRoutineStatus.TIMEOUT;
        //            return;
        //        }
        //    }
        //}

        //int navigating_count = 0;

        //void document_state_counter(bool completed)
        //{
        //    if (completed)
        //    {
        //        navigating_count--;
        //        if (navigating_count < 1)
        //        {
        //            navigating_count = 0;
        //            if(browser != null)
        //                browser.Stop();
        //            //WaitUntilDocumentCompleted.Set();
        //        }
        //    }
        //    else
        //    {
        //        navigating_count++;
        //    }
        //}

        delegate void _SetEncoding(string encoding);
        /// <summary>
        /// Set encoding for the downloaded page
        /// </summary>
        /// <param name="encoding"></param>
        public void SetEncoding(string encoding)
        {
            try
            {
                if (browser == null)
                    return;

                if (browser.InvokeRequired)
                {
                    browser.Invoke(new _SetEncoding(SetEncoding), encoding);
                    return;
                }

                if (browser.Document == null)
                    return;

                browser.Document.Encoding = encoding;
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception error)
            {
                Log.Error(error);
            }
        }

        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            lock (completed_urls)
            {
                completed_urls[e.Url.ToString()] = 0;
            }

            ResponseUrl = e.Url.ToString();
#if DEBUG
            //Log.Write("navigating_count=" + navigating_count.ToString());
#endif

            try
            {
                if (ResponseUrl == "about:blank")
                {
                    //document_state_counter(true);
                    return;
                }

                if (
                    browser.DocumentTitle == "No page to display"
                    || browser.DocumentTitle == "HTTP 404 Not Found"
                    || browser.DocumentTitle == "The page cannot be displayed"
                    )
                {
                    IE_result = false;
                    ErrorMessage = browser.DocumentTitle;
                }

                try
                {
                    //mshtml.HTMLDocument d = (mshtml.HTMLDocument)browser.Document.DomDocument;
                    //string s = d.documentElement.outerHTML;
                    html_result = browser.DocumentText;
                }
                catch
                {
                    html_result = "";
                }

                //waiting while refreshing/redirecting page
                //if (Regex.IsMatch(HtmlResult, @"<META [^>]*?HTTP-EQUIV=""*Refresh", RegexOptions.Compiled | RegexOptions.IgnoreCase))
                //{
                //    //document_state_counter(true);
                //    return;
                //}

                //ThreadRoutines.Wait(Properties.Web.Default.CrawlTimeIntervalInMss);

                HtmlDoc = browser.Document;

                HtmlFrameDocs.Clear();

                try
                {
                    get_frames(browser.Document.Window, null);
                }
                catch (Exception ex)
                {
                    Log.Main.Error(ex);
                }

                if (HtmlFrameDocs.Count > 0)
                {
                    foreach (string name in HtmlFrameDocs.FullNames)
                    {
                        if (HtmlFrameDocs.IsSaved(name))
                            continue;
                        //mshtml.IHTMLElement ie = ((mshtml.IHTMLDocument3)(HtmlFrameDocs[name].DomDocument)).documentElement;
                        //if (ie == null)
                        //    continue;
                        //Cache.SaveDownloadedFile(ie.outerHTML, get_page_number(), cycle_identifier + "_ie" + "_" + name);
                        //Cache.CacheDownloadedString( browser.Url.AbsoluteUri, null, e.Url.AbsoluteUri, ie.outerHTML, get_page_number(), cycle_identifier + "_ie", WebRoutineStatus.OK);
                        HtmlFrameDocs.Saved(name);
                    }
                }

                Cache.CacheDownloadedFile(browser.Url.AbsoluteUri, null, e.Url.AbsoluteUri, HtmlResult, get_page_number(), cycle_identifier + "_ie", WebRoutineStatus.OK);
                //IE_result = true;
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception error)
            {
                IE_result = false;
                web_routine_status = WebRoutineStatus.EXCEPTION;
                Log.Error(error.Message + "\nURL: " + ResponseUrl + "\nIE using");
                ErrorMessage = error.Message;
            }

            //document_state_counter(true);
        }

        Dictionary<string, int> completed_urls = new Dictionary<string, int>();

        /// <summary>
        /// recursively retrieves frames from browser window
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="window"></param>
        void get_frames(System.Windows.Forms.HtmlWindow window, string path)
        {
            foreach (System.Windows.Forms.HtmlWindow f in window.Frames)
            {
                //if (HtmlFrameDocs == null)
                //    HtmlFrameDocs = new HtmlFrames();

                string current_path = path + ((path == null) ? "" : ">") + f.Name;
                HtmlFrameDocs[current_path] = f.Document;
                get_frames(f, current_path);
            }
        }

        private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            //document_state_counter(false);

            string url = e.Url.ToString();

            try
            {
                if (url == "about:blank")
                {
                    e.Cancel = true;
                    return;
                }
                if (Regex.IsMatch(url, @"\.(gif|jpg|jpeg|png|bmp)&", RegexOptions.Compiled | RegexOptions.IgnoreCase))
                {
                    e.Cancel = true;
                    return;
                }

                HtmlDoc = null;
                //HtmlFrameDocs = null;
                //HtmlFrameDocs.Clear();
                //saved_file = null;

                //init_loading(url);

                //need to watch if browser works too long
                //timer.Interval = Properties.Web.Default.WebBrowserPageCompletedTimeoutInSeconds * 1000;
                //timer.Start();
                //timer.Tick += new EventHandler(timer_Tick);
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception error)
            {
                IE_result = false;
                web_routine_status = WebRoutineStatus.EXCEPTION;
                Log.Error(error.Message + "\nURL: " + url + "\nIE using");
                ErrorMessage = error.Message;
            }
        }

        void browser_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        public IEnumerable<HtmlElement> GetHtmlElementsByAttr(string attribute, string value = null, string tag = null, HtmlElement parent_he = null)
        {
            if (parent_he == null)
                parent_he = HtmlDoc.Body;
            return IeRoutines.GetHtmlElementsByAttr(parent_he, attribute, value, tag);
        }

        public void ClickHtmlElement(HtmlElement he)
        {
            browser.ClickHtmlElement(he);
        }

        delegate IntPtr _MouseLeftClick(Point p, string window_class);
        public IntPtr MouseLeftClick(Point p, string window_class = "Internet Explorer_Server")
        {
            return IntPtr.Zero;
            //if (browser.InvokeRequired)
            //    return (IntPtr)browser.Invoke((_MouseLeftClick)MouseLeftClick, p, window_class);

            //browser.ParentForm.Activate();
            //browser.Focus();
            //IntPtr handle = UserSimulateRoutines.FindWindowHandleByClassName(browser.Handle, window_class);
            //if (IntPtr.Zero == handle) Cliver.Message.Error("no found window!!!");
            //UserSimulateRoutines.MouseLeftClick(browser.Handle, p);
            //return handle;
        }
        
        public void InjectScript(string script)
        {
            HtmlElement he = browser.Document.CreateElement("script");
            he.SetAttribute("text", script);
            browser.Document.Body.AppendChild(he);
        }

        public object RunScript(string function, object[] parameters = null)
        {
            return browser.Document.InvokeScript(function, parameters);
        }
    }
}
