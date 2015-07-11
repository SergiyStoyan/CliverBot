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
using System.Windows.Forms;
using System.Collections;
using SeasideResearch.LibCurlNet;
using System.IO.Compression;


namespace CliverSoft
{
    /// <summary>
    /// Defines methods for web downloading files, using Curl client
    /// </summary>
    public class CurlRoutine
    {
        WebRoutine WR = null;

        /// <summary>
        /// Create instance of curl transport
        /// </summary>
        /// <param name="web_routine">parent class/param>
        internal CurlRoutine(WebRoutine web_routine)
        {
            try
            {
                WR = web_routine;

                if (Curl.GlobalInit((int)CURLinitFlag.CURL_GLOBAL_ALL) == CURLcode.CURLE_FAILED_INIT)
                    WR.log.Exit("Cannot initiate curl");

                CookieFile = Log.SessionDir + "\\" + "cookies_" + WR.BTC.ThreadId.ToString() + ".txt";
            }
            catch (Exception e)
            {
                WR.log.Exit(e);
            }
        }

        //*********************************************************************************************
        //*****************************************WEB ROUTINE*****************************************
        //*********************************************************************************************           
        public string CookieFile = null;

        //public void SetCookies(string url, string cookie)
        //{
        //    //CookieFile;
        //}

        //public string GetCookies(string url)
        //{
        //    //CookieFile;
        //    return "";
        //}

        static public bool UseCommonCookieFile = false;

        static string common_cookie_file = Log.SessionDir + "\\" + "common_cookie.txt";

        void copy_thread_cookie_file2common_cookie_file()
        {
            lock (WebRoutine.static_lock_variable)
            {
                try
                {
                    if (!File.Exists(CookieFile))
                        return;
                    File.Copy(CookieFile, common_cookie_file, true);
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception error)
                {
                    WR.log.Error(error);
                }
            }
        }

        void copy_common_cookie_file2thread_cookie_file()
        {
            lock (WebRoutine.static_lock_variable)
            {
                try
                {
                    if (!File.Exists(common_cookie_file))
                        return;
                    File.Copy(common_cookie_file, CookieFile, true);
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception error)
                {
                    WR.log.Error(error);
                }
            }
        }

        Proxy proxy = null;

        //public void SetProxy(string proxy_ip, string proxy_port, string proxy_login, string proxy_password)
        //{
        //    proxy = new Proxy();
        //    proxy.Ip = proxy_ip;
        //    proxy.Port = proxy_port;
        //    proxy.Login = proxy_login;
        //    proxy.Password = proxy_password;
        //}

        /// <summary>
        /// Flag whether proxy should be automatically changed before each request
        /// </summary>
        public bool AutoRotateProxy = true;

        /// <summary>
        /// get next proxy from proxy queue
        /// </summary>
        public void ChangeProxy()
        {
            proxy = Proxies.Next();
        }

        /// <summary>
        /// clear current cookie container
        /// </summary>
        public void ClearCookies()
        {
			TextWriter tw = new StreamWriter(CookieFile, false);
			tw.Close();
        }

        /// <summary>
        /// Downloads web page with sending cookie
        /// </summary>
        /// <param name="url">url of page</param>
        /// <returns>whether reqest was successful</returns>
        public bool GetPage(string url)
        {
            return GetPage(url, true);
        }

        //static object lock_variable = new object();
              
        /// <summary>
        /// Downloads web page synchronously
        /// </summary>
        /// <param name="url">url of page</param>
        /// <param name="send_cookies">defines if cookies should be sent</param>
        /// <returns>reqest status</returns>
        public bool GetPage(string url, bool send_cookies)
        {
            if (AutoRotateProxy)
                proxy = Proxies.Next();
            int attempt_count = 1;
            CURLcode rc = load_page(url, send_cookies);
            while (proxy != null
                && (rc == CURLcode.CURLE_GOT_NOTHING
                || rc == CURLcode.CURLE_COULDNT_RESOLVE_PROXY
                || rc == CURLcode.CURLE_OPERATION_TIMEOUTED
                || rc == CURLcode.CURLE_COULDNT_CONNECT
                || rc == CURLcode.CURLE_SEND_ERROR)
                )
            {
                Proxies.Delete(proxy);
                if (attempt_count >= Config.Proxy.MaxAttemptCountWithNewProxy)
                {
                    WR.log.Error("Attempt quota exeeded: " + attempt_count.ToString());
                    break;
                }
                proxy = Proxies.Next();
                attempt_count++;
                WR.log.Write("Attempt #: " + attempt_count.ToString());
                rc = load_page(url, send_cookies);    
            }
            return rc == CURLcode.CURLE_OK;
        }

