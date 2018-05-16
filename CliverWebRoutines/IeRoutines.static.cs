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
using System.Media;
using System.Web;
using System.Net.NetworkInformation;


namespace Cliver.BotWeb
{
    public static class IeRoutines
    {
        public static void Invoke(this Control c, MethodInvoker code)
        {
            if (c.InvokeRequired)
            {
                c.Invoke(code);
                return;
            }
            code.Invoke();
        }

        //public static object Invoke(Control c, Func<object> code)
        //{
        //    if (c.InvokeRequired)
        //    {
        //        return c.Invoke(code);
        //    }
        //    return code.Invoke();
        //}

        public static Point GetOffset(HtmlElement e)
        {
            Point p = new Point(0, 0);
            for (; e != null; e = e.OffsetParent)
            {
                p.X += e.OffsetRectangle.X;
                p.Y += e.OffsetRectangle.Y;
            }
            return p;
        }

        public static HtmlElement WaitForHtmlElement(this WebBrowser browser, Func<HtmlElement> get_html_element, int timeout_in_mss)
        {
            return (HtmlElement)WaitForCondition(browser, get_html_element, timeout_in_mss);
        }

        public static object WaitForCondition(this WebBrowser browser, Func<object> check_condition, int timeout_in_mss)
        {
            DateTime timeout = DateTime.Now.AddMilliseconds(timeout_in_mss);
            while (DateTime.Now < timeout)
            {
                object o = null;
                browser.Invoke(() =>
                {
                    if (browser.Document != null && browser.Document.Body != null)
                        o = check_condition();
                });
                if (o != null)
                    return o;
                SleepRoutines.Wait(20);
            }
            return null;
        }

        public static bool WaitForPixelColor(this WebBrowser b, Point p, Color c, int timeout_in_mss)
        {
            Bitmap bmp = new Bitmap(b.Size.Width, b.Size.Height);
            Rectangle rec = new Rectangle(0, 0, bmp.Width, bmp.Height);

            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout_in_mss);
            while (dt > DateTime.Now)
            {
                b.DrawToBitmap(bmp, rec);
                //bmp.Save(@"C:\temp\000.bmp");
                if (bmp.GetPixel(p.X, p.Y) == c)
                    return true;
                Application.DoEvents();
                Thread.Sleep(20);
            }
            return false;
        }

        public static Color GetPixelColor(this WebBrowser b, Point p)
        {
            Bitmap bmp = new Bitmap(b.Size.Width, b.Size.Height);
            Rectangle rec = new Rectangle(0, 0, bmp.Width, bmp.Height);
            b.DrawToBitmap(bmp, rec);
            return bmp.GetPixel(p.X, p.Y);
        }

        //public static HtmlElement WaitForHtmlElement2(WebBrowser browser, string url, int dalay_in_mss, Func<HtmlElement> get_html_element)
        //{
        //    HtmlElement he = null;
        //    while (he == null)
        //    {
        //        while (browser.Document == null || browser.Document.Body == null)
        //            Application.DoEvents();

        //        he = get_html_element();
        //        if (he == null)
        //        {
        //            if (browser.ReadyState == WebBrowserReadyState.Complete)
        //                throw new Exception("The page has an unknown layout.");
        //            else
        //                Wait(20);
        //        }
        //    }
        //    return he;
        //}

