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


namespace Cliver
{
    public static class IeRoutines
    {
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

        public static HtmlElement WaitForHtmlElement(WebBrowser browser, Func<HtmlElement> get_html_element, int mss)
        {
            return (HtmlElement)WaitForCondition(browser, get_html_element, mss);
        }

        delegate object delegateWaitForCondition(WebBrowser browser, Func<object> check_condition, int mss);
        public static object WaitForCondition(WebBrowser browser, Func<object> check_condition, int mss)
        {
            if (browser.InvokeRequired)
                return browser.Invoke(new delegateWaitForCondition(WaitForCondition), browser, check_condition, mss);

            DateTime timeout = DateTime.Now.AddMilliseconds(mss);
            while(DateTime.Now < timeout)
            {
                if (browser.Document != null && browser.Document.Body != null)
                {
                    object o = check_condition();
                    if (o != null)
                        return o;
                }
                ThreadRoutines.Wait(20);
            }
            return null;
        }

        public static bool WaitForPixelColor(this WebBrowser b, Point p, Color c, int mss)
        {
            Bitmap bmp = new Bitmap(b.Size.Width, b.Size.Height);
            Rectangle rec = new Rectangle(0, 0, bmp.Width, bmp.Height);

            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, mss);
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
        
        public static bool WaitForCompletion(this WebBrowser browser, int mss)
        {
            if (mss < 0)
                mss = int.MaxValue;
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, mss);
            while (dt > DateTime.Now)// && browser.ReadyState != WebBrowserReadyState.Interactive))//browser.ReadyState == WebBrowserReadyState.Loading)
            {
                if (browser.ReadyState == WebBrowserReadyState.Complete) return true;
                Application.DoEvents();
                Thread.Yield();
                Thread.Sleep(20);
            }
            return false;
        }

        public static void GetCompletedPage(this WebBrowser browser, string url)
        {
            browser.Navigate(url);
            while (browser.ReadyState != WebBrowserReadyState.Complete)// && browser.ReadyState != WebBrowserReadyState.Interactive))//browser.ReadyState == WebBrowserReadyState.Loading)
            {
                Application.DoEvents();
                Thread.Sleep(20);
                //System.Net.NetworkInformation.TcpConnectionInformation f = new 
                //            System.Net.NetworkInformation.TcpStatistics c = ;
                //                var c = new PerformanceCounter(".Net CLR Networking", "Bytes Sent", Process.GetCurrentProcess().ProcessName + "[" + Process.GetCurrentProcess().Id + "]");
                //                c.BeginInit();
                //                float f = c.NextValue();


                //System.Diagnostics.PerformanceCounter performanceCounter1;
                //performanceCounter1 = new System.Diagnostics.PerformanceCounter();
                //performanceCounter1.CategoryName = "Network Interface";
                //performanceCounter1.CounterName = "Bytes Sent/sec";
                //performanceCounter1.InstanceName = "3Com 3C920 Integrated Fast Ethernet Controller [3C905C-TX Compatible] - Packet Sc" + "heduler Miniport";
                //performanceCounter1.MachineName = "sha-lion-01";
                //MessageBox.Show(performanceCounter1.NextValue().ToString()); 

                //if (!NetworkInterface.GetIsNetworkAvailable())
                //    return;

                //NetworkInterface[] interfaces
                //    = NetworkInterface.GetAllNetworkInterfaces();

                //foreach (NetworkInterface ni in interfaces)
                //{
                //    Console.WriteLine("    Bytes Sent: {0}",
                //        ni.GetIPv4Statistics()..BytesSent);
                //    Console.WriteLine("    Bytes Received: {0}",
                //        ni.GetIPv4Statistics().BytesReceived);
                //}


            }
        }

        public static HtmlElement GetHtmlElementByFragment(HtmlElement parent, string tag, string fragment)
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

        public static HtmlElement GetHtmlElementByAttr(HtmlElement parent, string tag, string attribute, string value)
        {
            HtmlElementCollection hec;
            if (tag == null)
                hec = parent.All;
            else
                hec = parent.GetElementsByTagName(tag);
            foreach (HtmlElement he in hec)
                //if (he.InnerText!= null && he.InnerText.Contains('$'))
                //string className = ((mshtml.IHTMLElement)he.DomElement).className;
                if (he.GetAttribute(attribute) == value)
                    return he;
            return null;
        }

        public static List<HtmlElement> GetHtmlElementsByPath(HtmlElement parent, string path)
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
    }
}