        /// <summary>
        /// Downloads web page by POST synchronously with sending cookie. 
        /// </summary>
        /// <param name="form">form with parameters. /*//NOTICE: parameters with null value will not be submitted.*/</param>
        /// <param name="send_cookies">defines if cookies should be sent</param>
        /// <returns>reqest status</returns>
        //public bool PostPage(HtmlForm form)
        //{
        //    return PostPage(form, true);
        //}

        /// <summary>
        /// Downloads web page by POST synchronously. 
        /// </summary>
        /// <param name="form">form with parameters. /*//NOTICE: parameters with null value will not be submitted.*/</param>
        /// <param name="send_cookies">defines if cookies should be sent</param>
        /// <returns>reqest status</returns>
        //public bool PostPage(HtmlForm form, bool send_cookies)
        //{
        //    return load_page(null, form, send_cookies);
        //}

        /// <summary>
        /// Defines whether to use cookies from IE contaner
        /// </summary>
        //public bool UseIeCookies = false;

        Easy easy = null;

        /// <summary>
        /// Universal method to load page
        /// </summary>
        /// <param name="url">url to load. It is ignored if form is not null.</param>
        /// <param name="form">form to submit. Should be null if url is not null.</param>
        /// <param name="send_cookies"></param>
        /// <returns></returns>
        CURLcode load_page(string url, bool send_cookies)
        {
            try
            {
                if (Cache.GetCachedFile(ref url, out WR.HtmlResult, out WR.ResponseUrl))
                {
                    WR.web_routine_status = WebRoutineStatus.CACHED;
                    WR.log.Write("From cache: " + url);
                    return CURLcode.CURLE_OK;
                }

                if (UseCommonCookieFile)
                    copy_common_cookie_file2thread_cookie_file();

                result_ms.Position = 0;
                result_sb.Length = 0;
                encode_type = EncodeType.NONE;
                easy = new Easy(); 
                content_length = -1;
                WR.ErrorMessage = null;
                WR.HtmlResult = null;
                //WR.saved_file = null;
                WR.log.Write("Downloading:" + url);
                if (WR.BTC != null)
                {
                    WR.BTC.BTS.ProgressMax = 0;
                    WR.BTC.BTS.ProgressValue = 100;
                    WR.BTC.BTS.Status = "Sleeping Crawl Interval: " + Config.Web.CrawlTimeIntervalInMss.ToString();
                    WR.BTC.DisplayStatus();
                    Thread.Sleep(Config.Web.CrawlTimeIntervalInMss);
                    WR.BTC.BTS.Status = "URL:" + url;
                    WR.BTC.DisplayStatus();
                }
                WR.web_routine_status = WebRoutineStatus.UNDEFINED;

                //if (flagReceiveDebugMessages)
                //{
                //    Easy.DebugFunction df = new Easy.DebugFunction(OnDebug);
                //    easy.SetOpt(CURLoption.CURLOPT_DEBUGFUNCTION, df);
                //    easy.SetOpt(CURLoption.CURLOPT_VERBOSE, true);
                //}

                Easy.WriteFunction wf = new Easy.WriteFunction(OnWriteStringBuilder);
                easy.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, wf);

                easy.SetOpt(CURLoption.CURLOPT_URL, url);

                if(url.StartsWith("https"))
                    easy.SetOpt(CURLoption.CURLOPT_CAINFO, "ca-bundle.crt");

                Easy.HeaderFunction hf = new Easy.HeaderFunction(OnHeaderData);
                easy.SetOpt(CURLoption.CURLOPT_HEADERFUNCTION, hf);
                
                Slist sl = new Slist();
                sl.Append("Accept: " + WR.HttpRequestAcceptHeader);
                sl.Append("Referer: " + WR.HttpRequestRefererHeader);
                sl.Append("Accept-Encoding: gzip, deflate");
                easy.SetOpt(CURLoption.CURLOPT_HTTPHEADER, sl);

                easy.SetOpt(CURLoption.CURLOPT_USERAGENT, Config.Web.HttpUserAgent);

                easy.SetOpt(CURLoption.CURLOPT_COOKIEJAR, CookieFile);

                if (send_cookies)
                {
                    easy.SetOpt(CURLoption.CURLOPT_COOKIEFILE, CookieFile);
                }

                if (WR.MaxAutoRedirectionCount >= 0)
                    easy.SetOpt(CURLoption.CURLOPT_MAXREDIRS, WR.MaxAutoRedirectionCount);

                //Easy.ProgressFunction pf = new Easy.ProgressFunction(OnProgress);
                //easy.SetOpt(CURLoption.CURLOPT_PROGRESSDATA, );
                
                easy.SetOpt(CURLoption.CURLOPT_FOLLOWLOCATION, true);

                if (proxy != null)
                {
                    easy.SetOpt(CURLoption.CURLOPT_PROXY, proxy.Ip);
                    easy.SetOpt(CURLoption.CURLOPT_PROXYPORT, proxy.Port);
                    
                    easy.SetOpt(CURLoption.CURLOPT_PROXYAUTH, CURLhttpAuth.CURLAUTH_ANY);
                    if(proxy.Login != "" || proxy.Password != "")
                        easy.SetOpt(CURLoption.CURLOPT_PROXYUSERPWD, proxy.Login + ":" + proxy.Password);
                    if (proxy.Type == ProxyType.SOCKS5)
                        easy.SetOpt(CURLoption.CURLOPT_PROXYTYPE, CURLproxyType.CURLPROXY_SOCKS5);
                    else if (proxy.Type == ProxyType.SOCKS4)
                        easy.SetOpt(CURLoption.CURLOPT_PROXYTYPE, CURLproxyType.CURLPROXY_SOCKS4);
                    else
                        easy.SetOpt(CURLoption.CURLOPT_PROXYTYPE, CURLproxyType.CURLPROXY_HTTP);
                    easy.SetOpt(CURLoption.CURLOPT_PROXYAUTH, CURLhttpAuth.CURLAUTH_ANY);
                }
                if (WR.BTC != null)
                {
                    if (proxy != null)
                        WR.BTC.BTS.Proxy = proxy.Ip + ":" + proxy.Port;
                    else
                        WR.BTC.BTS.Proxy = "NOT USED";
                    WR.BTC.DisplayStatus();
                }
                
                easy.SetOpt(CURLoption.CURLOPT_TIMEOUT, Config.Web.HttpRequestTimeoutInSeconds);

                CURLcode rc = easy.Perform();

                easy.GetInfo(CURLINFO.CURLINFO_EFFECTIVE_URL, ref WR.ResponseUrl);

                easy.Cleanup();
             
                WR.HtmlResult = result_sb.ToString();

                if (rc != CURLcode.CURLE_OK)
                {
                    WR.ErrorMessage = easy.StrError(rc);
                    WR.web_routine_status = WebRoutineStatus.DOWNLOAD_ERROR;
                    WR.log.Error("Download error: " + WR.ErrorMessage + "\nURL:" + url + "\nPROXY: " + proxy.Ip + ":" + proxy.Port);
                    return rc;
                }
                
                if (UseCommonCookieFile)
                    copy_thread_cookie_file2common_cookie_file();

                if (WR.web_routine_status == WebRoutineStatus.UNDEFINED)
                    WR.web_routine_status = WebRoutineStatus.OK;

                WR.log.CacheDownloadedString(ref url, ref WR.ResponseUrl, ref  WR.HtmlResult, WR.page_number++, WR.CycleIdentifier);

                return rc;
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception error)
            {
                WR.web_routine_status = WebRoutineStatus.EXCEPTION;
                WR.log.Error(error.Message + "\nURL: " + url + "\nPROXY: " + proxy.Ip + ":" + proxy.Port);

                if (easy != null)
                    easy.Cleanup();
            }