        public static bool WaitForCompletion(this WebBrowser browser, int timeout_in_mss = -1)
        {
            if (timeout_in_mss < 0)
                timeout_in_mss = int.MaxValue;
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout_in_mss);
            while (dt > DateTime.Now)// && browser.ReadyState != WebBrowserReadyState.Interactive))//browser.ReadyState == WebBrowserReadyState.Loading)
            {
                WebBrowserReadyState o = WebBrowserReadyState.Uninitialized;
                browser.Invoke(() =>
                {
                    o = browser.ReadyState;
                });
                if (o == WebBrowserReadyState.Complete) 
                    return true;
                Thread.Yield();
                Thread.Sleep(20);
            }
            return false;
        }

        /// <summary>
        /// Get complete document by the url.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="url"></param>
        public static bool GetDoc(this WebBrowser browser, string url, int timeout_in_mss = -1)
        {
            browser.Navigate(url);
            return browser.WaitForCompletion(timeout_in_mss);
        }

        /// <summary>
        /// Get the previous page
        /// </summary>
        public static bool GetBack(this WebBrowser browser, int timeout_in_mss = -1)
        {
            browser.Invoke(() => { browser.GoBack(); });
            return browser.WaitForCompletion(timeout_in_mss);
        }

        public static void ClickHtmlElement(this HtmlElement he)
        {
            he.InvokeMember("click");
        }

        public static HtmlElement GetHtmlFirstElementContainingFragment(this HtmlElement parent, string tag, string fragment)
        {
            HtmlElementCollection hec;
            if (tag == null)
                hec = parent.All;
            else
                hec = parent.GetElementsByTagName(tag);
            foreach (HtmlElement he in hec)
                if (he.OuterHtml.Contains(fragment))
                    return he;
            return null;
        }

        public static IEnumerable<HtmlElement> GetHtmlElementsByAttr(this HtmlElement parent_he, string attribute, string value = null, string tag = null)
        {
            HtmlElementCollection hec;
            if (tag == null)
                hec = parent_he.All;
            else
                hec = parent_he.GetElementsByTagName(tag);
            foreach (HtmlElement he in hec)
            {
                //if (he.InnerText!= null && he.InnerText.Contains('$'))
                //string className = ((mshtml.IHTMLElement)he.DomElement).className;
                string a = he.GetAttribute(attribute);
                if (a == null)
                    continue;
                if (value == null || a == value)
                    yield return he;
            }
        }

        public static HtmlElement GetFirstHtmlElementByAttr(this HtmlElement parent, string tag, string attribute, string value)
        {
            return GetHtmlElementsByAttr(parent, tag, attribute, value).FirstOrDefault();
        }

        public static List<HtmlElement> GetHtmlElementsByPath(this HtmlElement parent, string path)
        {
            string[] tag_index_pairs = path.Split('/');
            List<HtmlElement> level_hes = new List<HtmlElement>();
            level_hes.Add(parent);
            foreach (string tag_index_pair in tag_index_pairs)
            {
                string[] p = tag_index_pair.Split('[', ']', ',');
                string tag = p[0].Trim();
                string _index = null;
                if (p.Length < 2)
                    _index = "*";
                else
                    _index = p[1].Trim();
                List<HtmlElement> child_hes = new List<HtmlElement>();
                if (_index == "*")
                {
                    foreach (HtmlElement lhe in level_hes)
                    {
                        HtmlElementCollection hes = lhe.Children;
                        foreach (HtmlElement he in hes)
                        {
                            if (he.TagName != tag)
                                continue;
                            child_hes.Add(he);
                        }
                    }
                }
                else
                {
                    int index = -1;
                    if (!int.TryParse(_index, out index) || index < 0)
                        throw (new Exception("Index '" + _index + "' in the path '" + path + "' is inadmissible. Index might be non-negative integer or '*' only."));

                    foreach (HtmlElement lhe in level_hes)
                    {
                        HtmlElementCollection hes = lhe.Children;
                        if (hes.Count <= index)
                            continue;
                        int count = 0;
                        foreach (HtmlElement he in hes)
                        {
                            if (he.TagName != tag)
                                continue;
                            if (count++ < index)
                                continue;
                            child_hes.Add(he);
                            break;
                        }
                    }
                }
                level_hes = child_hes;
            }

            return level_hes;
        }
        
        public static string RetrieveIeCookies(Uri uri)
        {
            try
            {                
                int datasize = 8192 * 16;
                StringBuilder cookie = new StringBuilder(datasize);
                if (!WinApi.Wininet.InternetGetCookie(uri.AbsoluteUri, null, cookie, ref datasize))
                //if (!Win32.InternetGetCookieEx(uri.ToString(), null, cookie, ref datasize, Win32.InternetCookieHttponly, IntPtr.Zero))
                {
                    //string t = (new Win32Exception(Win32.GetLastError())).Message;
                    if(datasize < 0)
                        return null;
                }
                return cookie.ToString();
            }
            catch (Exception error)
            {
                Log.Error(error);
            }
            return null;
        }
    }
}

