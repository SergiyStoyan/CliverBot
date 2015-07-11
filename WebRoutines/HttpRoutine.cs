//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        1 October 2007
//Copyright: (C) 2006-2007, Sergey Stoyan
//********************************************************************************************

using System;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace Cliver.Bot
{
    /// <summary>
    /// Defines methods for getting files from web, using .NET http client
    /// </summary>
    public partial class HttpRoutine:WebRoutine
    {
        public HttpRoutine()
        {
        }

        /// <summary>
        /// The last response object
        /// </summary>
        public HttpWebResponse HWResponse = null;

        //Check if network is available reasoning from the last web request
        public bool NetIsAvailable
        {
            get
            {
                if (ErrorMessage == null)
                    return true;

                if (ErrorMessage.Contains("The remote name could not be resolved")
                    || ErrorMessage.Contains("Unable to connect to the remote server")
                    )
                {
                    try
                    {
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.google.com/");
                        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        
        public CookieContainer CookieContainer = new CookieContainer();
        public static CookieContainer CommonCookieContainer = new CookieContainer();

        /// <summary>
        /// Set/unset cookies used/retrieved by the bot thread are common
        /// </summary>
        public bool UseCommonCookieContainer = false;

        /// <summary>
        /// Set user credentials required by some sites
        /// </summary>
        /// <param name="login">user name</param>
        /// <param name="password">user password</param>
        /// <param name="url">url</param>
        public void AddNetworkCredential(string login, string password, string url)
        {
            NetworkCredential credential = new NetworkCredential(login, password);
            credential_cache.Add(new Uri(url), "Basic", credential);
        }
        CredentialCache credential_cache = new CredentialCache();

        /// <summary>
        /// Downloads a binary file ignoring TextModeDownloadableContentTypePattern
        /// </summary>
        /// <param name="url">url of page</param>
        /// <returns>whether reqest was successful</returns>
        public bool Get(string url, bool send_cookies = true)
        {
            return Do(new HttpRequest(url, new Dictionary<string, string>() { { "Accept", "*/*" } }), send_cookies);
        }
        
        /// <summary>
        /// Downloads web page.
        /// </summary>
        /// <param name="url">url of page</param>
        /// <param name="send_cookies">defines if cookies should be sent</param>
        /// <returns>reqest status</returns>
        public bool GetPage(string url, bool send_cookies = true)
        {
            return Do(new HttpRequest(url, new Dictionary<string, string>() { { "Accept", Properties.Web.Default.TextModeHttpRequestAcceptHeader } }), send_cookies);
        }
        
        /// <summary>
        /// Downloads web page by POST. 
        /// </summary>
        /// <param name="form">form with parameters. /*//NOTICE: parameters with null value will not be submitted.*/</param>
        /// <param name="send_cookies">defines if cookies should be sent</param>
        /// <returns>reqest status</returns>
        public bool PostPage(HtmlForm form, bool send_cookies = true)
        {
            return Do(new HttpRequest(form, new Dictionary<string, string>() { { "Accept", Properties.Web.Default.TextModeHttpRequestAcceptHeader } }), send_cookies);
        }

        /// <summary>
        /// Downloads a binary file ignoring TextModeDownloadableContentTypePattern
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="content_type"></param>
        /// <param name="send_cookies"></param>
        /// <returns></returns>
        public bool Post(string url, byte[] data, string content_type, bool send_cookies = true)
        {
            return Do(new HttpRequest(url, new Dictionary<string, string>() { { "Content-Type", content_type }, { "Accept", "*/*" } }, HttpRequest.RequestMethod.POST, data), send_cookies);
        }

        /// <summary>
        /// Defines whether to use cookies from IE contaner
        /// </summary>
        public bool UseIeCookies = false;

        /// <summary>
        /// Perform HttpRequest.
        /// </summary>
        /// <param name="http_request"></param>
        /// <param name="send_cookies"></param>
        /// <returns></returns>
        public bool Do(HttpRequest http_request, bool send_cookies)
        {
            if (AutoRotateProxy)
                ChangeProxy();

            int attempt_count = 0;
            bool rc;
            while (true)
            {
                attempt_count++;
                rc = _do(http_request, send_cookies);
                if (rc)
                    break;
                if (Proxy != null)
                {
                    if (web_routine_status != WebRoutineStatus.EXCEPTION
                        && HWResponse.StatusCode != HttpStatusCode.ProxyAuthenticationRequired
                        && HWResponse.StatusCode != HttpStatusCode.GatewayTimeout
                        && HWResponse.StatusCode != HttpStatusCode.BadGateway
                    )
                        break;

                    Proxies.Delete(Proxy);
                    if (attempt_count >= Properties.Proxy.Default.MaxAttemptCountWithNewProxy)
                    {
                        Log.Error("Attempt quota exeeded: " + attempt_count.ToString());
                        break;
                    }
                    ChangeProxy();
                }
                if (attempt_count >= MaxAttemptCount)
                    break;
                Log.Write("Attempt #: " + attempt_count.ToString());
            }
            return rc;
        }

        /// <summary>
        /// Universal method to load page
        /// </summary>
        bool _do(HttpRequest http_request, bool send_cookies)
        {
            Stream res_stream = null;
            try
            {
                init_loading(http_request.Url);
                if (UseCache)
                {
                    if (Cache.GetCachedFile(http_request.Url, http_request.PostString, out binary_result, out ResponseUrl, out CachedFile))
                    {
                        web_routine_status = WebRoutineStatus.CACHED;
                        Log.Write("From cache: " + http_request.Url);
                        return true;
                    }
                }

                init_loading_progress(http_request.Url);

                if (http_request.Method == HttpRequest.RequestMethod.POST)
                    System.Net.ServicePointManager.Expect100Continue = false;

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(http_request.Url);

                req.Credentials = credential_cache;

                req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                if (send_cookies)
                {
                    if (UseCommonCookieContainer)
                        lock (static_lock_variable)
                        {
                            CookieContainer = HttpRoutine.CommonCookieContainer;
                        }

                    if (UseIeCookies)
                    {
                        Uri uri = new Uri(http_request.Url);
                        string cookie = IeRoutines.RetrieveIeCookies(uri);
                        if (cookie.Length > 0)
                        {
                            cookie = Regex.Replace(cookie, ";", ",");
                            CookieContainer.SetCookies(uri, cookie);
                        }
                    }

                    req.CookieContainer = CookieContainer;
                }

                if (Proxy != null)
                    req.Proxy = Proxy.WebProxy;

                req.Timeout = Properties.Web.Default.HttpRequestTimeoutInSeconds * 1000;
                req.ReadWriteTimeout = Properties.Web.Default.HttpRequestTimeoutInSeconds * 1000;

                if (HttpRequest.MaxAutoRedirectionCount == 0)
                    req.AllowAutoRedirect = false;
                else if (HttpRequest.MaxAutoRedirectionCount > 0)
                {
                    req.AllowAutoRedirect = true;
                    req.MaximumAutomaticRedirections = HttpRequest.MaxAutoRedirectionCount;
                }

                foreach (KeyValuePair<string, string> header in http_request.Headers)
                {
                    if (header.Value == null)
                        continue;
                    switch (header.Key)
                    {
                        case "User-Agent":
                            req.UserAgent = header.Value;
                            break;
                        case "Accept":
                            req.Accept = header.Value;
                            break;
                        case "Referer":
                            req.Referer = header.Value;
                            break;
                        case "Connection":
                            if(Regex.IsMatch(header.Value, "keep-alive", RegexOptions.IgnoreCase))
                                req.KeepAlive = true;
                            break;
                        case "Content-Type":
                            req.ContentType = header.Value;
                            break;
                        case "Expect":
                            req.Expect = header.Value;
                            break;
                        default:
                            req.Headers[header.Key] = header.Value;
                            break;
                    }
                }

                if (http_request.Method == HttpRequest.RequestMethod.POST)
                {
                    req.Method = "POST";
                    if (http_request.PostData != null)
                    {
                        req.ContentLength = http_request.PostData.Length;

                        Stream req_stream = req.GetRequestStream();
                        req_stream.Write(http_request.PostData, 0, http_request.PostData.Length);
                        req_stream.Close();
                    }
                }

                HWResponse = (HttpWebResponse)req.GetResponse();

                ResponseUrl = HWResponse.ResponseUri.ToString();

                if (HWResponse.StatusCode == HttpStatusCode.Redirect)
                {
                    web_routine_status = WebRoutineStatus.REDIRECTION;
                    throw new Exception("Redirected.\nURL:" + http_request.Url);
                }

                if (send_cookies)
                {
                    if (UseCommonCookieContainer)
                        lock (static_lock_variable)
                        {
                            HttpRoutine.CommonCookieContainer.Add(HWResponse.Cookies);
                        }
                    else
                        CookieContainer.Add(HWResponse.Cookies);
                }

                string accept;
                if (http_request.Headers.TryGetValue("Accept", out accept) 
                    && accept == Properties.Web.Default.TextModeHttpRequestAcceptHeader
                    && !Regex.IsMatch(HWResponse.ContentType, Properties.Web.Default.TextModeDownloadableContentTypePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase)
                )
                {
                    web_routine_status = WebRoutineStatus.UNACCEPTABLE_CONTENT_TYPE;
                    throw new Exception("Unacceptable Content-Type:" + HWResponse.ContentType + "\nURL:" + http_request.Url);
                }

                MemoryStream result_ms = new MemoryStream();

                int progress_max = (int)HWResponse.ContentLength;

                res_stream = HWResponse.GetResponseStream();

                byte[] buff = new byte[8192];
                int total_byte_count = 0;
                while (true)
                {
                    int byte_count = res_stream.Read(buff, 0, buff.Length);

                    if (byte_count < 1)
                        break;
                    
                    result_ms.Write(buff, 0, byte_count);

                    total_byte_count += byte_count;

                    show_progress(progress_max, total_byte_count);

                    if (Properties.Web.Default.MaxDownloadedFileLength > 0
                        && total_byte_count > Properties.Web.Default.MaxDownloadedFileLength
                        )
                    {
                        web_routine_status = WebRoutineStatus.FILE_TRUNCATED;
                        Log.Write("TRUNCATED. URL:" + http_request.Url);
                        break;
                    }
                }
                
                binary_result = result_ms.ToArray();

                //if (res.StatusCode != HttpStatusCode.OK)
                //{
                //    web_routine_status = WebRoutineStatus.DOWNLOAD_ERROR;
                //    string proxy = "";
                //    if (Proxy != null && Proxy.Address != null && Proxy.Address.Authority != null)
                //        proxy = Proxy.Address.Authority;
                //    Log.Error("Download error: " + res.StatusDescription + "\nURL:" + url + "\nPROXY: " + proxy);
                //    return false;
                //}

                if (web_routine_status == WebRoutineStatus.UNDEFINED)
                    web_routine_status = WebRoutineStatus.OK;

                bool content_is_text = false;
                if (Regex.IsMatch(HWResponse.ContentType, "text|json|xml", RegexOptions.Compiled | RegexOptions.IgnoreCase))
                    content_is_text = true;
                CachedFile = Cache.CacheDownloadedFile(content_is_text, http_request.Url, http_request.PostString, ResponseUrl, BinaryResult, get_page_number(), cycle_identifier, web_routine_status);

                return true;
            }
            catch (ThreadAbortException)
            {
            }
            catch (System.Net.WebException error)
            {
                string proxy = "";
                if (Proxy != null && Proxy.WebProxy.Address != null && Proxy.WebProxy.Address.Authority != null)
                    proxy = Proxy.WebProxy.Address.Authority;

                ErrorMessage = error.Message;
                if (web_routine_status == WebRoutineStatus.UNDEFINED)
                    web_routine_status = WebRoutineStatus.DOWNLOAD_ERROR;

                Log.Write("DOWNLOAD ERROR: " + error.Message + "\n" + error.StackTrace + "\nPROXY: " + proxy);
            }
            catch (Exception error)
            {
                ErrorMessage = error.Message;
                if (web_routine_status == WebRoutineStatus.UNDEFINED)
                    web_routine_status = WebRoutineStatus.DOWNLOAD_ERROR;
                Log.Error(error);
            }
            finally
            {
                if (res_stream != null)
                    res_stream.Close();
                if (HWResponse != null)
                    HWResponse.Close();
            }

            if (ErrorMessage == null)
                return false;
            return false;
        }
    }

    public class HttpRequest
    {
        public enum RequestMethod
        {
            GET,
            POST
        }

        public HttpRequest(string url, Dictionary<string, string> headers = null, HttpRequest.RequestMethod method = RequestMethod.GET, byte[] post_data = null)
        {
            Url = url;
            Method = method;
            if (post_data != null)
            {
                if (method != RequestMethod.POST) throw new Exception("RequestMethod is not POST while post_data is defined.");
                PostData = post_data;
            }
            if (headers != null)
                foreach(KeyValuePair<string,string> h in headers)
                    Headers[h.Key] = h.Value;
        }

        public HttpRequest(Cliver.Bot.HtmlForm html_form, Dictionary<string, string> headers = null)
        {
            if (headers != null)
                foreach (KeyValuePair<string, string> h in headers)
                    Headers[h.Key] = h.Value;
            Url = html_form.Url;
            string query = "";
            foreach (string parameter in html_form.Names)
            {
                if (html_form.GetType(parameter) == HtmlForm.ParameterType.RESET)
                    continue;
                foreach (string value in html_form[parameter])
                    query += "&" + HttpUtility.UrlEncode(parameter) + "=" + HttpUtility.UrlEncode(value);
            }
            Method = html_form.Method;
            if (Method == RequestMethod.POST)
            {
                post_string = query;
                PostData = Encoding.UTF8.GetBytes(post_string);
                Headers["Content-Type"] = "application/x-www-form-urlencoded";
            }
            else
            {
                if (Url.Contains("?"))
                    Url += query;
                else
                    Url += "?" + query.Substring(1);
            }
        }

        public HttpRequest.RequestMethod Method;

        public readonly string Url;

        public readonly Dictionary<string, string> Headers = DefaultHeaders;

        public readonly byte[] PostData;

        /// <summary>
        /// Used for caching requested file
        /// </summary>
        public string PostString
        {
            get
            {
                if (post_string == null && PostData != null)
                    post_string = Encoding.UTF8.GetString(PostData);
                return post_string;
            }
        }
        string post_string = null;

        /// <summary>
        /// Maximal redirection count
        /// </summary>
        static public int MaxAutoRedirectionCount = Properties.Web.Default.MaxHttpRedirectionCount;//-1;
        
        /// <summary>
        /// Predefined header collection in each request
        /// </summary>
        static public Dictionary<string, string> DefaultHeaders = new Dictionary<string, string>() { {"User-Agent", Properties.Web.Default.HttpUserAgent}};
    }
}