            return CURLcode.CURLE_FAILED_INIT;
        }

        //Int32 OnWriteString(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        //{
        //    try
        //    {
        //        WR.HtmlResult += System.Text.Encoding.UTF8.GetString(buf);

        //        if (Config.Web.MaxDownloadedFileLength > 0
        //            && WR.HtmlResult.Length > Config.Web.MaxDownloadedFileLength
        //            )
        //        {
        //            easy.Cleanup();
        //            WR.web_routine_status = WebRoutineStatus.FILE_TRUNCATED;
        //            WR.log.Write("TRUNCATED.");
        //        }
        //    }
        //    catch (ThreadAbortException)
        //    {
        //    }
        //    catch (Exception error)
        //    {
        //        WR.log.Error(error);
        //    }
        //    return size * nmemb;
        //}
        
        MemoryStream result_ms = new MemoryStream();
        byte[] buf2 = new byte[10000];
        GZipStream gzip = null;
        DeflateStream deflate = null;
        StringBuilder result_sb = new StringBuilder();
        EncodeType encode_type = EncodeType.NONE; 
        enum EncodeType
        {
            GZIP,
            DEFLATE,
            NONE
        }

        Int32 OnWriteStringBuilder(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            int buf_len = size * nmemb;

            try
            {
                received_length += buf_len;
                if (encode_type == EncodeType.GZIP)
                {
                    result_ms.Write(buf, 0, buf_len);
                    result_ms.Position -= buf_len;
                    int count = 0;
                    do
                    {
                        count = gzip.Read(buf2, 0, buf2.Length);
                        result_sb.Append(System.Text.Encoding.UTF8.GetString(buf2, 0, count));
                    }
                    while (count > 0);
                }
                else if (encode_type == EncodeType.DEFLATE)
                {
                    result_ms.Write(buf, 0, buf_len);
                    result_ms.Position -= buf_len;
                    int count = 0;
                    do
                    {
                        count = deflate.Read(buf2, 0, buf2.Length);
                        result_sb.Append(System.Text.Encoding.UTF8.GetString(buf2, 0, count));
                    }
                    while (count > 0);
                }
                else
                    result_sb.Append(System.Text.Encoding.UTF8.GetString(buf));

                if (WR.BTC != null)
                {
                    WR.BTC.BTS.ProgressMax = content_length;
                    WR.BTC.BTS.ProgressValue = received_length;
                    WR.BTC.DisplayStatus();
                }

                if (Config.Web.MaxDownloadedFileLength > 0
                    && result_sb.Length > Config.Web.MaxDownloadedFileLength
                    )
                {
                    easy.Cleanup();
                    WR.web_routine_status = WebRoutineStatus.FILE_TRUNCATED;
                    WR.log.Write("TRUNCATED.");
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception error)
            {
                WR.log.Error(error);
            }

            return buf_len;
        }

        //void OnDebug(CURLINFOTYPE infoType, String msg,
        //    Object extraData)
        //{
        //    DebugMessage += msg;
        //}

        int content_length = -1;
        int received_length = 0;

        Int32 OnHeaderData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            try
            {
                string s = System.Text.Encoding.ASCII.GetString(buf);

                //Match m = Regex.Match(s, @"Set-Cookie:\s*(.*?)(;\s*expires\s*=|;\s*domain\s*=|;\s*path\s*=|\r\n)", RegexOptions.Singleline);
                //if (m.Success)
                //    Cookie = m.Result("$1");

                Match m = Regex.Match(s, @"Content-Length:\s*(\d+)", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    content_length = int.Parse(m.Result("$1"));
                    received_length = 0;
                    return size * nmemb;
                }

                m = Regex.Match(s, @"Content-Encoding:\s*(\w+)", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    if (m.Result("$1").Contains("gzip"))
                    {
                        encode_type = EncodeType.GZIP;
                        result_ms = new MemoryStream();
                        gzip = new GZipStream(result_ms, CompressionMode.Decompress);
                    }
                    else if (m.Result("$1").Contains("deflate"))
                    {
                        encode_type = EncodeType.DEFLATE;
                        result_ms = new MemoryStream();
                        deflate = new DeflateStream(result_ms, CompressionMode.Decompress);
                    }

                    return size * nmemb;
                }

                m = Regex.Match(s, @"Content-Type:\s*(.+)", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    if (!Regex.IsMatch(m.Result("$1"), WR.DownloadableContentTypePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                    {
                        easy.Cleanup();
                        WR.web_routine_status = WebRoutineStatus.UNACCEPTABLE_CONTENT_TYPE;
                        WR.log.Write("Unacceptable Content-Type:" + m.Result("$1"));
                    }
                    return size * nmemb;
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception error)
            {
                WR.log.Error(error);
            }

            return size * nmemb;
        }
    }
}
